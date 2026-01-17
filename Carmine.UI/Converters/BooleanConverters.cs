using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace Carmine.UI.Converters;

public static class BooleanConverters
{
    public static readonly IValueConverter NullString = new FuncValueConverter<bool, string?, string?>((value, param) =>
            value ? null : param);


    public static readonly IValueConverter SidebarPadding = new FuncValueConverter<bool, Thickness>(value =>
            value ? new Thickness(16, 8) : new Thickness(8));

    public static readonly IValueConverter SidebarToggleButtonHorizontalAlignment = new FuncValueConverter<bool, HorizontalAlignment>(value =>
            value ? HorizontalAlignment.Right : HorizontalAlignment.Center);
}