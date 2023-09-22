using System.Text.Json.Serialization;

namespace Sugoi.Functions.Models.Discord
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("global_name")]
        public string GlobalName { get; set; } = string.Empty;
    }
}
