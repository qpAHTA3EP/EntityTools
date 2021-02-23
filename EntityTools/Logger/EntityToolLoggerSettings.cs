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
        /// Активания расширенной отладочной информации по инструментам для работы с Mission
        /// </summary>
        [Bindable(true)]
        public bool DebugMissionTools
        {
            get => _debugMissionTools;
            set
            {
                if (_debugMissionTools != value)
                {
                    _debugMissionTools = value;
                    NotifyPropertyChanged(nameof(DebugMissionTools));
                }
            }
        }
        private bool _debugMissionTools;

        [Serializable]
        public class QuesterActionLoggerSettings : PluginSettingsBase
        {
            /// <summary>
                        /// Активания расширенной отладочной информации по MoveToEntity
                        /// </summary>
            [Bindable(true)]
            public bool DebugMoveToEntity
            {
                get => _debugMoveToEntityo;
                set
                {
                    if (_debugMoveToEntityo != value)
                    {
                        _debugMoveToEntityo = value;
                        NotifyPropertyChanged(nameof(DebugMoveToEntity));
                    }
                }
            }
            private bool _debugMoveToEntityo;

            /// <summary>
            /// Активания расширенной отладочной информации по PickUpMissionExt
            /// </summary>
            [Bindable(true)]
            public bool DebugPickUpMissionExt
            {
                get => _debugPickUpMissionExt;
                set
                {
                    if (_debugPickUpMissionExt != value)
                    {
                        _debugPickUpMissionExt = value;
                        NotifyPropertyChanged(nameof(DebugPickUpMissionExt));
                    }
                }
            }
            private bool _debugPickUpMissionExt;

            /// <summary>
            /// Активания расширенной отладочной информации по TurnInMissionExt
            /// </summary>
            [Bindable(true)]
            public bool DebugTurnInMissionExt
            {
                get => _debugTurnInMissionExt;
                set
                {
                    if (_debugTurnInMissionExt != value)
                    {
                        _debugTurnInMissionExt = value;
                        NotifyPropertyChanged(nameof(DebugTurnInMissionExt));
                    }
                }
            }
            private bool _debugTurnInMissionExt;
        }
        [Description("Настройки логирования команд квестера")]
        public QuesterActionLoggerSettings QuesterActions { get; set; } = new QuesterActionLoggerSettings();

        public class QuesterConditionLoggerSettings : PluginSettingsBase
        {
            /// <summary>
                /// Активания расширенной отладочной информации по условию EntityCount
                /// </summary>
            [Bindable(true)]
            public bool DebugConditionEntityCount
            {
                get => _debugConditionEntityCount;
                set
                {
                    if (_debugConditionEntityCount != value)
                    {
                        _debugConditionEntityCount = value;
                        NotifyPropertyChanged(nameof(DebugConditionEntityCount));
                    }
                }
            }
            private bool _debugConditionEntityCount;
        }
        [Description("Настройки логирования условий квестера")]
        public QuesterConditionLoggerSettings QuesterConditions { get; set; } = new QuesterConditionLoggerSettings();

        public override string ToString()
        {
            return _active ? "Active" : "Disabled";
        }
    }
}
