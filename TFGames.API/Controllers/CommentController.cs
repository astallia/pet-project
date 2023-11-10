using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Enums;

namespace TFGames.API.Controllers
{
    [ApiController]
    [Route("articles/{articleId}/comments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{nameof(RoleNames.User)},{nameof(RoleNames.SuperAdmin)},{nameof(RoleNames.Author)}")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Get all comments by Article Id.
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     
        ///     GET /articles/{articleId}/comments
        /// 
        /// </remarks>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentResponseModel>))]
        public async Task<IActionResult> GetAllByArticleId([FromRoute] Guid articleId)
        {
            var comments = await _commentService.FindAllByArticleId(articleId);
            return Ok(comments);
        }

        /// <summary>
        /// Gets comment by id.
        /// </summary>
        /// <remarks>
        /// Sample request.
        /// 
        ///     GET /articles/{articleId}/comments/{commentId}
        /// </remarks>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpGet("{commentId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentResponseModel))]
        public async Task<IActionResult> GetCommentById([FromRoute] Guid commentId)
        {
            var comment = await _commentService.FindById(commentId);
            return Ok(comment);
        }

        /// <summary>
        /// Creates comments.
        /// </summary>
        /// <remarks>
        /// CommentId is optional (works in postman). If it's null would create a comment, if not null would create reply for specific comment.
        /// 
        /// Sample request
        /// 
        ///     POST /articles/{articleId}/comments/{commentId?}
        ///     {
        ///         "content": "Content"
        ///     }
        /// 
        /// </remarks>
        /// <param name="articleId"></param>
        /// <param name="model"></param>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpPost("{commentId?}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<CommentResponseModel>))]
        public async Task<IActionResult> Post([FromRoute] Guid articleId, [FromBody] CommentRequestModel model, [FromRoute] Guid? commentId=null)
        {
            var authorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var comment = await _commentService.CreateOrReply(commentId, model, articleId, authorId);
            
            return Created("/posts/comments", comment);
        }

        /// <summary>
        /// Edit comment
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     PUT /articles/{articleId}/comments/{commentId}
        /// 
        /// </remarks>
        /// <param name="commentId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPut("{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Edit([FromRoute] Guid commentId, [FromBody] string content)
        {
            var authorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            await _commentService.Edit(commentId, content, authorId);
            return NoContent();
        }

        /// <summary>
        /// Delete comment
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     DELETE /articles/{articleId}/comments/{commentId}
        /// 
        /// </remarks>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete("{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] Guid commentId)
        {
            var authorId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;

            await _commentService.Delete(commentId, authorId);

            return NoContent();
        }
    }
}