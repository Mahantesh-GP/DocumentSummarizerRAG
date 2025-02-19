using DocumentSummarizer.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentSummarizer.API.Controller
{
    [ApiController]
    [Route("api/summarizer")]
    public class SummarizationController : ControllerBase
    {
        private readonly SummarizationService _summarizationService;
        private readonly AzureStorageService _storageService;
        private readonly AzureSearchService _searchService;

        public SummarizationController(SummarizationService summarizationService, AzureStorageService storageService, AzureSearchService searchService)
        {
            _summarizationService = summarizationService;
            _storageService = storageService;
            _searchService = searchService;
        }

        [HttpGet("summarize")]
        public async Task<IActionResult> SummarizeDocument([FromQuery] string documentId, [FromQuery] string query = "")
        {
            if (string.IsNullOrEmpty(documentId))
            {
                return BadRequest("Document ID is required");
            }
            var content = await _searchService.RetrievePassagesAsync(documentId, query);
            var summary = await _summarizationService.SummarizeDocumentAsync(content);
            return Ok(new { Summary = summary });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }
            var fileUrl = await _storageService.UploadFileAsync(file);
            var content = await _storageService.GetFileContentAsync(file.FileName);
            await _searchService.IndexDocumentAsync(file.FileName, content, fileUrl);
            return Ok(new { FileUrl = fileUrl, DocumentId = file.FileName });
        }
    }
}

