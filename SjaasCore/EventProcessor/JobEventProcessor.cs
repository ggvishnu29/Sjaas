using Microsoft.Azure.Databricks.Client;
using Microsoft.Azure.Documents.Client;
using SjaasCore.DataModel;
using SjaasCore.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaasCore.Worker
{
    public class JobEventProcessor
    {
        private readonly ISjaasDataStore dataStore;

        private readonly DatabricksClient databricksClient;

        public JobEventProcessor(ISjaasDataStore dataStore, DatabricksClient databricksClient)
        {
            this.dataStore = dataStore;
            this.databricksClient = databricksClient;
        }

        public async Task ProcessJobAsync(DataModel.Job job)
        {
            switch(job.JobStatus)
            {
                case JobStatus.Submitted:
                    return;
                case JobStatus.Pending:
                    try
                    {
                        var clusters = await FindClustersForJobAsync(job);
                        if (clusters == null || clusters.Count == 0)
                        {
                            Console.WriteLine("no cluster found for submitting the job");
                            job.LastProcessedTime = DateTime.Now;
                            await dataStore.UpdateJobAsync(job);
                            return;
                        }
                        Random r = new Random();
                        int index = r.Next(0, clusters.Count);
                        var cluster = clusters[index];
                        //var sparkSubmitTask = new SparkSubmitTask()
                        //{
                        //    Parameters = job.SparkSubmitTaskParams
                        //};
                        var jobSettings = JobSettings
                            .GetNewSparkJarJobSettings(job.RefId, "class1", job.SparkSubmitTaskParams, job.SparkSubmitTaskParams)
                            .WithExistingCluster(cluster.SparkClusterId);
                        //var jobSettings = new JobSettings()
                        //{
                        //    SparkSubmitTask = sparkSubmitTask,
                        //    ExistingClusterId = cluster.SparkClusterId
                        //};
                        var jobId = await databricksClient.Jobs.Create(jobSettings);
                        var runId = (await databricksClient.Jobs.RunNow(jobId, null)).RunId;
                        job.ClusterId = cluster.SparkClusterId;
                        job.JobId = jobId.ToString();
                        job.RunId = runId.ToString();
                        job.JobStatus = JobStatus.Submitted;
                    } catch (Exception ex)
                    {
                        Console.WriteLine("error while submitting spark job: " + ex.Message);
                        job.LastProcessedTime = DateTime.Now;
                        await dataStore.UpdateJobAsync(job);
                        return;
                    }
                    await dataStore.UpdateJobAsync(job);
                    return;
            }
        }

        private async Task<List<Cluster>> FindClustersForJobAsync(DataModel.Job job)
        {
            var clusterSearchParams = new ClusterSearchParams
            {
                JobPriority = job.JobPriority,
                JobType = job.JobType,
                TenantId = job.TenantId
            };
            return await dataStore.SearchClustersAsync(clusterSearchParams);
        }
    }
}
