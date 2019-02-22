using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Options;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    [Extension("MongoDB")]
    public class MongoDbExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly MongoDbOptions _options;
        private readonly IMongoDbCollectionFactory _mongoDbCollectionFactory;
        
        public MongoDbExtensionConfigProvider(IOptions<MongoDbOptions> options, IMongoDbCollectionFactory mongoDbCollectionFactory)
        {
            _options = options.Value;
            _mongoDbCollectionFactory = mongoDbCollectionFactory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var inputOuputBindingRule = context.AddBindingRule<MongoDbAttribute>();
            inputOuputBindingRule.AddValidator(ValidateConnectionStringUri);
            inputOuputBindingRule.WhenIsNotNull(nameof(MongoDbAttribute.Id))
                .BindToValueProvider(CreateValueBinderAsync);

        }

        private void ValidateConnectionStringUri(MongoDbAttribute attribute, Type type)
        {
            var connectionStringUri = !string.IsNullOrEmpty(_options.ConnectionString)
                ? _options.ConnectionString
                : attribute.ConnectionString;

            if (string.IsNullOrEmpty(connectionStringUri))
            {
                var attributeProperty = $"{nameof(MongoDbAttribute)}.{nameof(MongoDbAttribute.ConnectionString)}";
                var optionsProperty = $"{nameof(MongoDbOptions)}.{nameof(MongoDbOptions.ConnectionString)}";

                throw new InvalidOperationException($"The MongoDB ConnectionString must be set either via the '{attributeProperty}' property or via '{optionsProperty}'.");
            }
        }

        private Task<IValueBinder> CreateValueBinderAsync(MongoDbAttribute attribute, Type type)
        {
            var connectionOptions = ConnectionOptionsBuilder.Build(attribute, _options);
            var binderType = typeof(MongoDbValueBinder<>).MakeGenericType(type);

            var valueBinder = (IValueBinder)Activator.CreateInstance(binderType, _mongoDbCollectionFactory, attribute, connectionOptions);

            return Task.FromResult(valueBinder);
        }
    }
}