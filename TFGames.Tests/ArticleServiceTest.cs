using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using TFGames.API.Mappers;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Configurations;
using TFGames.DAL.Entities;
using TFGames.Common.Enums;
using TFGames.DAL.Search;
using TFGames.DAL.Data.Repository;

namespace TFGames.Tests
{
    public class ArticleServiceTest
    {
        private readonly IArticleRepository _articleRepository;

        private readonly IArticleTagsRepository _articleTagsRepository;

        private readonly IApplicationSettingsService _applicationSettingsService;

        private readonly IUserRepository _userRepository;

        private readonly IImageService _imageService;

        private readonly User user;

        private readonly ICommentRepository _commentRepository;

        private static IMapper _mapper;

        private readonly IOptions<ValidImageProperties> _validationException = Options.Create<ValidImageProperties>(new ValidImageProperties());

        private readonly Comment comment;

        private readonly Article article = new Article
        {
            Id = Guid.Empty,
            Name = "Test",
            Author = new User
            {
                Id = "123",
                UserName = "Test",
                Name = "Name",
                Surname = "Surname"
            },
            MainImage = new ArticleImage
            {
                MainImage = new byte[] {
                    byte.MaxValue,
                    byte.MinValue
                },
                ContentType = "Test",
            },
            GameInfo = new GameInfo()
            {
                GameType = "Test",
                Platform = "Test",
                Year = 2032
            },
            Tags = new List<Tag>
            {
                new Tag
                {
                    Name = "Test",
                    Articles = new()
                },
                new Tag
                {
                    Name = "#test",
                    Articles = new()
                }
            },
            Description = "Test",
            Comments = new List<Comment>
            {
                new Comment
                {
                    Content = "Test",
                    Replies = new List<Comment>()
                }
            },
            Likes = new List<User>(),
            Content = new ArticleContent()
            {
                Content = "Test"
            },
            Created = new DateTime(2023, 08, 07, 12, 33, 9, DateTimeKind.Utc),
            Published = new DateTime(2023, 08, 07, 12, 33, 9, DateTimeKind.Utc)
        };

        private readonly ArticleImage image = new ArticleImage
        {
            Id = Guid.Empty,
            MainImage = new byte[] {
                byte.MaxValue,
                byte.MinValue
            },
            ContentType = "Test"
        };

        private readonly ImageResponse imageResponse = new ImageResponse
        {
            Id = Guid.Empty,
            Image = new byte[] {
                byte.MaxValue,
                byte.MinValue
            },
            ContentType = "Test"
        };

        private readonly ArticleResponseModel expectedArticle = new ArticleResponseModel
        {
            Name = "Test",
            Author = new AuthorResponseModel
            {
                Name = "Name",
                Surname = "Surname",
                UserName = "Test"
            },
            GameType = "Test",
            Platform = "Test",
            Year = 2032,
            Tags = new List<TagResponseModel>
            {
                new TagResponseModel
                {
                    Name = "Test",
                },
                new TagResponseModel
                {
                    Name = "#test",
                }
            },
            MainImage = new byte[] {
                byte.MaxValue,
                byte.MinValue
            },
            ContentType = "Test",
            Content = "Test",
            Comments = new List<CommentResponseModel>()
            {
                new CommentResponseModel
                {
                    Content = "Test",
                    Replies = new List<CommentResponseModel>()
                }
            },
            AuthorsArticles = new List<ArticleAuthorResponseModel>(),
            Likes = 0,
            HasCurrentUserLiked = false,
            Created = new DateTime(2023, 08, 07, 12, 33, 9, DateTimeKind.Utc),
            Published = new DateTime(2023, 08, 07, 12, 33, 9, DateTimeKind.Utc)
        };

        private readonly List<ArticlePreviewResponseModel> _articles;

