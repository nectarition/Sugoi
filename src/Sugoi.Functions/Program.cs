using Autofac;
using Autofac.Extensions.DependencyInjection;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
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

    services.AddSingleton(resolver =>
    {
        var configuration = resolver
            .GetRequiredService<IConfiguration>()
            .GetSection(SugoiConfiguration.Name)
            .Get<SugoiConfiguration>();

        var client = new DiscordSocketClient();
        client.LoginAsync(TokenType.Bot, configuration.Secrets.BotToken);
        client.StartAsync();

        return client;
    });
}

internal class ThisAssemblyReference { }
