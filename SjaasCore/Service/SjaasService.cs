using SjaasCore.DataModel;
using SjaasCore.DataStore;
using SjaasCore.Service.Exception;
using SjaasCore.Service.Request;
using SjaasCore.Service.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SjaasCore.Service
{
    public class SjaasService
    {

        private ISjaasDataStore datastore;

        public SjaasService(ISjaasDataStore datastore)
        {
            this.datastore = datastore;
        }

        public async Task<SubmitJobResponse> SubmitJobAsync(SubmitJobRequest req)
        {
            if (!Enum.TryParse<JobType>(req.JobCriteria.JobType, out JobType jobType))
            {
                throw new SjaasServiceValidationException("invalid job type specified: " + req.JobCriteria.JobType);
            }
            var job = new DataModel.Job
            {
                RefId = Guid.NewGuid().ToString(),
                JobPriority = req.JobCriteria.JobPriority,
                JobType = jobType,
                TenantId = req.TenantId,
                SparkSubmitTaskParams = req.SparkSubmitTaskParams,
                JobStatus = JobStatus.Pending,
                LastProcessedTime = DateTime.Now
            };
            await datastore.AddJobAsync(job);
            return new SubmitJobResponse(job.RefId);
        }

        public async Task<GetJobResponse> GetJobAsync(GetJobRequest req)
        {
            var job = await datastore.GetJobAsync(req.RefId);
            if (job == null)
            {
                return null;
            }
            return new GetJobResponse()
            {
                RefId = job.RefId,
                SparkSubmitTaskParams = job.SparkSubmitTaskParams,
                TenantId = job.TenantId,
                JobCriteria = new JobCriteria(job.JobPriority, job.JobType.ToString()),
                JobStatus = job.JobStatus.ToString(),
                LastProcessedTime = job.LastProcessedTime,
                SparkClusterId = job.ClusterId,
                SparkJobId = job.JobId,
                SparkJobRunId = job.RunId
            };
        }

        public async Task<AddClusterResponse> AddClusterAsync(AddClusterRequest req)
        {
            if (!Enum.TryParse<JobType>(req.ClusterCriteria.JobType, out JobType jobType))
            {
                throw new SjaasServiceValidationException("invalid job type specified: " + req.ClusterCriteria.JobType);
            }
            if (!Enum.TryParse<ClusterStatus>(req.ClusterStatus, out ClusterStatus clusterStatus))
            {
                throw new SjaasServiceValidationException("invalid cluster status specified: " + req.ClusterStatus);
            }
            var cluster = new DataModel.Cluster()
            {
                RefId = Guid.NewGuid().ToString(),
                SparkClusterId = req.SparkClusterId,
                JobPriority = req.ClusterCriteria.JobPriority,
                JobType = jobType,
                TenantId = req.ClusterCriteria.TenantId,
                ClusterStatus = clusterStatus
            };
            await datastore.AddClusterAsync(cluster);
            return new AddClusterResponse()
            {
                RefId = cluster.RefId
            };
        }

        public async Task<ListClustersResponse> ListAllClusters()
        {
            var clusters = await datastore.SearchClustersAsync(null);
            if (clusters == null || clusters.Count == 0)
            {
                return new ListClustersResponse()
                {
                    Clusters = null
                };
            }
            List<Response.Cluster> respClusters = new List<Response.Cluster>();
            foreach (var cluster in clusters)
            {
                var respCluster = new Response.Cluster()
                {
                    RefId = cluster.RefId,
                    SparkClusterId = cluster.SparkClusterId,
                    ClusterStatus = cluster.ClusterStatus.ToString(),
                    JobType = cluster.JobType.ToString(),
                    JobPriority = cluster.JobPriority.ToString(),
                    TenantId = cluster.TenantId
                };
                respClusters.Add(respCluster);
            }
            return new ListClustersResponse()
            {
                Clusters = respClusters
            };
        }
    }
}
