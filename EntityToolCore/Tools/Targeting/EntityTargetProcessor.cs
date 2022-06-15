#define DEBUG_CHANGE_TARGET

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

            /* BUG
             * [23:45] System.NullReferenceException: Ссылка на объект не указывает на экземпляр объекта.
                       в EntityCore.Tools.Targeting.EntityTargetProcessor.IsTargetMismatchedAndCanBeChanged(Entity target)
                       в EntityCore.UCC.Actions.ChangeTargetEngine.get_NeedToRun()
                       в EntityTools.UCC.Actions.ChangeTarget.get_NeedToRun() в D:\Source\EntityAddon\EntityTools\UCC\Actions\ChangeTarget.cs:строка 151
                       в EntityTools.Core.Proxies.UccActionProxy.get_NeedToRun() в D:\Source\EntityAddon\EntityTools\Core\Proxies\UCCActionProxy.cs:строка 24
                       в EntityTools.UCC.Actions.ChangeTarget.get_NeedToRun() в D:\Source\EntityAddon\EntityTools\UCC\Actions\ChangeTarget.cs:строка 151
                       в Astral.Logic.UCC.Classes.ActionsPlayer.playActionList(Boolean breakOnAction, Boolean waitSpell)
                       в Astral.Logic.UCC.Core.Run(Moment moment)
             */
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
                var entityId = selector.EntityID;
                _label = string.IsNullOrEmpty(entityId)
                    ? GetType().Name
                    : $"Target Entity [{entityId}]";
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
                    _key = new EntityCacheRecordKey(selector.EntityID, selector.EntityIdType,
                        selector.EntityNameType);
                return _key;

            }
        }

        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// </summary>        
        [Browsable(false)]
        public override Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                    _specialCheck = ett => !ett.IsDead && ett.Character.AttribsBasic.Health > 0;
                return _specialCheck;
            }
            set
            {
                _specialCheck = value;
                if (_specialCheck is null)
                {
                    _specialCheck = ett => !ett.IsDead && ett.Character.AttribsBasic.Health > 0;
                }
                else
                {
                    _specialCheck = ett => !ett.IsDead && ett.Character.AttribsBasic.Health > 0 && value(ett);
                }
            }
        }

        private Predicate<Entity> _specialCheck;
    }

}
