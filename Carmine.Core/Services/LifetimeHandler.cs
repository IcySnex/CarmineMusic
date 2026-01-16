using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Logging;

namespace Carmine.Core.Services;

public class LifetimeHandler
{
	readonly ILogger<LifetimeHandler> logger;
	readonly IClassicDesktopStyleApplicationLifetime lifetime;
	readonly Window mainWindow;

	public LifetimeHandler(
		ILogger<LifetimeHandler> logger,
		IClassicDesktopStyleApplicationLifetime lifetime,
		Window mainWindow)
	{
		this.logger = logger;
		this.lifetime = lifetime;
		this.mainWindow = mainWindow;

		lifetime.Startup += OnStartup;
		lifetime.ShutdownRequested += OnShutdownRequested;
	}

	
	void OnStartup(
		object? sender,
		ControlledApplicationLifetimeStartupEventArgs args)
	{
		logger.LogInformation("Starting application...");

		lifetime.MainWindow = mainWindow;
	}

	void OnShutdownRequested(
		object? sender,
		ShutdownRequestedEventArgs args)
	{
		logger.LogInformation("Stopping application...");
	}
}