using System.Reflection;

namespace Carmine.Core.Utilities;

public class PathResolver
{
    public static string ExecutableDirectory { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

    public static string LogsDirectory { get; } = Path.Combine(ExecutableDirectory, "Logs");
}