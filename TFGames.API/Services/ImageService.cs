using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TFGames.API.Mappers;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Configurations;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.Common.Exceptions;
using TFGames.DAL.Data.Repository;

namespace TFGames.API.Services
{
    public class ImageService : IImageService
    {
        private readonly ValidImageProperties _validImageProperties;

        private readonly IArticleRepository _articleRepository;

        private readonly IMemoryCache _memoryCache;

        private readonly Domains _domains;

        private readonly IApplicationSettingsService _applicationSettingsService;

        public ImageService(IOptions<ValidImageProperties> validImageProperties, IOptions<Domains> domains, IArticleRepository articleRepository, IMemoryCache memoryCache, IApplicationSettingsService applicationSettingsService)
        {
            _applicationSettingsService = applicationSettingsService;
            _validImageProperties = validImageProperties.Value;
            _articleRepository = articleRepository;
            _memoryCache = memoryCache;
            _domains = domains.Value;
        }

        public byte[] CompressImage(byte[] image, string contentType)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new Exception("Unsupported OS");
            }

            var imageExtension = contentType.Replace("image/", ".");

            var validExtension = Regex.Match(imageExtension, _validImageProperties.ValidExtension);

            if (!validExtension.Success)
            {
                throw new ConflictException(ErrorMessages.WrongFormat);
            }
            
            var imageStream = new MemoryStream(image);
            var resultStream = new MemoryStream();

            using (Bitmap bmp = new Bitmap(imageStream))
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                Encoder myEncoder = Encoder.Quality;

                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, _validImageProperties.CompressionQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp.Save(resultStream, jpgEncoder, myEncoderParameters);
            }

            var result = resultStream.ToArray();

            if (result.Length > _validImageProperties.ValidSize)
            {
                throw new ConflictException(ErrorMessages.WrongFileSize);
            }

            return result;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public async Task<UrlResponseModel> SaveContentImage(FileUpload file)
        {
            var image = new ArticleImage();

            var path = new UrlResponseModel();

            if (file.File.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.File.CopyTo(ms);

                    var fileBites = ms.ToArray();

                    if (fileBites.Length > _validImageProperties.ValidSize)
                    {
                        throw new ConflictException(ErrorMessages.WrongFileSize);
                    }

                    var fileExtension = Path.GetExtension(file.File.FileName);

                    var validImageExtension = fileExtension.Replace("image/", ".");

                    var applicationSettings = await _applicationSettingsService.Get();

                    if (applicationSettings.CompressImages)
                    {
                        image.MainImage = CompressImage(fileBites, validImageExtension);
                    }
                    else
                    {
                        image.MainImage = fileBites;
                    }

                    image.ContentType = "image/" + fileExtension.Replace(".", "");

                    await _articleRepository.SaveImage(image);

                    path.Url = $"{_domains.BackEnd}/articles/image/{image.Id}";
                }
            }

            return path;
        }

        public async Task<Dictionary<Guid, ImageResponse>> GetContentImage(params Guid[] ids)
        {
            var imageResponses = new Dictionary<Guid, ImageResponse>();

            var notCachedIds = new List<Guid>();

            foreach (var id in ids)
            {
                var isCached = _memoryCache.TryGetValue(id, out ArticleImage imageResponse);

                if (!isCached)
                {
                    notCachedIds.Add(id);
                }
                else if (isCached)
                {
                    imageResponses.Add(id, ImageMapper.Map(imageResponse));
                }
            }

            if (notCachedIds.Count > 0)
            {
                var images = await _articleRepository.GetImages(notCachedIds.ToArray());

                var applicationSettings = await _applicationSettingsService.Get();

                foreach (var image in images)
                {
                    if (applicationSettings.UseCache)
                    {
                        _memoryCache.Set(image.Key, image.Value, new MemoryCacheEntryOptions()
                                    .SetSize(1)
                                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
                    }

                    imageResponses.Add(image.Key, ImageMapper.Map(image.Value));
                }
            }

            return imageResponses;
        }
    }
}
