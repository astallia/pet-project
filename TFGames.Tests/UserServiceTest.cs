using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SendGrid;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.Common.Enums;
using TFGames.Common.Exceptions;
using TFGames.DAL.Data.Repository;

namespace TFGames.Tests
{
    public class UserServiceTest
    {
        private readonly IUserRepository _userRepository;

        private readonly UserManager<User> _userManager;

        private readonly IEmailService _emailService;

        private readonly IImageService _imageService;

        private readonly IOptions<Domains> _options;

        private static readonly UserRequestModel createDto = new UserRequestModel
        {
            UserName = "Username",
            Password = "Username2!",
            RepeatPassword = "Username2!",
            Email = "username@mail.com"
        };

        private static readonly User expectedUser = new User
        {
            Email = "username@mail.com"
        };

        private static readonly UserResponseModel expectedDto = new UserResponseModel
        {
            Email = "username@mail.com"
        };

        private readonly Response okResponse = new Response(HttpStatusCode.OK, null, null);

        public UserServiceTest()
        {
            _options = Options.Create<Domains>(new Domains { BackEnd = "backend", FrontEnd = "frontend" });

            _emailService = Substitute.For<IEmailService>();
            _userRepository = Substitute.For<IUserRepository>();
            _imageService = Substitute.For<IImageService>();

            var userStore = Substitute.For<IUserStore<User>>();
            _userManager = Substitute.For<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            var emailTokenProvider = new EmailTokenProvider<User>();
            _userManager.RegisterTokenProvider(TokenOptions.DefaultProvider, emailTokenProvider);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnUser()
        {
            // Arrange
            _userRepository.FindByEmailAsync(Arg.Any<string>()).ReturnsNull();

            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action
            var actual = await userService.CreateAsync(createDto, RoleNames.User.ToString());

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expectedDto.Email, actual.Email);
            await _userRepository.Received(1).CreateAsync(Arg.Is<User>(x => x.Email == createDto.Email), Arg.Is<string>(RoleNames.User.ToString()), Arg.Is<string>(x => x == createDto.Password));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowUserExists()
        {
            // Arrange
            var expectedException = ErrorMessages.UserExist;
            _userRepository.FindByEmailAsync(Arg.Any<string>()).Returns(new User());

            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(async () => await userService.CreateAsync(createDto, RoleNames.User.ToString()));

            Assert.Equal(expectedException, exception.Message);
            await _userRepository.Received(0).CreateAsync(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Theory]
        [InlineData("Username2!", "Username2#")]
        [InlineData("Password1@", "AnotherPassword3!2")]
        public async Task CreateAsync_ShouldThrowDifferentPasswords(string password, string repeatPassword)
        {
            // Arrange
            _userRepository.FindByEmailAsync(Arg.Any<string>()).ReturnsNull();

            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            var createWrongPassDto = new UserRequestModel
            {
                Password = password,
                RepeatPassword = repeatPassword
            };

            // Action & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(async () => await userService.CreateAsync(createWrongPassDto, RoleNames.User.ToString()));
            Assert.Equal(ErrorMessages.DifferentPassword, exception.Message);
            await _userRepository.Received(0).CreateAsync(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task GetAll_SholdReturnUsers()
        {
            // Arrange
            var expected = new List<UserResponseModel> { expectedDto };

            _userRepository.GetAll(1, 10).Returns((new List<User> { expectedUser }, 1));
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action
            var actual = await userService.GetAll(1, 10);

            // Assert
            Assert.NotNull(actual);

            for (int i = 0; i < actual.Users.Count; i++)
            {
                Assert.Equal(expected[i].Email, actual.Users[i].Email);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            _userRepository.FindByIdAsync(id).Returns(expectedUser);
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action
            var actual = await userService.GetByIdAsync(id);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expectedDto.Email, actual.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowUserNotFound()
        {
            // Arrange
            var expectedException = ErrorMessages.NotFound;
            var id = Guid.NewGuid().ToString();

            _userRepository.FindByIdAsync(id).ReturnsNull();
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.GetByIdAsync(id));
            Assert.Equal(expectedException, exception.Message);
        }

        [Fact]
        public async Task UpdateImage_ShouldBeCompleted()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var user = new User { Id = id };
            var imageRequest = new UpdateAvatarModel() { Image = "", ContentType = "" };
            var image = Convert.FromBase64String(imageRequest.Image);

            _userRepository.FindByIdAsync(id).Returns(user);
            _imageService.CompressImage(image, imageRequest.ContentType).Returns(image);
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Act
            await userService.UpdateAvatar(id, imageRequest);

            // Assert
            await _userRepository.Received(1).FindByIdAsync(Arg.Is<string>(id));
            _imageService.Received(1).CompressImage(Arg.Any<byte[]>(), Arg.Any<string>());
            await _userRepository.Received(1).UpdateAsync(Arg.Is<User>(user));
        }

        [Fact]
        public async Task UpdateImage_ShouldThrowNotFound()
        {
            // Arrange
            var expectedException = ErrorMessages.NotFound;
            var id = Guid.NewGuid().ToString();
            var imageRequest = new UpdateAvatarModel();

            _userRepository.FindByIdAsync(id).ReturnsNull();

            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.UpdateAvatar(id, imageRequest));

            Assert.Equal(expectedException, exception.Message);
            await _userRepository.Received(1).FindByIdAsync(Arg.Is<string>(id));
            _imageService.Received(0).CompressImage(Arg.Any<byte[]>(), Arg.Any<string>());
            await _userRepository.Received(0).UpdateAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task UpdateRole_ShouldBeCompleted()
        {
            // Arrange
            var models = new List<UpdateRoleRequestModel>
            {
                new UpdateRoleRequestModel {
                    UserId = Guid.Empty.ToString(),
                    Role = RoleNames.Author
                },
                new UpdateRoleRequestModel {
                    UserId = Guid.Empty.ToString(),
                    Role = RoleNames.Author
                }
            };

            var user1 = new User { Id = models[0].UserId };
            var user2 = new User { Id = models[1].UserId };

            _userRepository.FindByIdAsync(models[0].UserId).Returns(user1);
            _userRepository.FindByIdAsync(models[1].UserId).Returns(user2);
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Act
            await userService.UpdateRangeRoles(models);

            // Assert
            await _userRepository.Received(2).FindByIdAsync(Arg.Any<string>());
            await _userRepository.Received(2).UpdateRole(Arg.Any<User>(), Arg.Any<RoleNames>());
        }

        [Fact]
        public async Task UpdateRole_ShouldThrowUserNotFound()
        {
            // Arrange
            var expectedException = ErrorMessages.NotFound;
            var models = new List<UpdateRoleRequestModel>() { new UpdateRoleRequestModel { UserId = Guid.Empty.ToString() } };

            _userRepository.FindByIdAsync(Arg.Any<string>()).ReturnsNull();
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.UpdateRangeRoles(models));
            Assert.Equal(expectedException, exception.Message);
        }

        [Fact]
        public void DeleteAsync_ShouldBeCompleted()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            _userRepository.FindByIdAsync(id).Returns(expectedUser);
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action
            var actual = userService.DeleteAsync(id);
            actual.Wait();

            // Assert
            Assert.True(actual.IsCompleted);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowUserNotFound()
        {
            // Arrange
            var expectedException = ErrorMessages.NotFound;
            _userRepository.FindByIdAsync(Arg.Any<string>()).ReturnsNull();
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.DeleteAsync(Arg.Any<string>()));
            Assert.Equal(expectedException, exception.Message);
        }

        [Fact]
        public async Task SendConfirmationEmailAsync_ShouldBeCompleted()
        {
            // Arrange
            var toEmail = "a.kushch9@gmail.com";
            var user = new User { Id = Guid.NewGuid().ToString(), Email = toEmail, Name = "name", Surname = "surname" };

            _userRepository.FindByEmailAsync(toEmail).Returns(user);
            _userManager.GetSecurityStampAsync(user).Returns("security stamp");
            _userManager.GenerateEmailConfirmationTokenAsync(user).Returns("token");
            _emailService.SendEmail(Mail.FromEmail, toEmail, Mail.ConfirmationSubject, string.Format(Mail.ConfirmationTextHtml, "name surname", "backend", user.Id, "token")).Returns(okResponse);

            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action
            var actual = await userService.SendConfirmationEmailAsync(toEmail);

            // Assert
            Assert.Equal(okResponse.StatusCode, actual.StatusCode);
        }

        [Fact]
        public async Task SendConfirmationEmailAsync_ShouldThrowNotFound()
        {
            // Arrange
            var toEmail = "a.kushch9@gmail.com";

            _userManager.FindByEmailAsync(toEmail).ReturnsNull();
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assertation
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.SendConfirmationEmailAsync(toEmail));
            Assert.Equal(ErrorMessages.NotFound, exception.Message);
        }

        [Fact]
        public void ConfirmRegistrationAsync_ShouldBeCompleted()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var user = new User { Id = id };
            var token = "token";

            _userRepository.FindByIdAsync(id).Returns(user);
            _userManager.ConfirmEmailAsync(user, token).Returns(IdentityResult.Success);
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action
            var actual = userService.ConfirmRegistrationAsync(id, token);
            actual.Wait();

            // Assert
            Assert.True(actual.IsCompleted);
        }

