using System;
using System.ComponentModel;
using System.IO;
using Astral.Controllers;
using ACTP0Tools.Reflection;
using EntityTools.Quester.Actions;
using EntityTools.Quester.Conditions;
using EntityTools.Servises.SlideMonitor;
using EntityTools.UCC.Actions;

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
        /// Активация расширенной отладочной информации при поиске пути
        /// </summary>
        [Bindable(true)]
        [Description("Активация расширенной отладочной информации при поиске пути")]
        public bool DebugNavigation
        {
            get => _debugNavigation;
            set
            {
                if (_debugNavigation != value)
                {
                    _debugNavigation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _debugNavigation;


        /// <summary>
        /// Активация расширенной отладочной информации <see cref="SlideMonitor"/>
        /// </summary>
        [Bindable(true)]
        [Description("Активация расширенной отладочной информации " + nameof(SlideMonitor))]
        public bool DebugSlideMonitor
        {
            get => _debugSlideMonitor;
            set
            {
                if (_debugSlideMonitor != value)
                {
                    _debugSlideMonitor = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _debugSlideMonitor;

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
            [Description("Активация расширенной отладочной информации по команде " + nameof(MoveToEntity))]
            public bool DebugMoveToEntity
            {
                get => _debugMoveToEntity && EntityTools.Config.Logger.Active;
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
            /// Активация расширенной отладочной информации по <seealso cref="Quester.Actions.MoveToTeammate"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по команде " + nameof(MoveToTeammate))]
            public bool DebugMoveToTeammate
            {
                get => _debugMoveToTeammate && EntityTools.Config.Logger.Active;
                set
                {
                    if (_debugMoveToTeammate != value)
                    {
                        _debugMoveToTeammate = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugMoveToTeammate;

            /// <summary>
            /// Активация расширенной отладочной информации по <seealso cref="Quester.Actions.InteractEntities"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по команде " + nameof(InteractEntities))]
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
            [Description("Активация расширенной отладочной информации по команде " + nameof(BuySellItemsExt))]
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
            [Description("Активация расширенной отладочной информации по команде " + nameof(PickUpMissionExt))]
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
            [Description("Активация расширенной отладочной информации по команде " + nameof(TurnInMissionExt))]
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

            /// <summary>
            /// Активация расширенной отладочной информации по <seealso cref="Quester.Actions.ExecutePowerExt"/>
            /// </summary>
            [Bindable(true)]
            [Description("Активация расширенной отладочной информации по команде " + nameof(ExecutePowerExt))]
            public bool DebugExecutePowerExt
            {
                get => _debugExecutePowerExt;
                set
                {
                    if (_debugExecutePowerExt != value)
                    {
                        _debugExecutePowerExt = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugExecutePowerExt;

            public override string ToString()
            {
                int total = 0;
                int active = 0;
                foreach(var field in GetType().GetFields(ReflectionHelper.DefaultFlags))
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
            [Description("Активация расширенной отладочной информации по условию " + nameof(EntityCount))]
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
            [Description("Активация расширенной отладочной информации по команде " + nameof(ChangeTarget))]
            [Bindable(true)]
            public bool DebugChangeTarget
            {
                get => _debugChangeTarget;
                set
                {
                    if (_debugChangeTarget != value)
                    {
                        _debugChangeTarget = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugChangeTarget;

            /// <summary>
            /// Активация расширенной отладочной информации по команде <seealso cref="UCC.Actions.ExecuteSpecificPower"/> 
            /// </summary>
            [Description("Активация расширенной отладочной информации по команде " + nameof(ExecuteSpecificPower))]
            [Bindable(true)]
            public bool DebugExecuteSpecificPower
            {
                get => _debugExecuteSpecificPower;
                set
                {
                    if (_debugExecuteSpecificPower != value)
                    {
                        _debugExecuteSpecificPower = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugExecuteSpecificPower;

            /// <summary>
            /// Активация расширенной отладочной информации по команде <seealso cref="UCC.Actions.PluggedSkill"/> 
            /// </summary>
            [Description("Активация расширенной отладочной информации по команде " + nameof(PluggedSkill))]
            [Bindable(true)]
            public bool DebugPluggedSkill
            {
                get => _debugPluggedSkill;
                set
                {
                    if (_debugPluggedSkill != value)
                    {
                        _debugPluggedSkill = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _debugPluggedSkill;

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
