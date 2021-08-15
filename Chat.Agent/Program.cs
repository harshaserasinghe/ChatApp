using Chat.Common.Models;
using Chat.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                    services.AddHostedService<AgentTask>();
                    services.AddSingleton<ISupportService, SupportService>();
                    services.AddSingleton<IAgentService, AgentService>();
                    services.AddSingleton<IAzureServiceBusService, AzureServiceBusService>();
                    services.AddSingleton<ICosmosDBService, CosmosDBService>();
                    services.Configure<AzureServiceBusConfig>(hostContext.Configuration.GetSection("AzureServiceBus"));
                    services.Configure<CosmoDBConfig>(hostContext.Configuration.GetSection("AzureCosmosDB"));
                })
                .Build();

            await host.StartAsync();
            await host.WaitForShutdownAsync();
        }
    }
}
