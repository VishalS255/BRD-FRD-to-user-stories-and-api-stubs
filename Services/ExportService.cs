using System.Text;
using System.Text.Json;
using CapstoneProj.Models;

namespace CapstoneProj.Services
{
    public class ExportService
    {
        private readonly VectorStoreService _vectorStoreService;

        public ExportService(VectorStoreService vectorStoreService)
        {
            _vectorStoreService = vectorStoreService;
        }

        public string ExportAllAsJson()
        {
            var payload = new
            {
                documents = _vectorStoreService.GetDocuments(),
                chunks = _vectorStoreService.GetChunks(),
                requirementFacts = _vectorStoreService.GetRequirementFacts(),
                stories = _vectorStoreService.GetStories(),
                apiStubs = _vectorStoreService.GetApiStubs()
            };

            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        public string ExportStoriesAsCsv()
        {
            var stories = _vectorStoreService.GetStories();
            var sb = new StringBuilder();

            sb.AppendLine("StoryId,Title,AsA,IWant,SoThat,IsDerived,AssumptionNote,TraceabilityChunkIds");

            foreach (var story in stories)
            {
                sb.AppendLine(
                    $"{Escape(story.StoryId)}," +
                    $"{Escape(story.Title)}," +
                    $"{Escape(story.AsA)}," +
                    $"{Escape(story.IWant)}," +
                    $"{Escape(story.SoThat)}," +
                    $"{Escape(story.IsDerived.ToString())}," +
                    $"{Escape(story.AssumptionNote)}," +
                    $"{Escape(string.Join("|", story.TraceabilityChunkIds))}");
            }

            return sb.ToString();
        }

        public string ExportAcceptanceCriteriaAsCsv()
        {
            var stories = _vectorStoreService.GetStories();
            var sb = new StringBuilder();

            sb.AppendLine("StoryId,StoryTitle,AcceptanceCriterionId,Given,When,Then,TraceabilityChunkIds");

            foreach (var story in stories)
            {
                foreach (var ac in story.AcceptanceCriteria)
                {
                    sb.AppendLine(
                        $"{Escape(story.StoryId)}," +
                        $"{Escape(story.Title)}," +
                        $"{Escape(ac.Id)}," +
                        $"{Escape(ac.Given)}," +
                        $"{Escape(ac.When)}," +
                        $"{Escape(ac.Then)}," +
                        $"{Escape(string.Join("|", ac.TraceabilityChunkIds))}");
                }
            }

            return sb.ToString();
        }

        public string ExportApiStubsAsCsv()
        {
            var stubs = _vectorStoreService.GetApiStubs();
            var sb = new StringBuilder();

            sb.AppendLine("StubId,Method,Path,Summary,SampleRequestJson,SampleResponseJson,IsDerived,AssumptionNote,TraceabilityChunkIds");

            foreach (var stub in stubs)
            {
                var requestJson = JsonSerializer.Serialize(stub.SampleRequestJson);
                var responseJson = JsonSerializer.Serialize(stub.SampleResponseJson);

                sb.AppendLine(
                    $"{Escape(stub.StubId)}," +
                    $"{Escape(stub.Method)}," +
                    $"{Escape(stub.Path)}," +
                    $"{Escape(stub.Summary)}," +
                    $"{Escape(requestJson)}," +
                    $"{Escape(responseJson)}," +
                    $"{Escape(stub.IsDerived.ToString())}," +
                    $"{Escape(stub.AssumptionNote)}," +
                    $"{Escape(string.Join("|", stub.TraceabilityChunkIds))}");
            }

            return sb.ToString();
        }

        public string ExportApiStubsAsOpenApiYaml()
        {
            var stubs = _vectorStoreService.GetApiStubs();
            var sb = new StringBuilder();

            sb.AppendLine("openapi: 3.0.3");
            sb.AppendLine("info:");
            sb.AppendLine("  title: Generated API Stub Specification");
            sb.AppendLine("  version: 1.0.0");
            sb.AppendLine("paths:");

            if (!stubs.Any())
            {
                sb.AppendLine("  {}");
                return sb.ToString();
            }

            foreach (var stub in stubs)
            {
                var method = string.IsNullOrWhiteSpace(stub.Method)
                    ? "post"
                    : stub.Method.ToLowerInvariant();

                var path = string.IsNullOrWhiteSpace(stub.Path)
                    ? "/undefined"
                    : stub.Path;

                sb.AppendLine($"  {path}:");
                sb.AppendLine($"    {method}:");
                sb.AppendLine($"      summary: \"{EscapeYaml(stub.Summary)}\"");
                sb.AppendLine("      description: |");
                sb.AppendLine($"        Derived: {stub.IsDerived}");
                sb.AppendLine($"        Assumption Note: {EscapeYaml(stub.AssumptionNote)}");
                sb.AppendLine($"        Traceability Chunk IDs: {string.Join(", ", stub.TraceabilityChunkIds)}");

                sb.AppendLine("      requestBody:");
                sb.AppendLine("        required: false");
                sb.AppendLine("        content:");
                sb.AppendLine("          application/json:");
                sb.AppendLine("            schema:");
                sb.AppendLine("              type: object");
                sb.AppendLine("            example:");
                sb.Append(FormatObjectAsIndentedYaml(stub.SampleRequestJson, 14));

                sb.AppendLine("      responses:");
                sb.AppendLine("        '200':");
                sb.AppendLine("          description: Successful response");
                sb.AppendLine("          content:");
                sb.AppendLine("            application/json:");
                sb.AppendLine("              schema:");
                sb.AppendLine("                type: object");
                sb.AppendLine("              example:");
                sb.Append(FormatObjectAsIndentedYaml(stub.SampleResponseJson, 16));
            }

            return sb.ToString();
        }

        private static string Escape(string? value)
        {
            value ??= string.Empty;
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        private static string EscapeYaml(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value.Replace("\"", "\\\"");
        }

        private static string FormatObjectAsIndentedYaml(object? value, int spaces)
        {
            var indent = new string(' ', spaces);
            var sb = new StringBuilder();

            if (value == null)
            {
                sb.AppendLine($"{indent}{{}}");
                return sb.ToString();
            }

            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var lines = json.Split(Environment.NewLine);

            foreach (var line in lines)
            {
                sb.AppendLine($"{indent}{line}");
            }

            return sb.ToString();
        }
    }
}