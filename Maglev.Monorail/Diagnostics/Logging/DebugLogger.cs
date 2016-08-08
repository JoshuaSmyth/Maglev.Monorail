using System;
using System.Diagnostics;

namespace Maglev.Monorail.Diagnostics.Logging
{
    public class DebugLogger : IDebugLogger
    {
        public void Log(LogLevel level, string category, string message)
        {
            Trace.WriteLine(String.Format("Time:{0} Level:{1} Category:{2} Message:{3}", DateTime.Now, level, category, message));
        }

        public void LogException(Exception exception)
        {
            Trace.WriteLine(String.Format("EXCEPTION THROWN:{0} {1}", exception.Message, exception.StackTrace));
        }
    }
}
