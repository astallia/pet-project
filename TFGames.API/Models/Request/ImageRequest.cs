using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TFGames.API.Models.Request
{
    public class ImageRequest
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public Guid Id { get; set; }
    }
}
