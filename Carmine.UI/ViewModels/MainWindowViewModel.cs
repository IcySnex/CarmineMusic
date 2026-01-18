using Carmine.Core.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI;

namespace Carmine.UI.ViewModels;

public partial class MainWindowViewModel(
    Navigator navigator,
    ToastManager toastManager) : ObservableObject
{
    public Navigator Navigator { get; } = navigator;
    public ToastManager ToastManager { get; } = toastManager;


    [RelayCommand]
    void Navigate(string value) =>
        Navigator.Navigate(value);
}