using System;
using System.Collections.Generic;

namespace TweetBook.Contracts.V1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<TagResponse> Tags { get; set; }
        public string UserId { get; set; }
    }
}