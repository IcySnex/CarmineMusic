using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Carmine.Core.Services;
using Carmine.Core.Services.Abstractions;
using Carmine.Core.Utilities;
using Carmine.UI.ViewModels;
using Carmine.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ShadUI;
using System.IO;

namespace Carmine.UI;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime lifetime)
            return;

        BindingPlugins.DataValidators.RemoveAt(0);

        IHost host = Host.CreateDefaultBuilder()
            .UseSerilog((context, configuration) =>
            {
                const string Template = "[{Timestamp:HH:mm:ss} {Level:u3} {Class}] {Message:l}{NewLine:l}{Exception:l}";

                configuration.Enrich.With<SourceContextEnricher>();

                configuration.WriteTo.Console(
                    outputTemplate: Template);
                configuration.WriteTo.Debug(
                    outputTemplate: Template);
                configuration.WriteTo.File(
                    path: Path.Combine(LocalFileSystem.LogsDirectory, "Log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 10,
                    outputTemplate: Template);
            })
            .ConfigureServices((context, services) =>
            {
                // Services
                services.AddSingleton<LifetimeHandler>(provider => new(
                    provider,
                    lifetime,
                    new MainWindow()
                    {
                        DataContext = provider.GetRequiredService<MainWindowViewModel>()
                    }));

                services.AddSingleton<IConfigProvider, JsonConfigProvider>();
                services.AddSingleton<IFileSystem, LocalFileSystem>();
                services.AddSingleton<Navigator>();

                services.AddSingleton<ToastManager>();

                // ViewModels
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<HomeViewModel>();
                services.AddSingleton<SettingsViewModel>();
            })
            .Build();

        host.Services.GetRequiredService<LifetimeHandler>();
    }
}