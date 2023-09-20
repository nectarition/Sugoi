using System.Text.Json.Serialization;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Models.Interactions;

public class InteractionResult
{
    [JsonPropertyName("type")]
    public InteractionResponseTypes InteractionResponseType { get; set; }

    [JsonPropertyName("data")]
    public InteractionResultData? Data { get; set; }

}

public class InteractionResultData
{
    [JsonPropertyName("tts")]
    public bool IsTTS { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; } = string.Empty;
}
