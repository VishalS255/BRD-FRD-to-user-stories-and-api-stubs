using CapstoneProj.Models;

namespace CapstoneProj.Services
{
    public class RetrievalService
    {
        private readonly EmbeddingService _embeddingService;
        private readonly VectorStoreService _vectorStoreService;

        public RetrievalService(EmbeddingService embeddingService, VectorStoreService vectorStoreService)
        {
            _embeddingService = embeddingService;
            _vectorStoreService = vectorStoreService;
        }

        public async Task<List<SearchResult>> SearchAsync(string query, int topK = 5)
        {
            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
            var chunks = _vectorStoreService.GetChunks().Where(c => c.Embedding.Count > 0).ToList();

            var results = chunks
                .Select(c => new SearchResult
                {
                    ChunkId = c.ChunkId,
                    DocumentId = c.DocumentId,
                    FileName = c.FileName,
                    ChunkIndex = c.ChunkIndex,
                    SectionTitle = c.SectionTitle,
                    Text = c.Text,
                    Score = CosineSimilarity(queryEmbedding, c.Embedding)
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();

            return results;
        }

        private static double CosineSimilarity(List<float> a, List<float> b)
        {
            if (a.Count != b.Count || a.Count == 0)
                return 0;

            double dot = 0;
            double normA = 0;
            double normB = 0;

            for (int i = 0; i < a.Count; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }

            if (normA == 0 || normB == 0)
                return 0;

            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }
}