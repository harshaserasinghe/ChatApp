﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Common.Models
{
    public class Agent
    {
        public Agent(int id,string name, Level level)
        {
            Id = id;
            Name = name;
            Level = level;
            Multiplier = GetMutiplier(level);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Level Level { get; set; }
        public double Multiplier { get; set; }
        public Queue<Chat> Queue { get; set; } = new Queue<Chat>();

        public bool IsFull(double multiplier) =>
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
