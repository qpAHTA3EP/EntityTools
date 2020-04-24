using EntityTools.Services;
using EntityTools.Settings;
using System;
using System.ComponentModel;
using System.IO;

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
                    base.NotifyPropertyChanged(nameof(Active));
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
                    _logFilePath = value.Replace(Astral.Controllers.Directories.AstralStartupPath, ".");
                    base.NotifyPropertyChanged(nameof(Active));
                }
            }
        }
        private string _logFilePath = string.Concat(Astral.Controllers.Directories.LogsPath.Replace(Astral.Controllers.Directories.AstralStartupPath, "."), Path.DirectorySeparatorChar, nameof(EntityTools), Path.DirectorySeparatorChar);

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
                    base.NotifyPropertyChanged(nameof(ExtendedActionDebugInfo));
                }
            }
        }
        private bool _extendedActionDebugInfo = false;
    }
}
