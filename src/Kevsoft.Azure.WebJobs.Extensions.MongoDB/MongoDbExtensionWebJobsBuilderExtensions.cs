using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB
{
    public static class MongoDbExtensionWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddMongoDb(this IWebJobsBuilder builder, Action<MongoDbOptions> configure)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddMongoDb();
            builder.Services.Configure(configure);

            return builder;
        }

        public static IWebJobsBuilder AddMongoDb(this IWebJobsBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<MongoDbExtensionConfigProvider>()
                .ConfigureOptions<MongoDbOptions>((config, path, options) =>
                {
                    config.GetSection(path).Bind(options);  
                });

            builder.Services.AddSingleton<IMongoDbCollectionFactory, MongoDbCollectionFactory>();

            return builder;
        }

    }
}