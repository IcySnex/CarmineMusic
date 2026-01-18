using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Carmine.Core.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Carmine.Core.Services;

public class LifetimeHandler
{
    public static IServiceProvider Provider { get; private set; } = default!;


    readonly ILogger<LifetimeHandler> logger;

    readonly IClassicDesktopStyleApplicationLifetime lifetime;
    readonly Window mainWindow;

    public LifetimeHandler(
        IServiceProvider provider,
        IClassicDesktopStyleApplicationLifetime lifetime,
        Window mainWindow)
    {
        Provider = provider;

        logger = Provider.GetRequiredService<ILogger<LifetimeHandler>>();

        this.lifetime = lifetime;
        this.mainWindow = mainWindow;

        lifetime.Startup += OnStartup;
        lifetime.ShutdownRequested += OnShutdownRequested;

        lifetime.MainWindow = mainWindow;
    }


    IConfigProvider configProvider = default!;
    Navigator navigator = default!;


    async void OnStartup(
        object? sender,
        ControlledApplicationLifetimeStartupEventArgs args)
    {
        logger.LogInformation("Application started...");

        // Config
        configProvider = Provider.GetRequiredService<IConfigProvider>();
        await configProvider.LoadAsync();

        // Navigation
        navigator = Provider.GetRequiredService<Navigator>();
        navigator.Register(mainWindow.GetType().Assembly);

        if (args.Args.Length < 1 || !navigator.Navigate(args.Args[0]))
            navigator.Navigate("home");
    }

    async void OnShutdownRequested(
        object? sender,
        ShutdownRequestedEventArgs args)
    {
        logger.LogInformation("Application shutdown requested...");

        await configProvider.SaveAsync();
    }
}