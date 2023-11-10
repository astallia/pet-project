using System.ComponentModel.DataAnnotations;
using TFGames.Common.Constants;

namespace TFGames.API.Models.Request
{
    public class LoginRequestModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = ErrorMessages.PasswordLength), MaxLength(50)]
        public string Password { get; set; }
    }
}
