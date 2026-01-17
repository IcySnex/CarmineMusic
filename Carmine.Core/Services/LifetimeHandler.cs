using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Carmine.Core.Navigation;
using Microsoft.Extensions.Logging;

namespace Carmine.Core.Services;

public class LifetimeHandler
{
	readonly ILogger<LifetimeHandler> logger;
	readonly IClassicDesktopStyleApplicationLifetime lifetime;
	readonly Window mainWindow;
	readonly Navigator navigator;

	public LifetimeHandler(
		ILogger<LifetimeHandler> logger,
		IClassicDesktopStyleApplicationLifetime lifetime,
		Window mainWindow,
		Navigator navigator)
	{
		this.logger = logger;
		this.lifetime = lifetime;
		this.mainWindow = mainWindow;
		this.navigator = navigator;

		lifetime.Startup += OnStartup;
		lifetime.ShutdownRequested += OnShutdownRequested;
	}

	
	void OnStartup(
		object? sender,
		ControlledApplicationLifetimeStartupEventArgs args)
	{
		logger.LogInformation("Starting application...");

		lifetime.MainWindow = mainWindow;

		navigator.Navigate("home");

    }

	void OnShutdownRequested(
		object? sender,
		ShutdownRequestedEventArgs args)
	{
		logger.LogInformation("Stopping application...");
	}
}