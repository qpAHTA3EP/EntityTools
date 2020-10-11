using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Classes;
using EntityTools.Core.Proxies;
using EntityTools.Editors;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class TeamMembersCount : Condition, INotifyPropertyChanged
    {
        #region взаимодействие с ядром
        public event PropertyChangedEventHandler PropertyChanged;
#if CORE_INTERFACES
        internal IQuesterConditionEngine ConditionEngine;
#endif
        public TeamMembersCount()
        {
#if CORE_INTERFACES
            ConditionEngine = new QuesterConditionProxy(this);
#endif
            // EntityTools.Core.Initialize(this);
        }
        #endregion
        #region Опции команды
#if DEVELOPER
        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Location")]
#else
        [Browsable(false)]
#endif
        public List<string> CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
                }
            }
        }
        internal List<string> _customRegionNames;

#if DEVELOPER
        [Description("The Check of the Team member's location relative to the custom regions\n" +
            "Equals: Count only members located WITHIN the CustomRegions\n" +
            "NotEquals: Count only members located OUTSIDE the CustomRegions")]
        [Category("Location")]
#else
        [Browsable(false)]
#endif
        public Presence CustomRegionCheck
        {
            get => _customRegionCheck; set
            {
                _customRegionCheck = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
            }
        }
        internal Presence _customRegionCheck = Presence.Equal;

#if DEVELOPER
        [Description("The Value which is compared by 'DistanceSign' with the distance between Player and Team member")]
        [Category("Distance")]
#else
        [Browsable(false)]
#endif
        public float Distance
        {
            get => _distance; set
            {
                _distance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
            }
        }
        internal float _distance;

#if DEVELOPER
        [Description("Option specifies the comparison of the distance to the group member")]
        [Category("Distance")]
#else
        [Browsable(false)]
#endif
        public Relation DistanceSign
        {
            get => _distanceSign; set
            {
                _distanceSign = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
            }
        }
        internal Relation _distanceSign = Relation.Superior;

#if DEVELOPER
        [Description("The Value which is compared by 'Sign' with the counted Team members")]
        [Category("Members")]
#else
        [Browsable(false)]
#endif
        public int MemberCount
        {
            get => _memberCount; set
            {
                _memberCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
            }
        }
        internal int _memberCount = 3;

#if DEVELOPER
        [Description("Option specifies the comparison of 'MemberCount' and the counted Team members")]
        [Category("Members")]
#else
        [Browsable(false)]
#endif
        public Relation Sign
        {
            get => _sign; set
            {
                _sign = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
            }
        }
        internal Relation _sign = Relation.Inferior;

#if DEVELOPER
        [Description("The Check of the Team member's Region (not CustomRegion)):\n" +
            "True: Count Team member if it is located in the same Region as Player\n" +
            "False: Does not consider the region when counting Team members")]
        [Category("Members")]
#else
        [Browsable(false)]
#endif
        public bool RegionCheck
        {
            get => _regionCheck; set
            {
                _regionCheck = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
            }
        }
        internal bool _regionCheck;

/*#if DEVELOPER
        [Description("Time between searches of the TeamMembers (ms)")]
#else
        [Browsable(false)]
#endif
        public int SearchTimeInterval
        {
            get => _searchTimeInterval; set
            {
                _searchTimeInterval = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
            }
        }
        internal int _searchTimeInterval = 500;*/
        #endregion

#if CORE_INTERFACES
        public override bool IsValid => ConditionEngine.IsValid;
        public override void Reset() => ConditionEngine.Reset();
        public override string TestInfos => ConditionEngine.TestInfos;
        public override string ToString() => ConditionEngine.Label();
#endif
    }
}
