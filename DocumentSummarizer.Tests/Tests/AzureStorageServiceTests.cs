using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentSummarizer.API.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentSummarizer.Tests
{
    public class AzureStorageServiceTests
    {
        private readonly Mock<BlobServiceClient> _mockBlobServiceClient;
        private readonly Mock<BlobContainerClient> _mockBlobContainerClient;
        private readonly Mock<BlobClient> _mockBlobClient;
        private readonly AzureStorageService _azureStorageService;

        public AzureStorageServiceTests()
        {
            _mockBlobServiceClient = new Mock<BlobServiceClient>();
            _mockBlobContainerClient = new Mock<BlobContainerClient>();
            _mockBlobClient = new Mock<BlobClient>();

            _mockBlobServiceClient
                .Setup(b => b.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_mockBlobContainerClient.Object);

            _mockBlobContainerClient
                .Setup(c => c.GetBlobClient(It.IsAny<string>()))
                .Returns(_mockBlobClient.Object);

            _azureStorageService = new AzureStorageService(_mockBlobServiceClient.Object);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnBlobUri()
        {
            // Arrange
            var fileName = "test.txt";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Test content"));
            _mockBlobClient
                .Setup(b => b.UploadAsync(It.IsAny<Stream>(), true, default))
                .ReturnsAsync(It.IsAny<Response<BlobContentInfo>>());

            _mockBlobClient
                .Setup(b => b.Uri)
                .Returns(new System.Uri("https://fakeblobstorage.com/test.txt"));

            // Act
            var result = await _azureStorageService.UploadFileAsync(new FormFile(stream, 0, stream.Length, "test", fileName));

            // Assert
            result.ShouldBe("https://fakeblobstorage.com/test.txt");
        }
    }
}
