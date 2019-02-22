using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public interface IMongoDbFactory
    {
        IMongoCollection<T> GetCollection<T>(ConnectionOptions options);

        IMongoDatabase GetDatabase(ConnectionOptions options);

        MongoClient GetClient(ConnectionOptions options);
    }
}