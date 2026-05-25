namespace CapstoneProj.Models
{
    public class RequirementFact
    {
        public string FactId { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } = string.Empty; 
        // Actor, Action, Rule, Constraint, Unknown

        public string Value { get; set; } = string.Empty;

        public List<string> TraceabilityChunkIds { get; set; } = new();

        public string SourceFileName { get; set; } = string.Empty;

        public string EvidenceSnippet { get; set; } = string.Empty;
    }
}