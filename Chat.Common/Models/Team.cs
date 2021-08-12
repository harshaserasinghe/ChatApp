using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chat.Common.Models
{
    public class Team
    {
        public Team(int teamId, string name, Shift shift, List<Agent> agents)
        {
            Id = teamId.ToString();
            TeamId = teamId;
            Name = name;
            Shift = shift;
            Agents = agents;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public Shift Shift { get; set; }
        public bool IsAssign { get; set; }
        public List<Agent> Agents { get; set; } = new List<Agent>();
    }
}
