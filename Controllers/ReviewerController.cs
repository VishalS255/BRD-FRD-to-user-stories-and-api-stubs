using CapstoneProj.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewerController : ControllerBase
    {
        private readonly ReviewerChecklistService _reviewerChecklistService;

        public ReviewerController(ReviewerChecklistService reviewerChecklistService)
        {
            _reviewerChecklistService = reviewerChecklistService;
        }

        [HttpGet("checklist")]
        public IActionResult GetChecklist()
        {
            return Ok(_reviewerChecklistService.GetChecklist());
        }
    }
}