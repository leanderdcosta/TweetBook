using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter = null, PaginationFilter pagination = null);
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<bool> CreateAsync(Post post);
        Task<bool> UpdatePostAsync(Post post);
        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);
    }
}