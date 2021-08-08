using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EntityTools.Tools.Missions.Monitor
{
    public class MissionDefMonitor2
    {
        MissionDef _missionDef;

        public MissionDefMonitor2(MissionDef missionDef, int timeStamp = -1, bool expandProperties = true)
        {
            _missionDef = missionDef;
            _name = _missionDef.Name;
            Update(timeStamp, expandProperties);
        }

        public int TimeStamp
        {
            get => _timeStamp;
            set
            {
                _timeStamp = value;
                if (_expandProperties)
                {
                    foreach (var subMiss in _subMissions)
                        subMiss.TimeStamp = value;
                }
                Update(_timeStamp);
            }
        }
        int _timeStamp;

        public bool ExpandProperties
        {
            get => _expandProperties;
            set
            {
                if (value && !_expandProperties)
                {
                    _expandProperties = value;
                    _timeStamp = Environment.TickCount;
                    Update(_timeStamp);
                }
            }
        }
        bool _expandProperties;

        internal void Update(int timeStamp, bool expandProperties = true)
        {
            _label = string.Empty;
            _timeStamp = timeStamp <= 0 ? timeStamp : Environment.TickCount;
            _expandProperties = expandProperties;

            if (_expandProperties)
            {
#if false
                _name = _missionDef.Name;
                if (_nameHistory.Last?.Value.Item2 != _name)
                    _nameHistory.AddLast(Tuple.Create(_timeStamp, _name)); 
#endif

                _displayName = _missionDef.DisplayName;
                if (_displayNameHistory.Last?.Value.Item2 != _displayName)
                    _displayNameHistory.AddLast(Tuple.Create(_timeStamp, _displayName));

                _uiStringMsg = _missionDef.UIStringMsg;
                if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                    _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));

                _summary = _missionDef.Summary;
                if (_summaryHistory.Last?.Value.Item2 != _summary)
                    _summaryHistory.AddLast(Tuple.Create(_timeStamp, _summary));

                _relatedMission = _missionDef.RelatedMission;
                if (_relatedMissionHistory.Last?.Value.Item2 != _relatedMission)
                    _relatedMissionHistory.AddLast(Tuple.Create(_timeStamp, _relatedMission));

                _missionType = _missionDef.MissionType;
                if (_missionTypeHistory.Last?.Value.Item2 != _missionType)
                    _missionTypeHistory.AddLast(Tuple.Create(_timeStamp, _missionType));

                _canRepeat = _missionDef.CanRepeat;
                if (_canRepeatHistory.Last?.Value.Item2 != _canRepeat)
                    _canRepeatHistory.AddLast(Tuple.Create(_timeStamp, _canRepeat));

                UpdateSubmission(_timeStamp, _expandProperties);
            }
        }

        void UpdateSubmission(int timeStamp, bool expandProperties)
        {
            foreach (var subMiss in _missionDef.SubMissions)
                if (_subMissions.Contains(subMiss))
                    _subMissions[subMiss].TimeStamp = timeStamp;
                else _subMissions.Add(new MissionDefMonitor2(subMiss, timeStamp, true));
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (_expandProperties)
                    _label = _displayName;
                else _label = _missionDef.DisplayName;
            }
            return _label;
        }
        string _label = string.Empty;

        public string Name => _name;
        readonly string _name;
        //LinkedList<Tuple<int, string>> _nameHistory = new LinkedList<Tuple<int, string>>();

        public string DisplayName => _displayName;
        string _displayName;
        readonly LinkedList<Tuple<int, string>> _displayNameHistory = new LinkedList<Tuple<int, string>>();

        public string UIStringMsg => _uiStringMsg;
        string _uiStringMsg;
        readonly LinkedList<Tuple<int, string>> _uiStringMsgHistory = new LinkedList<Tuple<int, string>>();

        public string Summary => _summary;
        string _summary;
        readonly LinkedList<Tuple<int, string>> _summaryHistory = new LinkedList<Tuple<int, string>>();

        public string RelatedMission => _relatedMission;
        string _relatedMission;
        readonly LinkedList<Tuple<int, string>> _relatedMissionHistory = new LinkedList<Tuple<int, string>>();

        public uint MissionType => _missionType;
        uint _missionType;
        readonly LinkedList<Tuple<int, uint>> _missionTypeHistory = new LinkedList<Tuple<int, uint>>();

        public bool CanRepeat => _canRepeat;
        bool _canRepeat;
        readonly LinkedList<Tuple<int, bool>> _canRepeatHistory = new LinkedList<Tuple<int, bool>>();

        public ICollection<MissionDefMonitor2> SubMissions => _subMissions;
        readonly MissionDefMonitorCollection2 _subMissions = new MissionDefMonitorCollection2();
        //LinkedList<Tuple<int, ICollection<MissionDefMonitor>>> _subMissionsHistory = new LinkedList<Tuple<int, ICollection<MissionDefMonitor>>>();
        //internal static ICollection<MissionDefMonitor> EmptyMissionDefMonitorCollection = new List<MissionDefMonitor>().AsReadOnly();

        class MissionDefMonitorCollection2 : KeyedCollection<MissionDef, MissionDefMonitor2>
        {
            protected override MissionDef GetKeyForItem(MissionDefMonitor2 item)
            {
                return item._missionDef;
            }
        }
    }
}
