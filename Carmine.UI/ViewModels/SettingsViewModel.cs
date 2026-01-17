using Carmine.Core.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Carmine.UI.ViewModels;

[Navigable("settings")]
public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    string text = "Settings!!!";

    [RelayCommand]
    public void Test()
    {
        Text = "New Settings!!!";
    }


    [OnNavigatedFrom]
    public void OnNavigatedFrom()
    {
        Text = "See you later!";
    }

    [OnNavigatedTo]
    public async Task OnNavigatedToAsync()
    {
        await Task.Delay(1000);
        Text = "Hallo!";
    }
}