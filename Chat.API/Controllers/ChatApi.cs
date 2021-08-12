using Chat.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Chat.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatApi : ControllerBase
    {
        private readonly ILogger<ChatApi> logger;
        private readonly IChatService chatService;

        public ChatApi(ILogger<ChatApi> logger,
            IChatService chatService)
        {
            this.logger = logger;
            this.chatService = chatService;
        }

        [Route("GetChat")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string id)
        {
            //var chat = await chatService.GetChatAsync(id);
            var chat = await chatService.DequeueAsync();
            return Ok(chat);
        }

        [Route("AddChat")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(Common.Models.Chat chat)
        {
            await chatService.AddChatAsync(chat);
            return Ok();
        }

        [Route("AddChats")]
        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            await chatService.AddChatsAsync();
            return Ok();
        }

        [Route("AssignChat")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(string id, int teamId, int agentId)
        {
            await chatService.AssignChatAsync(id, teamId, agentId);
            return Ok();
        }

        [Route("DeleteChats")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync()
        {
            await chatService.DeleteChatsAsync();
            return Ok();
        }
    }
}
