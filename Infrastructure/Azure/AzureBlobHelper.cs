using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;

namespace DocumentSummarizer.Infrastructure.Azure
{
    public class AzureBlobHelper
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "documents";

        public AzureBlobHelper(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_containerName);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.None);
            var blobClient = blobContainer.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
            return blobClient.Uri.ToString();
        }

        public async Task<string> GetFileContentAsync(string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainer.GetBlobClient(fileName);
            var response = await blobClient.DownloadContentAsync();
            return response.Value.Content.ToString();
        }
    }
}
