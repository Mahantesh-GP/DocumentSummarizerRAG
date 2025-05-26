public class SearchApiClient
{
    private readonly HttpClient _http;

    public SearchApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> RunAgenticSearchAsync(string query)
    {
        var response = await _http.PostAsJsonAsync("api/AgenticSearch/query", new { query });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}