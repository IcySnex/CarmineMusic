using Carmine.Core.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Carmine.UI.ViewModels;

[Navigable("home")]
public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    string text = "Home!!!";

    [RelayCommand]
    public void Test()
    {
        Text = "New Home!!!";
    }
}