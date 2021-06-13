#define DEBUG_CHANGE_TARGET

using AcTp0Tools.Classes.UCC;
using EntityCore.Entities;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Actions.TargetSelectors;
using MyNW.Classes;
using System;
using System.ComponentModel;
using MyNW.Patchables.Enums;

namespace EntityCore.UCC.Classes
{
    /// <summary>
    /// Класс, реализующий обработку <seealso cref="EntityTarget"/>
    /// </summary>
    internal class EntityTargetProcessor : UccTargetProcessor
    {
        private ChangeTarget action;
        private EntityTarget selector;

#if DEBUG_CHANGE_TARGET
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object TargetCache => new SimpleEntityWrapper(_targetCache);//GetTarget();
#endif
        private Entity _targetCache;

        public EntityTargetProcessor(ChangeTarget changeTargetAction, EntityTarget target)
        {
            action = changeTargetAction ?? throw new ArgumentException(nameof(changeTargetAction));
            selector = target ?? throw new ArgumentException(nameof(target));

            selector.PropertyChanged += OnPropertyChanged;
            action.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _key = null;
            _specialCheck = null;
            _label = string.Empty;
        }

        public override void Dispose()
        {
            if (selector != null)
            {
                selector.PropertyChanged -= OnPropertyChanged;
                selector = null;
            }

            if (action != null)
            {
                action.PropertyChanged -= OnPropertyChanged;
                action = null;
            }
        }

        public override void Reset()
        {
            _key = null;
            _specialCheck = null;
            _label = string.Empty;
        }

        public override bool IsMatch(Entity target)
        {
            var key = EntityKey;
            var check = SpecialCheck;
            return key.Validate(target) && check(target);
        }

        public override bool IsTargetMismatchedAndCanBeChanged(Entity target)
        {
            var key = EntityKey;
            var check = SpecialCheck;
            if (key.Validate(target) && check(target))
                return false;

            return GetTarget() != null;
        }

        public override Entity GetTarget()
        {
            var key = EntityKey;
            var check = SpecialCheck;
            if (key.Validate(_targetCache) && check(_targetCache))
                return _targetCache;

            _targetCache = SearchCached.FindClosestEntity(key, check);
            return _targetCache;
        }

        public override string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                _label = string.IsNullOrEmpty(selector._entityId)
                    ? action.GetType().Name
                    : $"{action.GetType().Name} to Entity [{selector._entityId}]";
            }
            return _label;
        }
        private string _label;

        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        [Browsable(false)]
        public EntityCacheRecordKey EntityKey
        {
            get
            {
                if (_key is null)
                    _key = new EntityCacheRecordKey(selector._entityId, selector._entityIdType, selector._entityNameType);
                return _key;
            }
        }
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// Использовать самомодифицирующийся предиката нельзя, т.к. предикат передается в качестве аргумента в <seealso cref="SearchCached.FindClosestEntity(EntityCacheRecordKey, Predicate{Entity})"/>
        /// </summary>        
        [Browsable(false)]
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(true, 
                        action.Range, Astral.Controllers.Settings.Get.MaxElevationDifference, 
                        true, null, 
                        e=> e.RelationToPlayer == EntityRelation.Foe);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
    }

}
