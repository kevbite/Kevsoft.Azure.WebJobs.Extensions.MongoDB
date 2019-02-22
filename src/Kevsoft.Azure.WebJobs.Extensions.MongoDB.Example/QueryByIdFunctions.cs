using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Example
{
    public static class QueryByIdFunctions
    {
        [FunctionName("QueryIdByObjectId")]
        public static IActionResult RunByObjectId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{database}/{collection}/by-object-id/{id}")] HttpRequest req,
            [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl")]
            BsonDocument document,
            ILogger log)
        {
            var value = document.ToJson();

            return new OkObjectResult(value);
        }

        [FunctionName("QueryIdByInt")]
        public static IActionResult RunByInt(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{database}/{collection}/by-int/{id}")] HttpRequest req,
            [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl", IdType = typeof(int))]
            BsonDocument document,
            ILogger log)
        {
            var value = document.ToJson();

            return new OkObjectResult(value);
        }

        [FunctionName("QueryIdByString")]
        public static IActionResult RunByString(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{database}/{collection}/by-string/{id}")] HttpRequest req,
            [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl", IdType = typeof(string))]
            BsonDocument document,
            ILogger log)
        {
            var value = document.ToJson();

            return new OkObjectResult(value);
        }
    }
}
