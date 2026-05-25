namespace CapstoneProj.Models
{
    public class ApiStub
    {
        public string StubId { get; set; } = Guid.NewGuid().ToString();
        public string Method { get; set; } = "POST";
        public string Path { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;

        public object? SampleRequestJson { get; set; }
        public object? SampleResponseJson { get; set; }

        public bool IsDerived { get; set; }
        public string AssumptionNote { get; set; } = string.Empty;

        public List<string> TraceabilityChunkIds { get; set; } = new();
        public List<TraceabilityReference> TraceabilityReferences { get; set; } = new();
    }
}