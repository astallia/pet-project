using TFGames.DAL.Entities;
using TFGames.DAL.Search;

namespace TFGames.DAL.Data.Repository
{
    public interface ICommentRepository
    {
        ValueTask<List<Comment>> GetAll(Filters filters, OrderByProperties orderBy);

        Task<Comment> FindById(Guid id);

        Task<List<Comment>> FindAllByArticleId(Guid articleId);

        Task Create(Comment comment);

        Task Update(Comment comment);

        Task Delete(Comment comment);
    }
}