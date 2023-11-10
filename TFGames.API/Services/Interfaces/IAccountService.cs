using TFGames.API.Models.Request;
using TFGames.API.Models.Response;

namespace TFGames.API.Services.Interfaces
{
    public interface IAccountService
    {
        Task<LoginUserModel> Login(LoginRequestModel loginUser);

        Task<TokenModel> RefreshToken(TokenRequestModel tokenModel);
    }
}