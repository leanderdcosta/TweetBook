using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TweetBook;
using TweetBook.Data;
using TweetBook.Services;

namespace Tweetbook.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DataContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();

                var storedServices = scope.ServiceProvider;
                var identityService = storedServices.GetRequiredService<IIdentityService>();
                var logger = storedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<Startup>>>();

                try
                {
                    identityService.RegisterAsync(OPAConstants.UserName, OPAConstants.Password).Wait();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "An error occurred while creating test user {Message}", e.Message);
                }
            });
        }
    }
}