using System.Text.Json.Serialization;

namespace Sugoi.Functions.Models.Discord
{
    public class Message
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; } = null!;

        [JsonPropertyName("author")]
        public User User { get; set; } = default!;

        [JsonPropertyName("timestamp")]
        public DateTime CreatedAt { get; set; }
    }
}
