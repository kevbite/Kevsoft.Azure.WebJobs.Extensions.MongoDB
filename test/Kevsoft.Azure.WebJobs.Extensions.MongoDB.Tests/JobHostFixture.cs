using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Tests
{
    public class JobHostFixture<TFunction> : IAsyncLifetime
    {
        private IHost _host;

        public async Task InitializeAsync()
        {
            _host = BuildHost();

            await _host.StartAsync()
                .ConfigureAwait(false);
        }

        public async Task RunFunctionAsync(Expression<Action<TFunction>> expression)
        {
            await RunFunctionAsync(expression, new Dictionary<string, object>())
                .ConfigureAwait(false);
        }

        public async Task RunFunctionAsync(Expression<Action<TFunction>> expression, IDictionary<string, object> arguments)
        {
            var methodInfo = ((MethodCallExpression)expression.Body).Method;
            
            await RunFunctionAsync(methodInfo, arguments)
                .ConfigureAwait(false);
        }

        public async Task RunFunctionAsync(MethodInfo methodInfo, IDictionary<string, object> arguments)
        {
            var jobHost = (JobHost)_host.Services.GetService<IJobHost>();
            
            await jobHost.CallAsync(methodInfo, arguments);
        }

        private IHost BuildHost()
        {
            var typeLocator = new SimpleTypeLocator(
                typeof(TFunction)
            );

            var config = new Dictionary<string, string> {{"MongoDbUrl", TestConstant.MongoDbUrl}};

            var builder = new HostBuilder()
                .UseEnvironment("Development")
                .ConfigureAppConfiguration(x => { x.AddInMemoryCollection(config); })
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

            return builder.Build();
        }

        public async Task DisposeAsync()
        {
            await _host.StopAsync()
                .ConfigureAwait(false);

            _host.Dispose();
        }
    }
}