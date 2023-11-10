namespace TFGames.API.Models.Response
{
    public class AuthorResponseModel
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public ImageResponse Avatar { get; set; }
    }
}