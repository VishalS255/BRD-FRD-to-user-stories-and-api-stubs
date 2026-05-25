using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;

namespace CapstoneProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly OpenAIClient _client;

        public TestController(OpenAIClient client)
        {
            _client = client;
        }

        [HttpGet("ping-openai")]
        public async Task<IActionResult> PingOpenAI()
        {
            var chatClient = _client.GetChatClient("gpt-4.1-mini");
            var response = await chatClient.CompleteChatAsync("Say exactly: OpenAI connection successful");
            return Ok(new { message = response.Value.Content[0].Text });
        }
    }
}