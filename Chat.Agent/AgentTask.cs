using Chat.Common.Models;
using Chat.Service.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Agent
{
    public class AgentTask : IHostedService, IDisposable
    {
        private long executionCount = 0;
        private Timer _timer;
        private readonly ILogger<AgentTask> logger;
        private readonly ISupportService chatService;
        private readonly IAgentService agentService;

        public AgentTask(ILogger<AgentTask> logger,
            ISupportService chatService,
            IAgentService agentService)
        {
            this.logger = logger;
            this.chatService = chatService;
            this.agentService = agentService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Agent service started.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                if (Interlocked.Read(ref executionCount) > 10)
                {
                    StopAsync(new CancellationToken()).Wait();
                }

                var team = agentService.GetAssignedTeamAsync().Result;

                if (team == null)
                {
                    Console.WriteLine("Agents are not assigned.");
                    return;
                }

                if (!chatService.IsSupportRequestsAvailableAsync().Result)
                {
                    Console.WriteLine("There are no support requests.");
                    Interlocked.Increment(ref executionCount);
                    return;
                }

                if (agentService.IsAllAgentsBusy(team))
                {
                    Console.WriteLine("Agents are not busy.");
                    return;
                }

                Interlocked.Exchange(ref executionCount, 0);
                var supportRequest = chatService.DequeueSupportRequestAsync().Result;
                agentService.AssignSupportRequestToAgentAsync(supportRequest, team).Wait();
                chatService.UpdateSupportRequestAsync(supportRequest).Wait();
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
            Console.WriteLine("Agent service ended.");
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
