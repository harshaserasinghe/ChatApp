using System.Collections.Generic;

namespace Chat.Common.Models
{
    public class Agent
    {
        public Agent(int AgentId, string name, Level level, bool isOverflow = false)
        {
            this.AgentId = AgentId;
            Name = name;
            Level = level;
            Multiplier = GetMutiplier(level);
            IsOverflow = isOverflow;
        }

        public int AgentId { get; set; }
        public string Name { get; set; }
        public Level Level { get; set; }
        public double Multiplier { get; set; }
        public bool IsOverflow { get; set; }
        public Queue<SupportRequest> Queue { get; set; } = new Queue<SupportRequest>();

        public bool IsCapacityExceeded(double multiplier) =>
            (multiplier * 10) == Queue.Count;

        public double GetMutiplier(Level level)
        {
            double mutiplier = 0;

            switch (level)
            {
                case Level.Junior:
                    mutiplier = 0.4;
                    break;
                case Level.MidLevel:
                    mutiplier = 0.6;
                    break;
                case Level.Senior:
                    mutiplier = 0.8;
                    break;
                case Level.TeamLead:
                    mutiplier = 0.5;
                    break;
                default:
                    break;
            }

            return mutiplier;
        }
    }
}
