using Chat.Common.Exceptions;
using Chat.Common.Models;
using Chat.Service.Interfaces;
using Microsoft.Extensions.Options;
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

        public async Task EnqueueSupportRequestAsync(SupportRequest supportRequest) =>
            await azureServiceBusService.EnqueueAsync(supportRequest);

        public async Task<SupportRequest> DequeueSupportRequestAsync() =>
            await azureServiceBusService.DequeueAsync<SupportRequest>();

        public async Task<SupportRequest> GetSupportRequestAsync(string id) =>
          await cosmosDBService.GetEntityAsync<SupportRequest>(cosmoDBConfig.SupportRquestContainerId, id, id);

        public async Task AddSupportRequestAsync(SupportRequest supportRequest)
        {
            if (await IsCapacityExceededAsync())
            {
                throw new ServiceBusException(2, "Support request capacity has been exceeded.");
            }

            await cosmosDBService.AddEntityAsync(supportRequest, cosmoDBConfig.SupportRquestContainerId, supportRequest.Id);
            await EnqueueSupportRequestAsync(supportRequest);
        }

        public async Task AddSupportRequestsAsync(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                if (await IsCapacityExceededAsync())
                {
                    throw new ServiceBusException(2, "Support request capacity has been exceeded.");
                }

                var supportRequest = new SupportRequest(i, $"UserId {i}", $"Message {i}");
                await cosmosDBService.AddEntityAsync(supportRequest, cosmoDBConfig.SupportRquestContainerId, supportRequest.Id);
                await EnqueueSupportRequestAsync(supportRequest);
            }
        }

        public async Task UpdateSupportRequestAsync(SupportRequest supportRequest) =>
            await cosmosDBService.UpdateEntityAsync(supportRequest, cosmoDBConfig.SupportRquestContainerId, supportRequest.Id, supportRequest.Id);

        public async Task DeleteSupportAllRequestsAsync()
        {
            var query = "SELECT * FROM supportRequests";
            var supportRequests = await cosmosDBService.GetEntitiesAsync<SupportRequest>(cosmoDBConfig.SupportRquestContainerId, query);

            foreach (var supportRequest in supportRequests)
            {
                await cosmosDBService.DeleteEntityAsync<SupportRequest>(cosmoDBConfig.SupportRquestContainerId, supportRequest.Id, supportRequest.Id);
            }

            await azureServiceBusService.DequeueSupportRequestsAsync();
        }

        private async Task<bool> IsCapacityExceededAsync()
        {
            var messageCount = (await azureServiceBusService.GetMessageCountAsync()) + 1;
            var capacity = await agentService.GetCapacity();
            return capacity < messageCount;

        }

        public async Task<bool> IsSupportRequestsAvailableAsync() =>
            await azureServiceBusService.GetMessageCountAsync() > 0;
    }
}
