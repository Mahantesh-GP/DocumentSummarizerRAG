using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Knowledge;
using Azure.Search.Documents.Knowledge.Models;

var endpoint = new Uri("https://<your-search-service>.search.windows.net");
var credential = new AzureKeyCredential("<your-admin-key>");
var openAiEndpoint = "https://<your-openai-resource>.openai.azure.com/";
var deploymentName = "<gpt-deployment-name>";
var modelName = "<model-name>";

var indexClient = new SearchIndexClient(endpoint, credential);

// Create knowledge agent
var agent = new KnowledgeAgent(
    name: "hybrid-search-agent",
    models: new[]
    {
        new KnowledgeAgentAzureOpenAIModel(
            azureOpenAIParameters: new AzureOpenAIVectorizerParameters(
                resourceUrl: openAiEndpoint,
                deployment: deploymentName,
                modelName: modelName
            )
        )
    },
    targetIndexes: new[]
    {
        new KnowledgeAgentTargetIndex(
            indexName: "hybrid-search",
            defaultRerankerThreshold: 2.5
        )
    }
);

indexClient.CreateOrUpdateKnowledgeAgent(agent);
Console.WriteLine("Agent created successfully.");

// Perform retrieval
var agentClient = new KnowledgeAgentRetrieveClient(endpoint, credential);
var request = new KnowledgeAgentRetrieveRequest
{
    Messages = new[]
    {
        new KnowledgeAgentMessage(role: "user", content: "Show documents about 3 Cumings Court")
    },
    TargetIndexParameters = new[]
    {
        new KnowledgeAgentTargetIndexParameters("hybrid-search")
    }
};

var response = agentClient.Retrieve(agentName: "hybrid-search-agent", request);
Console.WriteLine("Agentic Response:\n" + response.Content[0].Text);
