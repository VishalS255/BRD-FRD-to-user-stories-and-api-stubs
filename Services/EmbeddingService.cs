using OpenAI;
using OpenAI.Embeddings;
using System.Linq;

namespace CapstoneProj.Services
{
    public class EmbeddingService
    {
        private readonly OpenAIClient _client;

        public EmbeddingService(OpenAIClient client)
        {
            _client = client;
        }

        public async Task<List<float>> GenerateEmbeddingAsync(string text)
        {
            var embeddingClient = _client.GetEmbeddingClient("text-embedding-3-small");
            var response = await embeddingClient.GenerateEmbeddingAsync(text);
            return response.Value.ToFloats().ToArray().ToList();
        }
    }
}