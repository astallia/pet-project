namespace TFGames.DAL.Entities
{
    public class Avatar
    {
        public Guid Id { get; set; }

        public byte[] Image { get; set; }

        public string ContentType { get; set; }
    }
}