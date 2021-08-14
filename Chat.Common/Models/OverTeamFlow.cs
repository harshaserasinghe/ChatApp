using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Common.Models
{
    public class OverTeamFlow
    {
        public OverTeamFlow(int teamId, string name, List<Agent> agents)
        {
            Id = Guid.NewGuid().ToString();
            TeamId = teamId;
            Name = name;
            Agents = agents;
            IsOverflow = true;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public bool IsOverflow { get; set; }
        public List<Agent> Agents { get; set; } = new List<Agent>();

        public bool IsCapacityExceeded() =>
          Agents.All(agent => agent.IsCapacityExceeded());
    }
}
