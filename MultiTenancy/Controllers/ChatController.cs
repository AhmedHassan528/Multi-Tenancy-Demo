using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Services.TrafficServices;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly OpenAIClient _openAiClient;
        private readonly ITrafficServices _trafficServices;

        public ChatController(OpenAIClient openAiClient, ITrafficServices trafficServices)
        {
            _openAiClient = openAiClient;
            _trafficServices = trafficServices;
        }

        [HttpPost]
        public async Task<IActionResult> GetChatResponse([FromBody] ChatRequest request)
        {
            await _trafficServices.AddReqCountAsync();

            var options = new ChatCompletionsOptions
            {
                DeploymentName = "gpt-4o",
                Messages =
            {
                new ChatRequestSystemMessage(
                    "You are a fashion expert assistant for a business. Answer only within the domain of fashion, including trends, styles, clothing, accessories, and industry insights. Do not respond to questions outside this scope, and politely redirect users to fashion topics if they stray."
                ),
                new ChatRequestUserMessage(request.Message)
            },
                Temperature = 0.7f,
                MaxTokens = 800
            };

            var response = await _openAiClient.GetChatCompletionsAsync(options);
            var reply = response.Value.Choices[0].Message.Content;

            return Ok(new { Response = reply });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
