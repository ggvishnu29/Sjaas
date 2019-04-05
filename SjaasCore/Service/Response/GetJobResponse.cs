using SjaasCore.Service.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaasCore.Service.Response
{
    public class GetJobResponse
    {
        public string RefId { get; set; }

        public List<string> SparkSubmitTaskParams { get; set; }

        public JobCriteria JobCriteria { get; set; }

        public string TenantId { get; set; }

        public string JobStatus { get; set; }

        public DateTime LastProcessedTime { get; set; }

        public string SparkClusterId { get; set; }

        public string SparkJobId { get; set; }

        public string SparkJobRunId { get; set; }
    }
}
