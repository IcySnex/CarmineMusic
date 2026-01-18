using CommunityToolkit.Mvvm.ComponentModel;

namespace Carmine.Core.Configuration;

public partial class Config : ObservableObject
{
    [ObservableProperty]
    string text = "hello";

    [ObservableProperty]
    bool isEnabled = true;
}