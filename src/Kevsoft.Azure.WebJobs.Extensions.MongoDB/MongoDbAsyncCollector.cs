using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDbAsyncCollector<T> : IAsyncCollector<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbAsyncCollector(MongoDbContext mongoDbContext)
        {
            _collection = mongoDbContext.GetCollection<T>();
        }

        public async Task AddAsync(T item, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(item, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public Task FlushAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }
    }
}