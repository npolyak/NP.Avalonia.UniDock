using NLog;
using NP.Utilities;

namespace NP.Demos.UniDockWindowsSample
{
    public class NLog : ILog
    {
        public static NLog Instance { get; } = new NLog();

        private ILogger _logger;

        private NLog()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Log(LogKind logKind, string component, string msg)
        {
            LogEventInfo logEventInfo = new LogEventInfo(logKind.ToLogLevel(), "DefaultLogger", msg);

            logEventInfo.Properties.Add("Component", component);

            _logger.Log(logEventInfo);
        }
    }
}
