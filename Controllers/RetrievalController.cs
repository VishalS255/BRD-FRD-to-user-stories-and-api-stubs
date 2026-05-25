using CapstoneProj.Models;
using CapstoneProj.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RetrievalController : ControllerBase
    {
        private readonly RetrievalService _retrievalService;

        public RetrievalController(RetrievalService retrievalService)
        {
            _retrievalService = retrievalService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("Query is required.");

            var results = await _retrievalService.SearchAsync(request.Query, request.TopK);
            return Ok(results);
        }
    }
}