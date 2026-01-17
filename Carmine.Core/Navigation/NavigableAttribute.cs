namespace Carmine.Core.Navigation;

[AttributeUsage(AttributeTargets.Class)]
public class NavigableAttribute(
    string name,
    bool cacheView = true) : Attribute
{
    public string Name { get; } = name;

    public bool CacheView { get; } = cacheView;
}