using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;

namespace TFGames.API.Controllers
{
    [ApiController]
    [Route("application_settings")]
    public class ApplicationSettingsController : ControllerBase
    {
        private readonly IApplicationSettingsService _applicationSettingsService;

        public ApplicationSettingsController(IApplicationSettingsService applicationSettingsService)
        {
            _applicationSettingsService = applicationSettingsService;
        }

        /// <summary>
        /// Get the application settings.
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     
        ///     GET /application_settings/
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationSettingsResponse))]
        public async Task<IActionResult> GetAll()
        {
            var applicationSettings = await _applicationSettingsService.Get();

            return Ok(applicationSettings);
        }

        /// <summary>
        /// Upsert the application settings.
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     
        ///     PUT /application_settings/
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationSettingsResponse))]
        public async Task<IActionResult> Upsert([FromBody] ApplicationSettingsRequest applicationSettingsRequest)
        {
            await _applicationSettingsService.Upsert(applicationSettingsRequest);

            return NoContent();
        }
    }
}
