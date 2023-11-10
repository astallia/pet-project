namespace TFGames.API.Models.Response
{
    public class ArticlePreviewResponseModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public AuthorResponseModel Author { get; set; }

        public string GameType { get; set; }

        public string Platform { get; set; }

        public int Year { get; set; }

        public List<TagResponseModel> Tags { get; set; } = new();

        public byte[] MainImage { get; set; }

        public string ContentType { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime Published { get; set; }

        public int Likes { get; set; }

        public bool HasCurrentUserLiked { get; set; }
    }
}
