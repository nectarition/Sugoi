using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Sugoi.Functions.Models.Aggregates;

public class User
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("userName")]
    public string? UserName { get; set; }

    [JsonProperty("postedAt")]
    public DateTime PostedAt { get; set; }

    [JsonPropertyName("lastMessageChannelId")]
    public string LastMessageChannelId { get; set; } = string.Empty;

    [JsonPropertyName("lastMessageId")]
    public string LastMessageId { get; set; } = string.Empty;
}
