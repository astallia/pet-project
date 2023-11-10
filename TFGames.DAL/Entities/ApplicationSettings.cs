using TFGames.Common.Enums;

namespace TFGames.DAL.Entities
{
    public class ApplicationSettings
    {
        public Guid Id { get; set; }

        public DbProvider DbProvider { get; set; }

        public bool UseBlob { get; set; }

        public bool UseCache { get; set; }

        public bool CompressImages { get; set; }
    }
}
