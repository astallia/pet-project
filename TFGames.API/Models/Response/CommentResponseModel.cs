namespace TFGames.API.Models.Response
{
    public class CommentResponseModel
    {
        public Guid Id { get; set; }

        public string ArticleId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public AuthorResponseModel Author { get; set; }

        public ICollection<CommentResponseModel> Replies { get; set; }
    }
}
