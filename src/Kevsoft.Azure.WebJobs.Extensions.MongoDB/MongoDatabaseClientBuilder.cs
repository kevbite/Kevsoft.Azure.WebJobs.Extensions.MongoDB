using Microsoft.Azure.WebJobs;
using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDatabaseClientBuilder : IConverter<MongoDbAttribute, IMongoDatabase>
    {
        private readonly IMongoDbContextProvider _mongoDbExtensionConfigProvider;

        public MongoDatabaseClientBuilder(IMongoDbContextProvider mongoDbExtensionConfigProvider)
        {
            _mongoDbExtensionConfigProvider = mongoDbExtensionConfigProvider;
        }

        public IMongoDatabase Convert(MongoDbAttribute input)
        {
            return _mongoDbExtensionConfigProvider.CreateMongoDbContext(input)
                .GetDatabase();
        }
    }
}