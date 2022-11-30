using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreSystem;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore
{
    internal static class Program
    {
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern int SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x112;
        public const int SC_RESTORE = 0xF120;
        [STAThread]
        static  void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
               // Application.Run(new  MainWindow());
               GameStart.StartGame();
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            // 后续处理，保存或输出
            Log.Error(str);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            // 后续处理，保存或输出
            Log.Error(str);

        }

        public static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
                sb.AppendLine("【异常方法】：" + ex.TargetSite);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }
        public static string GetExceptionMsg(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);

                sb.AppendLine("【异常方法】：" + ex.TargetSite);

            }
            else
            {
                sb.AppendLine("【未处理异常】：" + ex.Message);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }
    }
}

