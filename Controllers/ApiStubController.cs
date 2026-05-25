using CapstoneProj.Models;
using CapstoneProj.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiStubController : ControllerBase
    {
        private readonly RetrievalService _retrievalService;
        private readonly RequirementExtractionService _requirementExtractionService;
        private readonly ApiStubGenerationService _apiStubGenerationService;
        private readonly VectorStoreService _vectorStoreService;

        public ApiStubController(
            RetrievalService retrievalService,
            RequirementExtractionService requirementExtractionService,
            ApiStubGenerationService apiStubGenerationService,
            VectorStoreService vectorStoreService)
        {
            _retrievalService = retrievalService;
            _requirementExtractionService = requirementExtractionService;
            _apiStubGenerationService = apiStubGenerationService;
            _vectorStoreService = vectorStoreService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateApiStubs([FromBody] GenerateApiStubRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("Query is required.");

            var chunks = await _retrievalService.SearchAsync(request.Query, request.TopK);
            var facts = await _requirementExtractionService.ExtractFactsAsync(chunks);
            var stubs = await _apiStubGenerationService.GenerateApiStubsAsync(facts);

            _vectorStoreService.AddRequirementFacts(facts);
            _vectorStoreService.AddApiStubs(stubs);

            return Ok(new
            {
                retrievedChunkCount = chunks.Count,
                factCount = facts.Count,
                apiStubCount = stubs.Count,
                apiStubs = stubs
            });
        }

        [HttpGet]
        public IActionResult GetApiStubs()
        {
            return Ok(_vectorStoreService.GetApiStubs());
        }
    }
}