using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DocumentSummarizer.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Shouldly;
using Xunit;

namespace DocumentSummarizer.Tests
{
    public class SummarizationServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly SummarizationService _summarizationService;

        public SummarizationServiceTests()
        {
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandler.Object);

            var inMemorySettings = new Dictionary<string, string>
            {
                { "AZURE_OPENAI_API_KEY", "test-api-key" },
                { "AZURE_OPENAI_DEPLOYMENT_NAME", "test-deployment" },
                { "AZURE_OPENAI_ENDPOINT", "https://test-openai-instance.openai.azure.com" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _summarizationService = new SummarizationService(_httpClient, _configuration);
        }

        [Fact]
        public async Task SummarizeDocumentAsync_ShouldReturnSummary()
        {
            // Arrange
            var responseContent = new { choices = new[] { new { message = new { content = "Test summary response" } } } };
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(responseContent)
            };

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            // Act
            var result = await _summarizationService.SummarizeDocumentAsync("Test document content");

            // Assert
            result.ShouldBe("Test summary response");
        }
    }
}
