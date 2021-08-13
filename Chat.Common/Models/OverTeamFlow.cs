using Newtonsoft.Json;
using System.Collections.Generic;

namespace Chat.Common.Models
{
    public class OverTeamFlow
    {
        public OverTeamFlow(int teamId, string name, List<Agent> agents)
        {
            Id = teamId.ToString();//make it Guid
            TeamId = teamId;
            Name = name;
            Agents = agents;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public List<Agent> Agents { get; set; } = new List<Agent>();
    }
}
