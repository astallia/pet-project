using TFGames.Common.Enums;
using TFGames.API.Models.Request;
using TFGames.API.Services.Interfaces;

namespace TFGames.API.Helpers
{
    public class SuperAdminInitializer
    {
        public static async Task Initialize(IUserService userService)
        {
            var dto = new UserRequestModel
            {
                Name = "Admin",
                Surname = "Admin",
                Email = "admin@gmail.com",
                UserName = "admin",
                Password = "AdminPass23!",
                RepeatPassword = "AdminPass23!"
            };

            var isUserExist = await userService.IsUserExist(dto.Email);

            if (isUserExist)
            {
                return;
            }

            await userService.CreateAsync(dto, RoleNames.SuperAdmin.ToString());
        }
    }
}