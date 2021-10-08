using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Data;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;
        private readonly ITagService _tagsService;

        public PostService(DataContext dataContext, ITagService tagsService)
        {
            _dataContext = dataContext;
            _tagsService = tagsService;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _dataContext.Posts.Include(p => p.Tags).ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid guid)
        {
            return await _dataContext.Posts.Include(p => p.Tags).SingleOrDefaultAsync(x => x.Id == guid);
        }

        public async Task<bool> CreateAsync(Post post)
        {
            await _dataContext.Posts.AddAsync(post);

            await _tagsService.AddTagsFromPostAsync(post);

            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdatePostAsync(Post post)
        {
            if (await GetPostByIdAsync(post.Id) is null)
            {
                return false;
            }

            _dataContext.Posts.Update(post);

            var updated = await _dataContext.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);

            if (post is null)
            {
                return false;
            }

            _dataContext.Posts.Remove(post);

            var deleted = await _dataContext.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == postId);

            return post == null || post.UserId != userId;
        }
    }
}