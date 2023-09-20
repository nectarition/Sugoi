using Discord.WebSocket;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Sugoi.Functions.Functions;
public class AggregateMessages
{
    private ILogger Logger { get; }
    private DiscordSocketClient DiscordSocketClient { get; }

    public AggregateMessages(
        ILoggerFactory loggerFactory,
        DiscordSocketClient discordSocketClient)
    {
        DiscordSocketClient = discordSocketClient;
        Logger = loggerFactory.CreateLogger<AggregateMessages>();
    }

    //[Function(nameof(AggregateMessagesWithTimer))]
    //public void AggregateMessagesWithTimer([TimerTrigger("* */5 * * * *")] TimerInfo myTimer)
    //{
    //    AggregateMessagesCore();
    //}

    public void AggregateMessagesCore()
    {
        Logger.LogInformation("Triggered AgreegateMessagesCore");
    }
}
