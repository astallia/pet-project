using TFGames.API.Models.Request;
using TFGames.DAL.Entities;
using TFGames.DAL.Search;

namespace TFGames.API.Mappers
{
    public static class FilterMapper
    {
        public static Filters Map(FiltersRequest filters, User user=null)
        {
            return new Filters
            {
                Search = filters.Search,
                User = user
            };
        }

        public static OrderByProperties Map(OrderByRequest orderBy)
        {
            return new OrderByProperties
            {
                Order = orderBy.Order,
                SortParameters = orderBy.SortParameters
            };
        }

    }
}
