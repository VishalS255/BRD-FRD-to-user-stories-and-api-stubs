using System.Text.Json;
using CapstoneProj.Models;
using OpenAI;
using OpenAI.Chat;

namespace CapstoneProj.Services
{
    public class StoryGenerationService
    {
        private readonly OpenAIClient _client;

        public StoryGenerationService(OpenAIClient client)
        {
            _client = client;
        }

        public async Task<List<UserStory>> GenerateStoriesAsync(List<RequirementFact> facts)
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

You are generating user stories from structured requirement facts.

Rules:
- Use only the provided requirement facts.
- Do not invent unsupported technical details.
- Generate 2 to 5 atomic user stories.
- Return a JSON array.
-Each story must include:
  storyId, title, asA, iWant, soThat, isDerived, assumptionNote, traceabilityChunkIds, acceptanceCriteria
- isDerived must be true when the story is inferred from facts rather than explicitly stated.
- assumptionNote must explain the inference if isDerived is true.
- assumptionNote must be empty if isDerived is false.
- acceptanceCriteria must be an array of objects with:
  id, given, when, then, traceabilityChunkIds
- traceabilityChunkIds must come from the provided facts.

Requirement facts:
{factsText}
""";

            var response = await chatClient.CompleteChatAsync(prompt);
            var raw = response.Value.Content[0].Text;

            var cleaned = CleanJson(raw);

            try
            {
                var stories = TryDeserializeStories(cleaned);

                if (stories != null && stories.Count > 0)
                {
                    EnrichStoryTraceability(stories, facts);
                    return stories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("STORY DESERIALIZATION ERROR:");
                Console.WriteLine(ex.Message);
            }

            var fallback = new List<UserStory>
            {
                new UserStory
                {
                    StoryId = Guid.NewGuid().ToString(),
                    Title = "Fallback story generation failed",
                    AsA = "Reviewer",
                    IWant = "to inspect model output",
                    SoThat = "I can correct the generation logic",
                    TraceabilityChunkIds = facts.SelectMany(f => f.TraceabilityChunkIds).Distinct().ToList(),
                    AcceptanceCriteria = new List<AcceptanceCriterion>
                    {
                        new AcceptanceCriterion
                        {
                            Id = Guid.NewGuid().ToString(),
                            Given = "the model returned unparseable output",
                            When = "story generation is executed",
                            Then = "the system should return a fallback story",
                            TraceabilityChunkIds = facts.SelectMany(f => f.TraceabilityChunkIds).Distinct().ToList()
                        }
                    }
                }
            };

            EnrichStoryTraceability(fallback, facts);
            return fallback;
        }

        private static void EnrichStoryTraceability(List<UserStory> stories, List<RequirementFact> facts)
        {
            foreach (var story in stories)
            {
                story.TraceabilityReferences = BuildReferences(story.TraceabilityChunkIds, facts);

                foreach (var ac in story.AcceptanceCriteria)
                {
                    ac.TraceabilityReferences = BuildReferences(ac.TraceabilityChunkIds, facts);
                }
            }
        }

        private static List<TraceabilityReference> BuildReferences(List<string> chunkIds, List<RequirementFact> facts)
        {
            return facts
                .Where(f => f.TraceabilityChunkIds.Any(id => chunkIds.Contains(id)))
                .SelectMany(f => f.TraceabilityChunkIds
                    .Where(id => chunkIds.Contains(id))
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

        private static string CleanJson(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "[]";

            var cleaned = raw.Trim();
            cleaned = cleaned.Replace("```json", "", StringComparison.OrdinalIgnoreCase);
            cleaned = cleaned.Replace("```", "", StringComparison.OrdinalIgnoreCase);
            return cleaned.Trim();
        }

        private static List<UserStory>? TryDeserializeStories(string cleaned)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (cleaned.StartsWith("["))
            {
                return JsonSerializer.Deserialize<List<UserStory>>(cleaned, options);
            }

            if (cleaned.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(cleaned);
                var root = doc.RootElement;

                if (root.TryGetProperty("stories", out var storiesElement))
                {
                    return JsonSerializer.Deserialize<List<UserStory>>(storiesElement.GetRawText(), options);
                }

                if (root.TryGetProperty("data", out var dataElement))
                {
                    return JsonSerializer.Deserialize<List<UserStory>>(dataElement.GetRawText(), options);
                }
            }

            return null;
        }
    }
}