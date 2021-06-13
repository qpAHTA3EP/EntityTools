using System;
using System.ComponentModel;
using System.IO;
using Astral.Controllers;
using AcTp0Tools.Reflection;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки службы EntityToolsLogger
    /// </summary>
    [Serializable]
    public class ETLoggerSettings : PluginSettingsBase
    {
        /// <summary>
        /// Активация или деактивация логирования
        /// </summary>
        [Bindable(true)]
        [Description("Активация или деактивация логирования")]
        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _active = true;

        /// <summary>
        /// Путь сохранения файла логирования
        /// </summary>
        [Bindable(true)]
        [Description("Путь сохранения файла логирования")]
        public string LogsPath
        {
            get => _logFilePath;
            set
            {
                if(_logFilePath != value)
                {
                    _logFilePath = value.Replace(Directories.AstralStartupPath, ".");
                    NotifyPropertyChanged();
                }
            }
        }
        private string _logFilePath = string.Concat(Directories.LogsPath.Replace(Directories.AstralStartupPath, "."), Path.DirectorySeparatorChar, nameof(EntityTools), Path.DirectorySeparatorChar);

        /// <summary>
        /// Активация расширенной отладочной информации по инструментам для работы с Mission
        /// </summary>
        [Bindable(true)]
        [Description("Активация расширенной отладочной информации по инструментам для работы с Mission")]
        public bool DebugMissionTools
        {
            get => _debugMissionTools;
            set
            {
                if (_debugMissionTools != value)
                {
                    _debugMissionTools = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _debugMissionTools;

        /// <summary>
        /// Активация расширенной отладочной информации по инструментам для работы с Entity
        /// </summary>
        [Bindable(true)]
        [Description("Активация расширенной отладочной информации по инструментам для работы с Entity")]
        public bool DebugEntityTools
        {
            get => _debugEntityTools;
            set
            {
                if (_debugEntityTools != value)
                {
                    _debugEntityTools = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _debugEntityTools;

        [Serializable]
        public class QuesterActionLoggerSettings : PluginSettingsBase
        {
            /// <summary>
            /// Активация расширенной отладочной информации по <seealso cref="Quester.Actions.MoveToEntity"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по команде MoveToEntity")]
            public bool DebugMoveToEntity
            {
                get => _debugMoveToEntity;
                set
                {
                    if (_debugMoveToEntity != value)
                    {
                        _debugMoveToEntity = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugMoveToEntity;

            /// <summary>
            /// Активация расширенной отладочной информации по <seealso cref="Quester.Actions.InteractEntities"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по команде InteractEntities")]
            public bool DebugInteractEntities
            {
                get => _debugInteractEntities;
                set
                {
                    if (_debugInteractEntities != value)
                    {
                        _debugInteractEntities = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugInteractEntities;

            /// <summary>
            /// Активация расширенной отладочной информации по <seealso cref="Quester.Actions.BuySellItemsExt"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по команде BuySellItemsExt")]
            public bool DebugBuySellItems
            {
                get => _debugBuySellItem;
                set
                {
                    if (_debugBuySellItem != value)
                    {
                        _debugBuySellItem = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugBuySellItem;

            /// <summary>
            /// Активация расширенной отладочной информации по <seealso cref="Quester.Actions.PickUpMissionExt"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по команде PickUpMissionExt")]
            public bool DebugPickUpMissionExt
            {
                get => _debugPickUpMissionExt;
                set
                {
                    if (_debugPickUpMissionExt != value)
                    {
                        _debugPickUpMissionExt = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugPickUpMissionExt;

            /// <summary>
            /// Активация расширенной отладочной информации по <seealso cref="Quester.Actions.TurnInMissionExt"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по команде TurnInMissionExt")]
            public bool DebugTurnInMissionExt
            {
                get => _debugTurnInMissionExt;
                set
                {
                    if (_debugTurnInMissionExt != value)
                    {
                        _debugTurnInMissionExt = value;
                        NotifyPropertyChanged();
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

        [Serializable]
        public class QuesterConditionLoggerSettings : PluginSettingsBase
        {
            /// <summary>
            /// Активация расширенной отладочной информации по условию <seealso cref="Quester.Conditions.EntityCount"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по условию EntityCount")]
            public bool DebugEntityCount
            {
                get => _debugEntityCount;
                set
                {
                    if (_debugEntityCount != value)
                    {
                        _debugEntityCount = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugEntityCount;

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

        [Serializable]
        public class UccActionLoggerSettings : PluginSettingsBase
        {
            /// <summary>
            /// Активация расширенной отладочной информации по команде <seealso cref="UCC.Actions.ChangeTarget"/> 
            /// </summary>
            [Description("Активация расширенной отладочной информации по команде ChangeTarget")]
            [Bindable(true)]
            public bool DebugChangeTarget
            {
                get => _debugChangeTarget;
                set
                {
                    if (_debugChangeTarget != value)
                    {
                        _debugChangeTarget = value;
                        NotifyPropertyChanged(/*nameof(DebugConditionChangeTarget)*/);
                    }
                }
            }
            private bool _debugChangeTarget;

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
        [Description("Настройки логирования команд UCC")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UccActionLoggerSettings UccActions { get; set; } = new UccActionLoggerSettings();

        public override string ToString()
        {
            return _active ? "Active" : "Disabled";
        }
    }
}
