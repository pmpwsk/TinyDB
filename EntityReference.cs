using System.Text.Json.Serialization;

namespace TinyDB;

public class EntityReference<T> where T : AbstractEntity
{
    public string Id;

    [JsonIgnore]
    private T? LoadedValue;

    [JsonIgnore]
    public T Value
    {
        get
        {
            if (LoadedValue == null)
            {
                var store = Database.Load<T>();
                LoadedValue = store.Find(Id) ?? throw new Exception($"Entity of ID {Id} could not be found for type {typeof(T).Name}");
            }
            return LoadedValue;
        }
        set
        {
            Id = value.Id ?? throw new Exception("The entity doesn't have an ID.");
            LoadedValue = value;
        }
    }

    [JsonConstructor]
    public EntityReference(string id)
    {
        Id = id;
        LoadedValue = null;
    }

    public EntityReference(T value)
    {
        Id = value.Id ?? throw new Exception("The entity doesn't have an ID.");
        LoadedValue = value;
    }

    public static implicit operator EntityReference<T>(T value)
        => new(value);

    public static implicit operator T(EntityReference<T> reference)
        => reference.Value;
}
