using Microsoft.Azure.WebJobs;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDbCollectorBuilder<T> : IConverter<MongoDbAttribute, IAsyncCollector<T>>
    {
        private readonly IMongoDbContextProvider _contextProvider;

        public MongoDbCollectorBuilder(IMongoDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public IAsyncCollector<T> Convert(MongoDbAttribute attribute)
        {
            var mongoDbContext = _contextProvider.CreateMongoDbContext(attribute);

            return new MongoDbAsyncCollector<T>(mongoDbContext);
        }
    }
}