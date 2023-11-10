using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.DAL.Entities;

namespace TFGames.API.Mappers
{
    public static class CommentMapper
    {
        public static CommentResponseModel Map(Comment comment)
        {
            return new CommentResponseModel
            {
                Id = comment.Id,
                ArticleId = comment.Article.Id.ToString(),
                Content = comment.Content,
                Author = UserMapper.Map(comment.Author),
                Replies = comment.Replies?.ToList()?.ConvertAll(Map) ?? new List<CommentResponseModel>(),
                CreatedAt = comment.CreatedAt
            };
        }

        public static Comment Map(CommentRequestModel model)
        {
            return new Comment
            {
                Content = model.Content,
                Replies = new List<Comment>(),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}