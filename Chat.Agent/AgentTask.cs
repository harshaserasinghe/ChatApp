using Chat.Common.Exceptions;
using Chat.Common.Models;
using Chat.Service.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Agent
{
    public class AgentTask : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger<AgentTask> logger;
        private readonly ISupportService chatService;
        private readonly IAgentService teamService;

        public AgentTask(ILogger<AgentTask> logger,
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
            try
            {
                var team = teamService.GetAssignedTeamAsync().Result;

                if (team == null)
                {
                    Console.WriteLine("Team has not been assigned.");
                    return;
                }

                if (chatService.GetMessageCountAsync().Result < 1)
                {
                    Console.WriteLine("Support request queue is empty.");
                    return;
                }

                var supportRequest = chatService.DequeueSupportRequestAsync().Result;
                chatService.UpdateSupportRequestAsync(supportRequest, team.TeamId, 0).Wait();
                teamService.AssignSupportRequestToAgentAsync(supportRequest, team).Wait();
                GetTeamDetails(team);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
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

        public void GetTeamDetails(Team team)
        {
            foreach (var agent in team.Agents)
            {
                Console.WriteLine($"{agent.AgentId} {team.Name} {agent.Name} {agent.Level.ToString()} {agent.Queue.Count}");
            }

            if (team.HasOverflow)
            {
                foreach (var overFlowagent in team.OverTeamFlow.Agents)
                {
                    Console.WriteLine($"{overFlowagent.AgentId} {team.Name} {overFlowagent.Name} {overFlowagent.Level.ToString()} {overFlowagent.Queue.Count}");
                }
            }

            Console.WriteLine();
        }
    }
}
