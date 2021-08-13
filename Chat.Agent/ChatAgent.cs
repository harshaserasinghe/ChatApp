using Chat.Common.Models;
using Chat.Service.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Agent
{
    public class ChatAgent : IHostedService, IDisposable
    {
        private Timer _timer;
        //private int executionCount = 0;
        private readonly ILogger<ChatAgent> logger;
        private readonly ISupportService chatService;
        private readonly IAgentService teamService;

        public ChatAgent(ILogger<ChatAgent> logger,
            ISupportService chatService,
            IAgentService teamService)
        {
            this.logger = logger;
            this.chatService = chatService;
            this.teamService = teamService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //var count = Interlocked.Increment(ref executionCount);


            var team = teamService.GetAssignedTeamAsync().Result;

            if (team == null)
            {
                Console.WriteLine("Team has not been assigned.");
                return;
            }

            var supportRequest = chatService.DequeueSupportRequestAsync().Result;

            if (supportRequest == null)
            {
                Console.WriteLine("Support request queue is empty");
                return;
            }

            teamService.AssignSupportRequestToTeamAsync(supportRequest, team).Wait();
            GetTeamDetails(team);
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

        public void GetTeamDetails(Team team)
        {
            foreach (var agent in team.Agents)
            {
                Console.WriteLine($"{agent.AgentId} {team.Name} {agent.Name} {agent.Level.ToString()} {agent.Queue.Count}");
            }
            Console.WriteLine();
        }
    }
}
