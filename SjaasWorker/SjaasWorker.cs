using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor.FeedProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SjaasCore.Worker;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using SjaasCore.DataModel;
using Microsoft.Azure.Documents.Client;
using SjaasCore.DataStore;
using Microsoft.Azure.Databricks.Client;

namespace SjaasWorker
{
    class SjaasWorker : Microsoft.Azure.Documents.ChangeFeedProcessor.FeedProcessing.IChangeFeedObserver
    {

        private readonly JobEventProcessor jobEventProcessor;

        public SjaasWorker()
        {
            var docDbUri = "to-be-filled";
            var docDbPrimaryKey = "to-be-filled";
            var docDbClient = new DocumentClient(new Uri(docDbUri), docDbPrimaryKey);
            var database = "Sjaas";
            var jobCollection = "Job";
            var clusterCollection = "Cluster";
            var sjaasDataStore = new CosmosDataStore(docDbClient, database, jobCollection, clusterCollection);
            var databricksClient = DatabricksClient.CreateClient("to-be-filled", "to-be-filled");
            this.jobEventProcessor = new JobEventProcessor(sjaasDataStore, databricksClient);
        }

        static async Task RunAsync()
        {
            DocumentCollectionInfo feedCollectionInfo = new DocumentCollectionInfo()
            {
                DatabaseName = "Sjaas",
                CollectionName = "Job",
                Uri = new Uri("to-be-filled"),
                MasterKey = "to-be-filled"
            };

            DocumentCollectionInfo leaseCollectionInfo = new DocumentCollectionInfo()
            {
                DatabaseName = "Sjaas",
                CollectionName = "Lease",
                Uri = new Uri("to-be-filled"),
                MasterKey = "to-be-filled"
            };

            var builder = new ChangeFeedProcessorBuilder();
            var processor = await builder
                .WithHostName("SjaasWorker")
                .WithFeedCollection(feedCollectionInfo)
                .WithLeaseCollection(leaseCollectionInfo)
                .WithObserver<SjaasWorker>()
                .BuildAsync();

            await processor.StartAsync();

            Console.WriteLine("Change Feed Processor started. Press <Enter> key to stop...");
            Console.ReadLine();

            await processor.StopAsync();
        }

        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        public Task OpenAsync(IChangeFeedObserverContext context)
        {
            return Task.CompletedTask;
        }

        public Task CloseAsync(IChangeFeedObserverContext context, Microsoft.Azure.Documents.ChangeFeedProcessor.FeedProcessing.ChangeFeedObserverCloseReason reason)
        {
            return Task.CompletedTask;
        }

        public async Task ProcessChangesAsync(IChangeFeedObserverContext context, IReadOnlyList<Document> docs, CancellationToken cancellationToken)
        {
            foreach (Document doc in docs)
            {
                SjaasCore.DataModel.Job job = (dynamic) doc;
                await jobEventProcessor.ProcessJobAsync(job);

            }
        }
    }
}
