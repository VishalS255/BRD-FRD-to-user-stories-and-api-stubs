namespace CapstoneProj.Models
{
    public class UserStory
    {
        public string StoryId { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string AsA { get; set; } = string.Empty;
        public string IWant { get; set; } = string.Empty;
        public string SoThat { get; set; } = string.Empty;

        public bool IsDerived { get; set; }
        public string AssumptionNote { get; set; } = string.Empty;

        public List<AcceptanceCriterion> AcceptanceCriteria { get; set; } = new();

        public List<string> TraceabilityChunkIds { get; set; } = new();
        public List<TraceabilityReference> TraceabilityReferences { get; set; } = new();
    }
}