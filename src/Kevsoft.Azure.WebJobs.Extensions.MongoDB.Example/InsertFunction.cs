using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Example
{
    public static class InsertFunction
    {
        [FunctionName("InsertFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{database}/{collection}")] HttpRequest req,
            [MongoDb("{database}", "{collection}", ConnectionStringSetting = "MongoDbUrl")] IAsyncCollector<BsonDocument> documents,
            ILogger log)
        {
            var json = await req.ReadAsStringAsync().ConfigureAwait(false);

            var document = BsonDocument.Parse(json);

            await documents.AddAsync(document)
                .ConfigureAwait(false);

            return new AcceptedResult();
        }
    }
}
