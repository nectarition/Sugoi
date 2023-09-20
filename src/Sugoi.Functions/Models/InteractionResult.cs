using System.Text.Json.Serialization;
using static Sugoi.Functions.Enumerations;

namespace Sugoi.Functions.Models;

public class InteractionResult
{
    [JsonPropertyName("type")]
    public InteractionResponseTypes InteractionResponseType { get; set; }
}
