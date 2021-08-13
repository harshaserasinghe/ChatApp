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
        private readonly ITeamService teamService;

        public ChatAgent(ILogger<ChatAgent> logger,
            ISupportService chatService,
            ITeamService teamService)
        {
            this.logger = logger;
            this.chatService = chatService;
            this.teamService = teamService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //var count = Interlocked.Increment(ref executionCount);
            
            var team = teamService.GetAssignedTeamAsync().Result;

            if (team == null)
            {
                Console.WriteLine("No team");
                return;
            }

            var chat = chatService.DequeueSupportRequestAsync().Result;

            if (chat == null)
            {
                Console.WriteLine("No chats");
                return;
            }

            teamService.AssignChatToTeamAsync(chat, team).Wait();
            teamService.ShowTeam().Wait();
            Console.WriteLine();
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
