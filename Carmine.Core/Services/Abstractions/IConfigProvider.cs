namespace Carmine.Core.Services.Abstractions;

public interface IConfigProvider
{
    Task LoadAsync();

    Task SaveAsync();


    T Get<T>(
        string section) where T : class, new();
}