using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    [Extension("MongoDB")]
    public class MongoDbExtensionConfigProvider : IExtensionConfigProvider, IMongoDbContextProvider
    {
        private readonly MongoDbOptions _options;
        private readonly IMongoDbFactory _mongoDbFactory;
        
        public MongoDbExtensionConfigProvider(IOptions<MongoDbOptions> options, IMongoDbFactory mongoDbFactory)
        {
            _options = options.Value;
            _mongoDbFactory = mongoDbFactory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var inputOuputBindingRule = context.AddBindingRule<MongoDbAttribute>();
            inputOuputBindingRule.AddValidator(ValidateConnectionString);

            inputOuputBindingRule.BindToCollector<DocumentOpenType>(typeof(MongoDbCollectorBuilder<>), this);

            inputOuputBindingRule.BindToInput<MongoClient>(new MongoClientBuilder(this));
            inputOuputBindingRule.BindToInput<IMongoDatabase>(new MongoDatabaseClientBuilder(this));
                
            inputOuputBindingRule.BindToInput<IMongoCollection<DocumentOpenType>>(typeof(MongoCollectionBuilder<>), this);

            inputOuputBindingRule.WhenIsNotNull(nameof(MongoDbAttribute.Id))
                .BindToValueProvider(CreateValueBinderAsync);
        }

        private void ValidateConnectionString(MongoDbAttribute attribute, Type type)
        {
            var connectionString = !string.IsNullOrEmpty(_options.ConnectionString)
                ? _options.ConnectionString
                : attribute.ConnectionStringSetting;

            if (string.IsNullOrEmpty(connectionString))
            {
                var attributeProperty = $"{nameof(MongoDbAttribute)}.{nameof(MongoDbAttribute.ConnectionStringSetting)}";
                var optionsProperty = $"{nameof(MongoDbOptions)}.{nameof(MongoDbOptions.ConnectionString)}";

                throw new InvalidOperationException($"The MongoDB ConnectionString must be set either via the '{attributeProperty}' property or via '{optionsProperty}'.");
            }
        }

        private Task<IValueBinder> CreateValueBinderAsync(MongoDbAttribute attribute, Type type)
        {
            var binderType = typeof(MongoDbValueBinder<>).MakeGenericType(type);
            var context = CreateMongoDbContext(attribute);

            var valueBinder = (IValueBinder)Activator.CreateInstance(binderType, context);

            return Task.FromResult(valueBinder);
        }

        public MongoDbContext CreateMongoDbContext(MongoDbAttribute attribute)
        {
            var connectionOptions = ConnectionOptionsBuilder.Build(attribute, _options);

            return new MongoDbContext(attribute, _mongoDbFactory, connectionOptions);
        }

        private class DocumentOpenType : OpenType.Poco
        {
            public override bool IsMatch(Type type, OpenTypeMatchContext context)
            {
                if (type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return false;
                }

                if (type.FullName == "System.Object")
                {
                    return true;
                }

                return base.IsMatch(type, context);
            }
        }
    }

    public class MongoCollectionBuilder<T> : IConverter<MongoDbAttribute, IMongoCollection<T>>
    {
        private readonly IMongoDbContextProvider _mongoDbExtensionConfigProvider;

        public MongoCollectionBuilder(IMongoDbContextProvider mongoDbExtensionConfigProvider)
        {
            _mongoDbExtensionConfigProvider = mongoDbExtensionConfigProvider;
        }

        public IMongoCollection<T> Convert(MongoDbAttribute input)
        {
            return _mongoDbExtensionConfigProvider.CreateMongoDbContext(input)
                .GetCollection<T>();
        }
    }
}

