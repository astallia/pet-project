using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.DAL.Entities;

namespace TFGames.API.Mappers
{
    public static class ApplicationSettingsMapper
    {
        public static ApplicationSettingsResponse Map(ApplicationSettings applicationSettings)
        {
            return new ApplicationSettingsResponse
            {
                Id = applicationSettings.Id,
                DbProvider = applicationSettings.DbProvider,
                CompressImages = applicationSettings.CompressImages,
                UseBlob = applicationSettings.UseBlob,
                UseCache = applicationSettings.UseCache
            };
        }

        public static ApplicationSettings Map(ApplicationSettingsRequest applicationSettingsRequest)
        {
            return new ApplicationSettings
            {
                DbProvider = applicationSettingsRequest.DbProvider,
                CompressImages = applicationSettingsRequest.CompressImages,
                UseBlob = applicationSettingsRequest.UseBlob,
                UseCache = applicationSettingsRequest.UseCache
            };
        }
    }
}
