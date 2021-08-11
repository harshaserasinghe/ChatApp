using Azure.Messaging.ServiceBus;
using Chat.Common.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class AzureServiceBusService : IAzureServiceBusService
    {
        private readonly IConfiguration configuration;
        public readonly ServiceBusClient client;
        private readonly ServiceBusProcessor processor;

        public AzureServiceBusService()
        {
            //this.configuration = configuration;
            client = new ServiceBusClient("Endpoint=sb://chatdemo2.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oRtq3g6PSIST/DNKxZIwqf8huSu903//ezVoFKTe+YI=");
            //processor = client.CreateProcessor("chatqueue", new ServiceBusProcessorOptions());
            //processor.ProcessMessageAsync += MessageHandler;
            //processor.ProcessErrorAsync += ErrorHandler;
            //processor.StartProcessingAsync();
        }

        public async Task EnqueueAsync(Common.Models.Chat chatModel)
        {
            var sender = client.CreateSender("chatqueue");
            var message = new ServiceBusMessage(JsonSerializer.Serialize(chatModel));
            await sender.SendMessageAsync(message);
        }

        public async Task<Common.Models.Chat> DequeueAsync()
        {
            var receiver = client.CreateReceiver("chatqueue");
            var message = await receiver.ReceiveMessageAsync();
            await receiver.CompleteMessageAsync(message);
            var chatModel = JsonSerializer.Deserialize<Common.Models.Chat>(message.Body.ToString());
            return chatModel;
        }
        
        public async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var _chatModel = JsonSerializer.Deserialize<Common.Models.Chat>(args.Message.Body.ToString());
            await args.CompleteMessageAsync(args.Message);
        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
