namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }
    }
}