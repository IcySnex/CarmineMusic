using Carmine.Core.Models.Navigation;
using Carmine.Core.Services;
using Carmine.UI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Carmine.UI.ViewModels;

[Navigable<HomeView>("home")]
public partial class HomeViewModel(
    Navigator navigator) : ObservableObject
{
    [ObservableProperty]
    string text = "Home!!!";

    [RelayCommand]
    public void Test()
    {
        navigator.Navigate<SettingsViewModel>();
    }
}