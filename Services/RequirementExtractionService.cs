using System.Text.Json;
using CapstoneProj.Models;
using OpenAI;
using OpenAI.Chat;

namespace CapstoneProj.Services
{
    public class RequirementExtractionService
    {
        private readonly OpenAIClient _client;

        public RequirementExtractionService(OpenAIClient client)
        {
            _client = client;
        }

        public async Task<List<RequirementFact>> ExtractFactsAsync(List<SearchResult> chunks)
        {
            var chatClient = _client.GetChatClient("gpt-4.1-mini");

            var sourceText = string.Join(
                "\n\n---\n\n",
                chunks.Select(c =>
                    $"ChunkId: {c.ChunkId}\nFile: {c.FileName}\nChunkIndex: {c.ChunkIndex}\nText: {c.Text}")
            );

            var prompt = $"""
Return ONLY valid JSON.
Do not use markdown.
Do not wrap the answer in ```json.
Do not add explanations.

Extract requirement facts from the source text.

Return a JSON array.
Each array item must have exactly these fields:
- factId
- type
- value
- traceabilityChunkIds
- sourceFileName
- evidenceSnippet

Allowed values for type:
- Actor
- Action
- Rule
- Constraint
- Unknown

Rules:
- Use only the provided source text.
- Do not invent unsupported technical details.
- evidenceSnippet must be a short quote-like snippet from the source.
- traceabilityChunkIds must use chunk ids from the source.
- sourceFileName must match the file name from the source.

Source text:
{sourceText}
""";

            var response = await chatClient.CompleteChatAsync(prompt);
            var raw = response.Value.Content[0].Text;

            Console.WriteLine("RAW REQUIREMENT EXTRACTION OUTPUT:");
            Console.WriteLine(raw);

            var cleaned = CleanJson(raw);

            Console.WriteLine("CLEANED REQUIREMENT EXTRACTION OUTPUT:");
            Console.WriteLine(cleaned);

            try
            {
                var facts = TryDeserializeFacts(cleaned);

                if (facts != null && facts.Count > 0)
                {
                    return facts;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DESERIALIZATION ERROR:");
                Console.WriteLine(ex.Message);
            }

            return chunks.Select(c => new RequirementFact
            {
                FactId = Guid.NewGuid().ToString(),
                Type = "Unknown",
                Value = "Failed to parse structured requirement facts.",
                TraceabilityChunkIds = new List<string> { c.ChunkId },
                SourceFileName = c.FileName,
                EvidenceSnippet = c.Text.Substring(0, Math.Min(120, c.Text.Length))
            }).ToList();
        }

        private static string CleanJson(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "[]";

            var cleaned = raw.Trim();

            cleaned = cleaned.Replace("```json", "", StringComparison.OrdinalIgnoreCase);
            cleaned = cleaned.Replace("```", "", StringComparison.OrdinalIgnoreCase);
            cleaned = cleaned.Trim();

            return cleaned;
        }

        private static List<RequirementFact>? TryDeserializeFacts(string cleaned)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (cleaned.StartsWith("["))
            {
                return JsonSerializer.Deserialize<List<RequirementFact>>(cleaned, options);
            }

            if (cleaned.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(cleaned);
                var root = doc.RootElement;

                if (root.TryGetProperty("facts", out var factsElement))
                {
                    return JsonSerializer.Deserialize<List<RequirementFact>>(factsElement.GetRawText(), options);
                }

                if (root.TryGetProperty("data", out var dataElement))
                {
                    return JsonSerializer.Deserialize<List<RequirementFact>>(dataElement.GetRawText(), options);
                }
            }

            return null;
        }
    }
}