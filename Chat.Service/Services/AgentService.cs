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
                new Agent(1, "Junior", Level.Junior),
                new Agent(2, "MidLevel 1", Level.MidLevel),
                new Agent(3, "MidLevel 2", Level.MidLevel),
                new Agent(4, "TeamLead", Level.TeamLead)
            },
            true);

            var team2 = new Team(2, "Team B", Shift.NonOffice, new List<Agent>
            {
                new Agent(5, "Junior 1", Level.Junior),
                new Agent(6, "Junior 2", Level.Junior),
                new Agent(7, "MidLevel", Level.MidLevel),
                new Agent(8, "Senior", Level.Senior)
            });


            var team3 = new Team(3, "Team C", Shift.Night, new List<Agent>
            {
                new Agent(9, "MidLevel 1", Level.MidLevel),
                new Agent(10, "MidLevel 2", Level.MidLevel),
            });

            var teamOverflow = new Team(4, "Overflow", Shift.None, new List<Agent>
            {
                new Agent(11, "Junior Overflow 1", Level.Junior, true),
                new Agent(12, "Junior Overflow 2", Level.Junior, true),
                new Agent(13, "Junior Overflow 3", Level.Junior, true),
                new Agent(14, "Junior Overflow 4", Level.Junior, true),
                new Agent(15, "Junior Overflow 5", Level.Junior, true),
                new Agent(16, "Junior Overflow 6", Level.Junior, true),
            });

            await cosmosDBService.AddEntity(team, cosmoDBConfig.TeamContainerId, team.Id);
            await cosmosDBService.AddEntity(team2, cosmoDBConfig.TeamContainerId, team2.Id);
            await cosmosDBService.AddEntity(team3, cosmoDBConfig.TeamContainerId, team3.Id);
            await cosmosDBService.AddEntity(teamOverflow, cosmoDBConfig.TeamContainerId, teamOverflow.Id);
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
                        agent.Queue.Enqueue(supportRequest);
                        break;
                    }
                    else if (agent.Level.Equals(Level.MidLevel))
                    {
                        agent.Queue.Enqueue(supportRequest);
                        break;
                    }
                    else if (agent.Level.Equals(Level.Senior))
                    {
                        agent.Queue.Enqueue(supportRequest);
                        break;
                    }
                    else if (agent.Level.Equals(Level.TeamLead))
                    {
                        agent.Queue.Enqueue(supportRequest);
                        break;
                    }
                }
            }

            team.Agents = team.Agents
                .OrderBy(agent => agent.Level).ThenBy(agent => agent.Queue.Count).ToList();

            await cosmosDBService.UpdateEntity<Team>(team, cosmoDBConfig.TeamContainerId, team.Id, team.Id);
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
