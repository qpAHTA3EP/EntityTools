using Astral;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.XtraBars.Alerter;
using System.Threading.Tasks;

namespace EntityTools
{
    internal class ETLogger
    {
        private static StringBuilder LogCache = new StringBuilder(1024);
        internal static string LogFilePath = string.Empty;

        private static CancellationTokenSource cancellationTokenSource;
        private static CancellationToken cancellationToken;
        private static Task backgroundLoggerTask;

        internal static void Start()
        {
            if (backgroundLoggerTask?.Status != TaskStatus.Running
                && EntityTools.PluginSettings.Logger.Active)
            {
                LogFilePath = string.Concat(EntityTools.PluginSettings.Logger.LogsPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                                            //DateTime.Now.Year.ToString("00"), "-", DateTime.Now.Month.ToString("00"), "-", DateTime.Now.Day.ToString("00"), "_", 
                                            //DateTime.Now.Hour.ToString("00"), "-", DateTime.Now.Minute.ToString("00"), "-", DateTime.Now.Second.ToString("00"), 
                                            ".log");
                if (!Directory.Exists(EntityTools.PluginSettings.Logger.LogsPath))
                    Directory.CreateDirectory(EntityTools.PluginSettings.Logger.LogsPath);

                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;
                backgroundLoggerTask = Task.Factory.StartNew(Run, cancellationToken);
            }
        }

        public static void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        public static void WriteLine(string Text, bool toAstral = false)
        {
            if(EntityTools.PluginSettings.Logger.Active)
                WriteLine(LogType.Log, Text, toAstral);
        }

        public static void WriteLine(LogType logType, string Text, bool toAstral = false)
        {
            if (EntityTools.PluginSettings.Logger.Active)
            {
                try
                {

                    //LogCache.Append('[').Append(DateTime.Now.Hour.ToString("00")).Append('-').Append(DateTime.Now.Minute.ToString("00")).Append('-').Append(DateTime.Now.Second.ToString("00"));
                    LogCache.Append(DateTime.Now.ToString("[HH-mm-ss"));
                    switch(logType)
                    {
                        case LogType.Log:
                            LogCache.Append(" LOG] ");
                            if (toAstral)
                                Logger.WriteLine(Logger.LogType.Log, Text);
                            break;
                        case LogType.Debug:
                            LogCache.Append(" DBG] ");
                            if (toAstral)
                                Logger.WriteLine(Logger.LogType.Debug, Text);
                            break;
                        case LogType.Error:
                            LogCache.Append(" ERR] ");
                            if (toAstral)
                                Logger.WriteLine(Logger.LogType.Debug, Text);
                            break;
                    }
                    LogCache.AppendLine(Text);

                }
                catch { }
            }
        }

        private static void Run()
        {
            if (!string.IsNullOrEmpty(LogFilePath))
            {
                while (EntityTools.PluginSettings.Logger.Active
                       && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (LogCache.Length > 0)
                        {
                            File.AppendAllText(LogFilePath, LogCache.ToString());
                            LogCache.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, ex.ToString());
                    }
                    Thread.Sleep(200);
                }
            }
        }
    }

    internal enum LogType
    {
        /// <summary>
        /// Обычное сообщение
        /// </summary>
        Log,
        /// <summary>
        /// Отладочное сообщение
        /// </summary>
        Debug,
        /// <summary>
        /// Сообщени об ошибке
        /// </summary>
        Error
    }
}
