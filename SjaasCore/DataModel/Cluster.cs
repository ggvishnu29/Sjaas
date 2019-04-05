using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaasCore.DataModel
{

    public enum ClusterStatus
    {
        Active,
        Inactive
    }

    public class ClusterSearchParams : Resource
    {
        public string JobPriority { get; set; }

        public string TenantId { get; set; }

        public JobType JobType { get; set; }
    }

    public class Cluster : Resource
    {
        public string RefId { get; set; }

        public string SparkClusterId { get; set; }

        public ClusterStatus ClusterStatus { get; set; }

        public JobType JobType { get; set; }

        public string TenantId { get; set; }

        public string JobPriority { get; set; }
    }
}
