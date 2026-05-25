namespace CapstoneProj.Models
{
    public class SystemMetrics
    {
        public int TotalDocuments { get; set; }
        public int TotalChunks { get; set; }
        public int TotalFacts { get; set; }
        public int TotalStories { get; set; }
        public int TotalApiStubs { get; set; }

        public double AvgChunkSize { get; set; }
        public double AvgFactsPerChunk { get; set; }
    }
}