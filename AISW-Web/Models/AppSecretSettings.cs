using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISW_Web.Models
{
    public class AppSecretSettings
    {
        public string TableStorageConnectionString { get; set; }
        public string TableStorageContainerName { get; set; }
        public string TableStoragePartitionKey { get; set; }
    }
}
