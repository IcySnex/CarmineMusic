using Carmine.Core.Configuration;
using Carmine.Core.Models.Navigation;
using Carmine.Core.Services.Abstractions;
using Carmine.UI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Carmine.UI.ViewModels;

[Navigable<HomeView>("home")]
public partial class HomeViewModel(
    IConfigProvider configProvider) : ObservableObject
{
    readonly Config config = configProvider.Get<Config>();


    [ObservableProperty]
    string text = "Home!!!";

    [RelayCommand]
    public void Test()
    {
        config.Text = "Hello, Carmine!";
    }


    [OnNavigatedTo]
    void OnNavigatedTo()
    {
        Text = config.Text;
    }
}