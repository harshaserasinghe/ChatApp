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
        private readonly IAgentService agentService;

        public SupportService(IOptions<CosmoDBConfig> cosmoDBConfig,
            ICosmosDBService cosmosDBService,
            IAzureServiceBusService azureServiceBusService,
            IAgentService agentService)
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

        public async Task UpdateSupportRequestAsync(SupportRequest supportRequest, int teamId, int agentId)
        {
            supportRequest.IsAssign = true;
            supportRequest.TeamId = teamId;
            supportRequest.AgentId = agentId;
            await cosmosDBService.UpdateEntity(supportRequest, cosmoDBConfig.SupportRquestContainerId, supportRequest.Id, supportRequest.Id);
        }

        public async Task DeleteSupportRequestsAsync()
        {
            var query = "SELECT * FROM supportRequests";
            var supportRequests = await cosmosDBService.GetEntities<SupportRequest>(cosmoDBConfig.SupportRquestContainerId, query);

            foreach (var supportRequest in supportRequests)
            {
                await cosmosDBService.DeleteEntity<SupportRequest>(cosmoDBConfig.SupportRquestContainerId, supportRequest.Id, supportRequest.Id);
            }

            await azureServiceBusService.DequeueSupportRequestsAsync();
        }

        private async Task<bool> IsCapacityExceededAsync(int count)
        {
            var messageCount = (await azureServiceBusService.GetMessageCountAsync()) + count;
            var capacity =  await agentService.GetCapacity();

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
