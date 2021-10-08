using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Data;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public class TagService : ITagService
    {
        private readonly DataContext _dataContext;

        public TagService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task AddTagsFromPostAsync(Post post)
        {
            foreach (var postTag in post.Tags)
            {
                var existingTag = await _dataContext.Tags.FirstOrDefaultAsync(t => t.Name == postTag.TagName);

                if (existingTag != null)
                {
                    continue;
                }

                _dataContext.Tags.Add(new Tag { Name = postTag.TagName });
            }

            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _dataContext.Tags.ToListAsync();
        }
    }
}