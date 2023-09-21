using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sugoi.Functions.Configurations;
using Sugoi.Functions.Services;

var host = new HostBuilder()
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(ConfigureAppConfiguration)
    .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
    .ConfigureServices(ConfigureServices)
    .Build();

host.Run();

void ConfigureAppConfiguration(IConfigurationBuilder builder)
{
    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
    builder.AddUserSecrets<ThisAssemblyReference>();
}

void ConfigureContainer(HostBuilderContext context, ContainerBuilder builder)
{
    var functionLogAssembly = typeof(HelloService).Assembly;
    builder.RegisterAssemblyTypes(functionLogAssembly)
        .Where(t => !t.IsInterface)
        .Where(t => t.Name.EndsWith("Service"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
}

void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services
        .Configure<SugoiConfiguration>(context.Configuration.GetSection(SugoiConfiguration.Name));

    services
        .AddSingleton(resolver =>
        {
            var connectionString = resolver
                .GetRequiredService<IConfiguration>()
                .GetConnectionString("CosmosDbConnectionString");

            var client = new CosmosClient(connectionString);
            return client;
        });
}

internal class ThisAssemblyReference { }
