using TFGames.DAL.Entities;

namespace TFGames.DAL.Data.Repository
{
    public interface IApplicationSettingsRepository
    {
        Task Upsert(ApplicationSettings applicationSettings);

        ValueTask<ApplicationSettings> Get();
    }
}
