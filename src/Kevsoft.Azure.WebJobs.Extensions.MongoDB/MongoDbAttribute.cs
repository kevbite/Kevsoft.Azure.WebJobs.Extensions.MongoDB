using System;
using Microsoft.Azure.WebJobs.Description;
using MongoDB.Bson;

namespace Kevsoft.Azure.WebJobs
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class MongoDbAttribute : Attribute
    {
        public MongoDbAttribute()
        {
            
        }

        public MongoDbAttribute(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public MongoDbAttribute(string databaseName, string collectionName)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        public MongoDbAttribute(string databaseName, string collectionName, string id)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
            Id = id;
        }

        [AutoResolve]
        public string Id { get; set; }

        public Type IdType { get; set; } = typeof(ObjectId);
        
        [AppSetting]
        public string ConnectionStringSetting { get; set; }

        [AutoResolve]
        public string DatabaseName { get; set; }

        [AutoResolve]
        public string CollectionName { get; set; }

        public bool ReadOnly { get; set; }
    }
}