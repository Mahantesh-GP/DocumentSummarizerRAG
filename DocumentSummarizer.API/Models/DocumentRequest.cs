namespace DocumentSummarizer.API.Models
{
    public class DocumentRequest
    {
        /// <summary>
        /// The unique identifier of the document.
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// (Optional) A user query to extract specific information from the document.
        /// </summary>
        public string Query { get; set; }
    }
}