        public ArticleServiceTest()
        {
            article.Tags[0].Articles.Add(article);
            article.Tags[1].Articles.Add(article);

            user = new User { Id = "", UserName = "Test" , Favorites = new List<Article> { article } };

            comment = new Comment { Author = user, Article = article, Replies = new List<Comment>(), Content = "Test" };
            _articles = Get();
            _articleRepository = MockArticleRepository().Object;
            _userRepository = MockUserRepository().Object;
            _imageService = MockImageService().Object;
            _commentRepository = MockCommentRepository().Object;

            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new ArticleAutoMapper());
                });

                IMapper mapper = mappingConfig.CreateMapper();

                _mapper = mapper;
            }
        }

        private List<ArticlePreviewResponseModel> Get()
        {
            var articles = new List<ArticlePreviewResponseModel>()
            {
                new ArticlePreviewResponseModel
                {
                    Name = "Test",
                    Author = new AuthorResponseModel
                    {
                        Name = "Name",
                        Surname = "Surname",
                        UserName = "Test"
                    },
                    GameType = "Test",
                    Platform = "Test",
                    Year = 2032,
                    Tags = new List<TagResponseModel>
                    {
                        new TagResponseModel
                        {
                            Name = "Test",
                        },
                        new TagResponseModel
                        {
                            Name = "#test",
                        }
                    },
                    MainImage = new byte[] {
                        byte.MaxValue,
                        byte.MinValue
                    },
                    ContentType = "Test",
                    Description = "Test",
                    Created = new DateTime(2023, 08, 07, 12, 33, 9, DateTimeKind.Utc),
                    Published = new DateTime(2023, 08, 07, 12, 33, 9, DateTimeKind.Utc)
                }
            };

            return articles;
        }

        [Fact]
        public async void GetAll_Success()
        {
            //Arrange
            var expected = _articles;
            var service = new ArticleService(_articleRepository, _commentRepository, _mapper, _userRepository, _validationException, _articleTagsRepository, _imageService, _applicationSettingsService);
            var filters = new FiltersRequest();
            var orderBy = new OrderBy();
            var find = new FindBy();
            var sortParameters = new SortParameters();

            //Action
            var model = await service.GetAll(filters, orderBy, sortParameters, 1, 10, null, find);
            var result = model.Articles;
            
            //Assert
            Assert.NotNull(result);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(expected[i].Name, ((ArticlePreviewResponseModel)result[i]).Name);
                Assert.Equal(expected[i].Author.UserName, ((ArticlePreviewResponseModel)result[i]).Author.UserName);
                Assert.Equal(expected[i].MainImage, ((ArticlePreviewResponseModel)result[i]).MainImage);
                Assert.Equal(expected[i].GameType, ((ArticlePreviewResponseModel)result[i]).GameType);
                Assert.Equal(expected[i].Platform, ((ArticlePreviewResponseModel)result[i]).Platform);
                Assert.Equal(expected[i].Year, ((ArticlePreviewResponseModel)result[i]).Year);
                Assert.Equal(expected[i].Tags.FirstOrDefault().Name, ((ArticlePreviewResponseModel)result[i]).Tags.FirstOrDefault().Name);
                Assert.Equal(expected[i].ContentType, ((ArticlePreviewResponseModel)result[i]).ContentType);
                Assert.Equal(expected[i].Description, ((ArticlePreviewResponseModel)result[i]).Description);
                Assert.Equal(expected[i].Created, ((ArticlePreviewResponseModel)result[i]).Created);
                Assert.Equal(expected[i].Published, ((ArticlePreviewResponseModel)result[i]).Published);
            }
        }

        [Fact]
        public async void GetById_Success()
        {
            //Arrange
            var expected = expectedArticle;
            var service = new ArticleService(_articleRepository, _commentRepository, _mapper, _userRepository, _validationException, _articleTagsRepository, _imageService, _applicationSettingsService);

            //Action
            var result = await service.GetById(article.Id, user.Id);

            //Assert
            Assert.NotNull(result);

            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Author.UserName, result.Author.UserName);
            Assert.Equal(expected.MainImage, result.MainImage);
            Assert.Equal(expected.GameType, result.GameType);
            Assert.Equal(expected.Platform, result.Platform);
            Assert.Equal(expected.Year, result.Year);
            Assert.Equal(expected.Tags.FirstOrDefault().Name, result.Tags.FirstOrDefault().Name);
            Assert.Equal(expected.ContentType, result.ContentType);
            Assert.Equal(expected.Content, result.Content);
            Assert.Equal(expected.Comments.FirstOrDefault().Content, result.Comments.FirstOrDefault().Content);
            Assert.NotNull(result.AuthorsArticles);
            Assert.Equal(expected.Likes, result.Likes);
            Assert.Equal(expected.Created, result.Created);
            Assert.Equal(expected.Published, result.Published);
        }

        private Mock<IArticleRepository> MockArticleRepository()
        {
            var articleRepository = new Mock<IArticleRepository>();

            articleRepository.Setup(x => x.GetPage(It.IsAny<Filters>(), It.IsAny<OrderByProperties>(), 1, 10, Guid.Empty, It.IsAny<FindBy>()))
                .ReturnsAsync((new List<Article> { article }, 1));

            articleRepository.Setup(x => x.FindByIdWithInclude(article.Id))
               .ReturnsAsync(article);

            articleRepository.Setup(x => x.GetImages(article.MainImageId))
                .ReturnsAsync(new Dictionary<Guid, ArticleImage> { { image.Id, image} });

            return articleRepository;
        }

        private Mock<IUserRepository> MockUserRepository()
        {
            var userRepository = new Mock<IUserRepository>();

            userRepository.Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            return userRepository;
        }

        private Mock<ICommentRepository> MockCommentRepository()
        {
            var commentRepository = new Mock<ICommentRepository>();

            commentRepository.Setup(x => x.FindAllByArticleId(article.Id))
                .ReturnsAsync(new List<Comment> { comment });

            return commentRepository;
        }

        private Mock<IImageService> MockImageService()
        {
            var imageService = new Mock<IImageService>();

            var id = new Guid[] { article.MainImageId };

            imageService.Setup(x => x.GetContentImage(id))
                .ReturnsAsync(new Dictionary<Guid, ImageResponse> { { imageResponse.Id, imageResponse } });

            return imageService;
        }
    }
}
