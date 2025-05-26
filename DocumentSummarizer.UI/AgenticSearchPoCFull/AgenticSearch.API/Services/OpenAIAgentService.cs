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
        var searchResults = await _searchService.HybridSearchAsync(userQuery);
        var content = string.Join("\n", searchResults);

        var chat = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "Summarize documents retrieved based on user query."),
            new ChatMessage(ChatRole.User, $"Query: {userQuery}\nDocuments:\n{content}")
        };

        var options = new ChatCompletionsOptions
        {
            DeploymentName = "gpt-4-mini",
            Messages = { chat[0], chat[1] },
            Temperature = 0.3f
        };

        var result = await _openAI.GetChatCompletionsAsync(options);
        return result.Value.Choices[0].Message.Content;
    }
}