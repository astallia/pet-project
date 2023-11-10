using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Constants;
using TFGames.Common.Enums;
using TFGames.DAL.Search;

namespace TFGames.API.Controllers
{
    [ApiController]
    [Route("articles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{nameof(RoleNames.SuperAdmin)},{nameof(RoleNames.Author)}, {nameof(RoleNames.User)}")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        private readonly IImageService _imageService;

        public ArticleController(IArticleService articleService, IImageService imageService)
        {
            _articleService = articleService;
            _imageService = imageService;
        }

        /// <summary>
        /// Get the list of article preview and get the list of article by username or tags.
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     
        ///     GET /articles/
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleListResponseModel))]
        public async Task<IActionResult> GetAll([FromQuery] FiltersRequest filters, [FromQuery] OrderBy order, [FromQuery] SortParameters parameters, [FromQuery] FindBy findBy, [FromQuery] int page=1, [FromQuery] int size=10)
        {
            var userId = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var articleList = await _articleService.GetAll(filters, order, parameters, page, size, userId, findBy);

            return Ok(articleList);
        }

        /// <summary>
        /// Get Top Liked authors
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     GET /article/top/authors
        /// 
        /// </remarks>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("top/authors")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ArticlePreviewResponseModel>))]
        public async Task<IActionResult> GetTopAuthors([FromQuery] int size = 5)
        {
            var topAuthors = await _articleService.GetTopAuthors(size);
            return Ok(topAuthors);
        }

        /// <summary>
        /// Get most used Tags in different articles
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     GET /articles/top/tags
        /// 
        /// </remarks>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("top/tags")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ArticlePreviewResponseModel>))]
        public async Task<IActionResult> GetTopTags([FromQuery] int size = 5)
        {
            var topTags = await _articleService.GetTopTags(size);
            return Ok(topTags);
        }

        /// <summary>
        /// Current user puts like on article
        /// </summary>
        /// <remarks>
        /// If you send this request second time user will unlike the article.
        /// Sample request
        /// 
        ///     PUT /articles/{id}/like
        /// 
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/like")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Like(Guid id)
        {
            var userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)?.FirstOrDefault()?.Value;

            await _articleService.LikeBy(id, userId);

            return NoContent();
        }

        /// <summary>
        /// Get article by id.
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     
        ///     GET /articles/{id}
        /// 
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleResponseModel))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var articlePreviewList = await _articleService.GetById(id, userId);

            return Ok(articlePreviewList);
        }

        /// <summary>
        /// Create article.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     POST /articles/
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{nameof(RoleNames.SuperAdmin)}, {nameof(RoleNames.Author)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleResponseModel))]
        public async Task<IActionResult> Create([FromBody] ArticleRequestModel articlePreviewModel)
        {
            var authorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var articlePreview = await _articleService.Create(authorId, articlePreviewModel);

            return Ok(articlePreview);
        }

        /// <summary>
        /// Save image to the article content.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     POST /articles/image
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPost("image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveContentImage([FromForm] FileUpload file)
        {
            var image = await _imageService.SaveContentImage(file);

            return Ok(image);
        }

        /// <summary>
        /// Get image of the article content.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     GET /articles/image/{imageId}
        /// 
        /// </remarks>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [HttpGet("image/{imageId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        public async Task<IActionResult> GetContentImage(Guid imageId)
        {
            var images = await _imageService.GetContentImage(imageId);

            if (images?.Any() != true)
            {
                throw new NotFoundException(ErrorMessages.NotFoundImage);
            }

            var image = images.First().Value;

            return base.File(image.Image, image.ContentType);
        }

        /// <summary>
        /// Update article.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     PUT /articles/{articleId}
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPut("{articleId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{nameof(RoleNames.SuperAdmin)}, {nameof(RoleNames.Author)}")]
        public async Task<IActionResult> Update([FromRoute] Guid articleId, [FromBody] ArticleRequestModel articleRequest)
        {
            var authorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            await _articleService.Update(articleRequest, articleId, authorId);

            return NoContent();
        }

        /// <summary>
        /// Delete article.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     DELETE /articles/{articleId}
        /// 
        /// </remarks>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpDelete("{articleId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{nameof(RoleNames.SuperAdmin)}, {nameof(RoleNames.Author)}")]
        public async Task<IActionResult> Delete([FromRoute] Guid articleId)
        {
            var authorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            await _articleService.Delete(articleId, authorId);

            return NoContent();
        }
    }
}
