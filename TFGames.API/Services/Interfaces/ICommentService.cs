using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.Common.Enums;

namespace TFGames.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponseModel> Create(CommentRequestModel model, string authorId, Guid articleId);

        Task<CommentResponseModel> Reply(Guid id, CommentRequestModel reply, string authroId);

        Task<CommentResponseModel> CreateOrReply(Guid? commentId, CommentRequestModel model, Guid articleId, string authorId);

        Task<CommentResponseModel> FindById(Guid id);

        Task<List<CommentResponseModel>> FindAllByArticleId(Guid articleId);

        ValueTask<List<CommentResponseModel>> GetAll(FiltersRequest filters, OrderBy order, SortParameters parameters);

        Task Edit(Guid id, string content, string authorId);

        Task Delete(Guid id, string authorId);
    }
}