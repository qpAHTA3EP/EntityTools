using System;
using System.ComponentModel;
using System.IO;
using Astral.Controllers;
using EntityTools.Reflection;

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
                get => _debugMoveToEntity;
                set
                {
                    if (_debugMoveToEntity != value)
                    {
                        _debugMoveToEntity = value;
                        NotifyPropertyChanged(nameof(DebugMoveToEntity));
                    }
                }
            }
            private bool _debugMoveToEntity;

            /// <summary>
            /// Активания расширенной отладочной информации по BuySellItems
            /// </summary>
            [Bindable(true)]
            public bool DebugBuySellItems
            {
                get => _debugBuySellItem;
                set
                {
                    if (_debugBuySellItem != value)
                    {
                        _debugBuySellItem = value;
                        NotifyPropertyChanged(nameof(DebugBuySellItems));
                    }
                }
            }
            private bool _debugBuySellItem;

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

            public override string ToString()
            {
                int total = 0;
                int active = 0;
                foreach(var field in this.GetType().GetFields(ReflectionHelper.DefaultFlags))
                {
                    total++;
                    if (field.GetValue(this).Equals(true))
                        active++;
                }

                return $"Active {active} of {total}";
            }
        }
        [Description("Настройки логирования команд квестера")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
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

            public override string ToString()
            {
                int total = 0;
                int active = 0;
                foreach (var field in GetType().GetFields(ReflectionHelper.DefaultFlags))
                {
                    total++;
                    if (field.GetValue(this).Equals(true))
                        active++;
                }

                return $"Active {active} of {total}";
            }
        }
        [Description("Настройки логирования условий квестера")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public QuesterConditionLoggerSettings QuesterConditions { get; set; } = new QuesterConditionLoggerSettings();

        public override string ToString()
        {
            return _active ? "Active" : "Disabled";
        }
    }
}
