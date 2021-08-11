using Chat.Common.Models;
using Chat.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatApi : ControllerBase
    {
        private readonly IChatService chatService;
        private readonly IAzureRedisService azureRedisService;

        public ChatApi(IChatService chatService, IAzureRedisService azureRedisService)
        {
            this.chatService = chatService;
            this.azureRedisService = azureRedisService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Common.Models.Chat chatModel)
        {
            for (int i = 0; i < 10; i++)
            {
                chatModel = new Common.Models.Chat { Id = i, UserId = $"UserId {i}", Message = $"Message {i}" };
                azureRedisService.SetEntity(chatModel.Id.ToString(), chatModel);
                //await chatService.SetChatAsync(chatModel);
            }

            //await chatService.SetChatAsync(chatModel);
            azureRedisService.SetEntity(chatModel.Id.ToString(), chatModel);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int id)
        {
            //var chatModel =  await chatService.GetChatAsync();
            var chatModel = azureRedisService.GetEntity<Common.Models.Chat>(id.ToString());
            return Ok(chatModel);
        }
    }
}
