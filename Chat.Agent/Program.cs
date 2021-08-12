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
                    services.AddHostedService<ChatAgent>();
                    services.AddSingleton<IChatService, ChatService>();
                    services.AddSingleton<ITeamService, TeamService>();
                    services.AddSingleton<IAzureServiceBusService, AzureServiceBusService>();
                    services.AddSingleton<IAzureServiceBusService, AzureServiceBusService>();
                    services.AddSingleton<IAzureRedisService, AzureRedisService>();
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
