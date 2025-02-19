using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DocumentSummarizer.API.Services
{
    public class SummarizationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;
        private readonly string _deploymentName;
        private readonly string _openAiEndpoint;

        public SummarizationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _openAiApiKey = configuration["AZURE_OPENAI_API_KEY"];
            _deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_NAME"];
            _openAiEndpoint = configuration["AZURE_OPENAI_ENDPOINT"];
        }

        /// <summary>
        /// Summarizes the given document content using Azure OpenAI GPT API.
        /// </summary>
        /// <param name="documentContent">The extracted content of the document.</param>
        /// <returns>Summarized text.</returns>
        public async Task<string> SummarizeDocumentAsync(string documentContent)
        {
            if (string.IsNullOrEmpty(documentContent))
                return "No content available to summarize.";

            var requestBody = new
            {
                model = _deploymentName,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful AI assistant summarizing documents." },
                    new { role = "user", content = $"Summarize the following document:\n\n{documentContent}" }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_openAiEndpoint}/v1/chat/completions")
            {
                Content = JsonContent.Create(requestBody)
            };

            request.Headers.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();

            return result?.choices[0]?.message?.content ?? "No summary available.";
        }
    }
}
