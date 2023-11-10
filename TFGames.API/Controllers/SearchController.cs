using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TFGames.API.Models.Request;
using TFGames.API.Models.Response;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Enums;
using TFGames.DAL.Search;

namespace TFGames.API.Controllers
{
    [ApiController]
    [Route(template: "/search")]
    public class SearchController : ControllerBase
    {
        private readonly IArticleService _articleService;

        private readonly ICommentService _commentService;

        public SearchController(IArticleService articleService, ICommentService commentService)
        {
            _articleService = articleService;
            _commentService = commentService;
        }

        /// <summary>
        /// Search method.
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     
        ///    Search Category: Articles,
        ///                     Users,
        ///                     Tags,
        ///                     Comments
        ///     
        ///     Sort Parameter: LastPosts
        ///                     NewPosts
        ///     
        ///     Order By: CreatedDate
        ///     
        ///     
        ///     GET /search?{category}&{query}&{sortParameter}
        /// 
        ///     Example: 
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet(Name = "category/query/sortParameter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ArticlePreviewResponseModel>))]
        public async Task<IActionResult> Search([FromQuery] SearchCategory category, [FromQuery] OrderBy order, [FromQuery] SortParameters parameters, [FromQuery] FiltersRequest query, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            switch (category)
            {
                case SearchCategory.Tags:
                    {
                        var result = new TagsSearchResponseModel();
                        result.Tags = await _articleService.GetAllTags(query, order, parameters);
                        return Ok(result);
                    }
                case SearchCategory.Users:
                    {
                        var result = new UsersSearchResponseModel();
                        result.Users = await _articleService.GetAllAuthors(query, order, parameters);
                        return Ok(result);
                    }
                case SearchCategory.Comments:
                    {
                        var result = new CommentsSearchResponseModel();
                        result.Comments = await _commentService.GetAll(query, order, parameters);
                        return Ok(result);
                    }
                case SearchCategory.Articles:
                    {
                        var userId = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                        var find = new FindBy();

                        var result = await _articleService.GetAll(query, order, parameters, page, size, userId, find);
                        return Ok(result);
                    }
                default: return NoContent();
            }
        }
    }
}
