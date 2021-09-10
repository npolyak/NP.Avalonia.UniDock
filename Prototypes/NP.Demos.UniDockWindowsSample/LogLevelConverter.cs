using NLog;
using NP.Utilities;

namespace NP.Demos.UniDockWindowsSample
{
    public static class LogLevelConverter
    {
        public static LogLevel ToLogLevel(this LogKind logKind)
        {
            return logKind switch
            {
                LogKind.Trace => LogLevel.Trace,
                LogKind.Debug => LogLevel.Debug,
                LogKind.Info => LogLevel.Info,
                LogKind.Warning => LogLevel.Warn,
                LogKind.Error => LogLevel.Error,
                LogKind.Fatal => LogLevel.Fatal,
                _ => LogLevel.Off
            };
        }
    }
}
