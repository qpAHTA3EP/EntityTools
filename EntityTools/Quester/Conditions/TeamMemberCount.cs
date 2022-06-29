using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Tools.CustomRegions;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class TeamMembersCount : Condition, INotifyPropertyChanged
    {
        #region Опции команды
#if DEVELOPER
        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Location")]
        [DisplayName("CustomRegions")]
#else
        [Browsable(false)]
#endif
        public CustomRegionCollection CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();

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
                NotifyPropertyChanged();
            }
        }
        private Presence _customRegionCheck = Presence.Equal;

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
                if (value < 0)
                    value = 0;
                _distance = value;
                NotifyPropertyChanged();
            }
        }
        private float _distance;

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
                NotifyPropertyChanged();
            }
        }
        private Relation _distanceSign = Relation.Superior;

#if DEVELOPER
        [Description("The Value which is compared by 'Sign' with the counted Team members")]
        [Category("Members")]
#else
        [Browsable(false)]
#endif
        public uint MemberCount
        {
            get => _memberCount; set
            {
                _memberCount = value;
                NotifyPropertyChanged();
            }
        }
        private uint _memberCount = 3;

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
                NotifyPropertyChanged();
            }
        }
        private Relation _sign = Relation.Inferior;

#if DEVELOPER
        [Description("The Check of the Team member's Region (not CustomRegion)):\n" +
            "True: Count Team member if it is located in the same Region as Player\n" +
            "False: Does not consider the region when counting Team members")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool RegionCheck
        {
            get => _regionCheck; set
            {
                _regionCheck = value;
                NotifyPropertyChanged();
            }
        }
        private bool _regionCheck;
        #endregion

        #region взаимодействие с ядром
        [NonSerialized]
        internal IQuesterConditionEngine Engine;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TeamMembersCount()
        {
            Engine = new QuesterConditionProxy(this);
        }

        private IQuesterConditionEngine MakeProxie()
        {
            return new QuesterConditionProxy(this);
        }
        #endregion

        public override bool IsValid => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).IsValid;
        public override void Reset() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).Reset();
        public override string TestInfos => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).TestInfos;
        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).Label();
    }
}
