using Microsoft.AspNetCore.Identity;

namespace TFGames.DAL.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public Avatar Avatar { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryDate { get; set; }

        public List<Article> Favorites { get; set; } = new();
    }
}