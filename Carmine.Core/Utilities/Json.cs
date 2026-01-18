using System.Text.Json;

namespace Carmine.Core.Utilities;

public static class Json
{
    static readonly JsonSerializerOptions formattedOptions = new()
    {
        WriteIndented = true
    };

    public static string Serialize<T>(
        T obj,
        bool formatted = false) =>
        JsonSerializer.Serialize(obj, formatted ? formattedOptions : null);

    public static T Deserialize<T>(
        string json) =>
        JsonSerializer.Deserialize<T>(json) ?? throw new JsonException("Failed to deserialize JSON.");


    public static IDisposable Parse(
        string json,
        out JsonElement root)
    {
        JsonDocument document = JsonDocument.Parse(json);

        root = document.RootElement;
        return document;
    }
}