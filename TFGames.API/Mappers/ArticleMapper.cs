using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.DAL.Entities;

namespace TFGames.API.Mappers
{
    public static class ArticleMapper
    {
        public static ArticleResponseModel Map(Article article)
        {
            return new ArticleResponseModel
            {
                Id = article.Id,
                Name = article.Name,
                Author = UserMapper.Map(article.Author),
                GameType = article.GameInfo.GameType,
                Platform = article.GameInfo.Platform,
                Year = article.GameInfo.Year,
                Tags = article.Tags.ConvertAll(TagMapper.Map),
                MainImage = article.MainImage?.MainImage,
                ContentType = article.MainImage?.ContentType,
                Content = article.Content.Content,
                Comments = article.Comments?.ConvertAll(CommentMapper.Map) ?? new List<CommentResponseModel>(),
                Created = article.Created,
                Published = article.Published,
                Likes = article.Likes.Count
            };
        }

        public static ArticleResponseModel Map(Article article, ImageResponse image)
        {
            return new ArticleResponseModel
            {
                Id = article.Id,
                Name = article.Name,
                Author = UserMapper.Map(article.Author),
                GameType = article.GameInfo.GameType,
                Platform = article.GameInfo.Platform,
                Year = article.GameInfo.Year,
                Tags = article.Tags.ConvertAll(TagMapper.Map),
                MainImage = image.Image,
                ContentType = image.ContentType,
                Content = article.Content.Content,
                Comments = article.Comments?.ConvertAll(CommentMapper.Map) ?? new List<CommentResponseModel>(),
                Created = article.Created,
                Published = article.Published,
                Likes = article.Likes.Count
            };
        }

        public static Article Map(ArticleRequestModel article)
        {
            return new Article
            {
                Name = article.Name,
                GameInfo = new GameInfo() 
                { 
                    GameType = article.GameType, 
                    Platform = article.Platform, 
                    Year = article.Year
                },
                MainImage = new ArticleImage()
                {
                    MainImage = Convert.FromBase64String(article.MainImage),
                    ContentType = article.ContentType
                },
                Content = new ArticleContent() 
                { 
                    Content = article.Content 
                },
                Description = article.Description,
                Tags = article.Tags.ConvertAll(TagMapper.Map),
                Created = DateTime.UtcNow,
                Published = DateTime.UtcNow,
            };
        }
    }
}
