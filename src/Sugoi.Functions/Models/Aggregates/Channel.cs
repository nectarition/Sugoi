using Newtonsoft.Json;

namespace Sugoi.Functions.Models.Aggregates;

public class Channel
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("channelId")]
    public string ChannelId { get; set; } = string.Empty;

    [JsonProperty("lastMessageId")]
    public string? LastMessageId { get; set; }
}
