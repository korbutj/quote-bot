using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace QuoteBot.Helpers;

public static class FileCacher
{
    public static async Task SaveToFile(string path, object dic)
    {
        var serialized = JsonConvert.SerializeObject(dic);
        
        if (!File.Exists(path))
            File.Create(path);
        
        await File.WriteAllTextAsync(path, serialized);
    }

    public static async Task<T> UpdateFromFile<T>(string path) where T : class, new()
    {
        if (!File.Exists(path))
            return new T();

        var serialized = await File.ReadAllTextAsync(path);

        return JsonConvert.DeserializeObject<T>(serialized) ?? new T();
    }
}