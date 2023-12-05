using System.Text.Json.Serialization;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Models.Discord
{
    public class GuildChannel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("type")]
        public DiscordChannelTypes DiscordChannelType { get; set; }

        [JsonPropertyName("parent_id")]
        public string? ParentId { get; set; }

        [JsonPropertyName("last_message_id")]
        public string? LastMessageId { get; set; }
    }
}
