using Azure.Messaging.ServiceBus;
using Chat.Common.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class AzureServiceBusService : IAzureServiceBusService
    {
        private readonly AzureServiceBusConfig azureServiceBusConfig;
        public readonly ServiceBusClient client;

        public AzureServiceBusService(IOptions<AzureServiceBusConfig> azureServiceBusConfig)
        {
            this.azureServiceBusConfig = azureServiceBusConfig.Value;
            client = new ServiceBusClient(this.azureServiceBusConfig.ConnectionString);

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

            if (await receiver.PeekMessageAsync() == null)
            {
                return null;
            }

            var message = await receiver.ReceiveMessageAsync();
            await receiver.CompleteMessageAsync(message);
            var chat = JsonSerializer.Deserialize<Common.Models.Chat>(message.Body.ToString());
            return chat;
        }
    }
}
