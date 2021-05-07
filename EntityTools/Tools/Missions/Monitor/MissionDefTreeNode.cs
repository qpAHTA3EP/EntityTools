using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntityTools.Tools.Missions.Monitor
{
    public class MissionDefTreeNode : TreeNode
    {
        private readonly MissionDef _missionDef;

        public delegate void NotifyMissionDefChanged(MissionDefTreeNode sender, long timeStamp, string missionDefName, string propertyName, object oldValue, object newValue);

        ~MissionDefTreeNode()
        {
            OnMissionDefChanged = null;
            EntityTools.Config.MissionMonitor.MissionDef.PropertyChanged -= MissionDefSettingsChanged;
        }

        public event NotifyMissionDefChanged OnMissionDefChanged;

        public bool IsValid => _missionDef != null && _missionDef.IsValid && _missionDef.Name == _name;

        public MissionDefTreeNode(MissionDef missionDef, long timeStamp = -1, NotifyMissionDefChanged notifier = null)
        {
            _missionDef = missionDef ?? throw new ArgumentNullException(nameof(missionDef));

            _timeStamp = timeStamp > 0 ? timeStamp : DateTime.Now.Ticks;

            _name = _missionDef.Name;

            _displayName = _missionDef.DisplayName;
            _displayNameHistory.AddLast(Tuple.Create(_timeStamp, _displayName));
            DisplayNameNode = new TreeNode($@"{nameof(MissionDef.DisplayName)}: {_displayName}");

            _uiStringMsg = _missionDef.UIStringMsg;
            _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));
            UIStringNode = new TreeNode($@"{nameof(MissionDef.UIStringMsg)}: {_uiStringMsg}");

            _summary = _missionDef.Summary;
            _summaryHistory.AddLast(Tuple.Create(_timeStamp, _summary));
            SummaryNode = new TreeNode($@"{nameof(MissionDef.Summary)}: {_summary}");

            _relatedMission = _missionDef.RelatedMission;
            _relatedMissionHistory.AddLast(Tuple.Create(_timeStamp, _relatedMission));
            RelatedMissionNode = new TreeNode($@"{nameof(MissionDef.RelatedMission)}: {_relatedMission}");

            _missionType = _missionDef.MissionType;
            _missionTypeHistory.AddLast(Tuple.Create(_timeStamp, _missionType));
            MissionTypeNode = new TreeNode($@"{nameof(MissionDef.MissionType)}: {_missionType}");

            _canRepeat = _missionDef.CanRepeat;
            _canRepeatHistory.AddLast(Tuple.Create(_timeStamp, _canRepeat));
            CanRepeatNode = new TreeNode($@"{nameof(MissionDef.CanRepeat)}: {_canRepeat}");

#if false
            var eventHandler = OnMissionDefChanged;
            if (eventHandler != null)
            {
                eventHandler(this, _name, nameof(DisplayName), null, _displayName);
                eventHandler(this, _name, nameof(UIStringMsg), null, _uiStringMsg);
                eventHandler(this, _name, nameof(Summary), null, _summary);
                eventHandler(this, _name, nameof(RelatedMission), null, _relatedMission);
                eventHandler(this, _name, nameof(MissionType), null, _missionType);
                eventHandler(this, _name, nameof(CanRepeat), null, _canRepeat);
            } 
#endif

            Text = $@"{nameof(Mission.MissionDef)}[{_missionDef.Pointer.ToString("X2")}]: {_displayName}";
            Name = _name;

#if SubMissionsNode
            SubMissionsNode = new TreeNode();

            UpdateSubmission(_timeStamp); 
#endif
            if (EntityTools.Config.MissionMonitor.MissionDef.DisplayName)
                Nodes.Add(DisplayNameNode);
            if (EntityTools.Config.MissionMonitor.MissionDef.UIStringMsg)
                Nodes.Add(UIStringNode);
            if (EntityTools.Config.MissionMonitor.MissionDef.Summary)
                Nodes.Add(SummaryNode);
            if (EntityTools.Config.MissionMonitor.MissionDef.RelatedMission)
                Nodes.Add(RelatedMissionNode);
            if (EntityTools.Config.MissionMonitor.MissionDef.MissionType)
                Nodes.Add(MissionTypeNode);
            if (EntityTools.Config.MissionMonitor.MissionDef.CanRepeat)
                Nodes.Add(CanRepeatNode);

            OnMissionDefChanged = notifier;
            EntityTools.Config.MissionMonitor.MissionDef.PropertyChanged += MissionDefSettingsChanged;
        }

        private void MissionDefSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _settingsChanged = true;
        }

        private bool _settingsChanged;

