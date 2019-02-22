using Kevsoft.Azure.WebJobs.Extensions.MongoDB;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(MongoDBWebJobsStartup))]

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public class MongoDBWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddMongoDb();
        }
    }
}