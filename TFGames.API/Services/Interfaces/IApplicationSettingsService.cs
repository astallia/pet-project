using TFGames.API.Models.Request;
using TFGames.API.Models.Response;

namespace TFGames.API.Services.Interfaces
{
    public interface IApplicationSettingsService
    {
        ValueTask<ApplicationSettingsResponse> Get();

        Task Upsert(ApplicationSettingsRequest applicationSettingsRequest);
    }
}
