using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface ITagService
    {
        Task AddTagsFromPostAsync(Post post);
        Task<List<Tag>> GetAllAsync();
    }
}