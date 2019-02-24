using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Tests
{
    public class BindingNativeMongoDbObjectsTests : IClassFixture<JobHostFixture<BindingNativeMongoDbObjectsTests.Functions>>
    {
        private readonly JobHostFixture<Functions> _jobHostFixture;
        private readonly Fixture _fixture;

        public class Functions
        {
            public static MongoClient CapturedMongoClient { get; private set; }

            public static IMongoDatabase CapturedMongoDatabase { get; private set; }

            public static IMongoCollection<BsonDocument> CapturedMongoCollection { get; private set; }

            [FunctionName("NativeClient")]
            public void RunNativeClient(
                [MongoDb(ConnectionStringSetting = "MongoDbUrl")]
                MongoClient client)
            {
                CapturedMongoClient = client;
            }


            [FunctionName("NativeDatabase")]
            public void RunNativeDatabase(
                [MongoDb("{database}", ConnectionStringSetting = "MongoDbUrl")]
                IMongoDatabase mongoDatabase)
            {
                CapturedMongoDatabase = mongoDatabase;
            }

            [FunctionName("NativeCollection")]
            public void RunNativeCollection(
                [MongoDb("{database}", "{collection}", ConnectionStringSetting = "MongoDbUrl")]
                IMongoCollection<BsonDocument> mongoCollection)
            {
                CapturedMongoCollection = mongoCollection;
            }
        }

        public BindingNativeMongoDbObjectsTests(JobHostFixture<Functions> jobHostFixture)
        {
            _jobHostFixture = jobHostFixture;
            _fixture = new Fixture();
        }

        [Fact]
        public async Task ShouldBindMongoClient()
        {
            await _jobHostFixture.RunFunctionAsync(x => x.RunNativeClient(default));

            Functions.CapturedMongoClient.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldBindMongoDatabase()
        {
            var databaseName = _fixture.Create<string>();
            var arguments = new Dictionary<string, object>()
            {
                {"database", databaseName},
            };

            await _jobHostFixture.RunFunctionAsync(x => x.RunNativeDatabase(default), arguments);

            Functions.CapturedMongoDatabase.Should().NotBeNull();
            Functions.CapturedMongoDatabase.DatabaseNamespace.DatabaseName.Should().Be(databaseName);
        }

        [Fact]
        public async Task ShouldBindMongoCollection()
        {
            var databaseName = _fixture.Create<string>();
            var collectionName = _fixture.Create<string>();
            var arguments = new Dictionary<string, object>()
            {
                {"database", databaseName},
                {"collection", collectionName }
            };

            await _jobHostFixture.RunFunctionAsync(x => x.RunNativeCollection(default), arguments);

            Functions.CapturedMongoCollection.Should().NotBeNull();
            Functions.CapturedMongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName.Should().Be(databaseName);
            Functions.CapturedMongoCollection.CollectionNamespace.CollectionName.Should().Be(collectionName);
        }
    }

}
