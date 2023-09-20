using System.Text.Json.Serialization;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Models;

public class InteractionPayload
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("application_id")]
    public string ApplicationId { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public InteractionTypes InteractionType { get; set; }
}
