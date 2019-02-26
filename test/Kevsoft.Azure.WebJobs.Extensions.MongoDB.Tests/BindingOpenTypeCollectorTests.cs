using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Tests
{
    public class BindingOpenTypeCollectorTests : IClassFixture<JobHostFixture<BindingOpenTypeCollectorTests.Functions>>
    {
        private readonly JobHostFixture<Functions> _jobHostFixture;
        private readonly IMongoCollection<BsonDocument> _collection;

        public class Functions
        {
            private static readonly Fixture Fixture = new Fixture();

            [FunctionName("AddCollector")]
            public async Task RunAddCollector(
                [MongoDb("{database}", "{collection}", ConnectionStringSetting = "MongoDbUrl")]
                IAsyncCollector<BsonDocument> collector)
            {
                var values = Fixture.Create<Dictionary<string, string>>();
                var document = new BsonDocument(values);
                await collector.AddAsync(document);
            }
        }

        public BindingOpenTypeCollectorTests(JobHostFixture<Functions> jobHostFixture)
        {
            _jobHostFixture = jobHostFixture;

            var client = new MongoClient(TestConstant.MongoDbUrl);
            var database = client.GetDatabase(Guid.NewGuid().ToString());
            _collection = database.GetCollection<BsonDocument>(Guid.NewGuid().ToString());
        }

        [Fact]
        public async Task ShouldBindMongoClient()
        {
            await _jobHostFixture.RunFunctionAsync(x => x.RunAddCollector(default), CreateArguments());

            var bsonDocument = await _collection.Find(x => true)
                .SingleAsync();

            bsonDocument.Should().NotBeNull();
        }

        private Dictionary<string, object> CreateArguments()
        {
            var arguments = new Dictionary<string, object>()
            {
                {"database", _collection.CollectionNamespace.DatabaseNamespace.DatabaseName},
                {"collection", _collection.CollectionNamespace.CollectionName}
            };

            return arguments;
        }

    }
}