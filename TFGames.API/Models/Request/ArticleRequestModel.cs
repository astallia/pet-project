using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TFGames.Common.Constants;

namespace TFGames.API.Models.Request
{
    public class ArticleRequestModel
    {
        [Required]
        [MinLength(1), MaxLength(100)]
        public string Name { get; set; }

        [MinLength(1), MaxLength(100)]
        public string GameType { get; set; }

        [MinLength(1), MaxLength(100)]
        public string Platform { get; set; }

        public int Year { get; set; }

        [Required]
        public string MainImage { get; set; }

        [Required]
        public string ContentType { get; set; }

        public List<TagsModel> Tags { get; set; } = new();

        [Required]
        [MinLength(1), MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
