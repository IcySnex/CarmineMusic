using Avalonia.Controls;

namespace Carmine.Core.Models.Navigation;

[AttributeUsage(AttributeTargets.Class)]
public class NavigableAttribute<TView>(
    string name,
    bool cacheView = true) : Attribute where TView : Control
{
    public string Name { get; } = name;

    public bool CacheView { get; } = cacheView;
}