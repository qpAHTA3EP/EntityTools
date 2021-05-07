using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Tools.Missions.Monitor
{
    public class MissionDefMonitor1
    {
        MissionDef _missionDef;

        public MissionDefMonitor1(MissionDef missionDef, int timeStamp, bool expandProperties = false)
        {
            _missionDef = missionDef;
            Update(timeStamp, expandProperties);
        }

        [Browsable(false)]
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

        [Browsable(false)]
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

        internal void Update(int timeStamp, bool expandProperties = false)
        {
            _label = string.Empty;
            _timeStamp = timeStamp <= 0 ? timeStamp : Environment.TickCount;
            _expandProperties = expandProperties;

            if (_expandProperties)
            {
                _name = _missionDef.Name;
                if (_nameHistory.Last?.Value.Item2 != _name)
                    _nameHistory.AddLast(Tuple.Create(_timeStamp, _name));

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


            }
        }

        void UpdateSubmission(int timeStamp, bool expandProperties)
        {
            foreach (var subMiss in _missionDef.SubMissions)
                if (_subMissions.Contains(subMiss))
                    _subMissions[subMiss].TimeStamp = timeStamp;
                else _subMissions.Add(new MissionDefMonitor1(subMiss, timeStamp));
        }

        public override string ToString()
        {
            if(string.IsNullOrEmpty(_label))
            {
                if (_expandProperties)
                    _label = _displayName;
                else _label = _missionDef.DisplayName;
            }
            return _label;
        }
        string _label = string.Empty;

        public string Name => _name;
        string _name;
        LinkedList<Tuple<int, string>> _nameHistory = new LinkedList<Tuple<int, string>>();

        public string DisplayName => _displayName;
        string _displayName;
        LinkedList<Tuple<int, string>> _displayNameHistory = new LinkedList<Tuple<int, string>>();

        public string UIStringMsg => _uiStringMsg;
        string _uiStringMsg;
        LinkedList<Tuple<int, string>> _uiStringMsgHistory = new LinkedList<Tuple<int, string>>();

        public string Summary => _summary;
        string _summary;
        LinkedList<Tuple<int, string>> _summaryHistory = new LinkedList<Tuple<int, string>>();

        public string RelatedMission => _relatedMission;
        string _relatedMission;
        LinkedList<Tuple<int, string>> _relatedMissionHistory = new LinkedList<Tuple<int, string>>();

        public uint MissionType => _missionType;
        uint _missionType;
        LinkedList<Tuple<int, uint>> _missionTypeHistory = new LinkedList<Tuple<int, uint>>();

        public bool CanRepeat => _canRepeat;
        bool _canRepeat;
        LinkedList<Tuple<int, bool>> _canRepeatHistory = new LinkedList<Tuple<int, bool>>();

        public ICollection<MissionDefMonitor1> SubMissions => _subMissions;
        MissionDefMonitorCollection _subMissions = new MissionDefMonitorCollection();
        //LinkedList<Tuple<int, ICollection<MissionDefMonitor>>> _subMissionsHistory = new LinkedList<Tuple<int, ICollection<MissionDefMonitor>>>();
        //internal static ICollection<MissionDefMonitor> EmptyMissionDefMonitorCollection = new List<MissionDefMonitor>().AsReadOnly();

        class MissionDefMonitorCollection : KeyedCollection<MissionDef, MissionDefMonitor1>
        {
            protected override MissionDef GetKeyForItem(MissionDefMonitor1 item)
            {
                return item._missionDef;
            }
        }
    }
}
