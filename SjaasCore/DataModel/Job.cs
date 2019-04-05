using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace SjaasCore.DataModel
{
    public enum JobType
    {
        Memory,
        Compute
    }

    public enum JobStatus
    {
        Pending,
        Submitted
    }

    public class JobSearchParams : Resource
    {
        public string JobPriority { get; set; }

        public string TenantId { get; set; }

        public JobType JobType { get; set; }
    }

    public class Job : Resource
    {
        public string RefId { get; set; }

        public List<string> SparkSubmitTaskParams { get; set; }

        public string JobPriority { get; set; }

        public string TenantId { get; set; }

        public JobType JobType { get; set; }

        public JobStatus JobStatus { get; set; }

        public string JobId { get; set; }

        public string RunId { get; set; }

        public string ClusterId { get; set; }

        public DateTime LastProcessedTime { get; set; }

    }
}
