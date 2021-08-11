using Chat.Service;
using Chat.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Agent
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ChatAgent>();
                    services.AddSingleton<IChatService, ChatService>();
                    services.AddSingleton<IAgentService,AgentService>();
                    services.AddSingleton<IAzureServiceBusService, AzureServiceBusService>();
                })
                .Build();

            await host.StartAsync();
            await host.WaitForShutdownAsync();
        }
    }
}
