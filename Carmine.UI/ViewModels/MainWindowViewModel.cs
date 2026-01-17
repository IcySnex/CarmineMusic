using Carmine.Core.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Carmine.UI.ViewModels;

public partial class MainWindowViewModel(
    Navigator navigator) : ObservableObject
{
    public Navigator Navigator { get; } = navigator;


    [RelayCommand]
    void Navigate(string value) =>
        Navigator.Navigate(value);
}