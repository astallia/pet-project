using Microsoft.AspNetCore.Identity;
using TFGames.Common.Enums;

namespace TFGames.API.Helpers
{
    public static class RoleInitializer
    {
        public static async Task Initialize(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in Enum.GetValues(typeof(RoleNames)))
            {
                await roleManager.CreateAsync(
                    new IdentityRole
                    {
                        Name = roleName.ToString()
                    }
                );
            }
        }
    }
}
