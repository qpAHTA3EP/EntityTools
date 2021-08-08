using System;
using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace EntityTools.Settings
{
    [Serializable]
    public class MissionDefTreeNodeSettings : PluginSettingsBase
    {
#if false
        [Bindable(true)]
        public bool Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _name = true; 
#endif

        [Bindable(true)]
        public bool DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _displayName = true;

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
        public bool Summary
        {
            get => _summary;
            set
            {
                if (_summary != value)
                {
                    _summary = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _summary = true;

        [Bindable(true)]
        public bool RelatedMission
        {
            get => _relatedMission;
            set
            {
                if (_relatedMission != value)
                {
                    _relatedMission = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _relatedMission = true;

        [Bindable(true)]
        public bool MissionType
        {
            get => _missionType;
            set
            {
                if (_missionType != value)
                {
                    _missionType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _missionType = true;

        [Bindable(true)]
        public bool CanRepeat
        {
            get => _canRepeat;
            set
            {
                if (_canRepeat != value)
                {
                    _canRepeat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _canRepeat = true;

        public override string ToString()
        {
            int num = 0;
            if (_displayName) num++;
            if (_uiStringMsg) num++;
            if (_summary) num++;
            if (_relatedMission) num++;
            if (_missionType) num++;
            if (_canRepeat) num++;

            return $"({num} of 6)";
        }
    }
}
