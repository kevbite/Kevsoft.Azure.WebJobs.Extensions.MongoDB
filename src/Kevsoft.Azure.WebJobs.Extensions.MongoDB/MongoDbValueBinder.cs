using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    internal class MongoDbValueBinder<T> : IValueBinder where T : class
    {
        private readonly IMongoCollection<T> _collection;
        private readonly MongoDbAttribute _attribute;
        
        public MongoDbValueBinder(IMongoDbCollectionFactory mongoDbCollectionFactory, MongoDbAttribute attribute, ConnectionOptions options)
        {
            _collection = mongoDbCollectionFactory.GetCollection<T>(options);

            _attribute = attribute;
        }

        public async Task<object> GetValueAsync()
        {
            var filterDefinition = CreateFilterDefinition();

            var value = await _collection.Find(filterDefinition)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return value;
        }

        private FilterDefinition<T> CreateFilterDefinition()
        {
            var filterDefinition = Builders<T>.Filter.Eq("_id", _attribute.Id);
            return filterDefinition;
        }

        public string ToInvokeString()
        {
            return string.Empty;
        }
        
        public Type Type { get; } = typeof(T);

        public async Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            if (_attribute.ReadOnly)
                return;

            var filter = CreateFilterDefinition();

            await _collection.ReplaceOneAsync(filter, (T)value, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}