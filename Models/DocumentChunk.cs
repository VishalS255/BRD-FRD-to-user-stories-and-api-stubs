namespace CapstoneProj.Models
{
    public class DocumentChunk
    {
        public string ChunkId { get; set; } = Guid.NewGuid().ToString();
        public string DocumentId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public string SectionTitle { get; set; } = "Unknown";
        public string Text { get; set; } = string.Empty;
        public List<float> Embedding { get; set; } = new();
    }
}