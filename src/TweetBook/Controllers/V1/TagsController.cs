using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController : Controller
    {
        private readonly ITagService _tagsService;
        private readonly IMapper _mapper;

        public TagsController(ITagService tagsService, IMapper mapper)
        {
            _tagsService = tagsService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]
        [Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _tagsService.GetAllAsync();
            
            var response = _mapper.Map<List<TagResponse>>(tags);
            
            return Ok(response);
        }
    }
}