#if false
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
#endif
        private long _timeStamp;

        public string MissionDefName => _name;
        readonly string _name;

        public TreeNode DisplayNameNode { get; }
        public string DisplayName => _displayName;
        string _displayName;
        readonly LinkedList<Tuple<long, string>> _displayNameHistory = new LinkedList<Tuple<long, string>>();

        public TreeNode UIStringNode { get; }
        public string UIStringMsg => _uiStringMsg;
        string _uiStringMsg;
        readonly LinkedList<Tuple<long, string>> _uiStringMsgHistory = new LinkedList<Tuple<long, string>>();

        public TreeNode SummaryNode { get; }
        public string Summary => _summary;
        string _summary;
        readonly LinkedList<Tuple<long, string>> _summaryHistory = new LinkedList<Tuple<long, string>>();

        public TreeNode RelatedMissionNode { get; }
        public string RelatedMission => _relatedMission;
        string _relatedMission;
        readonly LinkedList<Tuple<long, string>> _relatedMissionHistory = new LinkedList<Tuple<long, string>>();

        public TreeNode MissionTypeNode { get; }
        public uint MissionType => _missionType;
        uint _missionType;
        readonly LinkedList<Tuple<long, uint>> _missionTypeHistory = new LinkedList<Tuple<long, uint>>();

        public TreeNode CanRepeatNode { get; }
        public bool CanRepeat => _canRepeat;
        bool _canRepeat;
        readonly LinkedList<Tuple<long, bool>> _canRepeatHistory = new LinkedList<Tuple<long, bool>>();

#if SubMissionsNode
        public TreeNode SubMissionsNode { get; private set; }
        public ICollection<MissionDefTreeNode> SubMissions => _subMissions;
        readonly MissionDefTreeNodeCollection _subMissions = new MissionDefTreeNodeCollection(); 
#endif

        internal void Update(long timeStamp)
        {
            _timeStamp = timeStamp <= 0 ? timeStamp : DateTime.Now.Ticks;
            
            var eventHandler = OnMissionDefChanged;
            if (eventHandler is null)
            {
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
            else
            {
                object oldValue = _displayName;
                _displayName = _missionDef.DisplayName;
                if (_displayNameHistory.Last?.Value.Item2 != _displayName)
                {
                    _displayNameHistory.AddLast(Tuple.Create(_timeStamp, _displayName));
                    eventHandler(this, _timeStamp, _name, nameof(DisplayName), oldValue, _displayName);
                }

                oldValue = _uiStringMsg;
                _uiStringMsg = _missionDef.UIStringMsg;
                if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                {
                    _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));
                    eventHandler(this, _timeStamp, _name, nameof(UIStringMsg), oldValue, _uiStringMsg);
                }

                oldValue = _summary;
                _summary = _missionDef.Summary;
                if (_summaryHistory.Last?.Value.Item2 != _summary)
                {
                    _summaryHistory.AddLast(Tuple.Create(_timeStamp, _summary));
                    eventHandler(this, _timeStamp, _name, nameof(Summary), oldValue, _summary);
                }

                oldValue = _relatedMission;
                _relatedMission = _missionDef.RelatedMission;
                if (_relatedMissionHistory.Last?.Value.Item2 != _relatedMission)
                {
                    _relatedMissionHistory.AddLast(Tuple.Create(_timeStamp, _relatedMission));
                    eventHandler(this, _timeStamp, _name, nameof(RelatedMission), oldValue, _relatedMission);
                }

                oldValue = _missionType;
                _missionType = _missionDef.MissionType;
                if (_missionTypeHistory.Last?.Value.Item2 != _missionType)
                {
                    _missionTypeHistory.AddLast(Tuple.Create(_timeStamp, _missionType));
                    eventHandler(this, _timeStamp, _name, nameof(MissionType), oldValue, _missionType);
                }

                oldValue = _canRepeat;
                _canRepeat = _missionDef.CanRepeat;
                if (_canRepeatHistory.Last?.Value.Item2 != _canRepeat)
                {
                    _canRepeatHistory.AddLast(Tuple.Create(_timeStamp, _canRepeat));
                    eventHandler(this, _timeStamp, _name, nameof(CanRepeat), oldValue, _canRepeat);
                }
            }

