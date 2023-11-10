using TFGames.DAL.Entities;
using TFGames.Common.Enums;

namespace TFGames.DAL.Data.Repository
{
    public interface IUserRepository
    {
        Task CreateAsync(User userDb, string role, string password);

        Task DeleteAsync(User userDb);

        Task UpdateAsync(User userDb);

        Task UpdateRole(User user, RoleNames role);

        ValueTask<User> FindByIdAsync(string id);

        ValueTask<User> FindByEmailAsync(string email);

        ValueTask<User> FindByNameAsync(string username);

        ValueTask<(List<User> Users, int Count)> GetAll(int page, int size);

        ValueTask<string> GetRoleAsync(User userDb);
    }
}
