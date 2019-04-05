using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaasCore.Service.Response
{
    public class Cluster
    {
        public string RefId { get; set; }

        public string SparkClusterId { get; set; }

        public string ClusterStatus { get; set; }

        public string JobType { get; set; }

        public string TenantId { get; set; }

        public string JobPriority { get; set; }
    }

    public class ListClustersResponse
    {
        public List<Cluster> Clusters { get; set; }
    }
}
