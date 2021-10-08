using System.Collections.Generic;

namespace TweetBook.Contracts.V1.Responses
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }

    public class ErrorModel
    {
        public string FieldName { get; set; }
        public string Message { get; set; }
    }
}