using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Common.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Shift Shift { get; set; }
        public List<Agent> Agents { get; set; } = new List<Agent>();
    }
}
