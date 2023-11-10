using System.Security.Claims;
using TFGames.API.Models.Response;

namespace TFGames.API.Services.Interfaces
{
    public interface IJwtService
    {
        Task<TokenModel> GenerateToken(string userId, string email);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
