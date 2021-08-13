using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Common.Models
{
    public class AzureRedisConfig
    {
        public string ConnectionString { get; set; }
        public string QueueCapacity { get; set; }
    }
}
