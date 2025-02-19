using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using DocumentSummarizer.API.Utility;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DocumentSummarizer.API.Services
{
    public class AzureSearchService
    {
        private readonly SearchClient _searchClient;

        public AzureSearchService(SearchClient searchClient)
        {
            _searchClient = searchClient;
        }

        public async Task IndexDocumentAsync(string documentId, string content,string FileUri)
        {
            var doc = new SearchDocument { { "id", documentId }, { "content", content },{ "metadata_storage_path", UrlSafeBase64.Encode(FileUri) } };
            await _searchClient.MergeOrUploadDocumentsAsync(new[] { doc });
        }

        public async Task<string> RetrievePassagesAsync(string documentId, string query = "")
        {
            var options = new SearchOptions
            {
                QueryType = SearchQueryType.Full,
                Select = { "content" },
                SearchMode = SearchMode.All
            };

            var searchQuery = string.IsNullOrEmpty(query) ? "*" : query;
            var response = await _searchClient.SearchAsync<SearchDocument>(searchQuery, options);

            return response.Value.GetResults().FirstOrDefault()?.Document["content"]?.ToString() ?? string.Empty;
        }
    }
}
