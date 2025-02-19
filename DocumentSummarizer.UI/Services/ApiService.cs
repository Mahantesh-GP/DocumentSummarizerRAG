using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace DocumentSummarizer.UI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadFileAsync(IBrowserFile file)
        {
            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("https://localhost:7238/api/summarizer/upload", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DocumentUploadResponse>();
                return result?.DocumentId ?? string.Empty;
            }

            return string.Empty;
        }

        public async Task<string> GetSummaryAsync(string documentId, string query)
        {
            var response = await _httpClient.GetFromJsonAsync<SummaryResponse>($"https://localhost:7238/api/summarizer/summarize?documentId={documentId}&query={query}");
            return response?.Summary ?? "No response available";
        }
    }

    public class DocumentUploadResponse
    {
        public string DocumentId { get; set; }
    }

    public class SummaryResponse
    {
        public string Summary { get; set; }
    }
}
