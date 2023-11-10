using System.ComponentModel.DataAnnotations;
using TFGames.Common.Constants;

namespace TFGames.API.Models.Request
{
    
    public class ResetPasswordModel
    {
        [Required]
        public string ResetToken { get; set; }

        [Required]
        [RegularExpression(AccountRegulars.Password, ErrorMessage = ErrorMessages.IncorrectPassword)]
        public string Password { get; set; }

        [Required]
        public string RepeatPassword { get; set; }
    }
}