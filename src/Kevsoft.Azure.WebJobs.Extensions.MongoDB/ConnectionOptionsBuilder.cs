using Microsoft.Azure.WebJobs;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    internal static class ConnectionOptionsBuilder
    {
        public static ConnectionOptions Build(MongoDbAttribute attribute, MongoDbOptions options)
        {
            return new ConnectionOptions(
                attribute.ConnectionStringSetting ?? options.ConnectionString,
                attribute.DatabaseName ?? options.DatabaseName,
                attribute.CollectionName ?? options.CollectionName
            );
        }
    }
}