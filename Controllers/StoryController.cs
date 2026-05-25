using CapstoneProj.Models;
using CapstoneProj.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoryController : ControllerBase
    {
        private readonly RetrievalService _retrievalService;
        private readonly RequirementExtractionService _requirementExtractionService;
        private readonly StoryGenerationService _storyGenerationService;
        private readonly VectorStoreService _vectorStoreService;

        public StoryController(
            RetrievalService retrievalService,
            RequirementExtractionService requirementExtractionService,
            StoryGenerationService storyGenerationService,
            VectorStoreService vectorStoreService)
        {
            _retrievalService = retrievalService;
            _requirementExtractionService = requirementExtractionService;
            _storyGenerationService = storyGenerationService;
            _vectorStoreService = vectorStoreService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateStories([FromBody] GenerateStoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("Query is required.");

            var chunks = await _retrievalService.SearchAsync(request.Query, request.TopK);
            var facts = await _requirementExtractionService.ExtractFactsAsync(chunks);
            var stories = await _storyGenerationService.GenerateStoriesAsync(facts);

            _vectorStoreService.AddRequirementFacts(facts);
            _vectorStoreService.AddStories(stories);

            return Ok(new
            {
                retrievedChunkCount = chunks.Count,
                factCount = facts.Count,
                storyCount = stories.Count,
                stories
            });
        }

        [HttpGet]
        public IActionResult GetStories()
        {
            return Ok(_vectorStoreService.GetStories());
        }
    }
}