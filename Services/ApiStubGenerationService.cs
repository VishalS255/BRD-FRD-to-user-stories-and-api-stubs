using System.Text.Json;
using CapstoneProj.Models;
using OpenAI;
using OpenAI.Chat;

namespace CapstoneProj.Services
{
    public class ApiStubGenerationService
    {
        private readonly OpenAIClient _client;

        public ApiStubGenerationService(OpenAIClient client)
        {
            _client = client;
        }

        public async Task<List<ApiStub>> GenerateApiStubsAsync(List<RequirementFact> facts)
        {
            var chatClient = _client.GetChatClient("gpt-4.1-mini");

            var factsText = string.Join(
                "\n\n",
                facts.Select(f =>
                    $"FactId: {f.FactId}\nType: {f.Type}\nValue: {f.Value}\nTraceability: {string.Join(", ", f.TraceabilityChunkIds)}\nSourceFile: {f.SourceFileName}\nEvidence: {f.EvidenceSnippet}")
            );

            var prompt = $"""
Return ONLY valid JSON.
Do not use markdown.
Do not wrap the answer in ```json.
Do not add explanations.

You are generating API stubs from structured requirement facts.

Rules:
- Use only the provided requirement facts.
- Do not invent unsupported technical assumptions.
- Return a JSON array.
- Generate 1 to 5 API stubs.
- Each API stub must include:
  stubId, method, path, summary, sampleRequestJson, sampleResponseJson, isDerived, assumptionNote, traceabilityChunkIds
- - Set isDerived to true unless the source facts explicitly define an API endpoint, API method, request, or response.
- Most API stubs generated from business actions should be marked isDerived = true.
- assumptionNote must explain what was inferred.
- assumptionNote must be empty only when the source explicitly defines the API contract.
- sampleRequestJson can be either a JSON object or a JSON string
- sampleResponseJson can be either a JSON object or a JSON string
- traceabilityChunkIds must come from the provided facts.

Requirement facts:
{factsText}
""";

            var response = await chatClient.CompleteChatAsync(prompt);
            var raw = response.Value.Content[0].Text;

            var cleaned = CleanJson(raw);

            try
            {
                var stubs = TryDeserializeApiStubs(cleaned);

                if (stubs != null && stubs.Count > 0)
                {
                    EnrichApiStubTraceability(stubs, facts);
                    return stubs;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("API STUB DESERIALIZATION ERROR:");
                Console.WriteLine(ex.Message);
            }

            var fallback = new List<ApiStub>
            {
                new ApiStub
                {
                    StubId = Guid.NewGuid().ToString(),
                    Method = "POST",
                    Path = "/fallback",
                    Summary = "Fallback API stub generation failed",
                    SampleRequestJson = new { },
                    SampleResponseJson = new { },
                    TraceabilityChunkIds = facts.SelectMany(f => f.TraceabilityChunkIds).Distinct().ToList()
                }
            };

            EnrichApiStubTraceability(fallback, facts);
            return fallback;
        }

        private static void EnrichApiStubTraceability(List<ApiStub> stubs, List<RequirementFact> facts)
        {
            foreach (var stub in stubs)
            {
                stub.TraceabilityReferences = facts
                    .Where(f => f.TraceabilityChunkIds.Any(id => stub.TraceabilityChunkIds.Contains(id)))
                    .SelectMany(f => f.TraceabilityChunkIds
                        .Where(id => stub.TraceabilityChunkIds.Contains(id))
                        .Select(id => new TraceabilityReference
                        {
                            ChunkId = id,
                            SourceFileName = f.SourceFileName,
                            EvidenceSnippet = f.EvidenceSnippet
                        }))
                    .GroupBy(x => new { x.ChunkId, x.SourceFileName, x.EvidenceSnippet })
                    .Select(g => g.First())
                    .ToList();
            }
        }

        private static string CleanJson(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "[]";

            var cleaned = raw.Trim();
            cleaned = cleaned.Replace("```json", "", StringComparison.OrdinalIgnoreCase);
            cleaned = cleaned.Replace("```", "", StringComparison.OrdinalIgnoreCase);
            return cleaned.Trim();
        }

        private static List<ApiStub>? TryDeserializeApiStubs(string cleaned)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (cleaned.StartsWith("["))
            {
                return JsonSerializer.Deserialize<List<ApiStub>>(cleaned, options);
            }

            if (cleaned.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(cleaned);
                var root = doc.RootElement;

                if (root.TryGetProperty("apiStubs", out var stubsElement))
                {
                    return JsonSerializer.Deserialize<List<ApiStub>>(stubsElement.GetRawText(), options);
                }

                if (root.TryGetProperty("data", out var dataElement))
                {
                    return JsonSerializer.Deserialize<List<ApiStub>>(dataElement.GetRawText(), options);
                }
            }

            return null;
        }
    }
}