using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.Common.Exceptions;
using TFGames.DAL.Data.Repository;

namespace TFGames.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _repository;

        private readonly SignInManager<User> _signInManager;

        private readonly IJwtService _jwtService;

        public AccountService(IUserRepository userRepository, IJwtService jwtService, SignInManager<User> signInManager)
        {
            _repository = userRepository;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<LoginUserModel> Login(LoginRequestModel loginUser)
        {
            User user = null;

            if (Regex.IsMatch(loginUser.Email, AccountRegulars.Username))
            {
                user = await _repository.FindByNameAsync(loginUser.Email);
            }
            else if (Regex.IsMatch(loginUser.Email, AccountRegulars.EmailDomain))
            {
                user = await _repository.FindByEmailAsync(loginUser.Email);
            }

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.NotFound);
            }

            if (!user.EmailConfirmed)
            {
                throw new ForbiddenException(ErrorMessages.NotConfirmedEmail);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);

            if (!result.Succeeded)
            {
                throw new BadRequestException(ErrorMessages.InvalidPassword);
            }

            var token = await _jwtService.GenerateToken(user.Id, user.Email);

            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpiryDate = token.AccessTokenExpireDate;

            await _repository.UpdateAsync(user);

            return new LoginUserModel
            {
                RefreshToken = user.RefreshToken,
                AccessToken = token.AccessToken
            };
        }

        public async Task<TokenModel> RefreshToken(TokenRequestModel tokenModel)
        {
            if (tokenModel is null)
            {
                throw new BadRequestException(ErrorMessages.InvalidRequest);
            }

            var principal = _jwtService.GetPrincipalFromExpiredToken(tokenModel.RefreshToken);

            if (principal == null)
            {
                throw new BadRequestException(ErrorMessages.InvalidTokens);
            }

            var userId = principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;

            var user = await _repository.FindByIdAsync(userId);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken)
            {
                throw new BadRequestException(ErrorMessages.InvalidTokens);
            }

            if (user.RefreshTokenExpiryDate <= DateTime.UtcNow)
            {
                throw new BadRequestException(ErrorMessages.TokenExpired);
            }

            var tokens = await _jwtService.GenerateToken(user.Id, user.Email);

            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiryDate = tokens.RefreshTokenExpireDate;

            await _repository.UpdateAsync(user);

            return new TokenModel
            {
                RefreshToken = tokens.RefreshToken,
                AccessToken = tokens.AccessToken,
                AccessTokenExpireDate = tokens.AccessTokenExpireDate,
                RefreshTokenExpireDate = tokens.RefreshTokenExpireDate,
            };
        }
    }
}