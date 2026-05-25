namespace CapstoneProj.Models
{
    public class GenerateStoryRequest
    {
        public string Query { get; set; } = "Generate user stories from the uploaded requirement document.";
        public int TopK { get; set; } = 5;
    }
}