using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace TMM.Core.Utils
{
    public class Log4Net
    {
        private static readonly log4net.ILog _log = LogManager.GetLogger(typeof(Log4Net));

        public static void Info(String message)
        {
            _log.Info(message);
        }

        public static void Error(String message)
        {
            _log.Error(message);
        }

        public static void Error(Exception e)
        {
            _log.Error(e.Message+"\r\n"+e.StackTrace);
        } 

        public static void LogError(log4net.ILog log, string message, Exception e)
        {
            if (log == null || e == null)
                return;

            if (log.IsErrorEnabled)
            {
                log.Error(message);
                log.Error(e.Message);
                log.Error(e.Source);
                log.Error(e.StackTrace);
                if (e.InnerException != null)
                {
                    log.Error(e.InnerException.Message);
                    log.Error(e.InnerException.Source);
                    log.Error(e.InnerException.StackTrace);
                }
            }
        }
    }
}
