using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.DAL.Entities;

namespace TFGames.API.Mappers
{
    public static class TagMapper
    {
        public static TagResponseModel Map(Tag tag)
        {
            return new TagResponseModel
            {
                Name = tag.Name,
                Posts = tag.Articles.Count
            };
        }

        public static Tag Map(TagsModel tag)
        {
            return new Tag
            {
                Name = tag.Name
            };
        }
    }
}