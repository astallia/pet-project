using TFGames.Common.Enums;

namespace TFGames.API.Models.Request
{
    public class ApplicationSettingsRequest
    {
        public DbProvider DbProvider { get; set; }

        public bool UseBlob { get; set; }

        public bool UseCache { get; set; }

        public bool CompressImages { get; set; }
    }
}