using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TinyDB;

public class Store<T> where T : AbstractEntity
{
    internal static Store<T>? Instance = null;

    public static byte[] Serialize(T obj)
        => JsonSerializer.SerializeToUtf8Bytes(obj, Database.SerializerOptions);

    public static T Deserialize(byte[] json)
        => JsonSerializer.Deserialize<T>(json, Database.SerializerOptions) ?? throw new Exception("Deserialization failed.");

    private readonly Dictionary<string, T> Objects;

    internal Store()
    {
        Objects = [];
        var dirInfo = Directory.CreateDirectory($"{Database.Location}{typeof(T).Name}/");
        foreach (var fileInfo in dirInfo.GetFiles())
        {
            var obj = Deserialize(File.ReadAllBytes(fileInfo.FullName));

            if (obj.Id == null)
                throw new Exception("Object doesn't have an ID.");
            if (obj.Id != fileInfo.Name[..^5])
                throw new Exception("Object ID doesn't match its file name.");

            Objects[obj.Id] = obj;
        }
    }

    private string GenerateId()
    {
        string id;
        do
        {
            id = RandomId();
        } while (Objects.ContainsKey(id));
        return id;
    }

    private static string RandomId()
    {
        StringBuilder result = new();
        while (result.Length < 16)
            result.Append(RandomChar(RandomChar("aA1") switch
            {
                'a' => "abcdefghijklmnopqrstuvwxyz",
                'A' => "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                '1' => "0123456789",
                _ => throw new Exception("Unrecognized character set!")
            }));
        return result.ToString();
    }

    public static char RandomChar(string characters)
        => characters[RandomNumberGenerator.GetInt32(characters.Length)];

    public void Save(T obj)
    {
        obj.Id ??= GenerateId();
        File.WriteAllBytes(
            $"{Database.Location}{typeof(T).Name}/{obj.Id}.json",
            Serialize(obj)
        );
        Objects[obj.Id] = obj;
    }

    public void Delete(T obj)
    {
        if (obj.Id == null)
            return;
        if (File.Exists($"{Database.Location}{typeof(T).Name}/{obj.Id}.json"))
            File.Delete($"{Database.Location}{typeof(T).Name}/{obj.Id}.json");
        Objects.Remove(obj.Id);
    }

    public T? Find(string id)
        => Objects.TryGetValue(id, out var obj) ? obj : null;

    public IReadOnlyCollection<T> List()
        => Objects.Values;
}
