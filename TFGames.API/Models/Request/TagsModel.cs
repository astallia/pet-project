using System.ComponentModel.DataAnnotations;
using TFGames.Common.Constants;

namespace TFGames.API.Models.Request
{
    public class TagsModel
    {
        [RegularExpression(ArticleRegulars.Tags, ErrorMessage = ErrorMessages.WrongTagsName)]
        public string Name { get; set; }
    }
}
