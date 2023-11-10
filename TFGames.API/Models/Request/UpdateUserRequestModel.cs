using System.ComponentModel.DataAnnotations;
using TFGames.Common.Constants;

namespace TFGames.API.Models.Request
{
    public class UpdateUserRequestModel
    {
        [RegularExpression(AccountRegulars.Username)]
        public string UserName { get; set; }

        [RegularExpression(AccountRegulars.Name, ErrorMessage = ErrorMessages.InappropriateNameFormat)]
        public string Name { get; set; }

        [RegularExpression(AccountRegulars.Name, ErrorMessage = ErrorMessages.InappropriateNameFormat)]
        public string Surname { get; set; }
    }
}