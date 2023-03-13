using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки отображения свойств <seealso cref="MyNW.Classes.Mission"/> в окне монитора <seealso cref="Forms.MissionMonitorForm"/>
    /// </summary>
    [Serializable]
    public class MissionTreeNodeSettings : PluginSettingsBase
    {
        [Bindable(true)]
        public bool MissionName
        {
            get => _missionName;
            set
            {
                if (_missionName != value)
                {
                    _missionName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _missionName = true;
        
        [Bindable(true)]
        public bool UIStringMsg
        {
            get => _uiStringMsg;
            set
            {
                if (_uiStringMsg != value)
                {
                    _uiStringMsg = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _uiStringMsg = true;

        [Bindable(true)]
        public bool Hidden
        {
            get => _hidden;
            set
            {
                if (_hidden != value)
                {
                    _hidden = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _hidden = true;

        [Bindable(true)]
        public bool MissionNameOverride
        {
            get => _missionNameOverride;
            set
            {
                if (_missionNameOverride != value)
                {
                    _missionNameOverride = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _missionNameOverride = true;

        [Bindable(true)]
        public bool RootDefOverride
        {
            get => _rootDefOverride;
            set
            {
                if (_rootDefOverride != value)
                {
                    _rootDefOverride = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _rootDefOverride = true;

        [Bindable(true)]
        public bool StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _startTime = true;

        [Bindable(true)]
        public bool ExpirationTime
        {
            get => _expirationTime;
            set
            {
                if (_expirationTime != value)
                {
                    _expirationTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _expirationTime = true;

        [Bindable(true)]
        public bool State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _state = true;

        [Bindable(true)]
        public bool MissionDef
        {
            get => _missionDef;
            set
            {
                if (_missionDef != value)
                {
                    _missionDef = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _missionDef = true;

        public override string ToString()
        {
            int num = 0;
            if (_missionName) num++;
            if (_uiStringMsg) num++;
            if (_hidden) num++;
            if (_missionNameOverride) num++;
            if (_rootDefOverride) num++;
            if (_startTime) num++;
            if (_expirationTime) num++;
            if (_state) num++;
            if (_missionDef) num++;
            return $"({num} of 9)";
        }
    }
}
