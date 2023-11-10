namespace TFGames.DAL.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Article> Articles { get; set; }
    }
}