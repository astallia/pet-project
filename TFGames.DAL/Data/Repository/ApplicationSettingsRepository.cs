using Microsoft.EntityFrameworkCore;
using TFGames.DAL.Entities;

namespace TFGames.DAL.Data.Repository
{
    public class ApplicationSettingsRepository : IApplicationSettingsRepository
    {
        private readonly DataContext _context;
        public ApplicationSettingsRepository(DataContext context) 
        { 
            _context = context;
        }

        public async Task Upsert(ApplicationSettings applicationSettings)
        {
            var entity = await Get();

            entity.DbProvider = applicationSettings.DbProvider;
            entity.UseBlob = applicationSettings.UseBlob;
            entity.UseCache = applicationSettings.UseCache;
            entity.CompressImages = applicationSettings.CompressImages;

            _context.Update(entity);

            await _context.SaveChangesAsync();
        }
        
        public async ValueTask<ApplicationSettings> Get()
        {
            var result = await _context.ApplicationSettings
                .SingleOrDefaultAsync();

            return result;
        }
    }
}
