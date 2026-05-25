using CapstoneProj.Models;

namespace CapstoneProj.Services
{
    public class MetricsService
    {
        private readonly VectorStoreService _vectorStoreService;

        public MetricsService(VectorStoreService vectorStoreService)
        {
            _vectorStoreService = vectorStoreService;
        }

        public SystemMetrics GetMetrics()
        {
            var documents = _vectorStoreService.GetDocuments();
            var chunks = _vectorStoreService.GetChunks();
            var facts = _vectorStoreService.GetRequirementFacts();
            var stories = _vectorStoreService.GetStories();
            var stubs = _vectorStoreService.GetApiStubs();

            var avgChunkSize = chunks.Count > 0
                ? chunks.Average(c => c.Text.Length)
                : 0;

            var avgFactsPerChunk = chunks.Count > 0
                ? (double)facts.Count / chunks.Count
                : 0;

            return new SystemMetrics
            {
                TotalDocuments = documents.Count,
                TotalChunks = chunks.Count,
                TotalFacts = facts.Count,
                TotalStories = stories.Count,
                TotalApiStubs = stubs.Count,
                AvgChunkSize = Math.Round(avgChunkSize, 2),
                AvgFactsPerChunk = Math.Round(avgFactsPerChunk, 2)
            };
        }
    }
}