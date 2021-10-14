using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Requests.Queries;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Helpers;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostsController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }

        [AllowAnonymous]
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPostsQuery filterQuery, [FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var filter = _mapper.Map<GetAllPostsFilter>(filterQuery);

            var posts = await _postService.GetPostsAsync(filter, pagination);

            var postResponse = _mapper.Map<List<PostResponse>>(posts);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PostResponse>(postResponse));
            }

            var pagedResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, postResponse);

            return Ok(pagedResponse);
        }

        [AllowAnonymous]
        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PostResponse>(post));
        }

        [Authorize(Policy = "MustWorkForTweetbook")]
        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
        {
            var postId = Guid.NewGuid();
            var post = new Post
            {
                Id = postId,
                Name = request.Name,
                UserId = HttpContext.GetUserId(),
                Tags = request.Tags.Select(x => new PostTag { PostId = postId, TagName = x }).ToList()
            };

            await _postService.CreateAsync(post);

            var uri = _uriService.GetPostUri(post.Id.ToString());

            var response = _mapper.Map<PostResponse>(post);

            return Created(uri, response);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new
                {
                    error = "You do not own this post."
                });
            }

            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = request.Name;

            var updated = await _postService.UpdatePostAsync(post);

            if (!updated)
            {
                return NotFound();
            }

            var response = _mapper.Map<PostResponse>(post);

            return Ok(response);
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new
                {
                    error = "You do not own this post."
                });
            }

            if (!await _postService.DeletePostAsync(postId))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}