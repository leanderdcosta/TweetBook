using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using Xunit;

namespace Tweetbook.IntegrationTests
{
    public class PostControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.GetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsAsync<PagedResponse<PostResponse>>();
            (content.Data).Should().BeEmpty();
        }

        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistsInTheDatabase()
        {
            // Arrange
            string postName = "Test post";
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequest { Name = postName });

            // Act
            var response = await TestClient.GetAsync(
                ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString())
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedPost = await response.Content.ReadAsAsync<PostResponse>();
            returnedPost.Id.Should().Be(createdPost.Id);
            returnedPost.Name.Should().Be(postName);
        }
    }
}