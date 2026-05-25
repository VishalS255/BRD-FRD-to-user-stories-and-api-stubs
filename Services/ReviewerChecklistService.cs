using CapstoneProj.Models;

namespace CapstoneProj.Services
{
    public class ReviewerChecklistService
    {
        public List<ReviewerChecklistItem> GetChecklist()
        {
            return new List<ReviewerChecklistItem>
            {
                new ReviewerChecklistItem
                {
                    Category = "Grounding & Evidence",
                    Check = "Each user story has traceability references.",
                    ExpectedResult = "Story includes chunk id, source file name, and evidence snippet.",
                    IsMandatory = true
                },
                new ReviewerChecklistItem
                {
                    Category = "Grounding & Evidence",
                    Check = "Each API stub has traceability references.",
                    ExpectedResult = "API stub includes source evidence from uploaded document.",
                    IsMandatory = true
                },
                new ReviewerChecklistItem
                {
                    Category = "No Unsupported Assumptions",
                    Check = "Derived outputs are marked clearly.",
                    ExpectedResult = "isDerived and assumptionNote are populated when output is inferred.",
                    IsMandatory = true
                },
                new ReviewerChecklistItem
                {
                    Category = "Acceptance Criteria",
                    Check = "Each user story has acceptance criteria.",
                    ExpectedResult = "Acceptance criteria use Given/When/Then format.",
                    IsMandatory = true
                },
                new ReviewerChecklistItem
                {
                    Category = "Retrieval Quality",
                    Check = "Retrieved chunks are relevant to the query.",
                    ExpectedResult = "Search results should relate to uploaded requirement content.",
                    IsMandatory = true
                },
                new ReviewerChecklistItem
                {
                    Category = "Export",
                    Check = "Artifacts can be exported.",
                    ExpectedResult = "JSON and CSV exports are available.",
                    IsMandatory = false
                },
                new ReviewerChecklistItem
                {
                    Category = "Metrics",
                    Check = "System metrics are available.",
                    ExpectedResult = "Metrics endpoint returns documents, chunks, facts, stories, and API stub counts.",
                    IsMandatory = false
                }
            };
        }
    }
}