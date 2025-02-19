using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentSummarizer.Infrastructure.Azure
{
    public class AzureSearchHelper
    {
        private readonly SearchClient _searchClient;

        public AzureSearchHelper(SearchClient searchClient)
        {
            _searchClient = searchClient;
        }

        public async Task IndexDocumentAsync(string documentId, string content)
        {
            var doc = new SearchDocument { { "id", documentId }, { "content", content } };
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

            var searchQuery = string.IsNullOrEmpty(query) ? "*" : query; // Default to wildcard search if no query

            var response = await _searchClient.SearchAsync<SearchDocument>(searchQuery, options);
            return response.Value.GetResults().FirstOrDefault()?.Document["content"]?.ToString() ?? string.Empty;
        }
    }
}
