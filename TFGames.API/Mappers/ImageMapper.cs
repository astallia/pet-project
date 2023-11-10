using TFGames.API.Models.Response;
using TFGames.DAL.Entities;

namespace TFGames.API.Mappers
{
    public static class ImageMapper
    {
        public static Avatar Map(ImageResponse model)
        {
            return new Avatar 
            {
                Image = model.Image,
                ContentType = model.ContentType
            };
        }

        public static ImageResponse Map(Avatar avatar)
        {
            return new ImageResponse
            {
                Id = avatar.Id,
                Image = avatar.Image,
                ContentType = avatar.ContentType
            };
        }

        public static ImageResponse Map(ArticleImage image)
        {
            return new ImageResponse
            {
                Id = image.Id,
                Image = image.MainImage,
                ContentType = image.ContentType
            };
        }
    }
}