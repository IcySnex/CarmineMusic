using Avalonia.Controls;
using Carmine.Core.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Carmine.Core.Navigation;

public partial class Navigator(
    IServiceProvider provider,
    ILogger<Navigator> logger) : ObservableObject
{
    class Page(
        Type type,
        Control? view,
        bool cacheView,
        MethodInfo? onNavigatedTo,
        MethodInfo? onNavigatedFrom)
    {
        public Type Type { get; } = type;

        public Control? View { get; set; } = view;

        public bool CacheView { get; } = cacheView;

        public MethodInfo? OnNavigatedTo { get; } = onNavigatedTo;

        public MethodInfo? OnNavigatedFrom { get; } = onNavigatedFrom;
    }


    readonly IServiceProvider provider = provider;
    readonly ILogger<Navigator> logger = logger;

    readonly Dictionary<string, Page> routes = new(AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => !type.IsInterface && !type.IsAbstract && type.GetCustomAttribute<NavigableAttribute>() is not null)
        .Select(type =>
        {
            NavigableAttribute attribute = type.GetCustomAttribute<NavigableAttribute>()!;

            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo? onNavigatedTo = methods.FirstOrDefault(m => m.GetCustomAttribute<OnNavigatedToAttribute>() is not null);
            MethodInfo? onNavigatedFrom = methods.FirstOrDefault(m => m.GetCustomAttribute<OnNavigatedFromAttribute>() is not null);

            return new KeyValuePair<string, Page>(attribute.Name, new(type, null, attribute.CacheView, onNavigatedTo, onNavigatedFrom));
        }));


    Control GetView(
        string route,
        object viewModel)
    {
        if (!routes.TryGetValue(route, out Page? page))
            logger.LogErrorAndThrow(new InvalidOperationException($"No page found for route '{route}'."), "Failed to get view.");

        if (!page.CacheView)
            return CreateView(viewModel);
        
        if (page.View is not null)
            return page.View;

        page.View = CreateView(viewModel);
        return page.View;
    }

    Control CreateView(
        object viewModel)
    {
        string viewModelName = viewModel.GetType().Name;
        string viewName = viewModelName.Replace("ViewModel", "View");

        logger.LogInformation("Creating view '{viewType}' for view model '{viewModelType}'...", viewName, viewModelName);

        Type? viewType = Type.GetType($"Carmine.UI.Views.{viewName}, Carmine.UI");
        if (viewType is null)
            logger.LogErrorAndThrow(new InvalidOperationException($"View type {viewName} not found"), "Failed to create view.");

        var view = (Control)Activator.CreateInstance(viewType)!;
        view.DataContext = viewModel;
        return view;
    }


    public string? CurrentRoute { get; private set; }

    public object? CurrentViewModel { get; private set; }

    public Control? CurrentView { get; private set; }


    MethodInfo? currentOnNavigatedFrom = null;


    public void Navigate(
        string route)
    {
        if (route == CurrentRoute)
            return;

        logger.LogInformation("Navigating to route '{route}'...", route);

        if (!routes.TryGetValue(route, out Page? page))
            logger.LogErrorAndThrow(new InvalidOperationException($"No page found for route '{route}'."), "Failed to navigate.");

        object viewModel = provider.GetRequiredService(page.Type);
        Control view = GetView(route, viewModel);


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

        page.OnNavigatedTo?.Invoke(viewModel, null);
        currentOnNavigatedFrom = page.OnNavigatedFrom;
    }
}