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

            [FunctionName("ByStringId")]
            public void RunByStringId(
                [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl", IdType = typeof(string))]
                BsonDocument document)
            {
                CapturedValues.Add(document);
            }

            [FunctionName("ByIntId")]
            public void RunByIntId(
                [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl", IdType = typeof(int))]
                BsonDocument document)
            {
                CapturedValues.Add(document);
            }

            [FunctionName("ChangeValue")]
            public void RunChangeValue(
                [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl")]
                BsonDocument document)
            {
                document.Add("changed", true);

                CapturedValues.Add(document);
            }

            [FunctionName("ReadOnly")]
            public void RunReadOnly(
                [MongoDb("{database}", "{collection}", "{id}", ReadOnly = true, ConnectionStringSetting = "MongoDbUrl")]
                BsonDocument document)
            {
                document.Add("changed", true);

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

            var arguments = CreateArguments(actual["_id"].AsObjectId.ToString());

            await _jobHostFixture.RunFunctionAsync(x => x.RunByObjectId(default), arguments);

            Functions.CapturedValues.Single().Should().BeEquivalentTo(actual);
        }

        [Fact]
        public async Task ShouldBindToDocumentByStringId()
        {
            var actual = new BsonDocument(_fixture.Create<Dictionary<string, string>>());
            var id = _fixture.Create<string>();
            actual.Add("_id", id);

            _collection.InsertOne(actual);

            var arguments = CreateArguments(id);

            await _jobHostFixture.RunFunctionAsync(x => x.RunByStringId(default), arguments);

            Functions.CapturedValues.Single().Should().BeEquivalentTo(actual);
        }

        [Fact]
        public async Task ShouldBindToDocumentByIntId()
        {
            var actual = new BsonDocument(_fixture.Create<Dictionary<string, string>>());
            var id = _fixture.Create<int>();
            actual.Add("_id", id);

            _collection.InsertOne(actual);

            var arguments = CreateArguments(id.ToString());

            await _jobHostFixture.RunFunctionAsync(x => x.RunByIntId(default), arguments);

            Functions.CapturedValues.Single().Should().BeEquivalentTo(actual);
        }

        [Fact]
        public async Task ShouldUpdateMongoDocument()
        {
            var initial = new BsonDocument(_fixture.Create<Dictionary<string, string>>());

            _collection.InsertOne(initial);

            var arguments = CreateArguments(initial["_id"].AsObjectId.ToString());

            await _jobHostFixture.RunFunctionAsync(x => x.RunChangeValue(default), arguments);

            var storedDocument = await _collection.Find(x => x["_id"] == initial["_id"])
                .SingleAsync();

            storedDocument.Should().NotBeEquivalentTo(initial);
            storedDocument.Should().BeEquivalentTo(Functions.CapturedValues.Single());
        }

        [Fact]
        public async Task ShouldNotUpdateMongoDocument_WhenSetToReadOnly()
        {
            var initial = new BsonDocument(_fixture.Create<Dictionary<string, string>>());

            _collection.InsertOne(initial);

            var arguments = CreateArguments(initial["_id"].AsObjectId.ToString());

            await _jobHostFixture.RunFunctionAsync(x => x.RunReadOnly(default), arguments);

            var storedDocument = await _collection.Find(x => x["_id"] == initial["_id"])
                .SingleAsync();

            storedDocument.Should().BeEquivalentTo(initial);
            storedDocument.Should().NotBeEquivalentTo(Functions.CapturedValues.Single());
        }

        private Dictionary<string, object> CreateArguments(string id)
        {
            var arguments = new Dictionary<string, object>()
            {
                {"database", _database.DatabaseNamespace.DatabaseName},
                {"collection", _collection.CollectionNamespace.CollectionName},
                {"id", id},
            };

            return arguments;
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            Functions.CapturedValues.Clear();

            await _client.DropDatabaseAsync(_database.DatabaseNamespace.DatabaseName)
                .ConfigureAwait(false);
        }
    }
}
