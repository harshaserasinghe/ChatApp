using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Common.Models
{
    public class ChatQueue
    {
        public ChatQueue(int count, int capacity)
        {
            Count = count;
            Capacity = capacity;
        }

        public int Count { get; set; }
        public int Capacity { get; set; }
    }
}
