using MyNW.Classes;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EntityTools.Tools.Missions.Monitor
{
    public class MissionMonitor2
    {
        private readonly Mission _mission;

        public MissionMonitor2(Mission mission, int timeStamp = -1, bool expandProperties = true)
        {
            if (mission is null)
                throw new ArgumentNullException(nameof(mission));

            _mission = mission;
            _missionName = _mission.MissionName;
            _missionDef = new MissionDefMonitor2(_mission.MissionDef, _timeStamp, _expandProperties);

            Update(timeStamp, expandProperties);
        }

        [Browsable(false)]
        public int TimeStamp
        {
            get => _timeStamp;
            set
            {
                _timeStamp = value;
                _missionDef.TimeStamp = value;
                foreach (var child in _childrens)
                {
                    child.TimeStamp = value;
                }

                Update(_timeStamp);
            }
        }
        int _timeStamp;

        public string MissionName => _missionName;
        readonly string _missionName;

        public MissionDefMonitor2 MissionDef => _missionDef;
        readonly MissionDefMonitor2 _missionDef;
        //LinkedList<Tuple<int, MissionDefMonitor2>> _missionDefHistory = new LinkedList<Tuple<int, MissionDefMonitor2>>();

        public string UIStringMsg => _uiStringMsg;
        string _uiStringMsg;
        LinkedList<Tuple<int, string>> _uiStringMsgHistory = new LinkedList<Tuple<int, string>>();

        public bool Hiden => _hiden;
        bool _hiden;
        readonly LinkedList<Tuple<int, bool>> _hidenHistory = new LinkedList<Tuple<int, bool>>();

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
        readonly LinkedList<Tuple<int, int>> _expirationTimeHistory = new LinkedList<Tuple<int, int>>();

        public MissionState State => _state;
        MissionState _state;
        readonly LinkedList<Tuple<int, MissionState>> _stateHistory = new LinkedList<Tuple<int, MissionState>>();

        public ICollection<MissionMonitor2> Childrens => _childrens;
        readonly MissionMonitorCollection2 _childrens = new MissionMonitorCollection2();

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

        [Browsable(false)]
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
#if false
                if (_missionDef is null)
                    _missionDef = new MissionDefMonitor(_mission.MissionDef, _timeStamp, _expandProperties);
                else 
#endif
                    _missionDef.Update(_timeStamp, _expandProperties);

                _uiStringMsg = _mission.UIStringMsg;
                if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                    _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));

                _hiden = _mission.Hiden;
                if (_hidenHistory.Last?.Value.Item2 != _hiden)
                    _hidenHistory.AddLast(Tuple.Create(_timeStamp, _hiden));

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

#if false
                _missionName = _mission.MissionName;
                if (_missionNameHistory.Last?.Value.Item2 != _missionName)
                    _missionNameHistory.AddLast(Tuple.Create(_timeStamp, _missionName)); 
#endif

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
                else _childrens.Add(new MissionMonitor2(child, timeStamp, expandProperties));
        }

        class MissionMonitorCollection2 : KeyedCollection<Mission, MissionMonitor2>
        {
            protected override Mission GetKeyForItem(MissionMonitor2 item)
            {
                return item._mission;
            }
        }
    }
}
