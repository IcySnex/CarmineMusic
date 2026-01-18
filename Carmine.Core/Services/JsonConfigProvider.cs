using Carmine.Core.Configuration;
using Carmine.Core.Services.Abstractions;
using Carmine.Core.Utilities;
using Microsoft.Extensions.Logging;
using static System.Collections.Specialized.BitVector32;

namespace Carmine.Core.Services;

public class JsonConfigProvider(
    ILogger<JsonConfigProvider> logger,
    IFileSystem fileSystem) : IConfigProvider
{
    readonly Dictionary<string, object> cache = [];


    async Task LoadAsync<T>(
        string section) where T : class, new()
    {
        if (cache.ContainsKey(section))
            return;

        try
        {
            logger.LogInformation("Loading config section '{section}'...", section);

            string path = Path.Combine(LocalFileSystem.ConfigDirectory, $"{section}.json");
            string json = await fileSystem.ReadAsync(path);
            T model = Json.Deserialize<T>(json);

            cache[section] = model;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load config section '{section}'. Creating new...", section);
            cache[section] = new T();
        }
    }


    public Task LoadAsync()
    {
        logger.LogInformation("Initializing JSON configs...");

        return Task.WhenAll(
            LoadAsync<Config>(nameof(Config)));
    }

    public async Task SaveAsync()
    {
        foreach (KeyValuePair<string, object> kvp in cache)
        {
            logger.LogInformation("Saving config section '{section}'...", kvp.Key);

            string path = Path.Combine(LocalFileSystem.ConfigDirectory, $"{kvp.Key}.json");
            string json = Json.Serialize(kvp.Value);
            await fileSystem.WriteAsync(path, json);
        }
    }


    public T Get<T>(
        string section) where T : class, new()
    {
        if (cache.TryGetValue(section, out object? model))
            return (T)model;

        throw new InvalidOperationException($"Config section '{section}' not loaded. Did you forget to call InitializeAsync()?");
    }
}