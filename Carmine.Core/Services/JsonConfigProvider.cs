using Carmine.Core.Configuration;
using Carmine.Core.Services.Abstractions;
using Carmine.Core.Utilities;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace Carmine.Core.Services;

public class JsonConfigProvider(
    ILogger<JsonConfigProvider> logger,
    IFileSystem fileSystem) : IConfigProvider
{
    readonly Dictionary<string, object> cache = [];


    async Task LoadAsync<T>() where T : class, new()
    {
        string name = typeof(T).Name;

        if (cache.ContainsKey(name))
            return;

        try
        {
            logger.LogInformation("Loading config '{name}'...", name);

            string path = Path.Combine(LocalFileSystem.ConfigDirectory, $"{name}.json");
            string json = await fileSystem.ReadAsync(path);
            T model = Json.Deserialize<T>(json);

            cache[name] = model;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load config '{name}'. Creating new...", name);
            cache[name] = new T();
        }
    }


    public Task LoadAsync()
    {
        logger.LogInformation("Initializing JSON configs...");

        return Task.WhenAll(
            LoadAsync<Config>());
    }

    public async Task SaveAsync()
    {
        foreach (KeyValuePair<string, object> kvp in cache)
        {
            logger.LogInformation("Saving config '{name}'...", kvp.Key);

            string path = Path.Combine(LocalFileSystem.ConfigDirectory, $"{kvp.Key}.json");
            string json = Json.Serialize(kvp.Value);
            await fileSystem.WriteAsync(path, json);
        }
    }


    public T Get<T>() where T : class, new()
    {
        string name = typeof(T).Name;

        if (cache.TryGetValue(name, out object? model))
            return (T)model;

        throw new InvalidOperationException($"Config '{name}' not loaded.");
    }
}