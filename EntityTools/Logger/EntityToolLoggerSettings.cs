using System;
using System.ComponentModel;
using System.IO;
using Astral.Controllers;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки службы EntityToolsLogger
    /// </summary>
    [Serializable]
    public class ETLoggerSettings : PluginSettingsBase
    {
        /// <summary>
        /// Активация или деактивация логгирования
        /// </summary>
        [Bindable(true)]
        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    NotifyPropertyChanged(nameof(Active));
                }
            }
        }
        private bool _active = true;

        /// <summary>
        /// Путь сохранения файла логгирования
        /// </summary>
        [Bindable(true)]
        public string LogsPath
        {
            get => _logFilePath;
            set
            {
                if(_logFilePath != value)
                {
                    _logFilePath = value.Replace(Directories.AstralStartupPath, ".");
                    NotifyPropertyChanged(nameof(Active));
                }
            }
        }
        private string _logFilePath = string.Concat(Directories.LogsPath.Replace(Directories.AstralStartupPath, "."), Path.DirectorySeparatorChar, nameof(EntityTools), Path.DirectorySeparatorChar);

        /// <summary>
        /// Активания расширенной отладочной информации
        /// </summary>
        [Bindable(true)]
        public bool ExtendedActionDebugInfo
        {
            get => _extendedActionDebugInfo;
            set
            {
                if (_extendedActionDebugInfo != value)
                {
                    _extendedActionDebugInfo = value;
                    NotifyPropertyChanged(nameof(ExtendedActionDebugInfo));
                }
            }
        }
        private bool _extendedActionDebugInfo;

        public override string ToString()
        {
            return _active ? "Active" : "Disabled";
        }
    }
}
