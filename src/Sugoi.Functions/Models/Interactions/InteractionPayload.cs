using System.Text.Json.Serialization;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Models.Interactions;

public class InteractionPayload
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("application_id")]
    public string ApplicationId { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public InteractionTypes InteractionType { get; set; }

    [JsonPropertyName("data")]
    public InteractionDataPayload? Data { get; set; }
}


public class InteractionDataPayload
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public ApplicationCommandTypes ApplicationCommandType { get; set; }

    [JsonPropertyName("options")]
    public InteractionDataOption[] Options { get; set; } = Array.Empty<InteractionDataOption>();
}

public class InteractionDataOption
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public ApplicationCommandOptionTypes ApplicationCommandOptionType { get; set; }

    [JsonPropertyName("options")]
    public InteractionDataOptionItem[] Items { get; set; } = Array.Empty<InteractionDataOptionItem>();
}

public class InteractionDataOptionItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public ApplicationCommandOptionTypes ApplicationCommandOptionType { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}