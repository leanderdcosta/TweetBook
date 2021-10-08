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
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public PostsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetPostsAsync();
            
            var response = _mapper.Map<List<PostResponse>>(posts);

            return Ok(response);
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

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var uri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

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