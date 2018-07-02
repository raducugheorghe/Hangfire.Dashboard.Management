using Hangfire.Logging;
using System;

namespace Hangfire.Host.Helpers
{
    public class HangfireLogProvider : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new HangefireLogger(name);
        }

        public class HangefireLogger : ILog
        {
            private readonly log4net.ILog _log4Netlogger;

            public HangefireLogger(string name)
            {
                _log4Netlogger = log4net.LogManager.GetLogger((typeof(Startup)).Namespace, name);
            }

            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
            {
                try
                {
                    var message = (messageFunc == null) ? null : messageFunc();
                    if (message != null || exception != null)
                    {

                        switch (logLevel)
                        {
                            case LogLevel.Fatal:
                                {
                                    _log4Netlogger.Fatal(message, exception);
                                }
                                break;
                            case LogLevel.Error:
                                {
                                    _log4Netlogger.Error(message, exception);
                                }
                                break;
                            case LogLevel.Warn:
                                {
                                    _log4Netlogger.Warn(message, exception);
                                }
                                break;
                            case LogLevel.Info:
                                {
                                    _log4Netlogger.Info(message, exception);
                                }
                                break;
                            case LogLevel.Trace:
                                {
                                    _log4Netlogger.Debug(message, exception);
                                }
                                break;
                            case LogLevel.Debug:
                                {
                                    _log4Netlogger.Debug(message, exception);
                                }
                                break;
                        }
                    }
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }
    }

}
