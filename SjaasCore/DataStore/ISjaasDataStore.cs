using SjaasCore.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SjaasCore.DataStore
{
    public interface ISjaasDataStore
    {
        Task AddJobAsync(Job job);

        Task UpdateJobAsync(Job job);

        Task DeleteJobAsync(Job job);

        Task<Job> GetJobAsync(string jobId);

        Task<List<Job>> SearchJobsAsync(JobSearchParams jobSearchParams);

        Task AddClusterAsync(Cluster cluster);

        Task UpdateClusterAsync(Cluster cluster);

        Task DeleteClusterAsync(Cluster cluster);

        Task<List<Cluster>> SearchClustersAsync(ClusterSearchParams clusterSearchParams);

    }
}
