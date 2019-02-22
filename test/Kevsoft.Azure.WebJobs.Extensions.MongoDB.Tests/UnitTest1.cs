using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Xunit;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Tests
{
    internal class SamplesTypeLocator : ITypeLocator
    {
        private Type[] _types;

        public SamplesTypeLocator(params Type[] types)
        {
            _types = types;
        }

        public IReadOnlyList<Type> GetTypes()
        {
            return _types;
        }
    }

 
    public class UnitTest1
    {
        public class Functions
        {
            [FunctionName("QueryIdByObjectId1")]
            public static void RunByObjectId(
                [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl", IdType = typeof(string))]
                BsonDocument document,
                ILogger log)
            {
                
            }
        }


        [Fact]
        public async Task ShouldRun()
        {
            var typeLocator = new SamplesTypeLocator(
                typeof(Functions)
                );
            var config = new Dictionary<string, string> { { "MongoDbUrl", "mongodb://localhost" } };

            var builder = new HostBuilder()
                .UseEnvironment("Development")
                .ConfigureAppConfiguration(x =>
                {
                    x.AddInMemoryCollection(config);
                })
                .ConfigureWebJobs(webJobsBuilder =>
                {
                    webJobsBuilder
                        .AddMongoDb();
                })
                .ConfigureLogging(b =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    b.AddConsole();
                })
                .ConfigureServices(s => { s.AddSingleton<ITypeLocator>(typeLocator); });

            var host = builder.Build();
            using (host)
            {
                // Some direct invocations to demonstrate various binding scenarios
                var jobHost = (JobHost)host.Services.GetService<IJobHost>();
                var dictionary = new Dictionary<string, object>()
                {
                    { "database", "test"},
                    { "collection", "test"},
                    { "id", "one"},
                };

                await jobHost.CallAsync(typeof(Functions).GetMethod("RunByObjectId"), dictionary);
            }

            await host.StopAsync();
        }
    }
}
