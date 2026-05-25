namespace CapstoneProj.Models
{
    public class SearchResult
    {
        public string ChunkId { get; set; } = string.Empty;
        public string DocumentId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public string SectionTitle { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public double Score { get; set; }
    }
}