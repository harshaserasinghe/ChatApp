using Newtonsoft.Json;
using System.Collections.Generic;

namespace Chat.Common.Models
{
    public class Team
    {
        public Team(int teamId, string name, Shift shift, List<Agent> agents, bool isAssigned = false)
        {
            Id = teamId.ToString();//make it Guid
            TeamId = teamId;
            Name = name;
            Shift = shift;
            Agents = agents;
            IsAssigned = isAssigned;
            IsOverflow = shift == Shift.Office;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public Shift Shift { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsOverflow { get; set; }
        public List<Agent> Agents { get; set; } = new List<Agent>();
    }
}
