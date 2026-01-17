#pragma warning disable CA2254 // Template should be a static expression

using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Carmine.Core.Utilities;

public static class LoggerExtensions
{
    extension(ILogger logger)
    {
        [DoesNotReturn]
        public void LogErrorAndThrow(
            Exception ex,
            string message,
            params object?[] args)
        {
            logger.LogError(ex, message, args);
            throw ex;
        }
    }
}