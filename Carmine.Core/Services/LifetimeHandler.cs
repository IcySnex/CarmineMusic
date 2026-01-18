using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Carmine.Core.Services;

public class LifetimeHandler
{
    public static IServiceProvider Provider { get; private set; } = default!;


    readonly IClassicDesktopStyleApplicationLifetime lifetime;
    readonly Window mainWindow;

    readonly ILogger<LifetimeHandler> logger;
    readonly Navigator navigator;

    public LifetimeHandler(
        IServiceProvider provider,
        IClassicDesktopStyleApplicationLifetime lifetime,
        Window mainWindow)
    {
        Provider = provider;

        this.lifetime = lifetime;
        this.mainWindow = mainWindow;

        logger = provider.GetRequiredService<ILogger<LifetimeHandler>>();
        navigator = provider.GetRequiredService<Navigator>();

        lifetime.Startup += OnStartup;
        lifetime.ShutdownRequested += OnShutdownRequested;
    }


    void OnStartup(
        object? sender,
        ControlledApplicationLifetimeStartupEventArgs args)
    {
        logger.LogInformation("Starting application...");

        // UI
        lifetime.MainWindow = mainWindow;

        // Navigation
        navigator.Register(mainWindow.GetType().Assembly);

        if (args.Args.Length < 1 || !navigator.Navigate(args.Args[0]))
            navigator.Navigate("home");
    }

    void OnShutdownRequested(
        object? sender,
        ShutdownRequestedEventArgs args)
    {
        logger.LogInformation("Stopping application...");
    }
}