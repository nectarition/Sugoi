using Newtonsoft.Json;

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
}
