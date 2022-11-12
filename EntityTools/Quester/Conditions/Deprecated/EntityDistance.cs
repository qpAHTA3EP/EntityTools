using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;

using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.Entities;

using MyNW.Classes;

namespace EntityTools.Quester.Conditions.Deprecated
{
    [Serializable]
    public class EntityDistance : Condition, INotifyPropertyChanged
    {
        private string label = string.Empty;

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




        public EntityDistance()
        {
            Distance = 0;
            Sign = Relation.Superior;
            EntityIdType = ItemFilterStringType.Simple;
            RegionCheck = false;
        }




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = default)
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InternalResetOnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            if (propertyName == nameof(EntityID)
                || propertyName == nameof(Sign)
                || propertyName == nameof(Distance))
                label = string.Empty;
        }
        #endregion




        public override bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(_entityID))
                {
                    Entity closestEntity = SearchCached.FindClosestEntity(_entityID, _entityIdType, EntityNameType.NameUntranslated, EntitySetType.Complete, false, 0, 0, _regionCheck);

                    switch (_sign)
                    {
                        case Relation.Equal:
                            return closestEntity != null && closestEntity.IsValid && (Math.Abs(closestEntity.Location.Distance3DFromPlayer - _distance) < 1);
                        case Relation.NotEqual:
                            return closestEntity != null && closestEntity.IsValid && (Math.Abs(closestEntity.Location.Distance3DFromPlayer - _distance) >= 1);
                        case Relation.Inferior:
                            return closestEntity != null && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer < _distance);
                        case Relation.Superior:
                            return closestEntity == null || !closestEntity.IsValid || (closestEntity.Location.Distance3DFromPlayer > _distance);
                    }
                }

                return false;
            }
        }

        public override void Reset() { }

        public override string TestInfos
        {
            get
            {
                Entity closestEntity = SearchCached.FindClosestEntity(_entityID, _entityIdType, EntityNameType.NameUntranslated, EntitySetType.Complete, false, 0, 0, _regionCheck);

                return closestEntity.IsValid 
                     ? $"Found closest Entity [{closestEntity.NameUntranslated}] at the {nameof(Distance)} = {closestEntity.Location.Distance3DFromPlayer}" 
                     : $"No one Entity matched to [{_entityID}]";
            }
        }

        public override string ToString()
        {
            label = string.IsNullOrEmpty(label) 
                  ? $"[Deprecated] Entity [{_entityID}] Distance {_sign} to {_distance}" 
                  : $"[Deprecated] {GetType().Name}";
            return label;
        }
    }
}