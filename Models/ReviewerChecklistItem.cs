namespace CapstoneProj.Models
{
    public class ReviewerChecklistItem
    {
        public string Category { get; set; } = string.Empty;
        public string Check { get; set; } = string.Empty;
        public string ExpectedResult { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
    }
}