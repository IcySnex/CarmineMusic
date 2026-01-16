using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Carmine.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Carmine.UI;

public class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
			return;

		BindingPlugins.DataValidators.RemoveAt(0);

		IHost host = Host.CreateDefaultBuilder()
			.UseSerilog((context, configuration) =>
			{
				configuration.WriteTo.Console();
				configuration.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10);
			})
			.ConfigureServices((context, services) =>
			{
				services.AddSingleton<LifetimeHandler>(provider => new(
					logger: provider.GetRequiredService<ILogger<LifetimeHandler>>(),
					lifetime: desktop,
					mainWindow: new MainWindow()));
			})
			.Build();
		
		host.Services.GetRequiredService<LifetimeHandler>();
	}
}