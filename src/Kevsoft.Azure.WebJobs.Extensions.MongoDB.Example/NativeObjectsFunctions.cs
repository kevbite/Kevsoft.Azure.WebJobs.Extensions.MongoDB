using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Example
{
    public static class NativeObjectsFunctions
    {
        [FunctionName("NativeClient")]
        public static async Task<IActionResult> RunNativeClient(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "native/client")] HttpRequest req,
            [MongoDb(ConnectionStringSetting = "MongoDbUrl")]
            MongoClient client,
            ILogger log)
        {
            var databases = await client.ListDatabaseNames()
                .ToListAsync()
                .ConfigureAwait(false);

            return new OkObjectResult(new {databases});
        }

        [FunctionName("NativeDatabase")]
        public static async Task<IActionResult> RunNativeDatabase(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "native/database")] HttpRequest req,
            [MongoDb("test", ConnectionStringSetting = "MongoDbUrl")]
            IMongoDatabase database,
            ILogger log)
        {
            var collections = await database.ListCollectionNames()
                .ToListAsync()
                .ConfigureAwait(false);

            return new OkObjectResult(new { collections });
        }

        [FunctionName("NativeCollection")]
        public static async Task<IActionResult> RunNativeCollection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "native/collection")] HttpRequest req,
            [MongoDb("test", "test", ConnectionStringSetting = "MongoDbUrl")]
            IMongoCollection<BsonDocument> collection,
            ILogger log)
        {
            var documents = await collection.AsQueryable().Take(10)
                .ToListAsync();
                
            return new OkObjectResult(new { documents });
        }
    }
}