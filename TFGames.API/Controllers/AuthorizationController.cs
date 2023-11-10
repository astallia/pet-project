using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;

namespace TFGames.API.Controllers
{
    [ApiController]
    [EnableCors]
    [Route("authorization")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthorizationController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        ///Login user.
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginUserModel))]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequest)
        {
            var userLogin = await _accountService.Login(loginRequest);

            return Ok(userLogin);
        }

        /// <summary>
        ///Refresh tokens.
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh-tokens")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenModel))]
        public async Task<IActionResult> Login([FromBody] TokenRequestModel tokenModel)
        {
            var tokens = await _accountService.RefreshToken(tokenModel);

            return Ok(tokens);
        }
    }
}
