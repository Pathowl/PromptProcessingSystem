using Xunit; 
using Microsoft.Extensions.Configuration;
using System.Net.Http; 
using System.Threading.Tasks;
using System.Collections.Generic;
using Backend.Services; 

namespace Tests.Services
{
    public class GeminiServiceTests
    {
        [Fact]
        public async Task ProcessPromptAsync_ShouldReturnResult()
        {
            // Arrange
            var myConfiguration = new Dictionary<string, string>
            {
                {"GeminiApiKey", "test-key"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var httpClient = new HttpClient();
            var service = new GeminiService(httpClient, configuration);

            // Act
            var result = await service.ProcessPromptAsync("Testowy prompt");

            // Assert
            Assert.NotNull(result);
        }
    }
}