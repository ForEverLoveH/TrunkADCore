
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrunkADCore.ADCoreSystem
{
    public  class LoggerHelper
    {
        private static readonly log4net.ILog LogInfo = log4net.LogManager.GetLogger("LogInfo");

        private static readonly log4net.ILog LogError = log4net.LogManager.GetLogger("LogError");

        private static readonly log4net.ILog LogMonitor = log4net.LogManager.GetLogger("LogMonitor");

        /// 

        /// 记录Error日志
        /// 

        /// 
        /// 
        public static void Error(string errorMsg, Exception ex = null)
        {
            if (ex != null)
            {
                LogError.Error(errorMsg, ex);
            }
            else
            {
                LogError.Error(errorMsg);
            }
        }
        public static void Debug(Exception ex)
        {
            if (ex != null)
            {
                string message = Program.GetExceptionMsg(ex);
                Log.Debug(message);

            }

        }

        /// 

        /// 记录Info日志
        /// 

        /// 
        /// 
        public static void Info(string msg, Exception ex = null)
        {
            if (ex != null)
            {
                LogInfo.Info(msg, ex);
            }
            else
            {
                LogInfo.Info(msg);
            }
        }

        /// 

        /// 记录Monitor日志
        /// 

        /// 
        public static void Monitor(string msg)
        {
            LogMonitor.Info(msg);
        }

        public static void Fatal(string msg)
        {
            try
            {
                Log.Fatal(msg);
            }
            catch (Exception ex)
            {


            }
        }
    }
}

