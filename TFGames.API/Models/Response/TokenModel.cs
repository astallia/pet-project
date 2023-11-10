namespace TFGames.API.Models.Response
{
    public class TokenModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime AccessTokenExpireDate { get; set; }

        public DateTime RefreshTokenExpireDate { get; set; }
    }
}
