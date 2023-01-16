using System;
using Serilog.Context;
using Serilog.Events;

namespace CoreData.Common
{
    public static class SerilogExtensions
    {
        private const string SuppressLoggingProperty = "SuppressLogging";

        /// <summary>
        ///     Get disposable token to supress logging for context.
        /// </summary>
        public static IDisposable SuppressLogging()
        {
            return LogContext.PushProperty(SuppressLoggingProperty, true);
        }

        /// <summary>
        ///     Determines whether the given log event suppressed.
        /// </summary>
        /// <remarks>
        ///     Also removes "SuppressLogging" property if present.
        /// </remarks>
        public static bool IsSuppressed(this LogEvent logEvent)
        {
            var containsProperty = logEvent.Properties.TryGetValue(SuppressLoggingProperty, out var val);

            if (!containsProperty)
                return false;

            // remove suppression property from logs
            logEvent.RemovePropertyIfPresent(SuppressLoggingProperty);

            if (val is ScalarValue scalar && scalar.Value is bool isSuppressed)
                return isSuppressed;

            return false;
        }
    }
}