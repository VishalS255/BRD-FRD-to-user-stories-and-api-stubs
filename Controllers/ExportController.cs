using CapstoneProj.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CapstoneProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly ExportService _exportService;

        public ExportController(ExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet("json")]
        public IActionResult ExportJson()
        {
            var json = _exportService.ExportAllAsJson();
            return File(Encoding.UTF8.GetBytes(json), "application/json", "export.json");
        }

        [HttpGet("stories-csv")]
        public IActionResult ExportStoriesCsv()
        {
            var csv = _exportService.ExportStoriesAsCsv();
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", "stories.csv");
        }

        [HttpGet("acceptance-criteria-csv")]
        public IActionResult ExportAcceptanceCriteriaCsv()
        {
            var csv = _exportService.ExportAcceptanceCriteriaAsCsv();
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", "acceptance-criteria.csv");
        }

        [HttpGet("api-stubs-csv")]
        public IActionResult ExportApiStubsCsv()
        {
            var csv = _exportService.ExportApiStubsAsCsv();
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", "api-stubs.csv");
        }

        [HttpGet("openapi-yaml")]
        public IActionResult ExportOpenApiYaml()
        {
            var yaml = _exportService.ExportApiStubsAsOpenApiYaml();
            return File(Encoding.UTF8.GetBytes(yaml), "application/x-yaml", "openapi-stubs.yaml");
        }
    }
}