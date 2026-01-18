namespace Carmine.Core.Services.Abstractions;

public interface IFileSystem
{
    Task WriteAsync(
        string path,
        string text);

    Task<string> ReadAsync(
        string path);

    Task<bool> ExistsAsync(
        string path);
}