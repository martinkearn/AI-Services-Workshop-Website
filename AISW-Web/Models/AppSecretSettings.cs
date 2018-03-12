using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISW_Web.Models
{
    //Requires the Microsoft.Extensions.Configuration.UserSecrets Nuget package. Set the data for this class using 'Manager User Secrets'. Follow the guide here: https://tahirnaushad.com/2017/08/31/asp-net-core-2-0-secret-manager/
    public class AppSecretSettings
    {
        public string TableStorageConnectionString { get; set; }
        public string TableStorageContainerName { get; set; }
        public string TableStoragePartitionKey { get; set; }
    }
}
