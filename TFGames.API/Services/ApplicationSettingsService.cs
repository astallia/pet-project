using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Errors.Model;
using TFGames.API.Mappers;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Configurations;
using TFGames.Common.Constants;
using TFGames.DAL.Data.Repository;

namespace TFGames.API.Services
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private const string CacheKey = "key";

        private readonly IApplicationSettingsRepository _applicationSettingsRepository;

        private readonly IMemoryCache _memoryCache;

        public ApplicationSettingsService(IApplicationSettingsRepository applicationSettingsRepository, IMemoryCache memoryCache) 
        {
            _applicationSettingsRepository = applicationSettingsRepository;
            _memoryCache = memoryCache;
        }

        public async ValueTask<ApplicationSettingsResponse> Get()
        {
            ApplicationSettingsResponse applicationSettingsResponse;

            var isCached = _memoryCache.TryGetValue(CacheKey, out applicationSettingsResponse);

            if (!isCached)
            {
                var applicationSettings = await _applicationSettingsRepository.Get();

                if (applicationSettings == null)
                {
                    throw new NotFoundException(ErrorMessages.NotFound);
                }

                applicationSettingsResponse = ApplicationSettingsMapper.Map(applicationSettings);

                _memoryCache.Set(CacheKey, applicationSettingsResponse, new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            }

            return applicationSettingsResponse;
        }

        public async Task Upsert(ApplicationSettingsRequest applicationSettingsRequest)
        {
            var applicationSettings = ApplicationSettingsMapper.Map(applicationSettingsRequest);

            await _applicationSettingsRepository.Upsert(applicationSettings);

            var isCached = _memoryCache.TryGetValue(CacheKey, out ApplicationSettingsResponse applicationSettingsResponse);

            if (isCached)
            {
                applicationSettingsResponse.DbProvider = applicationSettingsRequest.DbProvider;
                applicationSettingsResponse.UseCache = applicationSettingsRequest.UseCache;
                applicationSettingsResponse.UseBlob = applicationSettingsRequest.UseBlob;
                applicationSettingsResponse.CompressImages = applicationSettingsRequest.CompressImages;
            }
        }
    }
}
