using CapstoneProj.Models;
using CapstoneProj.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequirementController : ControllerBase
    {
        private readonly RetrievalService _retrievalService;
        private readonly RequirementExtractionService _requirementExtractionService;
        private readonly VectorStoreService _vectorStoreService;

        public RequirementController(
            RetrievalService retrievalService,
            RequirementExtractionService requirementExtractionService,
            VectorStoreService vectorStoreService)
        {
            _retrievalService = retrievalService;
            _requirementExtractionService = requirementExtractionService;
            _vectorStoreService = vectorStoreService;
        }

        [HttpPost("extract")]
        public async Task<IActionResult> ExtractRequirements([FromBody] ExtractRequirementsRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("Query is required.");

            var chunks = await _retrievalService.SearchAsync(request.Query, request.TopK);
            var facts = await _requirementExtractionService.ExtractFactsAsync(chunks);

            _vectorStoreService.AddRequirementFacts(facts);

            return Ok(new
            {
                retrievedChunkCount = chunks.Count,
                factCount = facts.Count,
                facts
            });
        }

        [HttpGet]
        public IActionResult GetRequirementFacts()
        {
            return Ok(_vectorStoreService.GetRequirementFacts());
        }
    }
}