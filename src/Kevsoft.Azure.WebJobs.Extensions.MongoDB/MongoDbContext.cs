using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDbContext
    {
        public MongoDbContext(MongoDbAttribute resolvedAttribute,
            IMongoDbCollectionFactory collectionFactory,
            ConnectionOptions connectionOptions)
        {
            ResolvedAttribute = resolvedAttribute;
            CollectionFactory = collectionFactory;
            ConnectionOptions = connectionOptions;
        }

        public MongoDbAttribute ResolvedAttribute { get; }

        public IMongoDbCollectionFactory CollectionFactory { get; }

        public ConnectionOptions ConnectionOptions { get; }

        public IMongoCollection<T> GetCollection<T>()
        {
            return CollectionFactory.GetCollection<T>(ConnectionOptions);
        }
    }
}