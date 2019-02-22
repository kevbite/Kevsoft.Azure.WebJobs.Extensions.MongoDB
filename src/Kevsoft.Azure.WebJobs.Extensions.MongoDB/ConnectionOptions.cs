namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class ConnectionOptions
    {
        public string ConnectionString { get; }

        public string DatabaseName { get; }

        public string CollectionName { get; }
        
        public ConnectionOptions(string connectionString, string databaseName, string collectionName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }
    }
}