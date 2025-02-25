using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace DocumentSummarizer.API.Models
{

    public class DocumentModel
    {
        [SimpleField(IsKey = true)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [SearchableField]
        public string Title { get; set; }

        [SearchableField]
        public string Content { get; set; }

        [SimpleField]
        public string EmbeddingVector { get; set; }
    }

}
