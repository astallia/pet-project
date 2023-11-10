using TFGames.API.Models.Request;

namespace TFGames.API.Models.Response
{
    public class ArticleAuthorResponseModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<TagResponseModel> Tags { get; set; }
    }
}
