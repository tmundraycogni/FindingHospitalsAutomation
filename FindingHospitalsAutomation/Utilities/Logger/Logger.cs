using NLog;
using NLogger = NLog.Logger;

namespace FindingHospitalsAutomation.Utilities.Logger
{
    public static class Log
    {
        private static readonly NLogger logger = LogManager.GetCurrentClassLogger();

        public static void Info(string message) => logger.Info(message);
        public static void Warn(string message) => logger.Warn(message);
        public static void Error(string message) => logger.Error(message);
    }
}
