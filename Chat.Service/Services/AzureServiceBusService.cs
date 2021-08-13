using Azure.Messaging.ServiceBus;
using Chat.Common.Models;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class AzureServiceBusService : IAzureServiceBusService
    {
        private readonly AzureServiceBusConfig azureServiceBusConfig;
        public readonly ServiceBusClient client;
        private readonly ManagementClient manager;

        public AzureServiceBusService(IOptions<AzureServiceBusConfig> azureServiceBusConfig)
        {          
            this.azureServiceBusConfig = azureServiceBusConfig.Value;
            client = new ServiceBusClient(this.azureServiceBusConfig.ConnectionString);
            manager = new ManagementClient(this.azureServiceBusConfig.ConnectionString);        
        }

        public async Task EnqueueAsync(SupportRequest supportrequest)
        {
            var sender = client.CreateSender(azureServiceBusConfig.Queue);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(supportrequest));
            await sender.SendMessageAsync(message);
        }

        public async Task<SupportRequest> DequeueAsync()
        {
            var receiver = client.CreateReceiver(azureServiceBusConfig.Queue);

            if (await GetMessageCountAsync() == 0)
            {
                return null;
            }

            var message = await receiver.ReceiveMessageAsync();
            await receiver.CompleteMessageAsync(message);
            var supportrequest = JsonSerializer.Deserialize<SupportRequest>(message.Body.ToString());
            return supportrequest;
        }

        public async Task DequeuesupportRequestsAsync()
        {
            var receiver = client.CreateReceiver(azureServiceBusConfig.Queue);
            await foreach (var message in receiver.ReceiveMessagesAsync())
            {

                if (await GetMessageCountAsync() == 0)
                {
                    break;
                }

                await receiver.CompleteMessageAsync(message);
            }
        }

        public async Task<int> GetMessageCountAsync()=>       
          (int)(await manager.GetQueueRuntimeInfoAsync(azureServiceBusConfig.Queue)).MessageCountDetails.ActiveMessageCount;
           
    }
}
