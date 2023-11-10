using SendGrid;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.Common.Enums;

namespace TFGames.API.Services.Interfaces
{
    public interface IUserService
    {
        ValueTask<UserResponseModel> CreateAsync(UserRequestModel userModel, string role);

        ValueTask<UserListResponseModel> GetAll(int page, int size);

        ValueTask<UserResponseModel> GetByIdAsync(string id);

        Task Update(string id, UpdateUserRequestModel model);

        Task UpdateAvatar(string id, UpdateAvatarModel models);

        Task UpdateRangeRoles(List<UpdateRoleRequestModel> models);

        Task DeleteAsync(string id);

        ValueTask<Response> SendConfirmationEmailAsync(string toEmail);

        Task ConfirmRegistrationAsync(string id, string token);

        ValueTask<Response> SendResetPasswordToken(string toEmail);

        Task ResetPassword(string id, ResetPasswordModel model);

        ValueTask<bool> IsUserExist(string email);
    }
}