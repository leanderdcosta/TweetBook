using FluentValidation;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.Validators
{
    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}