using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SendGrid;
using TFGames.API.Mappers;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.Common.Exceptions;
using TFGames.DAL.Data.Repository;
using TFGames.Common.Enums;

namespace TFGames.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        private readonly IEmailService _emailService;

        private readonly UserManager<User> _userManager;

        private readonly IImageService _imageService;

        private readonly Domains _domains;

        public UserService(IUserRepository repository, IEmailService emailService, UserManager<User> userManager, IOptions<Domains> domains, IImageService imageService)
        {
            _repository = repository;
            _emailService = emailService;
            _userManager = userManager;
            _domains = domains.Value;
            _imageService = imageService;
        }

        public async ValueTask<UserResponseModel> CreateAsync(UserRequestModel userModel, string role)
        {
            var user = await _repository.FindByEmailAsync(userModel.Email);

            if (user != null)
            {
                throw new BadRequestException(ErrorMessages.UserExist);
            }

            var existedUser = await _repository.FindByNameAsync(userModel.UserName);

            if (existedUser != null)
            {
                throw new BadRequestException(ErrorMessages.UserExist);
            }

            if (userModel.Password != userModel.RepeatPassword)
            {
                throw new BadRequestException(ErrorMessages.DifferentPassword);
            }

            user = UserMapper.Map(userModel);

            await _repository.CreateAsync(user, role, userModel.Password);

            if (role == RoleNames.SuperAdmin.ToString())
            {
                user.EmailConfirmed = true;
                await _repository.UpdateAsync(user);
            }

            var responseUser = UserMapper.Map(user, role);

            return responseUser;
        }

        public async ValueTask<bool> IsUserExist(string email)
        {
            var result = await _repository.FindByEmailAsync(email);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async ValueTask<UserListResponseModel> GetAll(int page, int size)
        {
            var users = await _repository.GetAll(page, size);

            var userResponse = new List<UserResponseModel>();

            foreach (var user in users.Users)
            {
                var role = await _repository.GetRoleAsync(user);
                userResponse.Add(UserMapper.Map(user, role));
            }

            return new UserListResponseModel()
            {
                Users = userResponse,
                TotalPages = users.Count,
                CurrentPage = page
            };
        }

        public async ValueTask<UserResponseModel> GetByIdAsync(string id)
        {
            var user = await _repository.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            var role = await _repository.GetRoleAsync(user);

            var responseUser = UserMapper.Map(user, role);

            return responseUser;
        }

        public async Task Update(string id, UpdateUserRequestModel model)
        {
            var user = await _repository.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            user.UserName = string.IsNullOrEmpty(model.UserName) ? user.UserName : model.UserName;
            user.Name = string.IsNullOrEmpty(model.Name) ? user.Name : model.Name;
            user.Surname = string.IsNullOrEmpty(model.Surname) ? user.Surname : model.Surname;

            await _repository.UpdateAsync(user);
        }

        public async Task UpdateAvatar(string id, UpdateAvatarModel model)
        {
            var user = await _repository.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            var avatar = user.Avatar ?? new Avatar();
            avatar.Image = _imageService.CompressImage(Convert.FromBase64String(model.Image), model.ContentType);
            avatar.ContentType = model.ContentType;

            user.Avatar = avatar;

            await _repository.UpdateAsync(user);
        }

        public async Task UpdateRangeRoles(List<UpdateRoleRequestModel> models)
        {
            foreach (var model in models)
            {
                var user = await _repository.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    throw new NotFoundException(ErrorMessages.NotFound);
                }

                await _repository.UpdateRole(user, model.Role);
            }
        }

        public async Task ResetPassword(string id, ResetPasswordModel model)
        {
            var user = await _repository.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            if (model.Password != model.RepeatPassword)
            {
                throw new BadRequestException(ErrorMessages.DifferentPassword);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException(ErrorMessages.RefreshTokenHasBeenUsed);
            }
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _repository.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            await _repository.DeleteAsync(user);
        }

        public async ValueTask<Response> SendConfirmationEmailAsync(string toEmail)
        {
            var toUser = await _repository.FindByEmailAsync(toEmail);

            if (toUser == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(toUser);
            confirmToken = HttpUtility.UrlEncode(confirmToken);

            var emailBody = string.Format(Mail.ConfirmationTextHtml, $"{toUser.Name} {toUser.Surname}", _domains.BackEnd, toUser.Id, confirmToken);
            return await _emailService.SendEmail(Mail.FromEmail, toEmail, Mail.ConfirmationSubject, emailBody);
        }

        public async ValueTask<Response> SendResetPasswordToken(string toEmail)
        {
            var toUser = await _repository.FindByEmailAsync(toEmail);

            if (toUser == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            if (!toUser.EmailConfirmed)
            {
                throw new ForbiddenException(ErrorMessages.NotConfirmedEmail);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(toUser);

            var emailBody = string.Format(Mail.ResetPasswordTextHtml, $"{toUser.Name} {toUser.Surname}", _domains.FrontEnd, toUser.Id, resetToken);
            return await _emailService.SendEmail(Mail.FromEmail, toEmail, Mail.ResetPasswordSubject, emailBody);
        }

        public async Task ConfirmRegistrationAsync(string id, string token)
        {
            var user = await _repository.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new BadRequestException(result.Errors?.FirstOrDefault()?.Description);
            }
        }

    }
}