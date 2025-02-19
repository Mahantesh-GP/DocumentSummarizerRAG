using Azure;
using Azure.Search.Documents;
using Azure.Storage.Blobs;
using DocumentSummarizer.Infrastructure.Azure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Azure Blob Storage
var blobServiceClient = new BlobServiceClient(configuration["AZURE_STORAGE_CONNECTION_STRING"]);
builder.Services.AddSingleton(blobServiceClient);
builder.Services.AddSingleton<AzureBlobHelper>();

// Azure Cognitive Search
var azureSearchApiKey = configuration["AZURE_SEARCH_API_KEY"];
if (string.IsNullOrEmpty(azureSearchApiKey))
{
    throw new InvalidOperationException("Azure Search API key is not configured.");
}

var searchClient = new SearchClient(
    new Uri($"https://{configuration["AZURE_SEARCH_SERVICE_NAME"]}.search.windows.net"),
    "index-name",
    new AzureKeyCredential(azureSearchApiKey)
);
builder.Services.AddSingleton(searchClient);
builder.Services.AddSingleton<AzureSearchHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
