namespace Carmine.Core.Services.Abstractions;

public interface IConfigProvider
{
    Task LoadAsync();

    Task SaveAsync();


    T Get<T>() where T : class, new();
}