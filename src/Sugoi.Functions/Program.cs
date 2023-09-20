using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sugoi.Functions.Configurations;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder => {
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        builder.AddUserSecrets<ThisAssemblyReference>();
    })
    .ConfigureServices((context, services) =>
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

    })
    .Build();

host.Run();

internal class ThisAssemblyReference { }
