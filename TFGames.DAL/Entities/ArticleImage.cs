namespace TFGames.DAL.Entities
{
    public class ArticleImage
    {
        public Guid Id { get; set; }
        
        public byte[] MainImage { get; set; }

        public string ContentType { get; set; }
    }
}