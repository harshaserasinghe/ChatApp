using Chat.Common.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class ChatService : IChatService
    {
        private readonly IAzureServiceBusService azureServiceBusService;
        private readonly CosmoDBConfig cosmoDBConfig;
        private readonly ICosmosDBService cosmosDBService;

        public ChatService(IOptions<CosmoDBConfig> cosmoDBConfig,
            ICosmosDBService cosmosDBService,
            IAzureServiceBusService azureServiceBusService)
        {
            this.cosmoDBConfig = cosmoDBConfig.Value;
            this.cosmosDBService = cosmosDBService;
            this.azureServiceBusService = azureServiceBusService;
        }

        public async Task<Common.Models.Chat> GetChatAsync(string id) =>
          await cosmosDBService.GetEntity<Common.Models.Chat>(cosmoDBConfig.ChatContainerId, id, id);

        public async Task EnqueueAsync(Common.Models.Chat chat) =>
            await azureServiceBusService.EnqueueAsync(chat);


        public async Task<Common.Models.Chat> DequeueAsync() =>
            await azureServiceBusService.DequeueAsync();


        public async Task AddChatAsync(Common.Models.Chat chat)
        {
            await cosmosDBService.AddEntity(chat, cosmoDBConfig.ChatContainerId, chat.Id);
            await EnqueueAsync(chat);
        }

        public async Task AddChatsAsync()
        {           
            for (int i = 1; i <= 20; i++)
            {
                var chat = new Common.Models.Chat(i, $"UserId {i}", $"Message {i}");
                await cosmosDBService.AddEntity(chat, cosmoDBConfig.ChatContainerId, chat.Id);
                await EnqueueAsync(chat);
            }
        }

        public async Task AssignChatAsync(string id, int teamId, int agentId)
        {
            var chat = await GetChatAsync(id);
            chat.IsAssign = true;
            chat.TeamId = teamId;
            chat.AgentId = agentId;
            await cosmosDBService.UpdateEntity(chat, cosmoDBConfig.ChatContainerId, chat.Id, chat.Id);
        }

        public async Task DeleteChatsAsync()
        {
            var query = "SELECT * FROM chat";
            var chats = await cosmosDBService.GetEntities<Common.Models.Chat>(cosmoDBConfig.ChatContainerId, query);

            foreach (var chat in chats)
            {
                await cosmosDBService.DeleteEntity<Common.Models.Chat>(cosmoDBConfig.ChatContainerId, chat.Id, chat.Id);
            }
        }
    }
}
