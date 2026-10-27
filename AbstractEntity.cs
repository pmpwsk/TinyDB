using System.Text.Json.Serialization;

namespace TinyDB;

public abstract class AbstractEntity
{
    [JsonInclude]
    public string? Id { get; internal set; } = null;
}
