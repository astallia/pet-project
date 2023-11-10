using AutoMapper;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.DAL.Entities;

namespace TFGames.API.Mappers
{
    public class ArticleAutoMapper : Profile
    {
        public ArticleAutoMapper()
        {
            this.CreateMap<Article, ArticlePreviewResponseModel>()
                .ForMember(x => x.Author, y => y.MapFrom(z => UserMapper.Map(z.Author)))
                .ForMember(x => x.GameType, y => y.MapFrom(z => z.GameInfo.GameType))
                .ForMember(x => x.Year, y => y.MapFrom(z => z.GameInfo.Year))
                .ForMember(x => x.Platform, y => y.MapFrom(z => z.GameInfo.Platform))
                .ForMember(x => x.Tags, y => y.MapFrom(z => z.Tags.Select(t => new TagResponseModel { Name = t.Name, Posts = t.Articles.Count})))
                .ForMember(x => x.Likes, y => y.MapFrom(z => z.Likes.Count))
                .ForMember(x => x.MainImage, y => y.MapFrom(z => z.MainImage.MainImage))
                .ForMember(x => x.ContentType, y => y.MapFrom(z => z.MainImage.ContentType));

            this.CreateMap<ArticleRequestModel, Article>()
                .ForPath(x => x.GameInfo.GameType, y => y.MapFrom(z => z.GameType))
                .ForPath(x => x.GameInfo.Year, y => y.MapFrom(z => z.Year))
                .ForPath(x => x.GameInfo.Platform, y => y.MapFrom(z => z.Platform))
                .ForMember(x => x.Created, y => y.MapFrom(z => DateTime.UtcNow))
                .ForMember(x => x.Published, y => y.MapFrom(z => DateTime.UtcNow))
                .ForMember(x => x.Content, y => y.MapFrom(z => new ArticleContent { Content = z.Content }))
                .ForMember(x => x.MainImage, y => y.MapFrom(z => new ArticleImage { MainImage = Convert.FromBase64String(z.MainImage), ContentType = z.ContentType }))
                .ForMember(x => x.Tags, y => y.Ignore())
                .ReverseMap();

            this.CreateMap<Article, ArticleAuthorResponseModel>()
                .ForMember(x => x.Tags, y => y.MapFrom(z => z.Tags.Select(t => new TagResponseModel { Name = t.Name, Posts = t.Articles.Count})));
        }
    }
}
