using System;
using System.Collections.Generic;
using System.Text;

namespace SjaasCore.Service.Request
{

    public class JobCriteria
    {
        public string JobPriority { get; set; }

        public string JobType { get; set; }

        public JobCriteria(string jobPriority, string jobType)
        {
            JobPriority = jobPriority;
            JobType = jobType;
        }
    }

    public class SubmitJobRequest
    {
        public List<string> SparkSubmitTaskParams { get; set; }

        public JobCriteria JobCriteria { get; set; }

        public string TenantId { get; set; }

    }
}
