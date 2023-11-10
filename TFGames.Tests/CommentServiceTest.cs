using NSubstitute;
using NSubstitute.ReturnsExtensions;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.Common.Exceptions;
using TFGames.DAL.Data.Repository;

namespace TFGames.Tests
{
    public class CommentServiceTest
    {
        private readonly ICommentRepository _commentRepository;

        private readonly IUserRepository _userRepository;

        private readonly IArticleRepository _articleRepository;

        private readonly Guid _articleId;

        private readonly string _userId;

        private readonly Guid _commentId;

        private readonly User _user;

        private readonly Comment _comment;

        private readonly Article _article;

        public CommentServiceTest()
        {
            _articleId = Guid.Empty;
            _userId = Guid.Empty.ToString();
            _commentId = Guid.Empty;
            _user = new User() { Id = _userId };
            _article = new Article() { Id = _articleId };
            _comment = new Comment { Author = _user, Article = _article, Replies = new List<Comment>() };

            _commentRepository = Substitute.For<ICommentRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _articleRepository = Substitute.For<IArticleRepository>();
        }

        [Theory]
        [InlineData("content")]
        public async Task Create_ShouldReturnComment(string content)
        {
            // Arrange
            var createComment = new CommentRequestModel { Content = content };
            var getComment = new CommentResponseModel { Content = content };

            _userRepository.FindByIdAsync(_userId).Returns(_user);
            _articleRepository.FindByIdWithInclude(_articleId).Returns(_article);

            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action
            var actualComment = await commentService.Create(createComment, _userId, _articleId);

            // Assert
            Assert.NotNull(actualComment);
            Assert.Equal(getComment.Content, actualComment.Content);
            await _userRepository.Received(1).FindByIdAsync(Arg.Is<string>(_userId));
            await _articleRepository.Received(1).FindByIdWithInclude(Arg.Is<Guid>(_articleId));
            await _commentRepository.Received(1).Create(Arg.Is<Comment>(c => c.Content == content));
        }

        [Theory]
        [InlineData("content")]
        public async Task Create_ShouldThrowUserNotFound(string content)
        {
            // Arrange
            var createComment = new CommentRequestModel { Content = content };
            var expectedMessage = ErrorMessages.NotFound;
            _userRepository.FindByIdAsync(_userId).ReturnsNull();
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action & Assert
            var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await commentService.Create(createComment, _userId, _articleId));

            Assert.Equal(expectedMessage, actualException.Message);
            await _userRepository.Received(1).FindByIdAsync(Arg.Is<string>(_userId));
            await _articleRepository.Received(0).FindByIdWithInclude(Arg.Any<Guid>());
            await _commentRepository.Received(0).Create(Arg.Any<Comment>());
        }

        [Theory]
        [InlineData("content")]
        public async Task Create_ShouldThrowArticleNotFound(string content)
        {
            // Arrange
            var createComment = new CommentRequestModel { Content = content };
            var expectedMessage = ErrorMessages.NotFound;
            _userRepository.FindByIdAsync(_userId).Returns(_user);
            _articleRepository.FindByIdWithInclude(_articleId).ReturnsNull();
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action & Assert
            var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await commentService.Create(createComment, _userId, _articleId));

