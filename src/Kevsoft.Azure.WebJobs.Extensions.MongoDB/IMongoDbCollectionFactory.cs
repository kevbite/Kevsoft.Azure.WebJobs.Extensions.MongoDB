using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public interface IMongoDbCollectionFactory
    {
        IMongoCollection<T> GetCollection<T>(ConnectionOptions options);
    }
}