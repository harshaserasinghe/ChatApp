using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Common.Models
{
    public class CosmoDBConfig
    {
        public string ConnectionString { get; set; }
        public string DataBaseId { get; set; }
        public string ChatContainerId { get; set; }
        public string TeamContainerId { get; set; }
    }
}
