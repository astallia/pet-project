using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.Common.Enums;

namespace TFGames.API.Services.Interfaces
{
    public interface IArticleService
    {
        Task<ArticleResponseModel> Create(string authroId, ArticleRequestModel articleRequest);

        ValueTask<ArticleListResponseModel> GetAll(FiltersRequest filters, OrderBy order, SortParameters parameters, int page, int size, string userId, FindBy find);

        ValueTask<List<AuthorResponseModel>> GetAllAuthors(FiltersRequest filters, OrderBy order, SortParameters parameters);

        ValueTask<List<TagResponseModel>> GetAllTags(FiltersRequest filters, OrderBy order, SortParameters parameters);

        ValueTask<List<AuthorResponseModel>> GetTopAuthors(int size);

        ValueTask<List<TagResponseModel>> GetTopTags(int size);

        Task LikeBy(Guid id, string userId);

        ValueTask<ArticleResponseModel> GetById(Guid id, string userId);

        Task Delete(Guid articleId, string userId);

        Task Update(ArticleRequestModel articleRequest, Guid articleId, string userId);
    }
}
