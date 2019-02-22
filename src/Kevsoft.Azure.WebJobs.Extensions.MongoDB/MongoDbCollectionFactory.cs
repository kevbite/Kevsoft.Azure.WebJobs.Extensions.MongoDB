using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDbCollectionFactory : IMongoDbCollectionFactory
    {
        public IMongoCollection<T> GetCollection<T>(ConnectionOptions options)
        {
            var client = new MongoClient(options.ConnectionString);
            var database = client.GetDatabase(options.DatabaseName);
            var collection = database.GetCollection<T>(options.CollectionName);

            return collection;
        }
    }
}