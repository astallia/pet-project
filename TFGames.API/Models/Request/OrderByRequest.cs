using TFGames.Common.Enums;

namespace TFGames.API.Models.Request
{
    public class OrderByRequest
    {
        public OrderBy Order { get; set; }

        public SortParameters SortParameters { get; set; }
    }
}
