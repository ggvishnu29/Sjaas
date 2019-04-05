using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaasCore.Service.Request
{

    public class ClusterCriteria
    {
        public string JobType { get; set; }

        public string TenantId { get; set; }

        public string JobPriority { get; set; }
    }

    public class AddClusterRequest
    {
        public string SparkClusterId { get; set; }

        public string ClusterStatus { get; set; }

        public ClusterCriteria ClusterCriteria { get; set; }

    }
}
