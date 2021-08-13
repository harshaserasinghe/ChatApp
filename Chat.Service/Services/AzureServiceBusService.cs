using Azure.Messaging.ServiceBus;
using Chat.Common.Models;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

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

        public async Task EnqueueAsync(Common.Models.Chat chat)
        {
            var sender = client.CreateSender(azureServiceBusConfig.Queue);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(chat));
            await sender.SendMessageAsync(message);
        }

        public async Task<Common.Models.Chat> DequeueAsync()
        {
            var receiver = client.CreateReceiver(azureServiceBusConfig.Queue);

            if (GetActiveMessageCount() == 0)
            {
                return null;
            }

            var message = await receiver.ReceiveMessageAsync();
            await receiver.CompleteMessageAsync(message);
            var chat = JsonSerializer.Deserialize<Common.Models.Chat>(message.Body.ToString());
            return chat;
        }

        //public async Task DequeueAllAsync()
        //{
        //    var receiver = client.CreateReceiver(azureServiceBusConfig.Queue);
        //    await foreach (var message in receiver.ReceiveMessagesAsync())
        //    {

        //        if (GetActiveMessageCount() == 0)
        //        {
        //            break;
        //        }

        //        await receiver.CompleteMessageAsync(message);
        //    }
        //}

        public int GetActiveMessageCount() =>
             (int)manager.GetQueueRuntimeInfoAsync(azureServiceBusConfig.Queue).Result.MessageCountDetails.ActiveMessageCount;
    }
}
