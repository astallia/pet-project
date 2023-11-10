using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.Common.Exceptions;
using TFGames.DAL.Data.Repository;

namespace TFGames.Tests
{
    public class AccountServiceTest
    {
        private readonly IUserRepository _userRepository;

        private readonly SignInManager<User> _signInManager;

        private readonly Mock<IJwtService> _jwtService = new();

        private readonly List<User> _users;

        private readonly LoginRequestModel loginModel = new();

        private const string WrongPassword = "WrongPassword!1";

        private const string RightPassword = "Qwerty123!";

        private readonly TokenModel tokens = new TokenModel
        {
            AccessToken = "123",
            RefreshToken = "321",
        };

        public AccountServiceTest()
        {
            _users = Get();
            _userRepository = MockUserRepository().Object;
            _signInManager = MockSignInManager().Object;
        }

        private List<User> Get()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = "12",
                    Email = "username@gmail.com",
                    UserName = "UserName",
                    EmailConfirmed = true
                }
            };

            return users;
        }

        [Theory]
        [InlineData("UserName", RightPassword)]
        [InlineData("username@gmail.com", RightPassword)]
        public async void Login_WithCorrectPasswordForExistingUser_ReturnsDataAndToken(string email, string password)
        {
            var user = _users.FirstOrDefault();

            loginModel.Email = email;
            loginModel.Password = password;

            _jwtService.Setup(x => x.GenerateToken(user.Id, user.Email))
                .Returns(Task.FromResult(tokens));

            var service = new AccountService(_userRepository, _jwtService.Object, _signInManager);

            var result = await service.Login(loginModel);

            Assert.NotNull(result);
            Assert.Equal(tokens.AccessToken, result.AccessToken);
            Assert.Equal(tokens.RefreshToken, result.RefreshToken);
        } 

        [Theory]
        [InlineData("fail@gmail.com", RightPassword)]
        [InlineData("fail@gmail.com", WrongPassword)]
        [InlineData("WrongUser", RightPassword)]
        [InlineData("WrongUser", WrongPassword)]
        public async void Login_WhileThrowsNotFoundException_Failed(string email, string password)
        {
            var expectedException = String.Format(ErrorMessages.NotFound);
            var user = _users.FirstOrDefault();

            loginModel.Email = email;
            loginModel.Password = password;

            var service = new AccountService(_userRepository, _jwtService.Object, _signInManager);

            var result = await Assert.ThrowsAsync<NotFoundException>(async () =>
             await service.Login(loginModel));

            Assert.Equal(expectedException, result.Message);
        }

        [Theory]
        [InlineData(WrongPassword)]
        public async void Login_WhileThrowsBadRequestException_Failed(string password)
        {
            var expectedException = String.Format(ErrorMessages.InvalidPassword);
            var user = _users.FirstOrDefault();

            loginModel.Email = user.Email;
            loginModel.Password = password;

            var service = new AccountService(_userRepository, _jwtService.Object, _signInManager);

            var result = await Assert.ThrowsAsync<BadRequestException>(async () =>
             await service.Login(loginModel));

            Assert.Equal(expectedException, result.Message);
        }

        private Mock<IUserRepository> MockUserRepository()
        {
            var userRepository = new Mock<IUserRepository>();

            var user = _users.Find(x => x.Id == "12");

            userRepository.Setup(x => x.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);

            userRepository.Setup(x => x.FindByNameAsync(user.UserName))
                .ReturnsAsync(user);

            return userRepository;
        }

        private Mock<SignInManager<User>> MockSignInManager()
        {
            var userManagerMock = new Mock<UserManager<User>>(
            /* IUserStore<TUser> store */Mock.Of<IUserStore<User>>(),
            /* IOptions<IdentityOptions> optionsAccessor */null,
            /* IPasswordHasher<TUser> passwordHasher */null,
            /* IEnumerable<IUserValidator<TUser>> userValidators */null,
            /* IEnumerable<IPasswordValidator<TUser>> passwordValidators */null,
            /* ILookupNormalizer keyNormalizer */null,
            /* IdentityErrorDescriber errors */null,
            /* IServiceProvider services */null,
            /* ILogger<UserManager<TUser>> logger */null);

            var signInManagerMock = new Mock<SignInManager<User>>(
            userManagerMock.Object,
            /* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
            /* IUserClaimsPrincipalFactory<TUser> claimsFactory */Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            /* IOptions<IdentityOptions> optionsAccessor */null,
            /* ILogger<SignInManager<TUser>> logger */null,
            /* IAuthenticationSchemeProvider schemes */null,
            /* IUserConfirmation<TUser> confirmation */null);

            var user = _users.Find(x => x.Id == "12");

            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, RightPassword, false))
                .ReturnsAsync(SignInResult.Success);

            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, WrongPassword, false))
                .ReturnsAsync(SignInResult.Failed);

            return signInManagerMock;
        }
    }
}