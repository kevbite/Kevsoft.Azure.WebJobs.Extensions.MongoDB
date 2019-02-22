using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDbFactory : IMongoDbFactory
    {
        public IMongoCollection<T> GetCollection<T>(ConnectionOptions options)
        {
            var database = GetDatabase(options);

            return database.GetCollection<T>(options.CollectionName);
        }

        public IMongoDatabase GetDatabase(ConnectionOptions options)
        {
            var client = GetClient(options);
            return client.GetDatabase(options.DatabaseName);
        }

        public MongoClient GetClient(ConnectionOptions options)
        {
            return new MongoClient(options.ConnectionString);
        }
    }
}