namespace CapstoneProj.Models
{
    public class ExtractRequirementsRequest
    {
        public string Query { get; set; } = "Extract actors, actions, rules, constraints, and unknowns from the uploaded requirement document.";
        public int TopK { get; set; } = 5;
    }
}