using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using SjaasCore.DataModel;

namespace SjaasCore.DataStore
{
    public class CosmosDataStore : ISjaasDataStore
    {

        private readonly DocumentClient documentClient;

        private readonly string database;

        private readonly string jobCollection;

        private readonly string clusterCollection;

        public CosmosDataStore(DocumentClient documentClient, string database, string jobCollection, string clusterCollection)
        {
            this.documentClient = documentClient;
            this.database = database;
            this.jobCollection = jobCollection;
            this.clusterCollection = clusterCollection;
        }

        public async Task AddClusterAsync(Cluster cluster)
        {
            try
            {
                await this.documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(database, clusterCollection), cluster);
            }
            catch (Exception ex)
            {
                throw new SjaasDatastoreException("error adding cluster", ex);
            }
        }

        public async Task AddJobAsync(Job job)
        {
            try
            {
                await this.documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(database, jobCollection), job);
            } catch (Exception ex)
            {
                throw new SjaasDatastoreException("error adding job", ex);
            }
        }

        public Task DeleteClusterAsync(Cluster cluster)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteJobAsync(Job job)
        {
            throw new NotImplementedException();
        }

        public Task<Job> GetJobAsync(string refId)
        {
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = -1,
                EnableCrossPartitionQuery = true
            };
            return Task.FromResult(this.documentClient.CreateDocumentQuery<Job>(
                UriFactory.CreateDocumentCollectionUri(database, jobCollection), queryOptions)
                .Where(j => j.RefId == refId)
                .AsEnumerable()
                .ToList().FirstOrDefault<Job>());
        }

        public Task<List<Cluster>> SearchClustersAsync(ClusterSearchParams clusterSearchParams)
        {
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = -1,
                EnableCrossPartitionQuery = true
            };
            if (clusterSearchParams == null)
            {
                return Task.FromResult(this.documentClient.CreateDocumentQuery<Cluster>(
                UriFactory.CreateDocumentCollectionUri(database, clusterCollection), queryOptions)
                .AsEnumerable()
                .ToList());
            }
            return Task.FromResult(this.documentClient.CreateDocumentQuery<Cluster>(
                UriFactory.CreateDocumentCollectionUri(database, clusterCollection), queryOptions)
                .Where(c => c.JobType == clusterSearchParams.JobType)
                .Where(c => c.JobPriority == clusterSearchParams.JobPriority)
                .Where(c => c.TenantId == "all" || c.TenantId == clusterSearchParams.TenantId)
                .Where(c => c.ClusterStatus == ClusterStatus.Active)
                .AsEnumerable()
                .ToList());
        }

        public async Task<List<Job>> SearchJobsAsync(JobSearchParams jobSearchParams)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateClusterAsync(Cluster cluster)
        {
            try
            {
                await this.documentClient.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(database, clusterCollection), cluster);
            }
            catch (Exception ex)
            {
                throw new SjaasDatastoreException("error updating cluster", ex);
            }
        }

        public async Task UpdateJobAsync(Job job)
        {
            try
            {
                await this.documentClient.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(database, jobCollection), job);
            }
            catch (Exception ex)
            {
                throw new SjaasDatastoreException("error updating job", ex);
            }
        }
    }
}
