using Chat.Common.Models;
using Chat.Service.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Agent
{
    public class ChatAgent : IHostedService, IDisposable
    {
        //private int executionCount = 0;
        private readonly ILogger<ChatAgent> _logger;
        private readonly IChatService chatService;
        private readonly IAgentService agentService;
        private Timer _timer;

        public ChatAgent(ILogger<ChatAgent> logger, IChatService chatService, IAgentService agentService)
        {
            _logger = logger;
            this.chatService = chatService;
            this.agentService = agentService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            var teamModel = new Team
            {
                Agents = new List<Common.Models.Agent>
                {
                    new Common.Models.Agent(1,"Mid1",Level.MidLevel),
                    new Common.Models.Agent(2,"Jr1",Level.Junior),
                    new Common.Models.Agent(3,"Mid2",Level.MidLevel),
                    new Common.Models.Agent(4,"Sern1",Level.Senior),
                    new Common.Models.Agent(5,"Jr2",Level.Junior),
                }
            };

            agentService.AssignTeam(teamModel);

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //var count = Interlocked.Increment(ref executionCount);
            var chatModel = chatService.GetChatAsync().Result;

            if(chatModel != null)
            {
                agentService.AssignChat(chatModel);
                agentService.ShowTeam();
                Console.WriteLine("--------------------------------------------------------------------");
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
