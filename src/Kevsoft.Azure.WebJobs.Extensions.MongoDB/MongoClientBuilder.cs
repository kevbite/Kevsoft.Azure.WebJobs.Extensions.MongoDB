using Microsoft.Azure.WebJobs;
using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoClientBuilder : IConverter<MongoDbAttribute, MongoClient>
    {
        private readonly IMongoDbContextProvider _mongoDbExtensionConfigProvider;

        public MongoClientBuilder(IMongoDbContextProvider mongoDbExtensionConfigProvider)
        {
            _mongoDbExtensionConfigProvider = mongoDbExtensionConfigProvider;
        }

        public MongoClient Convert(MongoDbAttribute input)
        {
            return _mongoDbExtensionConfigProvider.CreateMongoDbContext(input)
                .GetClient();
        }
    }
}