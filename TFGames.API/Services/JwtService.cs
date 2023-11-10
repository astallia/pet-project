using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Configurations;
using TFGames.Common.Constants;
using TFGames.Common.Exceptions;
using TFGames.DAL.Data.Repository;

namespace TFGames.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly IUserRepository _repository;

        private readonly JwtTokenSettings _jwtTokenSettings;

        public JwtService(IUserRepository repositoryBaseContext, IOptions<JwtTokenSettings> jwtTokenSettings)
        {
            _repository = repositoryBaseContext;
            _jwtTokenSettings = jwtTokenSettings.Value;
        }

        public async Task<List<Claim>> GetClaims(string userId, string email, bool isRefreshToken)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("email", email),
                new Claim("isRefresh", isRefreshToken.ToString())
            };

            var user = await _repository.FindByEmailAsync(email);

            var userRole = await _repository.GetRoleAsync(user);

            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole));

            return await Task.FromResult(claims);
        }

        public Task<TokenModel> GenerateToken(string userId, string email)
        {
            var claims = GetClaims(userId, email, false);

            var accessToken = CreateToken(claims.Result, _jwtTokenSettings.AccessTokenLifetime);

            claims = GetClaims(userId, email, true);

            var refreshTolen = CreateToken(claims.Result, _jwtTokenSettings.RefreshTokenLifetime);

            var now = DateTime.UtcNow;

            var tokenModel = new TokenModel()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshTolen),
                AccessTokenExpireDate = now.AddHours(_jwtTokenSettings.AccessTokenLifetime),
                RefreshTokenExpireDate = now.AddHours(_jwtTokenSettings.RefreshTokenLifetime)
            };

            return Task.FromResult(tokenModel);
        }

        public JwtSecurityToken CreateToken(List<Claim> claims, int lifetime)
        {
            return new JwtSecurityToken(
                _jwtTokenSettings.ValidIssuer,
                _jwtTokenSettings.ValidAudience,
                expires: DateTime.UtcNow.AddMinutes(lifetime),
                claims: claims,
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(_jwtTokenSettings.SymmetricSecurityKey), SecurityAlgorithms.HmacSha256)
            );
        }

        public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtTokenSettings.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtTokenSettings.ValidAudience,
                ValidateIssuerSigningKey = true,
                
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_jwtTokenSettings.SymmetricSecurityKey)
            ),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            if (CheckTokenIsValid(token) is false) 
            {
                throw new BadRequestException(ErrorMessages.TokenExpired);
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public static long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            return ticks;
        }

        public static bool CheckTokenIsValid(string token)
        {
            var tokenTicks = GetTokenExpirationTime(token);
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

            var now = DateTime.UtcNow;

            var valid = tokenDate >= now;

            return valid;
        }
    }
}
