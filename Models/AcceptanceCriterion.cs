namespace CapstoneProj.Models
{
    public class AcceptanceCriterion
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Given { get; set; } = string.Empty;
        public string When { get; set; } = string.Empty;
        public string Then { get; set; } = string.Empty;

        public List<string> TraceabilityChunkIds { get; set; } = new();
        public List<TraceabilityReference> TraceabilityReferences { get; set; } = new();
    }
}