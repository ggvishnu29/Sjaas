using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SjaasCore.DataStore;
using SjaasCore.Service;
using SjaasCore.Service.Exception;
using SjaasCore.Service.Request;

namespace Sjaas
{
    public static class SjaasController
    {

        private static SjaasService sjaasService;

        private static void InitService()
        {
            if (sjaasService != null)
            {
                return;
            }
            var docDbUri = "to-be-filled";
            var docDbPrimaryKey = "to-be-filled";
            var docDbClient = new DocumentClient(new Uri(docDbUri), docDbPrimaryKey);
            var database = "Sjaas";
            var jobCollection = "Job";
            var clusterCollection = "Cluster";
            var sjaasDataStore = new CosmosDataStore(docDbClient, database, jobCollection, clusterCollection);
            sjaasService = new SjaasService(sjaasDataStore);
        }

        [FunctionName("SubmitJob")]
        public static async Task<HttpResponseMessage> SubmitJobAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            InitService();
            string reqBody = await req.Content.ReadAsStringAsync();
            var submitJobReq = JsonConvert.DeserializeObject<SubmitJobRequest>(reqBody);
            if (!IsSubmitJobRequestValid(submitJobReq))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "the request parameters are not valid");
            }
            try
            {
                var resp = await sjaasService.SubmitJobAsync(submitJobReq);
                return req.CreateResponse(HttpStatusCode.Accepted, resp);
            }
            catch (SjaasServiceValidationException) 
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "the request parameters are not valid");
            }
            catch (Exception ex)
            {
                log.Error("error executing submit job request", ex, reqBody);
                return req.CreateResponse(HttpStatusCode.InternalServerError, "internal server error");
            }
        }

        [FunctionName("GetJob")]
        public static async Task<HttpResponseMessage> GetJobAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            InitService();
            string reqBody = await req.Content.ReadAsStringAsync();
            var getJobReq = JsonConvert.DeserializeObject<GetJobRequest>(reqBody);
            if (string.IsNullOrEmpty(getJobReq.RefId))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "the request parameters are not valid");
            }
            try
            {
                var resp = await sjaasService.GetJobAsync(getJobReq);
                if (resp == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound, "no job found");
                }
                return req.CreateResponse(HttpStatusCode.Accepted, resp);
            }
            catch (SjaasServiceValidationException)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "the request parameters are not valid");
            }
            catch (Exception ex)
            {
                log.Error("error executing get job request", ex, reqBody);
                return req.CreateResponse(HttpStatusCode.InternalServerError, "internal server error");
            }
        }

        [FunctionName("AddCluster")]
        public static async Task<HttpResponseMessage> AddClusterAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            InitService();
            string reqBody = await req.Content.ReadAsStringAsync();
            var addClusterReq = JsonConvert.DeserializeObject<AddClusterRequest>(reqBody);
            if (!IsAddClusterRequestValid(addClusterReq))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "the request parameters are not valid");
            }
            try
            {
                var resp = await sjaasService.AddClusterAsync(addClusterReq);
                return req.CreateResponse(HttpStatusCode.Accepted, resp);
            }
            catch (SjaasServiceValidationException)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "the request parameters are not valid");
            }
            catch (Exception ex)
            {
                log.Error("error executing add cluster request", ex, reqBody);
                return req.CreateResponse(HttpStatusCode.InternalServerError, "internal server error");
            }
        }

        [FunctionName("ListClusters")]
        public static async Task<HttpResponseMessage> ListClustersAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            InitService();
            try
            {
                var resp = await sjaasService.ListAllClusters();
                return req.CreateResponse(HttpStatusCode.Accepted, resp);
            }
            catch (SjaasServiceValidationException)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "the request parameters are not valid");
            }
            catch (Exception ex)
            {
                log.Error("error executing list clusters request", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, "internal server error");
            }
        }

        private static bool IsSubmitJobRequestValid(SubmitJobRequest req)
        {
            if (string.IsNullOrEmpty(req.TenantId) || req.JobCriteria == null || req.SparkSubmitTaskParams == null || req.SparkSubmitTaskParams.Count == 0)
            {
                return false;
            }
            var jobCriteria = req.JobCriteria;
            if (string.IsNullOrEmpty(jobCriteria.JobPriority) || string.IsNullOrEmpty(jobCriteria.JobType))
            {
                return false;
            }
            return true;
        }

        private static bool IsAddClusterRequestValid(AddClusterRequest req)
        {
            if (string.IsNullOrEmpty(req.SparkClusterId) || string.IsNullOrEmpty(req.ClusterStatus))
            {
                return false;
            }
            var clusterCriteria = req.ClusterCriteria;
            if (string.IsNullOrEmpty(clusterCriteria.JobPriority) || string.IsNullOrEmpty(clusterCriteria.JobType))
            {
                return false;
            }
            return true;
        }
    }
}
