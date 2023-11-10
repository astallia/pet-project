using System.ComponentModel.DataAnnotations.Schema;

namespace TFGames.DAL.Entities
{
    public class GameInfo
    {
        public Guid Id { get; set; }

        public string GameType { get; set; }

        public string Platform { get; set; }

        public int Year { get; set; }

        [ForeignKey("ArticleId")]
        public Article Article { get; set; }
    }
}