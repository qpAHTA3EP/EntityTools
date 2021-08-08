using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Astral;

namespace EntityTools
{
    internal class ETLogger
    {
        private static readonly object Locker = new object();
        private static readonly StringBuilder LogCache = new StringBuilder();
        internal static string LogFilePath => logFilePath;
        private static string logFilePath = string.Empty;

        private static CancellationTokenSource cancellationTokenSource;
        private static Task loggerTask;

        internal static void Start()
        {
            if (loggerTask?.Status == TaskStatus.Running || !EntityTools.Config.Logger.Active) return;

            logFilePath = string.Concat(EntityTools.Config.Logger.LogsPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                //DateTime.Now.Year.ToString("00"), "-", DateTime.Now.Month.ToString("00"), "-", DateTime.Now.Day.ToString("00"), "_", 
                //DateTime.Now.Hour.ToString("00"), "-", DateTime.Now.Minute.ToString("00"), "-", DateTime.Now.Second.ToString("00"), 
                ".log");
            if (!Directory.Exists(EntityTools.Config.Logger.LogsPath))
                Directory.CreateDirectory(EntityTools.Config.Logger.LogsPath);

            cancellationTokenSource = new CancellationTokenSource();
            loggerTask = Task.Factory.StartNew(() => Run(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        public static void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        public static void WriteLine(string text, bool toAstral = false)
        {
            if(EntityTools.Config.Logger.Active)
                WriteLine(LogType.Log, text, toAstral);
        }

        public static void WriteLine(LogType logType, string text, bool toAstral = false)
        {
            if (EntityTools.Config.Logger.Active)
            {
                try
                {
                    lock (Locker)
                    {
                        LogCache.Append(DateTime.Now.ToString("[HH:mm:ss]"));
                        switch (logType)
                        {
                            case LogType.Log:
                                LogCache.Append(" [LOG] ");
                                if (toAstral)
                                    Logger.WriteLine(Logger.LogType.Log, text);
                                break;
                            case LogType.Debug:
                                LogCache.Append(" [DBG] ");
                                if (toAstral)
                                    Logger.WriteLine(Logger.LogType.Debug, text);
                                break;
                            case LogType.Error:
                                LogCache.Append(" [ERR] ");
                                if (toAstral)
                                    Logger.WriteLine(Logger.LogType.Debug, text);
                                break;
                        }

                        LogCache.AppendLine(text);
                    }
                }
                catch(Exception e)
                {
                    File.AppendAllText(logFilePath, DateTime.Now.ToString("[HH:mm:ss] [ERR]") + e);
                }
            }
        }

        private static void Run(CancellationToken token)
        {
            if (!string.IsNullOrEmpty(LogFilePath))
            {
                while (EntityTools.Config.Logger.Active
                       && !token.IsCancellationRequested)
                {
                    try
                    {
                        lock (Locker)
                        {
                            if (LogCache.Length > 0)
                            {
                                File.AppendAllText(logFilePath, LogCache.ToString());
                                LogCache.Clear();
                            } 
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(Logger.LogType.Debug, ex.ToString());
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
        /// Сообщение об ошибке
        /// </summary>
        Error
    }
}
