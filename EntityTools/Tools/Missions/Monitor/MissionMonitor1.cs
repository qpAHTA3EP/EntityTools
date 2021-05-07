using MyNW.Classes;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EntityTools.Tools.Missions.Monitor
{
    public class MissionMonitor1
    {
        private readonly Mission _mission;
        public MissionMonitor1(Mission mission, int timeStamp = -1, bool expandProperties = false)
        {
            _mission = mission ?? throw new ArgumentNullException(nameof(mission));
            _missionDef = new MissionDefMonitor1(_mission.MissionDef, timeStamp, expandProperties);
            Update(timeStamp, expandProperties);
        }

        public int TimeStamp
        {
            get => _timeStamp;
            set
            {
                _timeStamp = value;
                _missionDef.TimeStamp = value;
                foreach (var child in _childrens)
                    child.TimeStamp = value;
                Update(_timeStamp);
            }
        }
        int _timeStamp;

        public MissionDefMonitor1 MissionDef => _missionDef;
        MissionDefMonitor1 _missionDef;

        public string UIStringMsg => _uiStringMsg;
        string _uiStringMsg;
        readonly LinkedList<Tuple<int, string>> _uiStringMsgHistory = new LinkedList<Tuple<int, string>>();

        public bool Hidden => _hidden;
        bool _hidden;
        readonly LinkedList<Tuple<int, bool>> _hiddenHistory = new LinkedList<Tuple<int, bool>>();

        public uint MissionNameOverride => _missionNameOverride;
        uint _missionNameOverride;
        readonly LinkedList<Tuple<int, uint>> _missionNameOverrideHistory = new LinkedList<Tuple<int, uint>>();

        public uint RootDefOverride => _rootDefOverride;
        uint _rootDefOverride;
        readonly LinkedList<Tuple<int, uint>> _rootDefOverrideHistory = new LinkedList<Tuple<int, uint>>();

        public int StartTime => _startTime;
        int _startTime;
        readonly LinkedList<Tuple<int, int>> _startTimeHistory = new LinkedList<Tuple<int, int>>();

        public int ExpirationTime => _expirationTime;
        int _expirationTime;
        LinkedList<Tuple<int, int>> _expirationTimeHistory = new LinkedList<Tuple<int, int>>();

        public ICollection<MissionMonitor1> Childrens => _childrens;
        readonly MissionMonitorCollection _childrens = new MissionMonitorCollection();

        public override string ToString()
        {
            if(string.IsNullOrEmpty(_label))
            {
                if (_expandProperties)
                    _label = _missionDef.DisplayName;
                else _label = _mission.MissionDef.DisplayName;
            }
            return _label;
        }
        string _label;

        public string MissionName => _missionName;
        string _missionName;
        readonly LinkedList<Tuple<int, string>> _missionNameHistory = new LinkedList<Tuple<int, string>>();

        public MissionState State => _state;
        MissionState _state;
        readonly LinkedList<Tuple<int, MissionState>> _stateHistory = new LinkedList<Tuple<int, MissionState>>();

        public bool ExpandProperties
        {
            get => _expandProperties;
            set
            {
                if (!value || _expandProperties) return;

                _expandProperties = true;
                _timeStamp = Environment.TickCount;
                Update(_timeStamp);
            }
        }
        bool _expandProperties;

        public void Update(int timeStamp, bool expandProperties = false)
        {
            _label = string.Empty;
            _timeStamp = timeStamp <= 0 ? timeStamp : Environment.TickCount;
            _expandProperties = expandProperties;
            if (_expandProperties && _mission.IsValid)
            {
                if (_missionDef is null)
                    _missionDef = new MissionDefMonitor1(_mission.MissionDef, _timeStamp, _expandProperties);
                else _missionDef.Update(_timeStamp, _expandProperties);

                _uiStringMsg = _mission.UIStringMsg;
                if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                    _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));

                _hidden = _mission.Hiden;
                if (_hiddenHistory.Last?.Value.Item2 != _hidden)
                    _hiddenHistory.AddLast(Tuple.Create(_timeStamp, _hidden));

                _missionNameOverride = _mission.MissionNameOverride;
                if (_missionNameOverrideHistory.Last?.Value.Item2 != _missionNameOverride)
                    _missionNameOverrideHistory.AddLast(Tuple.Create(_timeStamp, _missionNameOverride));

                _rootDefOverride = _mission.RootDefOverride;
                if (_rootDefOverrideHistory.Last?.Value.Item2 != _rootDefOverride)
                    _rootDefOverrideHistory.AddLast(Tuple.Create(_timeStamp, _rootDefOverride));

                _startTime = _mission.StartTime;
                if (_startTimeHistory.Last?.Value.Item2 != _startTime)
                    _startTimeHistory.AddLast(Tuple.Create(_timeStamp, _startTime));

                _expirationTime = _mission.ExpirationTime;
                if (_expirationTimeHistory.Last?.Value.Item2 != _expirationTime)
                    _expirationTimeHistory.AddLast(Tuple.Create(_timeStamp, _expirationTime));

                _missionName = _mission.MissionName;
                if (_missionNameHistory.Last?.Value.Item2 != _missionName)
                    _missionNameHistory.AddLast(Tuple.Create(_timeStamp, _missionName));

                _state = _mission.State;
                if (_stateHistory.Last?.Value.Item2 != _state)
                    _stateHistory.AddLast(Tuple.Create(_timeStamp, _state));

                UpdateChildMissions(_timeStamp, _expandProperties);
            }
        }

        void UpdateChildMissions(int timeStamp, bool expandProperties)
        {
            foreach (var child in _mission.Childrens)
                if (_childrens.Contains(child))
                    _childrens[child].Update(timeStamp, expandProperties);
                else _childrens.Add(new MissionMonitor1(child, timeStamp, expandProperties));
        }

        class MissionMonitorCollection : KeyedCollection<Mission, MissionMonitor1>
        {
            protected override Mission GetKeyForItem(MissionMonitor1 item)
            {
                return item._mission;
            }
        }
    }
}
