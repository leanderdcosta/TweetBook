
using System.Collections.Generic;

namespace TweetBook.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public string Name { get; set; }
        public string[] Tags { get; set; }

        public CreatePostRequest()
        {
            Tags = new string[0];
        }
    }
}