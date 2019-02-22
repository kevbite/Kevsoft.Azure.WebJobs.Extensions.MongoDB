using System;
using Microsoft.Azure.WebJobs.Description;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class MongoDbAttribute : Attribute
    {
        [AutoResolve]
        public string Id { get; set; }

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }

        public bool ReadOnly { get; set; }
    }
}