using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Common.Models
{
    public class AzureServiceBusConfig
    {
        public string ConnectionString { get; set; }
        public string Queue { get; set; }

    }
}
