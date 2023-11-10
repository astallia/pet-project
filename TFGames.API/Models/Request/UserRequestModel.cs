using System.ComponentModel.DataAnnotations;
using TFGames.Common.Constants;

namespace TFGames.API.Models.Request
{
    public class UserRequestModel
    {
        [Required]
        [RegularExpression(AccountRegulars.Username)]
        public string UserName { get; set; }

        [RegularExpression(AccountRegulars.Name, ErrorMessage = ErrorMessages.InappropriateNameFormat)]
        public string Name { get; set; }

        [RegularExpression(AccountRegulars.Name, ErrorMessage = ErrorMessages.InappropriateNameFormat)]
        public string Surname { get; set; }

        [Required]
        [RegularExpression(AccountRegulars.EmailDomain, ErrorMessage = ErrorMessages.InappropriateEmailFormat)]
        public string Email { get; set; }

        [Required]
        [RegularExpression(AccountRegulars.Password, ErrorMessage = ErrorMessages.IncorrectPassword)]
        public string Password { get; set; }

        [Required]
        public string RepeatPassword { get; set; }
    }
}