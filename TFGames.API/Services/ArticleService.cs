using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Options;
using TFGames.API.Mappers;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Configurations;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.Common.Enums;
using TFGames.Common.Exceptions;
using TFGames.DAL.Search;
using TFGames.DAL.Data.Repository;

namespace TFGames.API.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        private readonly ICommentRepository _commentRepository;

        private readonly IApplicationSettingsService _applicationSettingsService;

        private readonly IArticleTagsRepository _articleTagsRepository;

        private readonly IUserRepository _userRepository;

        private readonly IImageService _imageService;

        private readonly ValidImageProperties _validProperties;

        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository articleRepository, ICommentRepository commentRepository, IMapper mapper, IUserRepository userRepository, IOptions<ValidImageProperties> validProperties, IArticleTagsRepository articleTagsRepository, IImageService imageService, IApplicationSettingsService applicationSettingsService)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _validProperties = validProperties.Value;
            _articleTagsRepository = articleTagsRepository;
            _applicationSettingsService = applicationSettingsService;
            _imageService = imageService;
        }

        public async Task<ArticleResponseModel> Create(string authorId, ArticleRequestModel articleRequest)
        {
            var author = await _userRepository.FindByIdAsync(authorId);
            var article = _mapper.Map<Article>(articleRequest);
            article.Author = author;

            article.Tags = await AddTag(articleRequest, article);

            if (article.GameInfo.Year > 3000 || article.GameInfo.Year < 1000 & article.GameInfo.Year != 0)
            {
                throw new ConflictException(ErrorMessages.WrongYear);
            }

            if (article.Tags.Count > 5) 
            {
                throw new ConflictException(ErrorMessages.WrongTagsQuantity);
            }

            var applicationSettings = await _applicationSettingsService.Get();

            if (applicationSettings.CompressImages)
            {
                article.MainImage.MainImage = _imageService.CompressImage(article.MainImage.MainImage, article.MainImage.ContentType);
            }

            if (article.MainImage.MainImage.Length >= _validProperties.ValidSize)
            {
                throw new ConflictException(ErrorMessages.WrongFileSize);
            }

            await _articleRepository.Create(article);

            return ArticleMapper.Map(article);
        }

        public async ValueTask<List<AuthorResponseModel>> GetAllAuthors(FiltersRequest filters, OrderBy order, SortParameters parameters)
        {
            var filter = FilterMapper.Map(filters);

            var orderBy = new OrderByProperties();

            orderBy.SortParameters = parameters;
            orderBy.Order = order;

            var users = await _articleRepository.GetAllAuthors(filter, orderBy);

            if (users.Count == 0)
            {
                throw new NotFoundException(ErrorMessages.NoResult);
            }

            return users.ConvertAll(UserMapper.Map);
        }

        public async ValueTask<List<TagResponseModel>> GetAllTags(FiltersRequest filters, OrderBy order, SortParameters parameters)
        {
            var filter = FilterMapper.Map(filters);

            var orderBy = new OrderByProperties();

            orderBy.SortParameters = parameters;
            orderBy.Order = order;

            var tags = await _articleTagsRepository.GetAll(filter, orderBy);

            if (tags.Count == 0)
            {
                throw new NotFoundException(ErrorMessages.NoResult);
            }

            return tags.ConvertAll(TagMapper.Map);
        }

        public async ValueTask<ArticleListResponseModel> GetAll(FiltersRequest filters, OrderBy order, SortParameters parameters, int page, int size, string userId, FindBy find)
        {
            var user = await _userRepository.FindByIdAsync(userId);

            if (filters.IsFavorites && user == null)
            {
                throw new ForbiddenException(ErrorMessages.Forbidden);
            }

            var filter = FilterMapper.Map(filters, filters.IsFavorites ? user : null);

            var orderBy = new OrderByProperties();

            orderBy.SortParameters = parameters;

            orderBy.Order = order;

            var result = await _articleRepository.GetPage(filter, orderBy, page, size, Guid.Empty, find);

            var articlePreviewList = result.Articles;

            var totalArticles = result.Count;

            if (articlePreviewList == null)
            {
                throw new BadRequestException(ErrorMessages.NoPreviewArrticle);
            }

            var response = await Map(user, articlePreviewList);

            if (response.Count == 0) 
            {
                throw new NotFoundException(ErrorMessages.NoResult);
            }

            var responseList = new ArticleListResponseModel() 
            {
                Articles = response,
                TotalPages = totalArticles,
                CurrentPage = page
            };

            return responseList;
        }

        public async ValueTask<List<AuthorResponseModel>> GetTopAuthors(int size)
        {
            var topAuthors = await _articleRepository.GetTopAuthors(size);

            return topAuthors.ConvertAll(UserMapper.Map);
        }

        public async ValueTask<List<TagResponseModel>> GetTopTags(int size)
        {
            var topTags = await _articleTagsRepository.GetTop(size);

            return topTags.ConvertAll(TagMapper.Map);
        }

        public async Task LikeBy(Guid id, string userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ForbiddenException(ErrorMessages.Forbidden);
            }

            var article = await _articleRepository.FindByIdWithInclude(id);

            await _articleRepository.PutLike(user, article);
        }

        public async ValueTask<ArticleResponseModel> GetById(Guid id, string userId)
        {
            var article = await _articleRepository.FindByIdWithInclude(id);

            if (article == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            var image = await _imageService.GetContentImage(article.MainImageId);

            var comment = await _commentRepository.FindAllByArticleId(id);

            article.Comments = comment;

            var result = ArticleMapper.Map(article, image.First().Value);

            var filters = new Filters();

            filters.Search = article.Author.Id;

            var orderBy = new OrderByProperties();

            var authorsArticles = await _articleRepository.GetPage(filters, orderBy, 1 , 5, article.Id, FindBy.UserArticles);

            result.AuthorsArticles = _mapper.Map<List<ArticleAuthorResponseModel>>(authorsArticles.Articles);

            var user = await _userRepository.FindByIdAsync(userId);

            if (user != null)
            {
                if (article.Likes.Any(u => u == user))
                {
                    result.HasCurrentUserLiked = true;
                }
            }

            return result;
        }
        
        private async Task<List<ArticlePreviewResponseModel>> Map(User user, List<Article> articles)
        {
            var responses = new List<ArticlePreviewResponseModel>();

            var images = await _imageService.GetContentImage(articles.Select(i => i.MainImageId).ToArray());

            foreach (var article in articles)
            {

                if (images.TryGetValue(article.MainImageId, out var image))
                {
                    article.MainImage = new ArticleImage();

                    article.MainImage.MainImage = image.Image;
                    article.MainImage.ContentType = image.ContentType;
                }
                else
                {
                    throw new NotFoundException(ErrorMessages.NotFound);
                }

                var response = _mapper.Map<ArticlePreviewResponseModel>(article);

                response.HasCurrentUserLiked = article.Likes.Any(u => u == user) ? true : false;

                responses.Add(response);
            }

            return responses;
        }

        public async Task Update(ArticleRequestModel articleRequest, Guid articleId, string userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);

            var userRole = await _userRepository.GetRoleAsync(user);

            var article = await _articleRepository.FindByIdWithInclude(articleId);

            if (userRole == "Author" & article.Author.Id != userId)
            {
                throw new ForbiddenException(ErrorMessages.Forbidden);
            }

            var images = await _imageService.GetContentImage(article.MainImageId);

            var image = images.First().Value;

            article.MainImage = new ArticleImage
            {
                MainImage = image.Image,
                ContentType = image.ContentType
            };

            article.Name = articleRequest.Name;
            article.GameInfo.GameType = articleRequest.GameType;
            article.GameInfo.Platform = articleRequest.Platform;
            article.GameInfo.Year = articleRequest.Year;
            article.Description = articleRequest.Description;
            article.Content.Content = articleRequest.Content;
            article.MainImage.MainImage = Convert.FromBase64String(articleRequest.MainImage);
            article.MainImage.ContentType = articleRequest.ContentType;

            article.Tags = await AddTag(articleRequest, article);

            await _articleRepository.Update(article);
        }

        public async Task Delete(Guid articleId, string userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);

            var userRole = await _userRepository.GetRoleAsync(user);

            var article = await _articleRepository.FindByIdWithInclude(articleId);

            if (userRole == "Author" & article.Author.Id != userId)
            {
                throw new ForbiddenException(ErrorMessages.Forbidden);
            }

            await _articleRepository.Delete(articleId);
        }

        private async Task<List<Tag>> AddTag(ArticleRequestModel articleRequest, Article article)
        {
            var existingTags = await _articleTagsRepository.FindByNames(articleRequest.Tags.Select(t => t.Name).ToList());

            article.Tags = existingTags;

            var existing = existingTags.Select(t => t.Name).ToHashSet();

            foreach (var tag in articleRequest.Tags)
            {
                if (!existing.Contains(tag.Name))
                {
                    article.Tags.Add(TagMapper.Map(tag));
                }
            }

            return article.Tags;
        }
    }
}
