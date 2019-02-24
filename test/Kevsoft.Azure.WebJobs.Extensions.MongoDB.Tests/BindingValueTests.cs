using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Tests
{
    public class BindingValueTests : IClassFixture<JobHostFixture<BindingValueTests.Functions>>, IAsyncLifetime
    {
        private readonly JobHostFixture<Functions> _jobHostFixture;
        private readonly Fixture _fixture;
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        public class Functions
        {
            public static List<BsonDocument> CapturedValues { get; } = new List<BsonDocument>();

            [FunctionName("ByObjectId")]
            public void RunByObjectId(
                [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl")]
                BsonDocument document)
            {
                CapturedValues.Add(document);
            }
        }

        public BindingValueTests(JobHostFixture<Functions> jobHostFixture)
        {
            _jobHostFixture = jobHostFixture;
            _fixture = new Fixture();

            _client = new MongoClient(TestConstant.MongoDbUrl);
            _database = _client.GetDatabase(Guid.NewGuid().ToString());
            _collection = _database.GetCollection<BsonDocument>(Guid.NewGuid().ToString());
        }

        [Fact]
        public async Task ShouldBindToDocumentByObjectId()
        {
            var actual = new BsonDocument(_fixture.Create<Dictionary<string, string>>());
            _collection.InsertOne(actual);

            var arguments = new Dictionary<string, object>()
            {
                { "database", _database.DatabaseNamespace.DatabaseName},
                { "collection", _collection.CollectionNamespace.CollectionName},
                { "id", actual["_id"].AsObjectId.ToString()},
            };

            await _jobHostFixture.RunFunctionAsync(x => x.RunByObjectId(default), arguments);

            Functions.CapturedValues.Single().Should().BeEquivalentTo(actual);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await _client.DropDatabaseAsync(_database.DatabaseNamespace.DatabaseName)
                .ConfigureAwait(false);
        }
    }
}
