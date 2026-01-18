using Carmine.Core.Models.Navigation;
using Carmine.UI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carmine.UI.ViewModels;

[Navigable<SettingsView>("settings")]
public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    string text = "Settings!!!";

    [RelayCommand]
    public void Test()
    {
        Text = "New Settings!!!";
    }


    [OnNavigatedTo]
    public void OnNavigatedToAsync(
        Dictionary<string, string> parameters)
    {
        string input = parameters.GetValueOrDefault("input") ?? "Hallo";
        Text = input;
    }

    [OnNavigatedFrom]
    public async Task OnNavigatedFromAsync()
    {
        await Task.Delay(500);
        Text = "See you later!";
    }
}