        [Fact]
        public async Task ConfirmRegistrationAsync_ShouldThrowUserNotFound()
        {
            // Arrange
            var token = "";

            var expectedException = ErrorMessages.NotFound;
            _userRepository.FindByIdAsync(Arg.Any<string>()).ReturnsNull();
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.ConfirmRegistrationAsync(Arg.Any<string>(), token));
            Assert.Equal(expectedException, exception.Message);
        }

        [Fact]
        public async Task ConfirmRegistrationAsync_ShouldThrowNotSucceededAsync()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var user = new User { Id = id };
            var token = "token";
            var expectedException = new NotFoundException();

            _userRepository.FindByIdAsync(id).Returns(user);
            _userManager.ConfirmEmailAsync(user, token).Returns(IdentityResult.Failed());
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.ConfirmRegistrationAsync(Arg.Any<string>(), token));
            Assert.Equal(expectedException.GetType(), exception.GetType());
        }

        [Fact]
        public async Task SendResetPasswordToken_ShouldBeCompleted()
        {
            // Arrange
            var toEmail = "a.kushch9@gmail.com";
            var user = new User { Id = Guid.NewGuid().ToString(), Email = toEmail, Name = "name", Surname = "surname", EmailConfirmed = true };

            _userRepository.FindByEmailAsync(toEmail).Returns(user);
            _userManager.GeneratePasswordResetTokenAsync(user).Returns("token");
            _emailService.SendEmail(Mail.FromEmail, toEmail, Mail.ResetPasswordSubject, string.Format(Mail.ResetPasswordTextHtml, "name surname", "frontend", user.Id, "token")).Returns(okResponse);

            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action
            var actual = await userService.SendResetPasswordToken(toEmail);

            // Assert
            Assert.Equal(okResponse.StatusCode, actual.StatusCode);
            await _emailService.Received(1).SendEmail(Mail.FromEmail, toEmail, Mail.ResetPasswordSubject, string.Format(Mail.ResetPasswordTextHtml, "name surname", "frontend", user.Id, "token"));
        }

        [Fact]
        public async Task SendResetPasswordToken_ShouldThrowNotFound()
        {
            // Arrange
            var toEmail = "a.kushch9@gmail.com";

            _userManager.FindByEmailAsync(toEmail).ReturnsNull();
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assertation
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.SendResetPasswordToken(toEmail));
            Assert.Equal(ErrorMessages.NotFound, exception.Message);
            await _emailService.Received(0).SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task ResetPassword_ShouldBeCompleted()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var user = new User { Id = id };
            var model = new ResetPasswordModel
            {
                ResetToken = "token",
                Password = "Password1@",
                RepeatPassword = "Password1@"
            };

            _userRepository.FindByIdAsync(id).Returns(user);
            _userManager.ResetPasswordAsync(user, model.ResetToken, model.Password).Returns(IdentityResult.Success);
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Act
            await userService.ResetPassword(id, model);

            // Assert
            await _userManager.Received(1).ResetPasswordAsync(Arg.Is<User>(x => x.Id == user.Id), Arg.Is<string>(model.ResetToken), Arg.Is<string>(model.Password));
        }

        [Fact]
        public async Task ResetPassword_ShouldThrowNotFound()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var token = "token";
            var model = new ResetPasswordModel();

            var expectedException = ErrorMessages.NotFound;
            _userRepository.FindByIdAsync(token).ReturnsNull();
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Action & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await userService.ResetPassword(id, model));
            Assert.Equal(expectedException, exception.Message);
            await _userManager.Received(0).ResetPasswordAsync(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task ResetPassword_ShouldThrowDifferentPassword()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var user = new User { Id = id };
            var model = new ResetPasswordModel
            {
                ResetToken = "token",
                Password = "Password1@",
                RepeatPassword = "Password2@"
            };

            var expectedException = ErrorMessages.DifferentPassword;
            _userRepository.FindByIdAsync(id).Returns(user);
            var userService = new UserService(_userRepository, _emailService, _userManager, _options, _imageService);

            // Act
            var exception = await Assert.ThrowsAsync<BadRequestException>(async () => await userService.ResetPassword(id, model));
            Assert.Equal(expectedException, exception.Message);

            await _userManager.Received(0).ResetPasswordAsync(Arg.Is<User>(x => x.Id == user.Id), Arg.Is<string>(model.ResetToken), Arg.Is<string>(model.Password));
        }
    }
}