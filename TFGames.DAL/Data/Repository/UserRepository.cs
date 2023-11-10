using Microsoft.AspNetCore.Identity;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.Common.Enums;
using TFGames.Common.Exceptions;
using TFGames.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace TFGames.DAL.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task CreateAsync(User userDb, string role, string password)
        {
            var result = await _userManager.CreateAsync(userDb, password);

            if (!result.Succeeded)
            {
                throw new ConflictException(ErrorMessages.Conflict);
            }

            result = await _userManager.AddToRoleAsync(userDb, role);

            if (!result.Succeeded)
            {
                throw new ConflictException(result.Errors?.FirstOrDefault()?.Description);
            }
        }

        public async Task DeleteAsync(User userDb)
        {
            await _context.Entry(userDb).Reference(u => u.Avatar).LoadAsync();

            if (userDb.Avatar != null)
            {
                _context.Avatars.Remove(userDb.Avatar);
                await _context.SaveChangesAsync();
            }   

            var comments = await _context.Comments
                .Where(c => c.Author.Id == userDb.Id)
                .ToListAsync();

            foreach (var comment in comments)
            {
                _context.Comments.Remove(comment);
            }

            var result = await _userManager.DeleteAsync(userDb);

            if (!result.Succeeded)
            {
                throw new ConflictException(result.Errors?.FirstOrDefault()?.Description);
            }
        }

        public async Task UpdateAsync(User userDb)
        {
            var result = await _userManager.UpdateAsync(userDb);

            if (!result.Succeeded)
            {
                throw new ConflictException(result.Errors?.FirstOrDefault()?.Description);
            }
        }

        public async ValueTask<User> FindByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
                await _context.Entry<User>(user).Reference(u => u.Avatar).LoadAsync();

            return user;
        }

        public async ValueTask<User> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
                await _context.Entry<User>(user).Reference(u => u.Avatar).LoadAsync();

            return user;
        }

        public async ValueTask<User> FindByNameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
                await _context.Entry<User>(user).Reference(u => u.Avatar).LoadAsync();

            return user;
        }

        public async ValueTask<(List<User> Users, int Count)> GetAll(int page, int size)
        {            
            var users = _userManager.Users
                .Page(page, size)
                .ToList();

            foreach (var user in users)
            {
                await _context.Entry<User>(user).Reference(u => u.Avatar).LoadAsync();
            }

            int totalCount = (int) Math.Ceiling(_userManager.Users.Count() / (double) size);

            return (users, totalCount);
        }

        public async Task UpdateRole(User user, RoleNames role)
        {
            var userRole = await GetRoleAsync(user);

            if (userRole != null)
            {
                if (userRole.Equals(role))
                {
                    throw new ConflictException(ErrorMessages.RoleAlreadyAssigned);
                }

                await _userManager.RemoveFromRoleAsync(user, userRole);
            }

            await _userManager.AddToRoleAsync(user, role.ToString());
        }

        public async ValueTask<string> GetRoleAsync(User userDb)
        {
            var roles = await _userManager.GetRolesAsync(userDb);
            var role = roles.FirstOrDefault();
            return role;
        }
    }
}
