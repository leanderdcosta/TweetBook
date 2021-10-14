using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TweetBook;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Data;

namespace Tweetbook.IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient TestClient;
        private readonly IServiceProvider _serviceProvider;

        protected IntegrationTest()
        {
            var appFactory = new CustomWebApplicationFactory<Startup>();
            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            var authResponse = await LoginUserAsync();
            var token = authResponse.Token;
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
            return await response.Content.ReadAsAsync<PostResponse>();
        }

        private async Task<AuthSuccessResponse> LoginUserAsync()
        {
            var request = new UserLoginRequest
            {
                Email = OPAConstants.UserName,
                Password = OPAConstants.Password
            };

            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, request);

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return registrationResponse;
        }

        public void Dispose()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
            context.Database.EnsureDeleted();
        }
    }
}