namespace TFGames.DAL.Entities
{
    public class Article
    {
        public Guid Id { get; set; }
            
        public string Name { get; set; }

        public User Author { get; set; }

        public GameInfo GameInfo { get; set; }

        public List<Tag> Tags { get; set; }

        public Guid MainImageId { get; set; }
        
        public ArticleImage MainImage { get; set; }

        public ArticleContent Content { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime Published { get; set; }

        public List<Comment> Comments { get; set; }

        public List<User> Likes { get; set; } = new();
    }
}
