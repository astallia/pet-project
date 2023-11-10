using TFGames.DAL.Entities;
using TFGames.DAL.Search;

namespace TFGames.DAL.Data.Repository
{
    public interface IArticleTagsRepository
    {
        ValueTask<List<Tag>> GetTop(int size);

        ValueTask<List<Tag>> FindByNames(List<string> names);

        ValueTask<List<Tag>> GetAll(Filters filters, OrderByProperties orderBy);
    }
}