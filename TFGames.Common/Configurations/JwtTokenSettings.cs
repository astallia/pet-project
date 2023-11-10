namespace TFGames.Common.Configurations
{
    public class JwtTokenSettings
    {
        public string ValidIssuer { get; set; }

        public string ValidAudience { get; set; }

        public string SymmetricSecurityKey { get; set; }

        public int AccessTokenLifetime { get; set; }

        public int RefreshTokenLifetime { get; set; }
    }
}