#if SubMissionsNode
            UpdateSubmission(_timeStamp); 
#endif
            if (_settingsChanged)
            {
                Nodes.Clear();
                if (EntityTools.Config.MissionMonitor.MissionDef.DisplayName)
                    Nodes.Add(DisplayNameNode);
                if (EntityTools.Config.MissionMonitor.MissionDef.UIStringMsg)
                    Nodes.Add(UIStringNode);
                if (EntityTools.Config.MissionMonitor.MissionDef.Summary)
                    Nodes.Add(SummaryNode);
                if (EntityTools.Config.MissionMonitor.MissionDef.RelatedMission)
                    Nodes.Add(RelatedMissionNode);
                if (EntityTools.Config.MissionMonitor.MissionDef.MissionType)
                    Nodes.Add(MissionTypeNode);
                if (EntityTools.Config.MissionMonitor.MissionDef.CanRepeat)
                    Nodes.Add(CanRepeatNode);
#if SubMissionsNode
                Nodes.Add(SubMissionsNode);
#endif
                _settingsChanged = false;
            }
        }
        internal void DelayedUpdate(long timeStamp, ref Action updater)
        {
            _timeStamp = timeStamp <= 0 ? timeStamp : DateTime.Now.Ticks;

            var eventHandler = OnMissionDefChanged;
            if (eventHandler is null)
            {
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
            else
            {
                object oldValue = _displayName;
                _displayName = _missionDef.DisplayName;
                if (_displayNameHistory.Last?.Value.Item2 != _displayName)
                {
                    _displayNameHistory.AddLast(Tuple.Create(_timeStamp, _displayName));
                    eventHandler(this, _timeStamp, _name, nameof(DisplayName), oldValue, _displayName);
                }

                oldValue = _uiStringMsg;
                _uiStringMsg = _missionDef.UIStringMsg;
                if (_uiStringMsgHistory.Last?.Value.Item2 != _uiStringMsg)
                {
                    _uiStringMsgHistory.AddLast(Tuple.Create(_timeStamp, _uiStringMsg));
                    eventHandler(this, _timeStamp, _name, nameof(UIStringMsg), oldValue, _uiStringMsg);
                }

                oldValue = _summary;
                _summary = _missionDef.Summary;
                if (_summaryHistory.Last?.Value.Item2 != _summary)
                {
                    _summaryHistory.AddLast(Tuple.Create(_timeStamp, _summary));
                    eventHandler(this, _timeStamp, _name, nameof(Summary), oldValue, _summary);
                }

                oldValue = _relatedMission;
                _relatedMission = _missionDef.RelatedMission;
                if (_relatedMissionHistory.Last?.Value.Item2 != _relatedMission)
                {
                    _relatedMissionHistory.AddLast(Tuple.Create(_timeStamp, _relatedMission));
                    eventHandler(this, _timeStamp, _name, nameof(RelatedMission), oldValue, _relatedMission);
                }

                oldValue = _missionType;
                _missionType = _missionDef.MissionType;
                if (_missionTypeHistory.Last?.Value.Item2 != _missionType)
                {
                    _missionTypeHistory.AddLast(Tuple.Create(_timeStamp, _missionType));
                    eventHandler(this, _timeStamp, _name, nameof(MissionType), oldValue, _missionType);
                }

                oldValue = _canRepeat;
                _canRepeat = _missionDef.CanRepeat;
                if (_canRepeatHistory.Last?.Value.Item2 != _canRepeat)
                {
                    _canRepeatHistory.AddLast(Tuple.Create(_timeStamp, _canRepeat));
                    eventHandler(this, _timeStamp, _name, nameof(CanRepeat), oldValue, _canRepeat);
                }
            }

#if SubMissionsNode
            DelayedUpdateSubmission(_timeStamp, ref update); 
#endif
            if (_settingsChanged)
            {
                updater += () =>
                {
                    Nodes.Clear();
                    if (EntityTools.Config.MissionMonitor.MissionDef.DisplayName)
                        Nodes.Add(DisplayNameNode);
                    if (EntityTools.Config.MissionMonitor.MissionDef.UIStringMsg)
                        Nodes.Add(UIStringNode);
                    if (EntityTools.Config.MissionMonitor.MissionDef.Summary)
                        Nodes.Add(SummaryNode);
                    if (EntityTools.Config.MissionMonitor.MissionDef.RelatedMission)
                        Nodes.Add(RelatedMissionNode);
                    if (EntityTools.Config.MissionMonitor.MissionDef.MissionType)
                        Nodes.Add(MissionTypeNode);
                    if (EntityTools.Config.MissionMonitor.MissionDef.CanRepeat)
                        Nodes.Add(CanRepeatNode);
#if SubMissionsNode
                    Nodes.Add(SubMissionsNode);
#endif
                    _settingsChanged = false;
                };
            }
        }

#if SubMissionsNode
        void UpdateSubmission(int timeStamp)
        {
            int subMissCount = _subMissions.Count;
            foreach (var subMiss in _missionDef.SubMissions)
                if (_subMissions.Contains(subMiss))
                    _subMissions[subMiss].Update(timeStamp);
                else
                {
                    var newSubMission = new MissionDefTreeNode(subMiss, timeStamp, OnSubMissionDefChanged);
                    _subMissions.Add(newSubMission);
                    SubMissionsNode.Nodes.Add(newSubMission);
                }

            if (subMissCount < _subMissions.Count)
            {
                SubMissionsNode.Text = string.Concat(nameof(MissionDef.SubMissions) + " [", _subMissions.Count, ']');
                OnMissionDefChanged?.Invoke(this, _name, nameof(SubMissions) + '.' + nameof(SubMissions.Count), subMissCount, _subMissions.Count);
            }
        }
        void DelayedUpdateSubmission(int timeStamp, ref Action update)
        {
            int subMissCount = _subMissions.Count;
            foreach (var subMiss in _missionDef.SubMissions)
                if (_subMissions.Contains(subMiss))
                    _subMissions[subMiss].DelayedUpdate(timeStamp, ref update);
                else update += () =>
                    {
                        var newSubMission = new MissionDefTreeNode(subMiss, timeStamp, OnSubMissionDefChanged);
                        _subMissions.Add(newSubMission);
                        SubMissionsNode.Nodes.Add(newSubMission);
                    };

            update += () =>
            {
                if (subMissCount >= _subMissions.Count) return;

                SubMissionsNode.Text =
                    string.Concat(nameof(MissionDef.SubMissions) + " [", _subMissions.Count, ']');
                if (SubMissionsNode.Parent is null)
                    Nodes.Add(SubMissionsNode);
                OnMissionDefChanged?.Invoke(this, _name, nameof(SubMissions) + '.' + nameof(SubMissions.Count),
                    subMissCount, _subMissions.Count);
            };
        } 

        void OnSubMissionDefChanged(MissionDefTreeNode sender, string misionDefName, string propertyName,
            object oldValue, object newValue)
        {
            var eventHandler = OnMissionDefChanged;
            if (eventHandler is null) return;
            eventHandler(sender, _name + '/' + misionDefName, propertyName, oldValue, newValue);
        }

        class MissionDefTreeNodeCollection : KeyedCollection<IntPtr, MissionDefTreeNode>
        {
            protected override IntPtr GetKeyForItem(MissionDefTreeNode item) => item._missionDef.Pointer;
            public bool Contains(MissionDef missionDef) => base.Contains(missionDef.Pointer);
            public MissionDefTreeNode this[MissionDef missionDef] => base[missionDef.Pointer];

        } 
#endif
    }
}
