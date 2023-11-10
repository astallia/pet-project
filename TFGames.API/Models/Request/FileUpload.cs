using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TFGames.API.Models.Request
{
    public class FileUpload
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
