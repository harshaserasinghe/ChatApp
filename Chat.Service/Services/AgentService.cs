using Chat.Common.Models;
using Chat.Service.Interfaces;
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

        public async Task<Team> GetTeamAsync(string id)
        {
            var team = await cosmosDBService.GetEntityAsync<Team>(cosmoDBConfig.TeamContainerId, id, id);

            if (team != null && team.HasOverflow)
            {
                team.OverTeamFlow = await GetOverflowTeamAsync();
            }

            return team;
        }

        public async Task AddTeamsAsync()
        {
            var team = new Team(1, "Team A", Shift.Office, new List<Agent>
            {
                new Agent(1, "Junior", Level.Junior),
                new Agent(2, "MidLevel 1", Level.MidLevel),
                new Agent(3, "MidLevel 2", Level.MidLevel),
                new Agent(4, "TeamLead", Level.TeamLead)
            },
            true, true);

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

            var teamOverflow = new OverTeamFlow(4, "Overflow", new List<Agent>
            {
                new Agent(11, "Junior Overflow 1", Level.Junior, true),
                new Agent(12, "Junior Overflow 2", Level.Junior, true),
                new Agent(13, "Junior Overflow 3", Level.Junior, true),
                new Agent(14, "Junior Overflow 4", Level.Junior, true),
                new Agent(15, "Junior Overflow 5", Level.Junior, true),
                new Agent(16, "Junior Overflow 6", Level.Junior, true),
            });

            await cosmosDBService.AddEntityAsync(team, cosmoDBConfig.TeamContainerId, team.Id);
            await cosmosDBService.AddEntityAsync(team2, cosmoDBConfig.TeamContainerId, team2.Id);
            await cosmosDBService.AddEntityAsync(team3, cosmoDBConfig.TeamContainerId, team3.Id);
            await cosmosDBService.AddEntityAsync(teamOverflow, cosmoDBConfig.TeamContainerId, teamOverflow.Id);
        }

        public async Task ChangeTeamAsync(string name)
        {
            var currentTeam = await GetAssignedTeamAsync();

            if (currentTeam != null)
            {
                currentTeam.IsAssigned = false;
                await cosmosDBService.UpdateEntityAsync(currentTeam, cosmoDBConfig.TeamContainerId, currentTeam.Id, currentTeam.Id);

                if (currentTeam.HasOverflow)
                {
                    await cosmosDBService.UpdateEntityAsync(currentTeam.OverTeamFlow, cosmoDBConfig.TeamContainerId, currentTeam.OverTeamFlow.Id, currentTeam.OverTeamFlow.Id);
                }
            }

            var newTeam = await GetTeamByNameAsync(name);

            if (newTeam != null)
            {
                newTeam.IsAssigned = true;
                await cosmosDBService.UpdateEntityAsync(newTeam, cosmoDBConfig.TeamContainerId, newTeam.Id, newTeam.Id);
            }
        }

        public async Task<Team> GetTeamByNameAsync(string name)
        {
            var query = $"SELECT * FROM team WHERE UPPER(team.Name) = '{name.ToUpper()}'";
            var team = (await cosmosDBService.GetEntitiesAsync<Team>(cosmoDBConfig.TeamContainerId, query)).FirstOrDefault();

            if (team != null && team.HasOverflow)
            {
                team.OverTeamFlow = await GetOverflowTeamAsync();
            }

            return team;
        }

        public async Task<Team> GetAssignedTeamAsync()
        {
            var query = "SELECT * FROM team WHERE team.IsAssigned = true";
            var team = (await cosmosDBService.GetEntitiesAsync<Team>(cosmoDBConfig.TeamContainerId, query)).FirstOrDefault();

            if (team != null && team.HasOverflow)
            {
                team.OverTeamFlow = await GetOverflowTeamAsync();
            }

            return team;
        }

        public async Task<OverTeamFlow> GetOverflowTeamAsync()
        {
            var query = $"SELECT * FROM team WHERE team.IsOverflow = true";
            return (await cosmosDBService.GetEntitiesAsync<OverTeamFlow>(cosmoDBConfig.TeamContainerId, query)).FirstOrDefault();
        }

        public async Task AssignSupportRequestToAgentAsync(SupportRequest supportRequest, Team team)
        {
            team.Agents = team.Agents
                .OrderBy(agent => agent.Level)
                .ThenBy(agent => agent.Queue.Count)
                .ToList();

            if (!team.IsCapacityExceeded())
            {
                foreach (var agent in team.Agents)
                {
                    if (!agent.IsCapacityExceeded())
                    {
                        if (agent.Level.Equals(Level.Junior))
                        {
                            UpdateSupportRequest(supportRequest, team, agent);
                            agent.Queue.Enqueue(supportRequest);
                            break;
                        }
                        else if (agent.Level.Equals(Level.MidLevel))
                        {
                            UpdateSupportRequest(supportRequest, team, agent);
                            agent.Queue.Enqueue(supportRequest);
                            break;
                        }
                        else if (agent.Level.Equals(Level.Senior))
                        {
                            UpdateSupportRequest(supportRequest, team, agent);
                            agent.Queue.Enqueue(supportRequest);
                            break;
                        }
                        else if (agent.Level.Equals(Level.TeamLead))
                        {
                            UpdateSupportRequest(supportRequest, team, agent);
                            agent.Queue.Enqueue(supportRequest);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (team.HasOverflow)
                {
                    team.OverTeamFlow.Agents = team.OverTeamFlow.Agents
                        .OrderBy(agent => agent.Queue.Count)
                        .ToList();

                    foreach (var overFlowAgent in team.OverTeamFlow.Agents)
                    {
                        if (!overFlowAgent.IsCapacityExceeded())
                        {
                            UpdateSupportRequest(supportRequest, team, overFlowAgent);
                            overFlowAgent.Queue.Enqueue(supportRequest);
                            break;
                        }
                    }

                    team.OverTeamFlow.Agents = team.OverTeamFlow.Agents
                        .OrderBy(agent => agent.Queue.Count)
                        .ToList();

                    await cosmosDBService.UpdateEntityAsync<OverTeamFlow>(team.OverTeamFlow, cosmoDBConfig.TeamContainerId, team.OverTeamFlow.Id, team.OverTeamFlow.Id);
                }
            }

            team.Agents = team.Agents
                .OrderBy(agent => agent.Level)
                .ThenBy(agent => agent.Queue.Count)
                .ToList();

            await cosmosDBService.UpdateEntityAsync<Team>(team, cosmoDBConfig.TeamContainerId, team.Id, team.Id);
        }

        private void UpdateSupportRequest(SupportRequest supportRequest, Team team, Agent agent)
        {
            supportRequest.TeamId = team.TeamId;
            supportRequest.AgentId = agent.AgentId;
            supportRequest.IsAssign = true;
        }

        public async Task RestAllAgents()
        {
            var query = $"SELECT * FROM team WHERE team.IsOverflow = false";
            var teams = await cosmosDBService.GetEntitiesAsync<Team>(cosmoDBConfig.TeamContainerId, query);

            foreach (var team in teams)
            {
                team.Agents.ForEach(agent => agent.Queue.Clear());
                await cosmosDBService.UpdateEntityAsync(team, cosmoDBConfig.TeamContainerId, team.Id, team.Id);
            }

            var overFlowTeam = await GetOverflowTeamAsync();
            overFlowTeam.Agents.ForEach(agent => agent.Queue.Clear());
            await cosmosDBService.UpdateEntityAsync(overFlowTeam, cosmoDBConfig.TeamContainerId, overFlowTeam.Id, overFlowTeam.Id);

        }

        public async Task DeleteAllTeamsAsync()
        {
            var query = "SELECT * FROM teams";
            var teams = await cosmosDBService.GetEntitiesAsync<Team>(cosmoDBConfig.TeamContainerId, query);

            foreach (var team in teams)
            {
                await cosmosDBService.DeleteEntityAsync<Team>(cosmoDBConfig.TeamContainerId, team.Id, team.Id);
            }
        }

        public async Task<int> GetCapacity()
        {
            double capacity = 0;
            var team = await GetAssignedTeamAsync();
            capacity = capacity + (team.Agents.Sum(agent => (agent.Multiplier * 10)));

            if (team.HasOverflow)
            {
                capacity = capacity + (10 * 0.4 * 6);
            }

            return (int)Math.Floor(capacity * 1.5);
        }

        public bool IsAllAgentsBusy(Team team)
        {
            if (!team.HasOverflow)
            {
                return team.IsCapacityExceeded();
            }
            else
            {
                return team.IsCapacityExceeded() && team.OverTeamFlow.IsCapacityExceeded();
            }
        }
    }
}
