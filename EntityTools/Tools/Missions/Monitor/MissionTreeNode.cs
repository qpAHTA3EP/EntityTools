using MyNW.Classes;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace EntityTools.Tools.Missions.Monitor
{
    public class MissionTreeNode : TreeNode
    {
        private readonly Mission _mission;

        public delegate void NotifyMissionChanged(MissionTreeNode sender, long timeStamp, string missionName, string propertyName, string subPropertyName, object oldValue, object newValue);

        ~MissionTreeNode()
        {
            EntityTools.Config.MissionMonitor.Mission.PropertyChanged -= MissionSettingsChanged;
        }

        public bool IsValid => _mission != null && _mission.IsValid && _mission.MissionName == _missionName && _missionDefNode.IsValid;

        public event NotifyMissionChanged OnMissionChanged;

        public MissionTreeNode(Mission mission, long timeStamp = -1, NotifyMissionChanged notifier = null)
        {
            _timeStamp = timeStamp > 0 ? timeStamp : DateTime.Now.Ticks;

            _mission = mission ?? throw new ArgumentNullException(nameof(mission));

            _missionName = _mission.MissionName;

            _uiStringMsg = _mission.UIStringMsg;
            _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));
            UIStringNode = new TreeNode(string.Concat(nameof(Mission.UIStringMsg) + " : ",
                                                          string.IsNullOrEmpty(_uiStringMsg) ? "Empty" : _uiStringMsg));

            _hidden = _mission.Hiden;
            _hiddenHistory.AddLast(Tuple.Create(_timeStamp, _hidden));
            HiddenNode = new TreeNode($@"{nameof(Hidden)}: {_hidden}");

            _missionNameOverride = _mission.MissionNameOverride;
            _missionNameOverrideHistory.AddLast(Tuple.Create(_timeStamp, _missionNameOverride));
            MissionNameOverrideNode = new TreeNode($@"{nameof(MissionNameOverride)}: {_missionNameOverride}");

            _rootDefOverride = _mission.RootDefOverride;
            _rootDefOverrideHistory.AddLast(Tuple.Create(_timeStamp, _rootDefOverride));
            RootDefOverrideNode = new TreeNode($@"{nameof(Mission.RootDefOverride)}: {_rootDefOverride}");

            _startTime = _mission.StartTime;
            _startTimeHistory.AddLast(Tuple.Create(_timeStamp, _startTime));
            StartTimeNode = new TreeNode($@"{nameof(Mission.StartTime)}: {_startTime}");

            _expirationTime = _mission.ExpirationTime;
            _expirationTimeHistory.AddLast(Tuple.Create(_timeStamp, _expirationTime));
            ExpirationTimeNode = new TreeNode($@"{nameof(Mission.ExpirationTime)}: {_expirationTime}");

            _state = _mission.State;
            _stateHistory.AddLast(Tuple.Create(_timeStamp, _state));
            StateNode = new TreeNode($@"{nameof(Mission.State)}: {_state}");

#if false
            var eventHandler = OnMissionChanged;
            if (eventHandler != null)
            {
                eventHandler(this, _missionName, nameof(UIStringMsg), string.Empty, null, _uiStringMsg);
                eventHandler(this, _missionName, nameof(Hidden), string.Empty, null, _hidden);
                eventHandler(this, _missionName, nameof(MissionNameOverride), string.Empty, null, _missionNameOverride);
                eventHandler(this, _missionName, nameof(RootDefOverride), string.Empty, null, _rootDefOverride);
                eventHandler(this, _missionName, nameof(StartTime), string.Empty, null, _startTime);
                eventHandler(this, _missionName, nameof(ExpirationTime), string.Empty, null, _expirationTime);
                eventHandler(this, _missionName, nameof(State), string.Empty, null, _state);
            } 
#endif

            _missionDefNode = new MissionDefTreeNode(_mission.MissionDef, _timeStamp, MissionDefChanged);

            Text = Lable();
            Name = _missionName;

            if (EntityTools.Config.MissionMonitor.Mission.UIStringMsg)
                Nodes.Add(UIStringNode);
            if(EntityTools.Config.MissionMonitor.Mission.Hidden)
                Nodes.Add(HiddenNode);
            if (EntityTools.Config.MissionMonitor.Mission.MissionNameOverride)
                Nodes.Add(MissionNameOverrideNode);
            if (EntityTools.Config.MissionMonitor.Mission.RootDefOverride)
                Nodes.Add(RootDefOverrideNode);
            if (EntityTools.Config.MissionMonitor.Mission.StartTime)
                Nodes.Add(StartTimeNode);
            if (EntityTools.Config.MissionMonitor.Mission.ExpirationTime)
                Nodes.Add(ExpirationTimeNode);
            if (EntityTools.Config.MissionMonitor.Mission.State)
                Nodes.Add(StateNode);
            if (EntityTools.Config.MissionMonitor.Mission.MissionDef)
                Nodes.Add(MissionDefNode);
#if false
            if (ChildrensNode.Nodes.Count > 0)
                Nodes.Add(ChildrensNode);  
#endif

            ChildrensNode = new TreeNode(nameof(Mission.Childrens) + " [0]");

            UpdateChildrens(_timeStamp);

            OnMissionChanged = notifier;
            EntityTools.Config.MissionMonitor.Mission.PropertyChanged += MissionSettingsChanged;
        }

        private void MissionSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            _settingsChanged = true;
        }

        private bool _settingsChanged;

        long _timeStamp;

        public string MissionName => _missionName;
        readonly string _missionName;

        public TreeNode UIStringNode { get; }
        public string UIStringMsg => _uiStringMsg;
        string _uiStringMsg;
        LinkedList<Tuple<long, string>> _uiStringMsgHistory = new LinkedList<Tuple<long, string>>();

        public TreeNode HiddenNode { get; }
        public bool Hidden => _hidden;
        bool _hidden;
        readonly LinkedList<Tuple<long, bool>> _hiddenHistory = new LinkedList<Tuple<long, bool>>();

        public TreeNode MissionNameOverrideNode { get; }
        public uint MissionNameOverride => _missionNameOverride;
        uint _missionNameOverride;
        readonly LinkedList<Tuple<long, uint>> _missionNameOverrideHistory = new LinkedList<Tuple<long, uint>>();

        public TreeNode RootDefOverrideNode { get; }
        public uint RootDefOverride => _rootDefOverride;
        uint _rootDefOverride;
        readonly LinkedList<Tuple<long, uint>> _rootDefOverrideHistory = new LinkedList<Tuple<long, uint>>();

        public TreeNode StartTimeNode { get; }
        public int StartTime => _startTime;
        int _startTime;
        readonly LinkedList<Tuple<long, int>> _startTimeHistory = new LinkedList<Tuple<long, int>>();

        public TreeNode StateNode { get; }
        public MissionState State => _state;
        MissionState _state;
        readonly LinkedList<Tuple<long, MissionState>> _stateHistory = new LinkedList<Tuple<long, MissionState>>();

        public TreeNode ExpirationTimeNode { get; }
        public int ExpirationTime => _expirationTime;
        int _expirationTime;
        readonly LinkedList<Tuple<long, int>> _expirationTimeHistory = new LinkedList<Tuple<long, int>>();

        public MissionDefTreeNode MissionDefNode => _missionDefNode;
        readonly MissionDefTreeNode _missionDefNode;

        public TreeNode ChildrensNode { get; }
        public ICollection<MissionTreeNode> Childrens => _childrens;
        private readonly MissionMonitorTreeNodeCollection _childrens = new MissionMonitorTreeNodeCollection();

        public void Update(long timeStamp)
        {
            if(timeStamp < _timeStamp) return;

            _timeStamp = timeStamp > 0 ? timeStamp : DateTime.Now.Ticks;
            _label = string.Empty;

            bool isValid = IsValid;
            var eventHandler = OnMissionChanged;

            if (isValid)
            {
                if (eventHandler is null)
                {

                    _uiStringMsg = _mission.UIStringMsg;
                    if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                    {
                        _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));
                        UIStringNode.Text = string.Concat(nameof(Mission.UIStringMsg) + " : ",
                            string.IsNullOrEmpty(_uiStringMsg) ? "Empty" : _uiStringMsg);
                    }

                    _hidden = _mission.Hiden;
                    if (_hiddenHistory.Last?.Value.Item2 != _hidden)
                    {
                        _hiddenHistory.AddLast(Tuple.Create(_timeStamp, _hidden));
                        HiddenNode.Text = $@"{nameof(Mission.Hiden)}: {_hidden}";
                    }

                    _missionNameOverride = _mission.MissionNameOverride;
                    if (_missionNameOverrideHistory.Last?.Value.Item2 != _missionNameOverride)
                    {
                        _missionNameOverrideHistory.AddLast(Tuple.Create(_timeStamp, _missionNameOverride));
                        MissionNameOverrideNode.Text = $@"{nameof(MissionNameOverride)}: {_missionNameOverride}";
                    }

                    _rootDefOverride = _mission.RootDefOverride;
                    if (_rootDefOverrideHistory.Last?.Value.Item2 != _rootDefOverride)
                    {
                        _rootDefOverrideHistory.AddLast(Tuple.Create(_timeStamp, _rootDefOverride));
                        RootDefOverrideNode.Text = $@"{nameof(Mission.RootDefOverride)}: {_rootDefOverride}";
                    }

                    _startTime = _mission.StartTime;
                    if (_startTimeHistory.Last?.Value.Item2 != _startTime)
                    {
                        _startTimeHistory.AddLast(Tuple.Create(_timeStamp, _startTime));
                        StartTimeNode.Text = $@"{nameof(Mission.StartTime)}: {_startTime}";
                    }

                    _expirationTime = _mission.ExpirationTime;
                    if (_expirationTimeHistory.Last?.Value.Item2 != _expirationTime)
                    {
                        _expirationTimeHistory.AddLast(Tuple.Create(_timeStamp, _expirationTime));
                        ExpirationTimeNode.Text = $@"{nameof(Mission.ExpirationTime)}: {_expirationTime}";
                    }

                    _state = _mission.State;
                    if (_stateHistory.Last?.Value.Item2 != _state)
                    {
                        _stateHistory.AddLast(Tuple.Create(_timeStamp, _state));
                        StateNode.Text = $@"{nameof(Mission.State)}: {_state}";
                        Text = ToString();
                    }
                }
                else
                {
                    object oldValue = _uiStringMsg;
                    _uiStringMsg = _mission.UIStringMsg;
                    if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                    {
                        _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));
                        UIStringNode.Text = string.Concat(nameof(Mission.UIStringMsg) + " : ",
                            string.IsNullOrEmpty(_uiStringMsg) ? "Empty" : _uiStringMsg);
                        eventHandler(this, _timeStamp, _missionName, nameof(UIStringMsg), string.Empty, oldValue, _uiStringMsg);
                    }

                    oldValue = _hidden;
                    _hidden = _mission.Hiden;
                    if (_hiddenHistory.Last?.Value.Item2 != _hidden)
                    {
                        _hiddenHistory.AddLast(Tuple.Create(_timeStamp, _hidden));
                        HiddenNode.Text = $@"{nameof(Mission.Hiden)}: {_hidden}";
                        eventHandler(this, _timeStamp, _missionName, nameof(Hidden), string.Empty, oldValue, _hidden);
                    }

                    oldValue = _missionNameOverride;
                    _missionNameOverride = _mission.MissionNameOverride;
                    if (_missionNameOverrideHistory.Last?.Value.Item2 != _missionNameOverride)
                    {
                        _missionNameOverrideHistory.AddLast(Tuple.Create(_timeStamp, _missionNameOverride));
                        MissionNameOverrideNode.Text = $@"{nameof(MissionNameOverride)}: {_missionNameOverride}";
                        eventHandler(this, _timeStamp, _missionName, nameof(MissionNameOverride), string.Empty, oldValue, _missionName);
                    }

                    oldValue = _rootDefOverride;
                    _rootDefOverride = _mission.RootDefOverride;
                    if (_rootDefOverrideHistory.Last?.Value.Item2 != _rootDefOverride)
                    {
                        _rootDefOverrideHistory.AddLast(Tuple.Create(_timeStamp, _rootDefOverride));
                        RootDefOverrideNode.Text = $@"{nameof(Mission.RootDefOverride)}: {_rootDefOverride}";
                        eventHandler(this, _timeStamp, _missionName, nameof(RootDefOverride), string.Empty, oldValue, _rootDefOverride);
                    }

                    oldValue = _startTime;
                    _startTime = _mission.StartTime;
                    if (_startTimeHistory.Last?.Value.Item2 != _startTime)
                    {
                        _startTimeHistory.AddLast(Tuple.Create(_timeStamp, _startTime));
                        StartTimeNode.Text = $@"{nameof(Mission.StartTime)}: {_startTime}";
                        eventHandler(this, _timeStamp, _missionName, nameof(StartTime), string.Empty, oldValue, _startTime);
                    }

                    oldValue = _expirationTime;
                    _expirationTime = _mission.ExpirationTime;
                    if (_expirationTimeHistory.Last?.Value.Item2 != _expirationTime)
                    {
                        _expirationTimeHistory.AddLast(Tuple.Create(_timeStamp, _expirationTime));
                        ExpirationTimeNode.Text = $@"{nameof(Mission.ExpirationTime)}: {_expirationTime}";
                        eventHandler(this, _timeStamp, _missionName, nameof(ExpirationTime), string.Empty, oldValue, _expirationTime);
                    }

                    oldValue = _state;
                    _state = _mission.State;
                    if (_stateHistory.Last?.Value.Item2 != _state)
                    {
                        _stateHistory.AddLast(Tuple.Create(_timeStamp, _state));
                        StateNode.Text = $@"{nameof(Mission.State)}: {_state}";
                        Text = ToString();
                        eventHandler(this, _timeStamp, _missionName, nameof(State), string.Empty, oldValue, _state);
                    }
                }

                _missionDefNode.Update(_timeStamp);

                UpdateChildrens(timeStamp);


                if (_settingsChanged)
                {
                    Nodes.Clear();
                    if (EntityTools.Config.MissionMonitor.Mission.UIStringMsg)
                        Nodes.Add(UIStringNode);
                    if (EntityTools.Config.MissionMonitor.Mission.Hidden)
                        Nodes.Add(HiddenNode);
                    if (EntityTools.Config.MissionMonitor.Mission.MissionNameOverride)
                        Nodes.Add(MissionNameOverrideNode);
                    if (EntityTools.Config.MissionMonitor.Mission.RootDefOverride)
                        Nodes.Add(RootDefOverrideNode);
                    if (EntityTools.Config.MissionMonitor.Mission.StartTime)
                        Nodes.Add(StartTimeNode);
                    if (EntityTools.Config.MissionMonitor.Mission.ExpirationTime)
                        Nodes.Add(ExpirationTimeNode);
                    if (EntityTools.Config.MissionMonitor.Mission.State)
                        Nodes.Add(StateNode);
                    if (EntityTools.Config.MissionMonitor.Mission.MissionDef)
                        Nodes.Add(MissionDefNode);
                    Nodes.Add(ChildrensNode);
                    _settingsChanged = false;
                }
            }
            else
            {
                _label = string.IsNullOrEmpty(_uiStringMsg) ? $@"[{_missionName}] : {_state}"
                    : $@"{_uiStringMsg} [{_missionName}] : Invalid";
                eventHandler?.Invoke(this, _timeStamp, _missionName, nameof(IsValid), string.Empty, true, false);
            }
        }
        public void DelayedUpdate(long timeStamp, ref Action updater)
        {
            if (timeStamp < _timeStamp) return;

            _timeStamp = timeStamp > 0 ? timeStamp : DateTime.Now.Ticks;
            _label = string.Empty;

            bool isValid = IsValid;
            var eventHandler = OnMissionChanged;

            if (isValid)
            {
                if (eventHandler is null)
                {
                    _uiStringMsg = _mission.UIStringMsg;
                    if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                    {
                        _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));
                        UIStringNode.Text = string.Concat(nameof(Mission.UIStringMsg) + " : ",
                            string.IsNullOrEmpty(_uiStringMsg) ? "Empty" : _uiStringMsg);
                    }

                    _hidden = _mission.Hiden;
                    if (_hiddenHistory.Last?.Value.Item2 != _hidden)
                    {
                        _hiddenHistory.AddLast(Tuple.Create(_timeStamp, _hidden));
                        HiddenNode.Text = $@"{nameof(Mission.Hiden)}: {_hidden}";
                    }

                    _missionNameOverride = _mission.MissionNameOverride;
                    if (_missionNameOverrideHistory.Last?.Value.Item2 != _missionNameOverride)
                    {
                        _missionNameOverrideHistory.AddLast(Tuple.Create(_timeStamp, _missionNameOverride));
                        MissionNameOverrideNode.Text = $@"{nameof(MissionNameOverride)}: {_missionNameOverride}";
                    }

                    _rootDefOverride = _mission.RootDefOverride;
                    if (_rootDefOverrideHistory.Last?.Value.Item2 != _rootDefOverride)
                    {
                        _rootDefOverrideHistory.AddLast(Tuple.Create(_timeStamp, _rootDefOverride));
                        RootDefOverrideNode.Text = $@"{nameof(Mission.RootDefOverride)}: {_rootDefOverride}";
                    }

                    _startTime = _mission.StartTime;
                    if (_startTimeHistory.Last?.Value.Item2 != _startTime)
                    {
                        _startTimeHistory.AddLast(Tuple.Create(_timeStamp, _startTime));
                        StartTimeNode.Text = $@"{nameof(Mission.StartTime)}: {_startTime}";
                    }

                    _expirationTime = _mission.ExpirationTime;
                    if (_expirationTimeHistory.Last?.Value.Item2 != _expirationTime)
                    {
                        _expirationTimeHistory.AddLast(Tuple.Create(_timeStamp, _expirationTime));
                        ExpirationTimeNode.Text = $@"{nameof(Mission.ExpirationTime)}: {_expirationTime}";
                    }

                    _state = _mission.State;
                    if (_stateHistory.Last?.Value.Item2 != _state)
                    {
                        _stateHistory.AddLast(Tuple.Create(_timeStamp, _state));
                        StateNode.Text = $@"{nameof(Mission.State)}: {_state}";
                        Text = ToString();
                    }
                }
                else
                {
                    object oldValue = _uiStringMsg;
                    _uiStringMsg = _mission.UIStringMsg;
                    if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                    {
                        _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));
                        UIStringNode.Text = string.Concat(nameof(Mission.UIStringMsg) + " : ",
                            string.IsNullOrEmpty(_uiStringMsg) ? "Empty" : _uiStringMsg);
                        eventHandler(this, _timeStamp, _missionName, nameof(UIStringMsg), string.Empty, oldValue, _uiStringMsg);
                    }

                    oldValue = _hidden;
                    _hidden = _mission.Hiden;
                    if (_hiddenHistory.Last?.Value.Item2 != _hidden)
                    {
                        _hiddenHistory.AddLast(Tuple.Create(_timeStamp, _hidden));
                        HiddenNode.Text = $@"{nameof(Mission.Hiden)}: {_hidden}";
                        eventHandler(this, _timeStamp, _missionName, nameof(Hidden), string.Empty, oldValue, _hidden);
                    }

                    oldValue = _missionNameOverride;
                    _missionNameOverride = _mission.MissionNameOverride;
                    if (_missionNameOverrideHistory.Last?.Value.Item2 != _missionNameOverride)
                    {
                        _missionNameOverrideHistory.AddLast(Tuple.Create(_timeStamp, _missionNameOverride));
                        MissionNameOverrideNode.Text = $@"{nameof(MissionNameOverride)}: {_missionNameOverride}";
                        eventHandler(this, _timeStamp, _missionName, nameof(MissionNameOverride), string.Empty, oldValue, _missionName);
                    }

                    oldValue = _rootDefOverride;
                    _rootDefOverride = _mission.RootDefOverride;
                    if (_rootDefOverrideHistory.Last?.Value.Item2 != _rootDefOverride)
                    {
                        _rootDefOverrideHistory.AddLast(Tuple.Create(_timeStamp, _rootDefOverride));
                        RootDefOverrideNode.Text = $@"{nameof(Mission.RootDefOverride)}: {_rootDefOverride}";
                        eventHandler(this, _timeStamp, _missionName, nameof(RootDefOverride), string.Empty, oldValue, _rootDefOverride);
                    }

                    oldValue = _startTime;
                    _startTime = _mission.StartTime;
                    if (_startTimeHistory.Last?.Value.Item2 != _startTime)
                    {
                        _startTimeHistory.AddLast(Tuple.Create(_timeStamp, _startTime));
                        StartTimeNode.Text = $@"{nameof(Mission.StartTime)}: {_startTime}";
                        eventHandler(this, _timeStamp, _missionName, nameof(StartTime), string.Empty, oldValue, _startTime);
                    }

                    oldValue = _expirationTime;
                    _expirationTime = _mission.ExpirationTime;
                    if (_expirationTimeHistory.Last?.Value.Item2 != _expirationTime)
                    {
                        _expirationTimeHistory.AddLast(Tuple.Create(_timeStamp, _expirationTime));
                        ExpirationTimeNode.Text = $@"{nameof(Mission.ExpirationTime)}: {_expirationTime}";
                        eventHandler(this, _timeStamp, _missionName, nameof(ExpirationTime), string.Empty, oldValue, _expirationTime);
                    }

                    oldValue = _state;
                    _state = _mission.State;
                    if (_stateHistory.Last?.Value.Item2 != _state)
                    {
                        _stateHistory.AddLast(Tuple.Create(_timeStamp, _state));
                        StateNode.Text = $@"{nameof(Mission.State)}: {_state}";
                        Text = ToString();
                        eventHandler(this, _timeStamp, _missionName, nameof(State), string.Empty, oldValue, _state);
                    }
                }

                _missionDefNode.DelayedUpdate(_timeStamp, ref updater);

                DelayedUpdateChildrens(timeStamp, ref updater);

                if (_settingsChanged)
                {
                    updater += () =>
                    {
                        Nodes.Clear();
                        if (EntityTools.Config.MissionMonitor.Mission.UIStringMsg)
                            Nodes.Add(UIStringNode);
                        if (EntityTools.Config.MissionMonitor.Mission.Hidden)
                            Nodes.Add(HiddenNode);
                        if (EntityTools.Config.MissionMonitor.Mission.MissionNameOverride)
                            Nodes.Add(MissionNameOverrideNode);
                        if (EntityTools.Config.MissionMonitor.Mission.RootDefOverride)
                            Nodes.Add(RootDefOverrideNode);
                        if (EntityTools.Config.MissionMonitor.Mission.StartTime)
                            Nodes.Add(StartTimeNode);
                        if (EntityTools.Config.MissionMonitor.Mission.ExpirationTime)
                            Nodes.Add(ExpirationTimeNode);
                        if (EntityTools.Config.MissionMonitor.Mission.State)
                            Nodes.Add(StateNode);
                        if (EntityTools.Config.MissionMonitor.Mission.MissionDef)
                            Nodes.Add(MissionDefNode);
                        Nodes.Add(ChildrensNode);
                    };
                    _settingsChanged = false;
                } 
            }
            else
            {
                _label = string.IsNullOrEmpty(_uiStringMsg) ? $@"[{_missionName}] : {_state}"
                    : $@"{_uiStringMsg} [{_missionName}] : Invalid";
                eventHandler?.Invoke(this, _timeStamp, _missionName, nameof(IsValid), string.Empty, true, false);
            }
        }

        private void UpdateChildrens(long timeStamp)
        {
            int childrenCount = _childrens.Count;
            int addedChildNum = 0;

            var eventHandler = OnMissionChanged;
            if (eventHandler is null)
            {
                foreach (var child in _mission.Childrens)
                    if (_childrens.Contains(child))
                        _childrens[child].Update(timeStamp);
                    else
                    {
                        var newChildren = new MissionTreeNode(child, timeStamp, ChildMissionChanged);
                        _childrens.Add(newChildren);
                        ChildrensNode.Nodes.Add(newChildren);
                        addedChildNum++;
                    }

                if (addedChildNum > 0)
                {
                    var newChildrenCount = childrenCount + addedChildNum;
                    ChildrensNode.Text = string.Concat(nameof(Mission.Childrens) + " [", newChildrenCount, ']');
                    if (ChildrensNode.Parent is null)
                        Nodes.Add(ChildrensNode);
                }
            }
            else
            {
                foreach (var child in _mission.Childrens)
                    if (_childrens.Contains(child))
                        _childrens[child].Update(timeStamp);
                    else
                    {
                        var newChildren = new MissionTreeNode(child, timeStamp, ChildMissionChanged);
                        _childrens.Add(newChildren);
                        ChildrensNode.Nodes.Add(newChildren);
                        eventHandler(this, timeStamp, string.Concat(_missionName, '/', newChildren._missionName), nameof(Mission.State), string.Empty, null, MissionState.Acquired);
                        addedChildNum++;
                    }

                if (addedChildNum > 0)
                {
                    var newChildrenCount = childrenCount + addedChildNum;
                    ChildrensNode.Text = string.Concat(nameof(Mission.Childrens) + " [", newChildrenCount, ']');
                    if (ChildrensNode.Parent is null)
                        Nodes.Add(ChildrensNode);
#if false
                    eventHandler(this, timeStamp, _missionName, nameof(Childrens), nameof(Childrens.Count), childrenCount, _childrens.Count); 
#endif
                }
            }
        }
        private void DelayedUpdateChildrens(long timeStamp, ref Action updater)
        {
            int childrenCount = _childrens.Count;
            int addedChildNum = 0;
            foreach (var child in _mission.Childrens)
                if (_childrens.Contains(child))
                    _childrens[child].DelayedUpdate(timeStamp, ref updater);
                else 
                {
                    var newChildren = new MissionTreeNode(child, timeStamp, ChildMissionChanged);
                    _childrens.Add(newChildren);
                    addedChildNum++;
                    updater += () => ChildrensNode.Nodes.Add(newChildren);

                    OnMissionChanged?.Invoke(this, timeStamp, string.Concat(_missionName, '/', child.MissionName), nameof(Mission.State), string.Empty, null, MissionState.Acquired);
                }


            if (addedChildNum > 0)
            {
                updater += () =>
                {
                    int newChildrenCount = childrenCount + addedChildNum;
                    ChildrensNode.Text = string.Concat(nameof(Mission.Childrens) + " [", newChildrenCount, ']');
                    if (ChildrensNode.Parent is null)
                        Nodes.Add(ChildrensNode);
#if false
                    OnMissionChanged?.Invoke(this, timeStamp, _missionName, nameof(Childrens), nameof(Childrens.Count),
                                    childrenCount, newChildrenCount); 
#endif
                }; 
            }
        }

        public override string ToString() => Lable();

        private string Lable()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (string.IsNullOrEmpty(_uiStringMsg))
                {
                    var displayName = _missionDefNode?.DisplayName;
                    _label = string.IsNullOrEmpty(displayName)
                        ? $@"[{_missionName}] : {_state}"
                        : $@"{displayName} [{_missionName}] : {_state}";
                }
                else _label = $@"{_uiStringMsg} [{_missionName}] : {_state}";
            }

            return _label;
        }
        private string _label;

        protected virtual void ChildMissionChanged(MissionTreeNode sender, long timeStamp, string missionName, string propertyName, string subPropertyName,
            object oldValue, object newValue)
        {
            var eventHandler = OnMissionChanged;
            if (eventHandler is null) return;
            eventHandler(sender, timeStamp, _missionName + '/' + missionName, propertyName, subPropertyName, oldValue, newValue);
        }

        protected virtual void MissionDefChanged(MissionDefTreeNode sender, long timeStamp, string missionDefName, string propertyName,
            object oldValue, object newValue)
        {
            var eventHandler = OnMissionChanged;
            if (eventHandler is null) return;
            eventHandler(this, timeStamp, _missionName, String.Concat(nameof(Mission.MissionDef) + " [", missionDefName, ']'), propertyName, oldValue, newValue);
        }

        class MissionMonitorTreeNodeCollection : KeyedCollection<IntPtr, MissionTreeNode>
        {
            protected override IntPtr GetKeyForItem(MissionTreeNode item) => item._mission.Pointer;
            public bool Contains(Mission mission) => base.Contains(mission.Pointer);
            public MissionTreeNode this [Mission mission] => base[mission.Pointer];
        }
    }
}