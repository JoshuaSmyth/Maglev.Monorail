using System;

namespace Maglev.Monorail.Diagnostics.Logging
{
    public enum LogLevel
    {
        Debug,
        Warning,
        Error,
        Info
    }

    public interface IDebugLogger
    {
        void Log(LogLevel level, String category, String message);
        void LogException(Exception exception);
    }
}
