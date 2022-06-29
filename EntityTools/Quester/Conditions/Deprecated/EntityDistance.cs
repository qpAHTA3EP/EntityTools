using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class EntityDistance : Condition, INotifyPropertyChanged
    {
        #region Взаимодействие с ядром EntityTools
        public event PropertyChangedEventHandler PropertyChanged;
        
        [NonSerialized]
        internal IQuesterConditionEngine Engine;

        private IQuesterConditionEngine MakeProxie()
        {
            return new QuesterConditionProxy(this);
        }
        #endregion

        public EntityDistance()
        {
            Engine = new QuesterConditionProxy(this);

            Distance = 0;
            Sign = Relation.Superior;
            EntityIdType = ItemFilterStringType.Simple;
            RegionCheck = false;
        }

        #region Опции команды
#if DEVELOPER
        [Description("ID (an untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public string EntityID
        {
            get => _entityID; set
            {
                if (_entityID != value)
                {
                    _entityID = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityID)));
                }
            }
        }
        internal string _entityID = string.Empty;

#if DEVELOPER
        [Description("Type of and EntityID:\n" +
                     "Simple: Simple test string with a mask (char '*' means any chars)\n" +
                     "Regex: Regular expression")]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public ItemFilterStringType EntityIdType
        {
            get => _entityIdType; set
            {
                if (_entityIdType != value)
                {
                    _entityIdType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityIdType)));
                }
            }
        }
        internal ItemFilterStringType _entityIdType;

#if DEVELOPER
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public float Distance
        {
            get => _distance; set
            {
                if (_distance != value)
                {
                    _distance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Distance)));
                }
            }
        }
        internal float _distance;

#if DEVELOPER
        [Description("Check Entity's Region:\n" +
            "True: Search an Entity only if it located in the same Region as Player\n" +
            "False: Does not consider the region when searching Entities")]
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public bool RegionCheck
        {
            get => _regionCheck; set
            {
                if (_regionCheck != value)
                {
                    _regionCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegionCheck)));
                }
            }
        }
        internal bool _regionCheck;

#if DEVELOPER
        [Description("Distance comparison type to the closest Entity")]
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public Relation Sign
        {
            get => _sign;
            set
            {
                if (_sign != value)
                {
                    _sign = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sign)));
                }
            }
        }
        internal Relation _sign;
        #endregion

        public override bool IsValid => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).IsValid;

        public override void Reset() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).Reset();

        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).Label();

        public override string TestInfos => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).TestInfos;
    }
}