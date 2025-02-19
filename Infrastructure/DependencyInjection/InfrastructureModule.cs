using Azure.Storage.Blobs;
using Azure.Search.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocumentSummarizer.Infrastructure.Azure;
using Azure;

namespace DocumentSummarizer.Infrastructure.DependencyInjection
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Azure Blob Storage
            var blobServiceClient = new BlobServiceClient(configuration["AZURE_STORAGE_CONNECTION_STRING"]);
            services.AddSingleton(blobServiceClient);
            services.AddSingleton<AzureBlobHelper>();

            // Azure Cognitive Search
            var searchClient = new SearchClient(
                new Uri($"https://{configuration["AZURE_SEARCH_SERVICE_NAME"]}.search.windows.net"),
                "index-name",
                new AzureKeyCredential(configuration["AZURE_SEARCH_API_KEY"])
            );
            services.AddSingleton(searchClient);
            services.AddSingleton<AzureSearchHelper>();

            return services;
        }
    }
}