            Assert.Equal(expectedMessage, actualException.Message);
            await _userRepository.Received(1).FindByIdAsync(Arg.Is<string>(_userId));
            await _articleRepository.Received(1).FindByIdWithInclude(Arg.Any<Guid>());
            await _commentRepository.Received(0).Create(Arg.Any<Comment>());
        }

        [Theory]
        [InlineData("content")]
        public async Task Reply_ShouldReturnComment(string content)
        {
            // Arrange
            var reply = new CommentRequestModel
            {
                Content = content
            };

            _userRepository.FindByIdAsync(_userId).Returns(_user);
            _commentRepository.FindById(_commentId).Returns(_comment);

            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action
            var actualReply = await commentService.Reply(_commentId, reply, _userId);

            // Assert
            Assert.Equal(reply.Content, actualReply.Content);
            await _userRepository.Received(1).FindByIdAsync(Arg.Is<string>(_userId));
            await _commentRepository.Received(1).FindById(Arg.Is<Guid>(_commentId));
            await _commentRepository.Received(1).Update(Arg.Is<Comment>(_comment));
        }

        [Fact]
        public async Task Reply_ShouldThrowNotFoundAuthor()
        {
            // Arrange
            var reply = new CommentRequestModel();

            var expectedMessage = ErrorMessages.NotFound;
            _userRepository.FindByIdAsync(_userId).ReturnsNull();
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action & Assert
            var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await commentService.Reply(_commentId, reply, _userId));

            Assert.Equal(expectedMessage, actualException.Message);
            await _userRepository.Received(1).FindByIdAsync(Arg.Is<string>(_userId));
            await _commentRepository.Received(0).FindById(Arg.Is<Guid>(_commentId));
            await _commentRepository.Received(0).Update(Arg.Any<Comment>());
        }

        [Fact]
        public async Task Reply_ShouldThrowNotFoundArticle()
        {
            // Arrange
            var reply = new CommentRequestModel();

            var expectedMessage = ErrorMessages.NotFound;
            _userRepository.FindByIdAsync(_userId).Returns(_user);
            _commentRepository.FindById(_commentId).ReturnsNull();
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action & Assert
            var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await commentService.Reply(_commentId, reply, _userId));

            Assert.Equal(expectedMessage, actualException.Message);
            await _userRepository.Received(1).FindByIdAsync(Arg.Is<string>(_userId));
            await _commentRepository.Received(1).FindById(Arg.Is<Guid>(_commentId));
            await _commentRepository.Received(0).Update(Arg.Any<Comment>());
        }

        [Theory]
        [InlineData("content")]
        public async Task Edit_ShouldBeCompleted(string content)
        {
            // Arrange
            _commentRepository.FindById(_commentId).Returns(_comment);
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action
            await commentService.Edit(_commentId, content, _user.Id);

            // Assert
            await _commentRepository.Received(1).Update(Arg.Is<Comment>(_comment));
        }

        [Fact]
        public async Task Edit_ShouldThrowNotFound()
        {
            // Arrange
            var expectedMessage = ErrorMessages.NotFound;
            _commentRepository.FindById(_commentId).ReturnsNull();
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action
            var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await commentService.Edit(_commentId, "", _userId));

            // Assert
            Assert.Equal(expectedMessage, actualException.Message);
            await _commentRepository.Received(0).Update(Arg.Is<Comment>(_comment));
        }

        [Fact]
        public async Task Edit_ShouldThrowForbidden()
        {
            // Arrange
            var expectedMessage = ErrorMessages.ForbiddenToEditComment;
            var incorrectAuthorId = Guid.NewGuid().ToString();
            _commentRepository.FindById(_commentId).Returns(_comment);
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action
            var actualException = await Assert.ThrowsAsync<ForbiddenException>(async () => await commentService.Edit(_commentId, "", incorrectAuthorId));

            // Assert
            Assert.Equal(expectedMessage, actualException.Message);
            await _commentRepository.Received(0).Update(Arg.Is<Comment>(_comment));
        }

        [Fact]
        public async Task Delete_ShouldBeCompleted()
        {
            // Arrange
            _commentRepository.FindById(_commentId).Returns(_comment);
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action
            await commentService.Delete(_commentId, _userId);

            // Assert
            await _commentRepository.Received(1).Delete(Arg.Is<Comment>(_comment));
        }

        [Fact]
        public async Task Delete_ShouldThrowNotFound()
        {
            // Arrange
            var expectedMessage = ErrorMessages.NotFound;

            _commentRepository.FindById(_commentId).ReturnsNull();
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action & Assert
            var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await commentService.Delete(_commentId, _userId));

            Assert.Equal(expectedMessage, actualException.Message);
            await _commentRepository.Received(1).FindById(Arg.Is<Guid>(_commentId));
            await _commentRepository.Received(0).Delete(_comment);
        }

        [Fact]
        public async Task FindById_ShouldReturnComment()
        {
            // Arrange
            _comment.Content = "content";
            _commentRepository.FindById(_commentId).Returns(_comment);
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action
            var actualComment = await commentService.FindById(_commentId);

            // Assert
            Assert.Equal(_comment.Content, actualComment.Content);
            await _commentRepository.Received(1).FindById(Arg.Is<Guid>(_commentId));
        }

        [Fact]
        public async Task FindById_ShouldThrowNotFound()
        {
            // Arrange
            var expectedMessage = ErrorMessages.NotFound;
            _commentRepository.FindById(_commentId).ReturnsNull();
            var commentService = new CommentService(_commentRepository, _userRepository, _articleRepository);

            // Action & Assert
            var actualException = await Assert.ThrowsAsync<NotFoundException>(async () => await commentService.FindById(_commentId));

            Assert.Equal(expectedMessage, actualException.Message);
            await _commentRepository.Received(1).FindById(Arg.Is<Guid>(_commentId));
        }
    }
}