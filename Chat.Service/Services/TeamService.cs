using Chat.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class TeamService : ITeamService
    {
        private readonly ILogger<TeamService> logger;
        private readonly CosmoDBConfig cosmoDBConfig;
        private readonly ICosmosDBService cosmosDBService;
        //private readonly IChatService chatService;

        public TeamService(ILogger<TeamService> logger,
            IOptions<CosmoDBConfig> cosmoDBConfig,
            ICosmosDBService cosmosDBService)
            //IChatService chatService)
        {
            this.logger = logger;
            this.cosmoDBConfig = cosmoDBConfig.Value;
            this.cosmosDBService = cosmosDBService;
            //this.chatService = chatService;
        }

        public async Task<Team> GetTeamAsync(string id) =>
           await cosmosDBService.GetEntity<Team>(cosmoDBConfig.TeamContainerId, id, id);

        public async Task AddTeamsAsync()
        {
            var team = new Team(1, "Team A", Shift.Morning, new List<Agent>
            {
                new Agent(1, "Udara", Level.Junior),
                new Agent(2, "Chathuranga", Level.MidLevel),
                new Agent(3, "Bassnayaka", Level.MidLevel),
                new Agent(4, "Chanaka", Level.TeamLead)
            });


            var team2 = new Team(2, "Team B", Shift.Afternoon, new List<Agent>
            {
                new Agent(4, "Hasitha", Level.Junior),
                new Agent(5, "Piyumi", Level.Junior),
                new Agent(6, "Pavithra", Level.MidLevel),
                new Agent(7, "Amila", Level.Senior)
            });


            var team3 = new Team(3, "Team C", Shift.Night, new List<Agent>
            {
                new Agent(8, "Harsha", Level.MidLevel),
                new Agent(9, "Uthpala", Level.MidLevel),
            });

            await cosmosDBService.AddEntity(team, cosmoDBConfig.TeamContainerId, team.Id);
            await cosmosDBService.AddEntity(team2, cosmoDBConfig.TeamContainerId, team2.Id);
            await cosmosDBService.AddEntity(team3, cosmoDBConfig.TeamContainerId, team3.Id);
        }

        public async Task ChangeShiftAsync(string id)
        {
            Team currentTeam = await GetAssignedTeamAsync();

            if (currentTeam != null)
            {
                currentTeam.IsAssigned = false;
                currentTeam.Agents.ForEach(agent => agent.Queue.Clear());
                await cosmosDBService.UpdateEntity(currentTeam, cosmoDBConfig.TeamContainerId, currentTeam.Id, currentTeam.Id);
            }

            var newTeam = await cosmosDBService.GetEntity<Team>(cosmoDBConfig.TeamContainerId, id, id);
            newTeam.IsAssigned = true;
            await cosmosDBService.UpdateEntity(newTeam, cosmoDBConfig.TeamContainerId, newTeam.Id, newTeam.Id);
        }

        public async Task<Team> GetAssignedTeamAsync()
        {
            var query = "SELECT * FROM team WHERE team.IsAssigned = true";
            return (await cosmosDBService.GetEntities<Team>(cosmoDBConfig.TeamContainerId, query)).FirstOrDefault();
        }

        public async Task AssignChatToTeamAsync(Common.Models.SupportRequest chat, Team team)
        {
            team.Agents = team.Agents
                .OrderBy(agent => agent.Level).ThenBy(agent => agent.Queue.Count).ToList();

            foreach (var agentModel in team.Agents)
            {
                if (!agentModel.IsCapacityExceeded(agentModel.Multiplier))
                {
                    if (agentModel.Level.Equals(Level.Junior))
                    {
                        await AssignChatToAgentAsync(chat, team, agentModel);
                        break;
                    }
                    else if (agentModel.Level.Equals(Level.MidLevel))
                    {
                        await AssignChatToAgentAsync(chat, team, agentModel);
                        break;
                    }
                    else if (agentModel.Level.Equals(Level.Senior))
                    {
                        await AssignChatToAgentAsync(chat, team, agentModel);
                        break;
                    }
                    else if (agentModel.Level.Equals(Level.TeamLead))
                    {
                        await AssignChatToAgentAsync(chat, team, agentModel);
                        break;
                    }
                }
            }

            team.Agents = team.Agents
                .OrderBy(agent => agent.Level).ThenBy(agent => agent.Queue.Count).ToList();

            await cosmosDBService.UpdateEntity<Team>(team, cosmoDBConfig.TeamContainerId, team.Id, team.Id);
        }

        private async Task AssignChatToAgentAsync(Common.Models.SupportRequest chat, Team team, Agent agentModel)
        {
            //await chatService.AssignChatAsync(chat.Id, team.TeamId, agentModel.AgentId);
            agentModel.Queue.Enqueue(chat);
        }

        public int GetCapacity()
        {
            var team  =  GetAssignedTeamAsync().Result;

            double capacity = 0;

            team.Agents.ForEach(agent =>
            {
                capacity = capacity + (10 * agent.Multiplier);
            });

            return (int)Math.Floor(capacity * 1.5);
        }

        public async Task ShowTeam()
        {

            var team = await GetAssignedTeamAsync();

            foreach (var agent in team.Agents)
            {
                Console.WriteLine($"{agent.AgentId} {team.Name} {agent.Name} {agent.Level.ToString()} {agent.Queue.Count}");
            }
        }
    }
}
