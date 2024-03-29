﻿using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Astral;

namespace Infrastructure
{
    public class ETLogger
    {
        private static readonly object Locker = new object();
        private static readonly StringBuilder LogCache = new StringBuilder();

        private static CancellationTokenSource cancellationTokenSource;
        private static Task loggerTask;


        /// <summary>
        /// Активация или деактивация логирования
        /// </summary>
        public static bool Active 
        { 
            get => _active;
            set
            {
                if (value != _active)
                {
                    if (value)
                        Start();
                    else Stop();
                    _active = value;
                }
            }
        }

        private static bool _active;
        /// <summary>
        /// Путь сохранения файла логирования
        /// </summary>
        [Bindable(true)]
        [Description("Путь сохранения файла логирования")]
        public static string LogFilePath { get; set; }


        public static void Start()
        {
            if (loggerTask?.Status == TaskStatus.Running || !Active) return;

            LogFilePath = string.Concat(
                                    LogFilePath, 
                                    DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                                    ".log"
                                );
            if (!Directory.Exists(LogFilePath))
                Directory.CreateDirectory(LogFilePath);

            cancellationTokenSource = new CancellationTokenSource();
            loggerTask = Task.Factory.StartNew(() => Run(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        public static void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        public static void WriteLine(string text, bool toAstral = false)
        {
            WriteLine(LogType.Log, text, toAstral);
        }

        public static void WriteLine(LogType logType, string text, bool toAstral = false)
        {
            if (Active)
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
                                break;
                            case LogType.Debug:
                                LogCache.Append(" [DBG] ");
                                break;
                            case LogType.Error:
                                LogCache.Append(" [ERR] ");
                                break;
                        }

                        LogCache.AppendLine(text);
                    }
                }
                catch(Exception e)
                {
                    File.AppendAllText(LogFilePath, DateTime.Now.ToString("[HH:mm:ss] [ERR]") + e);
                }
            }
            if (toAstral)
            {
                Logger.WriteLine(Logger.LogType.Debug, text);
            }
        }

        private static void Run(CancellationToken token)
        {
            if (!string.IsNullOrEmpty(LogFilePath))
            {
                while (Active
                       && !token.IsCancellationRequested)
                {
                    try
                    {
                        lock (Locker)
                        {
                            if (LogCache.Length > 0)
                            {
                                File.AppendAllText(LogFilePath, LogCache.ToString());
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

    public enum LogType
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
