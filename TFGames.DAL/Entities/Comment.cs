namespace TFGames.DAL.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public Comment Parent { get; set; }

        public User Author { get; set; }

        public Article Article { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Comment> Replies { get; set; }

        public bool IsDeleted { get; set; }
    }
}