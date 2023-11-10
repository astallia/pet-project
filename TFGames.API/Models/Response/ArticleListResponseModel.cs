namespace TFGames.API.Models.Response
{
    public class ArticleListResponseModel
    {
        public List<ArticlePreviewResponseModel> Articles { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }
    }
}