namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public interface IMongoDbContextProvider
    {
        MongoDbContext CreateMongoDbContext(MongoDbAttribute attribute);
    }
}