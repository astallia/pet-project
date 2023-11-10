using TFGames.DAL.Entities;
using TFGames.Common.Enums;
using TFGames.DAL.Search;

namespace TFGames.DAL.Data.Repository
{
    public interface IArticleRepository
    {
        ValueTask<(List<Article> Articles, int Count)> GetPage(Filters filters, OrderByProperties orderBy, int page, int size, Guid articleId, FindBy? find);

        ValueTask<List<User>> GetAllAuthors(Filters filters, OrderByProperties orderBy);

        Task PutLike(User user, Article article);

        Task SaveChanges();

        Task Create(Article article);

        Task Delete(Guid articleId);

        Task SaveImage(ArticleImage image);

        Task<Dictionary<Guid, ArticleImage>> GetImages(params Guid[] ids);

        Task Update(Article article);

        ValueTask<Article> FindByIdWithInclude(Guid id);

        ValueTask<List<User>> GetTopAuthors(int size);
    }
}
