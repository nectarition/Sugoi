namespace Sugoi.Functions.Configurations;

public class SugoiConfiguration
{
    public const string Name = "sugoi:Sugoi.Configuration";

    public Secrets Secrets { get; set; } = new();
}

public class Secrets
{
    public string BotToken { get; set; } = string.Empty;
    public string BotPublicKey { get; set; } = string.Empty;
}
