#define DEBUG_CHANGE_TARGET

using AcTp0Tools.Classes.Targeting;
using EntityCore.Entities;
using EntityTools.Tools.Targeting;
using MyNW.Classes;
using System;
using System.ComponentModel;

namespace EntityCore.Tools.Targeting
{
    /// <summary>
    /// Класс, реализующий обработку <seealso cref="EntityTarget"/>
    /// </summary>
    internal class EntityTargetProcessor : TargetProcessor
    {
        private EntityTarget selector;

#if DEBUG_CHANGE_TARGET
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object TargetCache => new SimpleEntityWrapper(_targetCache);
#endif
        private Entity _targetCache;

        public EntityTargetProcessor(EntityTarget target, Predicate<Entity> specialCheck = null)
        {
            selector = target ?? throw new ArgumentException(nameof(target));
            selector.PropertyChanged += OnPropertyChanged;

            _specialCheck = specialCheck;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
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

            _specialCheck = null;
        }

        public override void Reset()
        {
            _key = null;
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
                    ? GetType().Name
                    : $"Target Entity [{selector._entityId}]";
            }
            return _label;
        }
        private string _label;

        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        [Browsable(false)]
        public EntityCacheRecordKey EntityKey =>
            _key ?? (_key = new EntityCacheRecordKey(selector._entityId, selector._entityIdType,
                selector._entityNameType));

        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// </summary>        
        [Browsable(false)]
        public override Predicate<Entity> SpecialCheck
        {
            get => _specialCheck;
            set { _specialCheck = value ?? (ett => true); }
        }

        private Predicate<Entity> _specialCheck;
    }

}
