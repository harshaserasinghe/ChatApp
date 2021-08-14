using Azure.Messaging.ServiceBus;
using Chat.Common.Exceptions;
using Chat.Common.Models;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using System;
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

        public async Task EnqueueAsync<T>(T supportrequest)
        {
            var sender = client.CreateSender(azureServiceBusConfig.Queue);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(supportrequest));
            await sender.SendMessageAsync(message);
        }

        public async Task<T> DequeueAsync<T>()
        {
            var receiver = client.CreateReceiver(azureServiceBusConfig.Queue);

            if (await GetMessageCountAsync() == 0)
            {
                throw new Common.Exceptions.ServiceBusException(1,"Support request queue is empty.");
            }

            var message = await receiver.ReceiveMessageAsync();
            await receiver.CompleteMessageAsync(message);
            var supportrequest = JsonSerializer.Deserialize<T>(message.Body.ToString());
            return supportrequest;
        }

        public async Task DequeueSupportRequestsAsync()
        {
            var receiver = client.CreateReceiver(azureServiceBusConfig.Queue);

            while (0 < (await GetMessageCountAsync()))
            {
                var message = await receiver.ReceiveMessageAsync();
                await receiver.CompleteMessageAsync(message);
            }
        }

        public async Task<int> GetMessageCountAsync()=>       
          (int)(await manager.GetQueueRuntimeInfoAsync(azureServiceBusConfig.Queue)).MessageCountDetails.ActiveMessageCount;
    }
}
