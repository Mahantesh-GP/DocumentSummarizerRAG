using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DocumentSummarizer.API.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Embeddings;
namespace DocumentSummarizer.API.Services
{


    public class DocumentService
    {
        private readonly SearchClient _searchClient;
        private readonly AzureOpenAIClient _openAiClient;
        private ITextEmbeddingGenerationService _textEmbeddingGenerationService;
        private IVectorStoreRecordCollection<string, Email> _collection;

        public DocumentService(IConfiguration config, ITextEmbeddingGenerationService textEmbeddingGenerationService,
                IVectorStoreRecordCollection<string, Email> collection)
        {
            string searchEndpoint = config["AzureSearch:Endpoint"] ?? throw new ArgumentNullException(nameof(config), "AzureSearch:Endpoint is missing in configuration.");
            string searchApiKey = config["AzureSearch:ApiKey"] ?? throw new ArgumentNullException(nameof(config), "AzureSearch:ApiKey is missing in configuration.");
            string openAiEndpoint = config["AzureOpenAI:Endpoint"] ?? throw new ArgumentNullException(nameof(config), "AzureOpenAI:Endpoint is missing in configuration.");
            string openAiKey = config["AzureOpenAI:ApiKey"] ?? throw new ArgumentNullException(nameof(config), "AzureOpenAI:ApiKey is missing in configuration.");
            string indexName = config["AzureSearch:IndexName"] ?? throw new ArgumentNullException(nameof(config), "AzureSearch:IndexName is missing in configuration.");

            _searchClient = new SearchClient(new Uri(searchEndpoint), indexName, new AzureKeyCredential(searchApiKey));
            _openAiClient = new AzureOpenAIClient(new Uri(openAiEndpoint), new AzureKeyCredential(openAiKey));

            _collection = collection;           
            _textEmbeddingGenerationService = textEmbeddingGenerationService;
        }

        public async Task<string> GetEmbedding(string text)
        {




            //EmbeddingsOptions embeddingOptions = new()
            //{
            //    DeploymentName = "text-embedding-3-large",
            //    Input = { "Your text string goes here" },
            //};

            var returnValue = _openAiClient.GetEmbeddingClient("text-embedding-3-large");
            var result = returnValue.GenerateEmbeddingAsync(text);
            //foreach (float item in result.Result.Value..ToArray())
            //{
            //    Console.WriteLine(item);
            //}


            return string.Join(",", result);
        }

        public async Task UploadDocument(string title, string content)
        {
            var embedding = await GetEmbedding(content);
            var document = new DocumentModel { Title = title, Content = content, EmbeddingVector = embedding };
            await _searchClient.UploadDocumentsAsync(new[] { document });
        }

        public async Task GenerateEmbeddingsAndUpsertAsync(int count = int.MaxValue)
        {
            List<Email> emails = await GetMailsFromOutlook(count);

            foreach (Email email in emails)
            {
                ReadOnlyMemory<float> embedding = await _textEmbeddingGenerationService.GenerateEmbeddingAsync(email.ToString());
                email.Embedding = embedding;
                await _collection.UpsertAsync(email);
            }
        }
        //public async Task<List<DocumentModel>> RetrieveRelevantDocuments(string query)
        //{
        //    var embedding = await GetEmbedding(query);
        //    var searchOptions = new SearchOptions { QueryType = SearchQueryType.Semantic, VectorSearch = new VectorSearchOptions { K = 3, Fields = new[] { "EmbeddingVector" } } };

        //    var results = await _searchClient.SearchAsync<DocumentModel>(embedding, searchOptions);
        //    return results.Value.GetResults().Select(r => r.Document).ToList();
        //}

        //private static async Task<float[]> ConvertToEmbeddingAsync(string documentText)
        //{

        //    var client = new OpenAIClient(new Uri(OpenAIEndpoint), new AzureKeyCredential(OpenAIKey));
        //    var embeddingOptions = new EmbeddingsOptions()
        //    {
        //        Input = new[] { documentText },
        //        Model = "text-embedding-ada-002"
        //    };

        //    var response = await client.GetEmbeddingsAsync(embeddingOptions);
        //    return response.Value.Data.First().Embedding.ToArray();

        //}

    }
}
