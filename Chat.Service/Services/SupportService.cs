using Chat.Common.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class SupportService : ISupportService
    {
        private readonly CosmoDBConfig cosmoDBConfig;
        private readonly IAzureServiceBusService azureServiceBusService;
        private readonly ICosmosDBService cosmosDBService;
        private readonly ITeamService agentService;

        public SupportService(IOptions<CosmoDBConfig> cosmoDBConfig,
            ICosmosDBService cosmosDBService,
            IAzureServiceBusService azureServiceBusService,
            ITeamService agentService)
        {
            this.cosmoDBConfig = cosmoDBConfig.Value;
            this.cosmosDBService = cosmosDBService;
            this.azureServiceBusService = azureServiceBusService;
            this.agentService = agentService;
        }
        
        public async Task EnqueueSupportRequestAsync(SupportRequest supportRequest)=>           
            await azureServiceBusService.EnqueueAsync(supportRequest);
        
        public async Task<SupportRequest> DequeueSupportRequestAsync() =>
            await azureServiceBusService.DequeueAsync();

        public async Task<SupportRequest> GetSupportRequestAsync(string id) =>
          await cosmosDBService.GetEntity<SupportRequest>(cosmoDBConfig.SupportRquestContainerId, id, id);

        public async Task AddSupportRequestAsync(SupportRequest supportRequest)
        {
            if (await IsCapacityExceededAsync(1))
            {
                throw new Exception("Support request capacity has been exceeded.");
            }

            await cosmosDBService.AddEntity(supportRequest, cosmoDBConfig.SupportRquestContainerId, supportRequest.Id);
            await EnqueueSupportRequestAsync(supportRequest);
        }

        public async Task AddSupportRequestsAsync(int count)
        {
            if (await IsCapacityExceededAsync(count))
            {
                throw new Exception("Support request capacity has been exceeded.");
            }

            for (int i = 1; i <= count; i++)
            {
                var supportRequest = new SupportRequest(i, $"UserId {i}", $"Message {i}");
                await cosmosDBService.AddEntity(supportRequest, cosmoDBConfig.SupportRquestContainerId, supportRequest.Id);
                await EnqueueSupportRequestAsync(supportRequest);
            }
        }

        //need to optimize
        public async Task AssignSupportRequestAsync(string id, int teamId, int agentId)
        {
            var chat = await GetSupportRequestAsync(id);
            chat.IsAssign = true;
            chat.TeamId = teamId;
            chat.AgentId = agentId;
            await cosmosDBService.UpdateEntity(chat, cosmoDBConfig.SupportRquestContainerId, chat.Id, chat.Id);
        }

        public async Task DeleteSupportRequestsAsync()
        {
            var query = "SELECT * FROM supportRequests";
            var supportRequests = await cosmosDBService.GetEntities<SupportRequest>(cosmoDBConfig.SupportRquestContainerId, query);

            foreach (var supportRequest in supportRequests)
            {
                await cosmosDBService.DeleteEntity<SupportRequest>(cosmoDBConfig.SupportRquestContainerId, supportRequest.Id, supportRequest.Id);
            }

            await azureServiceBusService.DequeuesupportRequestsAsync();
        }

        private async Task<bool> IsCapacityExceededAsync(int count)
        {
            var messageCount = (await azureServiceBusService.GetMessageCountAsync()) + count;
            var capacity =  agentService.GetCapacity();

            if (capacity < messageCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
