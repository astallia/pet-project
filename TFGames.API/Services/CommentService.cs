using TFGames.API.Mappers;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Constants;
using TFGames.Common.Enums;
using TFGames.Common.Exceptions;
using TFGames.DAL.Data.Repository;
using TFGames.DAL.Search;

namespace TFGames.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        
        private readonly IUserRepository _userRepository;

        private readonly IArticleRepository _articleRepository;

        public CommentService(ICommentRepository commentRepository, IUserRepository userRepository, IArticleRepository articleRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _articleRepository = articleRepository;
        }

        public async Task<CommentResponseModel> Create(CommentRequestModel model, string authorId, Guid articleId)
        {
            var content = model.Content.Trim();

            if (content.Length == 0)
            {
                throw new BadRequestException(ErrorMessages.CommentWhiteSpace);
            }

            var author = await _userRepository.FindByIdAsync(authorId);

            if (author == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            var article = await _articleRepository.FindByIdWithInclude(articleId);

            if (article == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            var comment = CommentMapper.Map(model);

            comment.Author = author;
            comment.Article = article;
            
            await _commentRepository.Create(comment);

            return CommentMapper.Map(comment);
        }

        public async Task<CommentResponseModel> Reply(Guid id, CommentRequestModel replyModel, string authorId)
        {
            var author = await _userRepository.FindByIdAsync(authorId);

            if (author == null) 
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            var reply = CommentMapper.Map(replyModel);
            var comment = await _commentRepository.FindById(id);

            if (comment == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            reply.Author = author;
            reply.Article = comment.Article;
            comment.Replies.Add(reply);
            
            await _commentRepository.Update(comment);

            return CommentMapper.Map(reply);
        }

        public async Task<CommentResponseModel> CreateOrReply(Guid? commentId, CommentRequestModel model, Guid articleId, string authorId)
        {
            CommentResponseModel comment;

            if (!commentId.HasValue)
            {
                comment = await Create(model, authorId, articleId);
            }
            else
            {
                comment = await Reply(commentId.Value, model, authorId);
            }

            return comment;
        }

        public async Task Delete(Guid id, string authorId)
        {
            var author = await _userRepository.FindByIdAsync(authorId);
            var authorRole = await _userRepository.GetRoleAsync(author);
            var comment = await _commentRepository.FindById(id);

            if (comment == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            if (authorId != comment.Author.Id && authorRole != RoleNames.SuperAdmin.ToString())
            {
                throw new ForbiddenException(ErrorMessages.ForbiddenToEdit);
            }
            
            await _commentRepository.Delete(comment);
        }

        public async Task Edit(Guid id, string content, string authorId)
        {
            var author = await _userRepository.FindByIdAsync(authorId);
            var authorRole = await _userRepository.GetRoleAsync(author);
            var comment = await _commentRepository.FindById(id);

            if (comment == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            if (authorId != comment.Author.Id && authorRole != RoleNames.SuperAdmin.ToString())
            {
                throw new ForbiddenException(ErrorMessages.ForbiddenToEditComment);
            }

            comment.Content = content;

            await _commentRepository.Update(comment);
        }

        public async Task<List<CommentResponseModel>> FindAllByArticleId(Guid articleId)
        {
            var comments = await _commentRepository.FindAllByArticleId(articleId);
            return comments.ConvertAll(CommentMapper.Map);
        }

        public async ValueTask<List<CommentResponseModel>> GetAll(FiltersRequest filters, OrderBy order, SortParameters parameters)
        {
            var filter = FilterMapper.Map(filters);

            var orderBy = new OrderByProperties();

            orderBy.SortParameters = parameters;
            orderBy.Order = order;

            var comments = await _commentRepository.GetAll(filter, orderBy);


            if (comments.Count == 0)
            {
                throw new NotFoundException(ErrorMessages.NoResult);
            }

            return comments.ConvertAll(CommentMapper.Map);
        }

        public async Task<CommentResponseModel> FindById(Guid id)
        {
            var comment = await _commentRepository.FindById(id);

            if (comment == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            return CommentMapper.Map(comment);
        }
    }
}