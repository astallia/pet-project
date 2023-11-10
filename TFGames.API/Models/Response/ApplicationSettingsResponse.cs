using TFGames.Common.Enums;

namespace TFGames.API.Models.Response
{
    public class ApplicationSettingsResponse
    {
        public Guid Id { get; set; }

        public DbProvider DbProvider { get; set; }

        public bool UseBlob { get; set; }

        public bool UseCache { get; set; }

        public bool CompressImages { get; set; }
    }
}