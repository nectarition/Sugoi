namespace Sugoi.Functions.Configurations;

public class SugoiConfiguration
{
    public const string Name = "sugoi:Sugoi.Configuration";

    public Aggregates Aggregates { get; set; } = new();

    public Secrets Secrets { get; set; } = new();
}

public class Aggregates
{
    public string GuildId { get; set; } = null!;

    public string TargetParentId { get; set; } = null!;

    public string[] AllowChannelIds { get; set; } = Array.Empty<string>();

    public string[] ExceptParentIds { get; set; } = Array.Empty<string>();
}

public class Secrets
{
    public string BotToken { get; set; } = null!;

    public string BotPublicKey { get; set; } = null!;
}
