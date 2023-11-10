using System.ComponentModel.DataAnnotations;

namespace TFGames.API.Models.Request
{
    public class CommentRequestModel
    {
        [MinLength(1), MaxLength(200)]
        public string Content { get; set; }
    }
}