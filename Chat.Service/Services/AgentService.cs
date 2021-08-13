using Chat.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class AgentService : IAgentService
    {
        private readonly ILogger<AgentService> logger;
        private readonly CosmoDBConfig cosmoDBConfig;
        private readonly ICosmosDBService cosmosDBService;

        public AgentService(ILogger<AgentService> logger,
            IOptions<CosmoDBConfig> cosmoDBConfig,
            ICosmosDBService cosmosDBService)
        {
            this.logger = logger;
            this.cosmoDBConfig = cosmoDBConfig.Value;
            this.cosmosDBService = cosmosDBService;
        }

        public async Task<Team> GetTeamAsync(string id) =>
           await cosmosDBService.GetEntity<Team>(cosmoDBConfig.TeamContainerId, id, id);

        public async Task AddTeamsAsync()
        {
            var team = new Team(1, "Team A", Shift.Office, new List<Agent>
            {
                new Agent(1, "Udara", Level.Junior),
                new Agent(2, "Chathuranga", Level.MidLevel),
                new Agent(3, "Bassnayaka", Level.MidLevel),
                new Agent(4, "Chanaka", Level.TeamLead)
            },
            true);

            var team2 = new Team(2, "Team B", Shift.NonOffice, new List<Agent>
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

            var teamOverflow = new Team(3, "Overflow", Shift.Night, new List<Agent>
            {
                new Agent(8, "Harsha", Level.MidLevel),
                new Agent(9, "Uthpala", Level.MidLevel),
            });

            await cosmosDBService.AddEntity(team, cosmoDBConfig.TeamContainerId, team.Id);
            await cosmosDBService.AddEntity(team2, cosmoDBConfig.TeamContainerId, team2.Id);
            await cosmosDBService.AddEntity(team3, cosmoDBConfig.TeamContainerId, team3.Id);
        }

        public async Task ChangeTeamAsync(string name)
        {
            var currentTeam = await GetAssignedTeamAsync();

            if (currentTeam != null)
            {
                currentTeam.IsAssigned = false;
                currentTeam.Agents.ForEach(agent => agent.Queue.Clear());
                await cosmosDBService.UpdateEntity(currentTeam, cosmoDBConfig.TeamContainerId, currentTeam.Id, currentTeam.Id);
            }

            var newTeam = await GetTeamByNameAsync(name);
            if (newTeam != null)
            {
                newTeam.IsAssigned = true;
                await cosmosDBService.UpdateEntity(newTeam, cosmoDBConfig.TeamContainerId, newTeam.Id, newTeam.Id);
            }
        }

        public async Task<Team> GetTeamByNameAsync(string name)
        {
            var query = $"SELECT * FROM team WHERE team.Name = '{name}'";
            return (await cosmosDBService.GetEntities<Team>(cosmoDBConfig.TeamContainerId, query)).FirstOrDefault();
        }

        public async Task<Team> GetAssignedTeamAsync()
        {
            var query = "SELECT * FROM team WHERE team.IsAssigned = true";
            return (await cosmosDBService.GetEntities<Team>(cosmoDBConfig.TeamContainerId, query)).FirstOrDefault();
        }

        public async Task AssignSupportRequestToTeamAsync(SupportRequest supportRequest, Team team)
        {
            team.Agents = team.Agents
                .OrderBy(agent => agent.Level).ThenBy(agent => agent.Queue.Count).ToList();

            foreach (var agent in team.Agents)
            {
                if (!agent.IsCapacityExceeded(agent.Multiplier))
                {
                    if (agent.Level.Equals(Level.Junior))
                    {
                        await AssignSupportRequestToAgentAsync(supportRequest, team, agent);
                        break;
                    }
                    else if (agent.Level.Equals(Level.MidLevel))
                    {
                        await AssignSupportRequestToAgentAsync(supportRequest, team, agent);
                        break;
                    }
                    else if (agent.Level.Equals(Level.Senior))
                    {
                        await AssignSupportRequestToAgentAsync(supportRequest, team, agent);
                        break;
                    }
                    else if (agent.Level.Equals(Level.TeamLead))
                    {
                        await AssignSupportRequestToAgentAsync(supportRequest, team, agent);
                        break;
                    }
                }
            }

            team.Agents = team.Agents
                .OrderBy(agent => agent.Level).ThenBy(agent => agent.Queue.Count).ToList();

            await cosmosDBService.UpdateEntity<Team>(team, cosmoDBConfig.TeamContainerId, team.Id, team.Id);
        }

        private async Task AssignSupportRequestToAgentAsync(SupportRequest chat, Team team, Agent agent)
        {
            agent.Queue.Enqueue(chat);
        }

        public async Task DeleteTeamsAsync()
        {
            var query = "SELECT * FROM teams";
            var teams = await cosmosDBService.GetEntities<Team>(cosmoDBConfig.TeamContainerId, query);

            foreach (var team in teams)
            {
                await cosmosDBService.DeleteEntity<SupportRequest>(cosmoDBConfig.TeamContainerId, team.Id, team.Id);
            }
        }

        public async Task<int> GetCapacity()
        {
            var team = await GetAssignedTeamAsync();

            double capacity = 0;

            team.Agents.ForEach(agent =>
            {
                capacity = capacity + (10 * agent.Multiplier);
            });

            if (team.IsOverflow)
            {
                capacity = capacity + (10 * 0.4 * 6);
            }

            return (int)Math.Floor(capacity * 1.5);
        }
    }
}
