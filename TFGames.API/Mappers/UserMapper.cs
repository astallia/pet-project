using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.DAL.Entities;

namespace TFGames.API.Mappers
{
    public static class UserMapper
    {
        public static User Map(UserRequestModel dto)
        {
            return new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                UserName = dto.UserName
            };
        }

        public static UserResponseModel Map(User user, string role)
        {
            
            return new UserResponseModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                Role = role,
                Avatar = user.Avatar != null ? ImageMapper.Map(user.Avatar) : null
            };
        }

        public static AuthorResponseModel Map(User user)
        {
            return new AuthorResponseModel
            {
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                Avatar = user.Avatar != null ? ImageMapper.Map(user.Avatar) : null
            };
        }
    }
}