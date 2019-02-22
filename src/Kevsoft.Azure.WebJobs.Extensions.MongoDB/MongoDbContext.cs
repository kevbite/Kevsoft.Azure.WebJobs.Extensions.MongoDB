using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDbContext
    {
        public MongoDbContext(MongoDbAttribute resolvedAttribute,
            IMongoDbFactory factory,
            ConnectionOptions connectionOptions)
        {
            ResolvedAttribute = resolvedAttribute;
            Factory = factory;
            ConnectionOptions = connectionOptions;
        }

        public MongoDbAttribute ResolvedAttribute { get; }

        public IMongoDbFactory Factory { get; }

        public ConnectionOptions ConnectionOptions { get; }

        public IMongoCollection<T> GetCollection<T>()
        {
            return Factory.GetCollection<T>(ConnectionOptions);
        }

        public IMongoDatabase GetDatabase()
        {
            return Factory.GetDatabase(ConnectionOptions);
        }

        public MongoClient GetClient()
        {
            return Factory.GetClient(ConnectionOptions);
        }
    }
}