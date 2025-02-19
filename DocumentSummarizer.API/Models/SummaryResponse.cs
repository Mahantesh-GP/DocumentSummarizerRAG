namespace DocumentSummarizer.API.Models
{
    public class SummaryResponse
    {
        /// <summary>
        /// The unique identifier of the document.
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// The generated summary of the document.
        /// </summary>
        public string Summary { get; set; }
    }
}
