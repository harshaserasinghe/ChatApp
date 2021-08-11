using Chat.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Service.Services
{
    public class AgentService : IAgentService
    {
        public Team TeamModel { get; private set; }
        public void AssignTeam(Team teamModel)
        {
            TeamModel = teamModel;
        }

        public void ShowTeam()
        {
            foreach (var agent in TeamModel.Agents)
            {
                Console.WriteLine($"{agent.Id} {agent.Name} {agent.Level.ToString()} {agent.Queue.Count}");
            }
        }

        public void AssignChat(Common.Models.Chat chatModel)
        {
            TeamModel.Agents = TeamModel.Agents
                .OrderBy(agent => agent.Level).ThenBy(agent => agent.Queue.Count).ToList();

            foreach (var agentModel in TeamModel.Agents)
            {
                if (!agentModel.IsFull(agentModel.Multiplier))
                {
                    if (agentModel.Level.Equals(Level.Junior))
                    {
                        agentModel.Queue.Enqueue(chatModel);
                        break;
                    }
                    else if (agentModel.Level.Equals(Level.MidLevel))
                    {
                        agentModel.Queue.Enqueue(chatModel);
                        break;
                    }
                    else if (agentModel.Level.Equals(Level.Senior))
                    {
                        agentModel.Queue.Enqueue(chatModel);
                        break;
                    }
                    else if (agentModel.Level.Equals(Level.TeamLead))
                    {
                        agentModel.Queue.Enqueue(chatModel);
                        break;
                    }
                }
            }

            TeamModel.Agents = TeamModel.Agents
                .OrderBy(agent => agent.Level).ThenBy(agent => agent.Queue.Count).ToList();
        }

        public int GetTeamCapacity(Team teamModel)
        {
            double capacity = 0;

            teamModel.Agents.ForEach(agent =>
            {
                capacity = capacity + (10 * agent.Multiplier);
            });

            return (int)Math.Floor(capacity * 1.5);
        }
    }
}
