using Chat.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class ChatService : IChatService
    {
        private readonly IAzureServiceBusService azureServiceBusService;

        public ChatService(IAzureServiceBusService azureServiceBusService)
        {
            this.azureServiceBusService = azureServiceBusService;
        }

        public async Task SetChatAsync(Common.Models.Chat chatModel)
        {
            await azureServiceBusService.EnqueueAsync(chatModel);
        }

        public async Task<Common.Models.Chat> GetChatAsync()
        {
            return await azureServiceBusService.DequeueAsync();
        }      
    }
}
