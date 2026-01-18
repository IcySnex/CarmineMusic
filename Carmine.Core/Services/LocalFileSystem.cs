using Carmine.Core.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Carmine.Core.Services;

public class LocalFileSystem(
    ILogger<LocalFileSystem> logger) : IFileSystem
{
    public static string RootDirectory { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

    public static string LogsDirectory { get; } = Path.Combine(RootDirectory, "Logs");
    public static string ConfigDirectory { get; } = Path.Combine(RootDirectory, "Config");


    public Task WriteAsync(
        string path,
        string text)
    {
        logger.LogInformation("Writing file (text) asynchronously at path '{path}'.", path);

        string? directory = Path.GetDirectoryName(path);
        if (directory is not null && !Directory.Exists(directory))
        {
            logger.LogInformation("Creating directory '{directory}'.", directory);
            Directory.CreateDirectory(directory);
        }

        return File.WriteAllTextAsync(path, text);
    }

    public Task<string> ReadAsync(
        string path)
    {
        logger.LogInformation("Reading file (text) asynchronously at path '{path}'.", path);

        return File.ReadAllTextAsync(path);
    }

    public Task<bool> ExistsAsync(
        string path)
    {
        logger.LogInformation("Checking file existance at path '{path}'.", path);

        return Task.FromResult(File.Exists(path));
    }
}