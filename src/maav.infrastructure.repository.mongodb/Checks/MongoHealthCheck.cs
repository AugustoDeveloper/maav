using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

namespace MAAV.Infrastrucuture.Repository.MongoDB.Checks
{
    public sealed class MongoHealthCheck : IHealthCheck
    {
        private readonly MongoUrl mongoUrl;
        
        public MongoHealthCheck(string connectionstring)
            => this.mongoUrl = new MongoUrl(connectionstring);

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = new MongoClient(this.mongoUrl);
                var ping = await client.GetDatabase(this.mongoUrl.DatabaseName ?? "admin")
                    .RunCommandAsync<BsonDocument>(new BsonDocument { { "ping", 1} },
                                                   default,
                                                   cancellationToken
                                                    );
                if (ping.Contains("ok") &&
                    ping["ok"].IsDouble && (int)ping["ok"].AsDouble == 1 ||
                    ping["ok"].IsInt32 && ping["ok"].AsInt32 == 1 )
                {
                    return client.Cluster.Description.State == ClusterState.Connected ?
                           HealthCheckResult.Healthy($"The connection on mongodb is healthy") :
                           HealthCheckResult.Unhealthy($"The connection on mongodb is unhealthy");
                }

                return HealthCheckResult.Unhealthy($"The connection with mongodb is broken",data: new Dictionary<string, object> { { "ping", ping.ToJson() }} );
            }
            catch(Exception ex)
            {
                return HealthCheckResult.Unhealthy("Something's wrong with mongodb", ex);
            }
        }
    }
}