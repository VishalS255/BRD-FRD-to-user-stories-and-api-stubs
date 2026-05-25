namespace CapstoneProj.Models
{
    public class GenerateApiStubRequest
    {
        public string Query { get; set; } = "Generate API stubs from the uploaded requirement document.";
        public int TopK { get; set; } = 5;
    }
}