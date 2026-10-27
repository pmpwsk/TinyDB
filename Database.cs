using System.Text.Json;
using System.Text.Json.Serialization;

namespace TinyDB;

public class Database
{
    private static string LocationInternal = "./Data/";
    private static bool LocationUsed = false;
    public static string Location
    {
        get
        {
            if (!LocationUsed)
                LocationUsed = true;
            return LocationInternal;
        }
        set
        {
            value = value.Replace('\\', '/');
            if (!value.EndsWith('/'))
                value = $"{value}/";
            LocationInternal = value;
        }
    }

    internal static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        IgnoreReadOnlyProperties = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower, false)
        }
    };

    public static Store<T> Load<T>() where T : AbstractEntity
        => Store<T>.Instance ??= new Store<T>();
}
