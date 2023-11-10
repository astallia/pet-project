using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Constants;
using TFGames.Common.Enums;

namespace TFGames.API.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly Domains _domains;

        public UserController(IUserService userService, IOptions<Domains> domains)
        {
            _userService = userService;
            _domains = domains.Value;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Get /users/
        /// 
        /// </remarks>
        /// <returns></returns>
        [Authorize(Roles = nameof(RoleNames.SuperAdmin))]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserResponseModel>))]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var usersResponse = await _userService.GetAll(page, size);
            return Ok(usersResponse);
        }


        /// <summary>
        /// Gets current authorized.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Get /users/current
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet("current")]
        [Authorize(Roles = $"{nameof(RoleNames.User)},{nameof(RoleNames.SuperAdmin)},{nameof(RoleNames.Author)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseModel))]
        public async Task<IActionResult> GetCurrent()
        {
            var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userResponse = await _userService.GetByIdAsync(id);
            return Ok(userResponse);
        }

        /// <summary>
        /// Gets user by id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Get /users/{id}
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = nameof(RoleNames.SuperAdmin))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseModel))]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var userResponse = await _userService.GetByIdAsync(id.ToString());
            return Ok(userResponse);
        }

        /// <summary>
        /// Confirms registration of user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /users/confirm/{id}?token={token}
        /// 
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("confirm/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> ConfirmRegistrationAsync([FromRoute] Guid id, [FromQuery] string token)
        {
            await _userService.ConfirmRegistrationAsync(id.ToString(), token);
            return Redirect($"{_domains.FrontEnd}/confirmation");
        }

        /// <summary>
        /// Sends Confirmation letter to email
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     POST /users/confirm?email={email}
        /// 
        /// </remarks>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("confirm")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SendConfirmationEmail([FromQuery] string email)
        {
            await _userService.SendConfirmationEmailAsync(email);
            return NoContent();
        }

        /// <summary>
        /// Creates the User and sends confirmation email.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /users/
        ///     {
        ///         "username": "string",
        ///         "name": "Uutbwhwcmpqpyg",
        ///         "surname": "Kcordtlxbvpywqfswfwedk",
        ///         "email": "user@example.com",
        ///         "password": "string",
        ///         "repeatPassword": "string"
        ///     }
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponseModel))]
        public async Task<IActionResult> CreateAndSendConfirmationEmailAsync([FromBody] UserRequestModel dto)
        {
            var userResponse = await _userService.CreateAsync(dto, RoleNames.User.ToString());
            await _userService.SendConfirmationEmailAsync(userResponse.Email);
            return Created("/accounts/", userResponse);
        }

        /// <summary>
        /// Sends Reset Password Token to specific email
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     POST /users/reset-password
        /// 
        /// </remarks>
        /// <param name="toEmail"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SendResetPasswordToken([FromQuery] string toEmail)
        {
            await _userService.SendResetPasswordToken(toEmail);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequestModel model)
        {
            var userId = User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            await _userService.Update(userId, model);
            return NoContent();
        }

        /// <summary>
        /// Updates avatar for current authorized user
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     PUT /users/{id}/avatar
        ///     {
        ///         "image": base64,
        ///         "contentType": "image/png"
        ///     }
        ///     
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("avatar")]
        [Authorize(Roles = $"{nameof(RoleNames.SuperAdmin)}, {nameof(RoleNames.Author)}, {nameof(RoleNames.User)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarModel model)
        {
            var userId = User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            await _userService.UpdateAvatar(userId, model);
            return NoContent();
        }

        /// <summary>
        /// Updates user roles.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPatch]
        [Authorize(Roles = nameof(RoleNames.SuperAdmin))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateRoles([FromBody] List<UpdateRoleRequestModel> models)
        {
            await _userService.UpdateRangeRoles(models);
            return NoContent();
        }

        /// <summary>
        /// Reset password for specific user.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     PATCH /users/reset-password/{id}
        /// 
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch("reset-password/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResetPassword([FromRoute] Guid id, [FromBody] ResetPasswordModel model)
        {
            await _userService.ResetPassword(id.ToString(), model);
            return NoContent();
        }

        /// <summary>
        /// Deletes.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /users/{id}
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(RoleNames.SuperAdmin))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            await _userService.DeleteAsync(id.ToString());
            return NoContent();
        }
    }
}