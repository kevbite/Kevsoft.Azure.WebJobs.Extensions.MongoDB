using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Example
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "items/{id}")] HttpRequest req,
            [MongoDb(ConnectionString = "mongodb://localhost", CollectionName = "test", DatabaseName = "test", Id = "{id}")]
            BsonDocument document,
            ILogger log)
        {
            var value = document.ToJson();

            return new OkObjectResult(value);
        }
    }
}
