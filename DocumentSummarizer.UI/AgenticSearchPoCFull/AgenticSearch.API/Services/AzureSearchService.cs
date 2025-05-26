public class AzureSearchService
{
    private readonly SearchClient _client;

    public AzureSearchService(IConfiguration config)
    {
        _client = new SearchClient(
            new Uri(config["AzureSearch:Endpoint"]),
            config["AzureSearch:IndexName"],
            new AzureKeyCredential(config["AzureSearch:Key"]));
    }

    public async Task<List<string>> HybridSearchAsync(string query)
    {
        var options = new SearchOptions { Size = 5, QueryType = SearchQueryType.Semantic };
        var response = await _client.SearchAsync<SearchDocument>(query, options);
        return response.Value.GetResults().Select(r => r.Document["content"].ToString()).ToList();
    }
}