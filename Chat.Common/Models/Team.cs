using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Common.Models
{
    public class Team
    {
        public Team(int teamId, string name, Shift shift, List<Agent> agents, bool isAssigned = false, bool hasOverflow = false)
        {
            Id = Guid.NewGuid().ToString();
            TeamId = teamId;
            Name = name;
            Shift = shift;
            Agents = agents;
            IsAssigned = isAssigned;
            HasOverflow = hasOverflow;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public Shift Shift { get; set; }
        public bool IsAssigned { get; set; }
        public bool HasOverflow { get; set; }
        public List<Agent> Agents { get; set; } = new List<Agent>();

        [JsonIgnore]
        public OverTeamFlow OverTeamFlow { get; set; }

        public bool IsCapacityExceeded() =>
          Agents.All(agent => agent.IsCapacityExceeded());
    }
}
