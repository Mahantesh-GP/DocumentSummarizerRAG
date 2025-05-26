public class OpenAIAgentService
{
    private readonly AzureSearchService _searchService;
    private readonly OpenAIClient _openAI;

    public OpenAIAgentService(AzureSearchService searchService, IConfiguration config)
    {
        _searchService = searchService;
        _openAI = new OpenAIClient(new Uri(config["OpenAI:Endpoint"]), new AzureKeyCredential(config["OpenAI:Key"]));
    }

    public async Task<string> HandleQueryAsync(string userQuery)
    {
        var chat = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an intelligent agent who answers questions using a document search function."),
            new ChatMessage(ChatRole.User, userQuery)
        };

        var functions = new List<FunctionDefinition>
        {
            new FunctionDefinition
            {
                Name = "search_documents",
                Description = "Search documents in Azure Cognitive Search",
                Parameters = BinaryData.FromObjectAsJson(new
                {
                    type = "object",
                    properties = new {
                        query = new { type = "string", description = "The query to run against Azure Search" }
                    },
                    required = new[] { "query" }
                })
            }
        };

        var options = new ChatCompletionsOptions
        {
            DeploymentName = "gpt-4-mini",
            Messages = chat,
            Functions = functions,
            FunctionCall = FunctionCall.Auto
        };

        var result = await _openAI.GetChatCompletionsAsync(options);
        var message = result.Value.Choices[0].Message;

        if (message.FunctionCall?.Name == "search_documents")
        {
            var args = JsonSerializer.Deserialize<SearchArgs>(message.FunctionCall.Arguments.ToString());
            var docs = await _searchService.HybridSearchAsync(args.query);
            var serializedDocs = JsonSerializer.Serialize(docs);

            chat.Add(new ChatMessage(ChatRole.Function, serializedDocs) { Name = "search_documents" });

            var finalOptions = new ChatCompletionsOptions
            {
                DeploymentName = "gpt-4-mini",
                Messages = chat
            };

            var final = await _openAI.GetChatCompletionsAsync(finalOptions);
            return final.Value.Choices[0].Message.Content;
        }

        return message.Content ?? "No function called.";
    }
}