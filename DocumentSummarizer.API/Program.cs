using DocumentSummarizer.Infrastructure.Azure;
using Azure.Storage.Blobs;
using Azure.Search.Documents;
using Azure;

using DocumentSummarizer.API.Services;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using DocumentSummarizer.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
var configuration = builder.Configuration;

// Register Azure Blob Storage
var blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
builder.Services.AddSingleton(blobServiceClient);
builder.Services.AddSingleton<AzureBlobHelper>();

// Register Azure Cognitive Search
var searchClient = new SearchClient(
    new Uri($"https://{configuration["ConnectionStrings:AzureSearchService"]}.search.windows.net"),
    "azureblob-index-ragdoc",
    new AzureKeyCredential(configuration["ConnectionStrings:AzureSearchApiKey"])
);
builder.Services.AddSingleton(searchClient);
builder.Services.AddSingleton<AzureSearchHelper>();

// Register OpenAI API configuration
builder.Services.Configure<AzureOpenAIOptions>(configuration.GetSection("AzureOpenAI"));

// Register Semantic Kernel with OpenAI
builder.Services.AddSingleton<Kernel>(sp =>
{
    var openAiOptions = sp.GetRequiredService<IConfiguration>().GetSection("AzureOpenAI").Get<AzureOpenAIOptions>();

    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: openAiOptions.DeploymentName,
        endpoint: openAiOptions.Endpoint,
        apiKey: openAiOptions.ApiKey
    );

    return kernelBuilder.Build();
});

builder.Services.AddHttpClient();
// Register API Services
builder.Services.AddScoped<AzureStorageService>();
builder.Services.AddScoped<AzureSearchService>();
builder.Services.AddScoped<SummarizationService>();

// Register Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var allowedOrigins = "AllowBlazorClient";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7167") // ✅ Change this to match Blazor UI URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors(allowedOrigins);

app.Run();
