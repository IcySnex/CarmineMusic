using Avalonia.Controls;
using Carmine.Core.Navigation;
using Carmine.Core.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShadUI;
using System.Reflection;

namespace Carmine.Core.Services;

public partial class Navigator(
    ILogger<Navigator> logger,
    ToastManager toastManager) : ObservableObject
{
    class Page(
        Type viewModelType,
        Type viewType,
        Control? view,
        bool cacheView,
        MethodInfo? onNavigatedTo,
        bool onNavigatedToAcceptsParameters,
        MethodInfo? onNavigatedFrom)
    {
        public Type ViewModelType { get; } = viewModelType;
        public Type ViewType { get; } = viewType;

        public Control? View { get; set; } = view;
        public bool CacheView { get; } = cacheView;

        public MethodInfo? OnNavigatedTo { get; } = onNavigatedTo;
        public bool OnNavigatedToAcceptsParameters { get; } = onNavigatedToAcceptsParameters;

        public MethodInfo? OnNavigatedFrom { get; } = onNavigatedFrom;
    }


    readonly Dictionary<string, Page> routes = [];

    public string? CurrentRoute { get; private set; }
    public object? CurrentViewModel { get; private set; }
    public Control? CurrentView { get; private set; }

    MethodInfo? currentOnNavigatedFrom = null;


    (string Route, Dictionary<string, string> Parameters) ParseUri(
        string uri)
    {
        string route;
        Dictionary<string, string> parameters = [];

        // Remove protocol if present
        if (uri.StartsWith("carmine://", StringComparison.OrdinalIgnoreCase))
            uri = uri["carmine://".Length..];

        // Split route and query
        string[] parts = uri.Split('?', 2);
        route = parts[0].TrimEnd('/'); // remove trailing slash

        if (parts.Length > 1)
        {
            string[]? query = parts[1].Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (string kv in query)
            {
                string[] kvParts = kv.Split('=', 2);

                if (kvParts.Length >= 1 && !string.IsNullOrWhiteSpace(kvParts[0]))
                {
                    string key = kvParts[0].Trim();
                    string value = kvParts.Length > 1 ? kvParts[1].Trim() : string.Empty;

                    try
                    {
                        key = Uri.UnescapeDataString(key);
                    }
                    catch
                    { }

                    try
                    {
                        value = Uri.UnescapeDataString(value);
                    }
                    catch
                    { }

                    parameters[key] = value;
                }
            }
        }

        logger.LogInformation("Parsed URI route '{route}' with {count} parameters", route, parameters.Count);
        return (route, parameters);
    }


    Control GetView(
        string route)
    {
        if (!routes.TryGetValue(route, out Page? page))
            logger.LogErrorAndThrow(new InvalidOperationException($"No page found for route '{route}'."), "Failed to get view.");

        if (!page.CacheView)
            return CreateView(page.ViewType);

        if (page.View is not null)
            return page.View;

        page.View = CreateView(page.ViewType);
        return page.View;
    }

    Control CreateView(
        Type type)
    {
        logger.LogInformation("Creating view '{viewType}'...", type.Name);

        Control view = (Control)Activator.CreateInstance(type)!;
        return view;
    }


    public void Register(
        Assembly assembly)
    {
        logger.LogInformation("Scanning assembly '{assembly}' for navigable pages...", assembly.FullName);

        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsInterface || type.IsAbstract)
                continue;

            CustomAttributeData? attribute = type.CustomAttributes.FirstOrDefault(attribute => attribute.AttributeType.IsGenericType && attribute.AttributeType.GetGenericTypeDefinition() == typeof(NavigableAttribute<>));
            if (attribute is null)
                continue;

            string name = (string)attribute.ConstructorArguments[0].Value!;

            Type viewType = attribute.AttributeType.GenericTypeArguments[0];
            bool cacheView = attribute.ConstructorArguments.Count <= 1 || (bool)attribute.ConstructorArguments[1].Value!;
            MethodInfo? onNavigatedTo = null;
            bool onNavigatedToAcceptsParameters = false;
            MethodInfo? onNavigatedFrom = null;

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (onNavigatedTo is null && method.IsDefined(typeof(OnNavigatedToAttribute)))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 0)
                        onNavigatedToAcceptsParameters = false;
                    else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Dictionary<string, string>))
                        onNavigatedToAcceptsParameters = true;
                    else
                        logger.LogErrorAndThrow(new InvalidOperationException("Method signature doesn't match expected.", new("Parameters are not allowed.")), "Failed to register assembly.");

                    onNavigatedTo = method;
                }
                if (onNavigatedFrom is null && method.IsDefined(typeof(OnNavigatedFromAttribute)))
                {
                    if (method.GetParameters().Length != 0)
                        logger.LogErrorAndThrow(new InvalidOperationException("Method signature doesn't match expected.", new("Parameters are not allowed.")), "Failed to register assembly.");

                    onNavigatedFrom = method;
                }

                if (onNavigatedTo is not null && onNavigatedFrom is not null)
                    break;
            }

            routes[name] = new Page(type, viewType, null, cacheView, onNavigatedTo, onNavigatedToAcceptsParameters, onNavigatedFrom);
        }
    }


    public bool Navigate(
        string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            logger.LogWarning("Tried to navigate to empty uri.");
            return false;
        }

        logger.LogInformation("Navigation '{uri}' requested...", uri);

        (string? route, Dictionary<string, string> parameters) = ParseUri(uri);
        if (route == CurrentRoute)
        {
            logger.LogWarning("Tried to navigate to current route. Skipping...");
            return false;
        }

        if (!routes.TryGetValue(route, out Page? page))
        {
            toastManager.CreateToast("Something went wrong!")
                .WithContent($"It looks like the page '{route}' doesn't exist.")
                .DismissOnClick()
                .ShowWarning();
            return false;
        }

        // Get ViewModel and View
        object viewModel = LifetimeHandler.Provider.GetRequiredService(page.ViewModelType);

        Control view = GetView(route);
        view.DataContext = viewModel;

        // Navigate
        currentOnNavigatedFrom?.Invoke(CurrentViewModel!, null);

        OnPropertyChanging(nameof(CurrentRoute));
        CurrentRoute = route;
        OnPropertyChanged(nameof(CurrentRoute));

        OnPropertyChanging(nameof(CurrentViewModel));
        CurrentViewModel = viewModel;
        OnPropertyChanged(nameof(CurrentViewModel));

        OnPropertyChanging(nameof(CurrentView));
        CurrentView = view;
        OnPropertyChanged(nameof(CurrentView));

        page.OnNavigatedTo?.Invoke(viewModel, page.OnNavigatedToAcceptsParameters ? [parameters] : null);
        currentOnNavigatedFrom = page.OnNavigatedFrom;

        return true;
    }
}