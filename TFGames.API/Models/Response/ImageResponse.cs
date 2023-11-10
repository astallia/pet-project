namespace TFGames.API.Models.Response
{
    public class ImageResponse
    {
        public Guid Id { get; set; }

        public byte[] Image { get; set; }

        public string ContentType { get; set; }
    }
}