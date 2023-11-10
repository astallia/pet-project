using TFGames.API.Models.Request;
using TFGames.API.Models.Response;

namespace TFGames.API.Services.Interfaces
{
    public interface IImageService
    {
        byte[] CompressImage(byte[] image, string contentType);

        Task<UrlResponseModel> SaveContentImage(FileUpload file);

        Task<Dictionary<Guid, ImageResponse>> GetContentImage(params Guid[] ids);
    }
}
