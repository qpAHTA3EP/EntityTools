using Infrastructure;
using AStar;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.UIEditors;
using EntityTools.Forms;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Quester.Mapper;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Entities;
using EntityTools.Tools.Extensions;
using EntityTools.Tools.Powers;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;
using Timeout = Astral.Classes.Timeout;
// ReSharper disable InconsistentNaming

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class MoveToEntity : Action, INotifyPropertyChanged, IEntityDescriptor
    {
        //BUG: не находит Entity, если в CustomRegions задан лишь Exclude
        #region вспомогательные Данные 
        private string _idStr = string.Empty;
        private string _label = string.Empty;
        private Entity targetEntity;
        private Entity closestEntity;
        private readonly Timeout internalCacheTimer = new Timeout(0);
        private Timeout entityAbsenceTimer;
        private readonly PowerCache powerCache;
        #endregion

        public MoveToEntity()
        {
            _idStr = $"{GetType().Name}[{ActionID}]";
            powerCache = new PowerCache(string.Empty,
                                        _idStr,
                                        () => ExtendedDebugInfo);
        }




        #region Опции команды
        #region Entity
        [Description("The identifier of the Entity for the search.")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    _key = null;
                    _label = string.Empty;
                    targetEntity = null;
                    closestEntity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _entityId = string.Empty;

        [Description("Type of the Entity identifier:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols),\n" +
            "Regex: Regular expression.")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType
        {
            get => _entityIdType;
            set
            {
                if (_entityIdType != value)
                {
                    _entityIdType = value;
                    _key = null;
                    _label = string.Empty;
                    targetEntity = null;
                    closestEntity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

        [Description("The Entity property that uses as identifier.")]
        [Category("Entity")]
        public EntityNameType EntityNameType
        {
            get => _entityNameType;
            set
            {
                if (_entityNameType != value)
                {
                    _entityNameType = value;
                    _key = null;
                    _label = string.Empty;
                    targetEntity = null;
                    closestEntity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the Entity searching.")]
        [Category("Entity")]
        public string TestSearch => @"Push button '...' =>";
        #endregion


        #region EntitySearchOptions
        [Description("Checking the health of Entity not zero (it's alive):\n" +
            "True: Only Entities with nonzero health are detected,\n" +
            "False: Entity's health does not checked during search.")]
        [Category("Entity Search Options")]
        public bool HealthCheck
        {
            get => _healthCheck; set
            {
                if (_healthCheck != value)
                {
                    _healthCheck = value;
                    _specialEntityCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _healthCheck = true;

        [Description("Checking the distance between target Entity and the Player:\n" +
                     "True: Do not change the target Entity while it is alive or until the Bot within '" + nameof(Distance) + "' of it.\n" +
                     "False: Constantly scan an area and target the nearest Entity.")]
        [Category("Entity Search Options")]
        public bool HoldTargetEntity
        {
            get => _holdTargetEntity; set
            {
                if (_holdTargetEntity != value)
                {
                    _holdTargetEntity = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _holdTargetEntity = true;
        #endregion


        #region SearchArea
        [Description("The maximum distance from the character within which the Entity is searched.\n" +
            "The default value is 0, which disables distance checking.")]
        [Category("Search Area")]
        public float ReactionRange
        {
            get => _reactionRange; set
            {
                if (value < 0)
                    value = 0;
                if (Math.Abs(_reactionRange - value) > 0.1f)
                {
                    _reactionRange = value;
                    _specialEntityCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionRange;

        [Description("The maximum Z-coordiante difference with Player within which the Entity is searched.\n" +
                     "The default value is 0, which disables Z-coordiante checking.")]
        [Category("Search Area")]
        public float ReactionZRange
        {
            get => _reactionZRange; set
            {
                if (value < 0)
                    value = 0;
                if (Math.Abs(_reactionZRange - value) > 0.1f)
                {
                    _reactionZRange = value;
                    _specialEntityCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionZRange;

        [Description("Checking the Entity and the Player is in the same ingame Region (not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected,\n" +
            "False: Entity's Region does not checked during search.")]
        [Category("Entity Search Options")]
        public bool RegionCheck
        {
            get => _regionCheck; set
            {
                if (_regionCheck != value)
                {
                    _regionCheck = value;
                    _specialEntityCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _regionCheck;

        [Description("The collection of the CustomRegion that define the search area.")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Search Area")]
        [XmlElement("CustomRegionNames")]
        [DisplayName("CustomRegions")]
        [NotifyParentProperty(true)]
        public CustomRegionCollection CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
                    _specialEntityCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();

        [Description("Checking the Player is in the area, defined by 'CustomRegions'")]
        [Category("Search Area")]
        public bool CustomRegionsPlayerCheck
        {
            get => customRegionsPlayerCheck; set
            {
                if (customRegionsPlayerCheck != value)
                {
                    customRegionsPlayerCheck = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool customRegionsPlayerCheck;

        [Description("The name of the Map where current action can be run.\n" +
                     "Ignored if empty.")]
        [Editor(typeof(CurrentMapEdit), typeof(UITypeEditor))]
        [Category("Search Area")]
        public string CurrentMap
        {
            get => currentMap;
            set
            {
                if (value != currentMap)
                {
                    currentMap = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string currentMap;
        #endregion


        #region ManageCombatOptions
        [Description("Distance to the Entity by which it is necessary to approach.\n" +
                     "The minimum value is 5.")]
        [Category("Manage Combat Options")]
        [DisplayName("CombatDistance")]

        public float Distance
        {
            get => _distance; set
            {
                if (value < 5)
                    value = 5;

                if (Math.Abs(_distance - value) > 0.1f)
                {
                    _distance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _distance = 30;

        [Description("Ignore the enemies while approaching the target Entity.")]
        [Category("Manage Combat Options")]
        public bool IgnoreCombat
        {
            get => _ignoreCombat; set
            {
                if (_ignoreCombat != value)
                {
                    _ignoreCombat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _ignoreCombat = true;

        [Description("Sets the ucc option '" + nameof(IgnoreCombatMinHP) + "' when disabling combat mode.\n" +
                     "Options ignored if the value is -1.")]
        [Category("Manage Combat Options")]
        public int IgnoreCombatMinHP
        {
            get => _ignoreCombatMinHp; set
            {
                if (value < -1)
                    value = -1;
                if (value > 100)
                    value = 100;
                if (_ignoreCombatMinHp != value)
                {
                    _ignoreCombatMinHp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _ignoreCombatMinHp = -1;

        [Description("Special check before disabling combat while playing action.\n" +
                     "The condition is checking when option '" + nameof(IgnoreCombat) + "' is active.")]
        [Category("Manage Combat Options")]
        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Astral.Quester.Classes.Condition IgnoreCombatCondition
        {
            get => _ignoreCombatCondition;
            set
            {
                if (value != _ignoreCombatCondition)
                {
                    _ignoreCombatCondition = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Astral.Quester.Classes.Condition _ignoreCombatCondition;

        [Description("The battle is aborted outside '" + nameof(AbortCombatDistance) + "' radius from the target entity.\n" +
                     "The combat is restored within the '" + nameof(Distance) + "' radius.\n" +
                     "However, this is not performed if the value less than '" + nameof(Distance) + "' or '" + nameof(IgnoreCombat) + "' is False.")]
        [Category("Manage Combat Options")]
        public uint AbortCombatDistance
        {
            get => _abortCombatDistance; set
            {
                if (_abortCombatDistance != value)
                {
                    _abortCombatDistance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private uint _abortCombatDistance;

        [Description("The identifier of the skill applying on the target Entity at the first strike.")]
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
        [Category("Manage Combat Options")]
        public string PowerId
        {
            get => _powerId;
            set
            {
                if (_powerId != value)
                {
                    _powerId = value;
                    powerCache.Reset(_powerId);
                    NotifyPropertyChanged();
                }
            }
        }
        private string _powerId = string.Empty;

        [Description("The time needed to activate the skill '" + nameof(PowerId) + "'.")]
        [Category("Manage Combat Options")]
        [DisplayName("PowerCastingTime (ms)")]
        public int CastingTime
        {
            get => _castingTime;
            set
            {
                if (value < 0)
                    value = 0;
                if (_castingTime != value)
                {
                    _castingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _castingTime;

        [Description("True: Clear the list of attackers and attack the target Entity when it is approached.\n" +
                     "This option is ignored when '" + nameof(IgnoreCombat) + "' does not set.")]
        [Category("Manage Combat Options")]
        public bool AttackTargetEntity
        {
            get => _attackTargetEntity; set
            {
                if (_attackTargetEntity != value)
                {
                    _attackTargetEntity = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _attackTargetEntity = true;
        #endregion


        #region Interruptions
        [Description("True: Complete an action when the Entity is closer than 'Distance'.\n" +
                     "False: Follow an Entity regardless of its distance.")]
        [Category("Interruptions")]
        public bool StopOnApproached
        {
            get => _stopOnApproached; set
            {
                if (_stopOnApproached != value)
                {
                    _stopOnApproached = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _stopOnApproached;

        [Description("The command is interrupted upon entity search timer reaching zero (ms).\n" +
                     "Set zero value to infinite search.")]
        [Category("Interruptions")]
        public int EntitySearchTime
        {
            get => _entitySearchTime;
            set
            {
                if (value < 0)
                    value = 0;
                if (_entitySearchTime != value)
                {
                    _entitySearchTime = value;
                    entityAbsenceTimer = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _entitySearchTime;
        #endregion


        [Description("Reset current HotSpot after approaching the target Entity.")]
        public bool ResetCurrentHotSpot
        {
            get => _resetCurrentHotSpot; set
            {
                if (_resetCurrentHotSpot != value)
                {
                    _resetCurrentHotSpot = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _resetCurrentHotSpot; 
        #endregion




        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion



        
        public override bool NeedToRun
        {
            get
            {
                bool extendedDebugInfo = ExtendedDebugInfo;
                string currentMethodName = extendedDebugInfo
                    ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun)}"
                    : string.Empty;

                if (!IntenalConditions)
                {
                    // Чтобы завершить команду нужно перейти к Run() и вернуть ActionResult.Fail
                    // Переход к Run() возможен только при возврате true
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(IntenalConditions)} are False. Force calling of {nameof(Run)}");
                    return true;
                }

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins");

                // Команда работает с 2 - мя целями:
                //   1-я цель (targetEntity) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //   2-я ближайшая цель (closest) управляет флагом IgnoreCombat
                // Если HoldTargetEntity ВЫКЛЮЧЕН и обе цели совпадают - это ближайшая цель 
                EntityPreprocessingResult entityPreprocessingResult = Preprocessing_Entity(targetEntity);

                var entityNameType = EntityNameType;
                var holdTargetEntity = HoldTargetEntity;

                if (extendedDebugInfo)
                {
                    string debugMsg = targetEntity is null
                        ? $"{currentMethodName}: Target[NULL] processing result: '{entityPreprocessingResult}'"
                        : $"{currentMethodName}: {targetEntity.GetDebugString(entityNameType, "Target", EntityDetail.Pointer)} processing result: '{entityPreprocessingResult}'";
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                {
                    // targetEntity недействительный - сбрасываем
                    targetEntity = null;
                }

                if (entityPreprocessingResult != EntityPreprocessingResult.Succeeded && internalCacheTimer.IsTimedOut)
                {
                    // targetEntity не был обработан
                    Entity entity = SearchCached.FindClosestEntity(EntityKey, SpecialEntityCheck);

                    if (entity != null)
                    {
                        ResetEntitySearchTimer();
                        closestEntity = entity;
                        bool closestIsTarget = targetEntity != null && targetEntity.ContainerId == closestEntity.ContainerId;

                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Found {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)}{(closestIsTarget ? " that equals to Target" : string.Empty)}");

                        if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                        {
                            // сохраняем ближайшую сущность в targetEntity
                            if (extendedDebugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: Change Target[INVALID] to the {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)}");
                            }
                            targetEntity = closestEntity;
                        }
                        else if (!closestIsTarget)
                        {
                            // targetEntity не является ближайшей сущностью
                            if (!holdTargetEntity)
                            {
                                // Фиксация на targetEntity не требуется
                                // ближайшую цель можно сохранить в targetEntity
                                if (extendedDebugInfo)
                                {
                                    string debugMsg = targetEntity is null
                                        ? $"{currentMethodName}: Change Target[NULL] to the {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)}"
                                        : $"{currentMethodName}: Change {targetEntity.GetDebugString(entityNameType, "Target", EntityDetail.Pointer)} to the {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)}";
                                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                                }
                                targetEntity = closestEntity;
                            }

                            // обрабатываем ближайшую сущность closestEntity
                            entityPreprocessingResult = Preprocessing_Entity(closestEntity);
                            if (extendedDebugInfo)
                            {
                                if (closestEntity is null)
                                    ETLogger.WriteLine(LogType.Debug,
                                        $"{currentMethodName}: ClosestEntity[NULL] processing result: '{entityPreprocessingResult}'");
                                else ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)} processing result: '{entityPreprocessingResult}'");
                            }
                        }
                    }
                    else
                    {
                        StartEntitySearchTimer();
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ClosestEntity not found");

                    }

                    internalCacheTimer.ChangeTime(EntityTools.Config.EntityCache.LocalCacheTime);
                }

                bool needToRun = entityPreprocessingResult == EntityPreprocessingResult.Succeeded;

                if (!needToRun)
                {
                    if (IgnoreCombat && CheckingIgnoreCombatCondition())
                    {
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, IgnoreCombatMinHP, 5_000);
                        if (AbortCombatDistance > Distance)
                        {
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: Disable combat and set abort combat condition");
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Disable combat");
                    }
                }

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Result '{needToRun}'");

                return needToRun;
            }
        }

        public override ActionResult Run()
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = extendedDebugInfo ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)}"
                : string.Empty;

            if (!IntenalConditions)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(IntenalConditions)} failed. ActionResult={ActionResult.Fail}.");
                if (IgnoreCombat)
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                return ActionResult.Fail;
            }

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins");

            var entityKey = EntityKey;

            if (entityKey.Validate(closestEntity))
                Attack_Entity(closestEntity);
            else if (entityKey.Validate(targetEntity))
                Attack_Entity(targetEntity);

            ActionResult actionResult = StopOnApproached
                ? ActionResult.Completed
                : ActionResult.Running;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult={actionResult}");

            // Возобновление таймера остановки поиска
            ResetEntitySearchTimer();

            if (ResetCurrentHotSpot)
                CurrentHotSpotIndex = -1;
            return actionResult;
        }

        #region Декомпозиция основного функционала
        /// <summary>
        /// Анализ <paramref name="entity"/> на предмет возможности вступления в бой
        /// </summary>
        private EntityPreprocessingResult Preprocessing_Entity(Entity entity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = extendedDebugInfo ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Preprocessing_Entity)}" : string.Empty;

            bool validationResult = EntityKey.Validate(entity);

            if (validationResult)
            {
                // entity валидно
                bool healthCheck = HealthCheck;
                bool healthResult = !HealthCheck || !entity.IsDead;
                double distance = entity.Location.Distance3DFromPlayer;
                bool distanceResult = distance <= Distance;
                var entityNameType = EntityNameType;
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", entity.GetDebugString(entityNameType, "Entity", EntityDetail.Pointer),
                        " verification=", healthResult && distanceResult, " {Valid; ", healthCheck ? (healthResult ? "Alive; " : "Dead; ") : "Skip; ",
                        distanceResult ? "Near (" : "Faraway (", distance.ToString("N2"), ")}"));

                if (!healthResult) return EntityPreprocessingResult.Failed;

                if (distanceResult)
                {
                    // entity в пределах заданного расстояния
                    return EntityPreprocessingResult.Succeeded;
                }
                return EntityPreprocessingResult.Faraway;
            }
            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity Verification=False {{Invalid}}");

            return EntityPreprocessingResult.Failed;
        }

        #region обработка AbortCombatDistance
        /// <summary>
        /// Нападение на сущность <paramref name="entity"/> в зависимости от настроек команды
        /// </summary>
        private void Attack_Entity(Entity entity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Attack_Entity)}"
                : string.Empty;

            if (entity is null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity is NULL. Break");
                return;
            }

            if (AttackTargetEntity || powerCache.IsInitialized)
            {
                // entity нужно атаковать
                if (entity.IsLineOfSight()
                    && !entity.DoNotDraw
                    && !entity.IsUntargetable)
                {
                    Attackers.List.Clear();

                    // entity враждебно и должно быть атаковано
                    string entityStr = string.Empty;
                    if (extendedDebugInfo)
                        entityStr = entity.GetDebugString(EntityNameType, "Entity", EntityDetail.Pointer | EntityDetail.RelationToPlayer);

                    Astral.Quester.API.IgnoreCombat = false;

                    if (AbortCombatDistance > Distance)
                    {
                        // устанавливаем прерывание боя
                        AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Set abort combat condition, engage combat and attack {entityStr} at the distance {entity.CombatDistance3:N2}");
                    }
                    else if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{currentMethodName}: Engage combat and attack {entityStr} at the distance {entity.CombatDistance3:N2}");

                    // Применяем умение на targetEntity
                    if (powerCache.IsInitialized)
                    {
                        var pow = powerCache.Power;
                        if (pow != null)
                        {
                            if (extendedDebugInfo)
                            {
                                var powDef = pow.PowerDef;
                                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Activating Power '{powDef.DisplayName}'[{powDef.InternalName}] on {entityStr}");
                            }

                            pow.ExecutePower(entity, CastingTime, (int)Distance, false, extendedDebugInfo);
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Fail to get Power '{PowerId}' by '" + nameof(PowerId) + "'");
                    }
                    // атакуем враждебное targetEntity
                    if (entity.RelationToPlayer == EntityRelation.Foe)
                        Astral.Logic.NW.Combats.CombatUnit(entity);

                    return;
                }
            }
            if (IgnoreCombat && CheckingIgnoreCombatCondition())
            {
                // entity в пределах досягаемости, но не может быть атакована (entity.RelationToPlayer != EntityRelation.Foe) 
                // или не должна быть атакована принудительно (!AttackTargetEntity)
                if (AbortCombatDistance > Distance)
                {
                    AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: SetAbortCombatCondition");
                }
                else if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Engage combat");

                Astral.Quester.API.IgnoreCombat = false;
            }
        }

        /// <summary>
        /// Делегат, сравнивающий <see cref="MoveToEntity.AbortCombatDistance"/> с расстоянием между игроком и <see cref="closestEntity"/> или <see cref="targetEntity"/>,
        /// и прерывающий бой, при удалении персонажа от <paramref name="combatTarget"/>
        /// </summary>
        /// <returns>true, если нужно прервать бой</returns>
        private bool CombatShouldBeAborted(ref Entity combatTarget)
        {
            var combatTargetContainerId = combatTarget?.ContainerId ?? uint.MaxValue;

            if (ExtendedDebugInfo)
            {
                string currentMethodName =
                    $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(CombatShouldBeAborted)}";

                // Бой не должен прерываться, если  HP < IgnoreCombatMinHP
                var healthPercent = EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent;
                var ignoreCombatMinHP = AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP;
                if (healthPercent < ignoreCombatMinHP)
                {
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: Player health ({healthPercent}%) below IgnoreCombatMinHP ({ignoreCombatMinHP}%). Continue...");
                    return false;
                }

                var entityNameType = EntityNameType;
                var abortCombatDistance = AbortCombatDistance;
                var healthCheck = HealthCheck;

                if (EntityKey.Validate(closestEntity))
                {
                    // Если атакуемая цель combatTarget является closestEntity => продолжаем бой
                    if (closestEntity.ContainerId == combatTargetContainerId)
                        return false;

                    string entityStr = closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer);
                    if (!HealthCheck || !closestEntity.IsDead)
                    {
                        // closestEntity является живым или проверка healthCheck не требуется
                        double dist = closestEntity.Location.Distance3DFromPlayer;
                        if (dist <= abortCombatDistance)
                        {
                            // расстояние до closestEntity меньше дистанции прерывания боя => продолжаем бой 
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Player withing {nameof(AbortCombatDistance)} ({dist:N2} to {entityStr}). Continue...");
                            return false;
                        }
                        //else
                        //{
                        //    // расстояние до closestEntity больше дистанции прерывания боя => можно прервать бой, но лучше проверим targetEntity  
                        //    ETLogger.WriteLine(LogType.Debug,
                        //        string.Concat(currentMethodName, ": Player outside ", nameof(AbortCombatDistance),
                        //            " (", dist.ToString("N2"), " to ", entityStr, "). Combat have to be aborted"));
                        //    return true;
                        //}
                    }

                    // ClosestEntity мертво или далеко => можно прервать бой, но лучше проверим targetEntity
                    //ETLogger.WriteLine(LogType.Debug,
                    //    string.Concat(currentMethodName, ": ", entityStr, " is dead. Combat have to be aborted"));
                    //return true;
                }

                if (EntityKey.Validate(targetEntity))
                {
                    // Если атакуемая цель combatTarget является targetEntity => продолжаем бой
                    if (targetEntity.ContainerId == combatTargetContainerId)
                        return false;

                    string entityStr = targetEntity.GetDebugString(entityNameType, "Entity", EntityDetail.Pointer);
                    if (healthCheck && targetEntity.IsDead)
                    {
                        // targetEntity мертво => прерываем бой
                        ETLogger.WriteLine(LogType.Debug,
                            $"{currentMethodName}: {entityStr} is dead. Combat have to be aborted");
                        return true;
                    }

                    double dist = targetEntity.Location.Distance3DFromPlayer;
                    if (dist >= abortCombatDistance)
                    {
                        // расстояние до targetEntity больше дистанции прерывания боя => прерываем бой
                        ETLogger.WriteLine(LogType.Debug,
                            $"{currentMethodName}: Player outside {nameof(AbortCombatDistance)} ({dist:N2} to {entityStr}). Combat have to be aborted");
                        return true;
                    }

                    // targetEntity - живое и расстояние до него меньше дистанции прерывания боя => ghjljk;ftv ,jq
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: Player withing {nameof(AbortCombatDistance)} ({dist:N2} to {entityStr}). Continue...");
                    return false;
                }

                // targetEntity и closestEntity невалидны => прерываем бой
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity[INVALID]. Combat have to be aborted");
                return true;
            }
            else
            {
                // проверка необходимости перерывания боя без вывода отладочной информации
                if (EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent <
                      AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP) return false;

                var abortCombatDistance = AbortCombatDistance;
                var healthCheck = HealthCheck;

                if (EntityKey.Validate(closestEntity))
                {
                    if (closestEntity.ContainerId == combatTargetContainerId)
                        return false;

                    if (healthCheck && closestEntity.IsDead) return true;

                    if (closestEntity.Location.Distance3DFromPlayer <= abortCombatDistance)
                        return false;
                }

                if (EntityKey.Validate(targetEntity))
                {
                    if (targetEntity.ContainerId == combatTargetContainerId)
                        return false;

                    if (healthCheck && targetEntity.IsDead) return true;

                    return targetEntity.Location.Distance3DFromPlayer >= abortCombatDistance;
                }
                return true;
            }
        }
        private bool ShouldRemoveAbortCombatCondition()
        {
            return Completed;
        }
        #endregion
        #endregion

        public override string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(_label))
                    _label = $"{GetType().Name} [{EntityID}]";
                return _label;
            }
        }

        public override string InternalDisplayName => string.Empty;

        protected override bool IntenalConditions
        {
            get
            {
                if (EntityNameType != EntityNameType.Empty
                    && string.IsNullOrEmpty(EntityID))
                    return false;

                var player = EntityManager.LocalPlayer;

                var map = CurrentMap;
                if (!string.IsNullOrEmpty(map) &&
                    !map.Equals(player.MapState.MapName, StringComparison.Ordinal))
                {
                    if (ExtendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(IntenalConditions)}: Player is out of map '{map}' .");
                    }
                    return false;
                }

                if (CustomRegionsPlayerCheck && CustomRegionNames.Outside(player.Location))
                {
                    if (ExtendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(IntenalConditions)}: {nameof(CustomRegionsPlayerCheck)} failed. Player is outside CustomRegions.");
                    }
                    return false;
                }

                if (entityAbsenceTimer != null && entityAbsenceTimer.IsTimedOut)
                {
                    if (ExtendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(IntenalConditions)}: {nameof(EntitySearchTime)} is out.");
                    }
                    return false;
                }

                return true;
            }
        }

        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(EntityID) && EntityNameType != EntityNameType.Empty)
                    return new ActionValidity($"The Entity identifier is not valid.\n" +
                        $"Check options '{nameof(EntityID)}' and '{nameof(EntityNameType)}'.");
                return new ActionValidity();
            }
        }

        public override bool UseHotSpots => true;

        protected override Vector3 InternalDestination
        {
            get
            {
                if (!IntenalConditions)
                    return Vector3.Empty;

                if (EntityKey.Validate(targetEntity))
                {
                    if (targetEntity.Location.Distance3DFromPlayer > Distance)
                        return targetEntity.Location.Clone();
                    return EntityManager.LocalPlayer.Location.Clone();
                }
                return Vector3.Empty;
            }
        }

        public override void InternalReset()
        {
            targetEntity = null;
            closestEntity = null;

            _idStr = $"{GetType().Name}[{ActionID}]";

            _key = null;
            _label = string.Empty;
            _specialEntityCheck = null;

            powerCache.Reset(_powerId);
            internalCacheTimer.ChangeTime(0);
            entityAbsenceTimer = null;


            ResetEntitySearchTimer();
            AstralAccessors.Logic.NW.Combats.RemoveAbortCombatCondition();

            if (ExtendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}.{nameof(InternalReset)}");
        }

        public override void GatherInfos()
        {
            var player = EntityManager.LocalPlayer;
            if (player.IsValid && !player.IsLoading)
            {
                if (HotSpots.Count == 0)
                {
                    var pos = player.Location.Clone();
                    Node node = AstralAccessors.Quester
                                               .Core
                                               .CurrentProfile
                                               .CurrentMesh
                                               .ClosestNode(pos.X, 
                                                            pos.Y, 
                                                            pos.Z, 
                                                            out double distance,
                                                            false);
                    if (node != null
                        && distance < 10)
                        HotSpots.Add(node.Position);
                    else HotSpots.Add(pos);
                }

                CurrentMap = player.MapState.MapName;

                var entityId = EntityID;
                if (string.IsNullOrEmpty(entityId))
                {
                    var entityIdType = EntityIdType;
                    var entityNameType = EntityNameType;
                    if (EntityViewer.GUIRequest(ref entityId, ref entityIdType, ref entityNameType) != null)
                    {
                        EntityID = entityId;
                        EntityIdType = entityIdType;
                        EntityNameType = entityNameType;
                    }
                }
            }

            _label = string.Empty;
        }

        public override void OnMapDraw(GraphicsNW graphics)
        {
            var entityKey = EntityKey;
            if (entityKey.Validate(targetEntity))
            {
                var distance = Distance;
                var abortCombatDistance = AbortCombatDistance;

                if (graphics is MapperGraphics mapGraphics)
                {
                    float x = targetEntity.Location.X,
                        y = targetEntity.Location.Y,
                        diaD = distance * 2,
                        diaACD = abortCombatDistance * 2;

                    mapGraphics.FillRhombCentered(Brushes.Yellow, targetEntity.Location, 16, 16);
                    if (distance > 11)
                    {
                        mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaD, true);
                        if (abortCombatDistance > distance)
                            mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaACD, true);
                    }

                    if (entityKey.Validate(closestEntity) && targetEntity.ContainerId != closestEntity.ContainerId)
                    {
                        x = closestEntity.Location.X;
                        y = closestEntity.Location.Y;

                        mapGraphics.FillRhombCentered(Brushes.White, closestEntity.Location, 16, 16);

                        if (distance > 5)
                        {
                            mapGraphics.DrawCircleCentered(Pens.White, x, y, diaD, true);
                            if (abortCombatDistance > distance)
                                mapGraphics.DrawCircleCentered(Pens.White, x, y, diaACD, true);
                        }
                    }
                }
                else
                {
                    float x = targetEntity.Location.X,
                                  y = targetEntity.Location.Y;
                    List<Vector3> coords = new List<Vector3>() {
                        new Vector3(x, y - 5, 0),
                        new Vector3(x - 4.33f, y + 2.5f, 0),
                        new Vector3(x + 4.33f, y + 2.5f, 0)
                    };
                    graphics.drawFillPolygon(coords, Brushes.Yellow);

                    int diaD = (int)(distance * 2.0f * graphics.Zoom);
                    int diaACD = (int)(abortCombatDistance * 2.0f * graphics.Zoom);
                    if (distance > 5)
                    {
                        graphics.drawEllipse(targetEntity.Location, new Size(diaD, diaD), Pens.Yellow);

                        if (abortCombatDistance > distance)
                        {
                            graphics.drawEllipse(targetEntity.Location, new Size(diaACD, diaACD), Pens.Orange);
                        }
                    }

                    if (entityKey.Validate(closestEntity) && targetEntity.ContainerId != closestEntity.ContainerId)
                    {
                        x = closestEntity.Location.X;
                        y = closestEntity.Location.Y;

                        coords[0].X = x; coords[0].Y = y - 5;
                        coords[1].X = x - 4.33f; coords[1].Y = y + 2.5f;
                        coords[2].X = x + 4.33f; coords[2].Y = y + 2.5f;
                        graphics.drawFillPolygon(coords, Brushes.LightYellow);

                        if (distance > 5)
                        {
                            graphics.drawEllipse(closestEntity.Location, new Size(diaD, diaD), Pens.Yellow);
                            if (abortCombatDistance > distance)
                                graphics.drawEllipse(closestEntity.Location, new Size(diaACD, diaACD), Pens.Orange);
                        }
                    }
                }
            }
        }

        #region Вспомогательные инструменты
        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной ниформации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugMoveToEntity && logConf.Active;
            }
        }

        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey
        {
            get
            {
                if (_key is null)
                {
                    _key = new EntityCacheRecordKey(EntityID, EntityIdType, EntityNameType);
                }
                return _key;
            }
        }
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// Использовать самомодифицирующийся предиката, т.к. предикат передается в <seealso cref="SearchCached.FindClosestEntity(EntityCacheRecordKey, Predicate{Entity})"/>
        /// </summary>        
        private Predicate<Entity> SpecialEntityCheck
        {
            get
            {
                if (_specialEntityCheck is null)
                    _specialEntityCheck = SearchHelper.Construct_EntityAttributePredicate(HealthCheck,
                                                            ReactionRange, ReactionZRange,
                                                            RegionCheck,
                                                            CustomRegionNames);
                return _specialEntityCheck;
            }
        }
        private Predicate<Entity> _specialEntityCheck;

        /// <summary>
        /// Обновление таймера остановки поиска 
        /// </summary>
        private void ResetEntitySearchTimer()
        {
            var entitySearchTime = EntitySearchTime;
            if (entitySearchTime > 0)
            {
                if (entityAbsenceTimer != null)
                    entityAbsenceTimer.ChangeTime(entitySearchTime);
                else entityAbsenceTimer = new Timeout(entitySearchTime);
            }
        }
        /// <summary>
        /// Запуск таймера остановки поиска.
        /// Если тайме запущен, то отсчет продолжается
        /// </summary>
        private void StartEntitySearchTimer()
        {
            var entitySearchTime = EntitySearchTime;
            if (entitySearchTime > 0)
            {
                if (entityAbsenceTimer is null)
                    entityAbsenceTimer = new Timeout(entitySearchTime);
            }
        }

        /// <summary>
        /// Проверка условия отключения боя <see cref="MoveToEntity.IgnoreCombatCondition"/>
        /// </summary>
        /// <returns>Результат проверки <see cref="MoveToEntity.IgnoreCombatCondition"/> либо True, если оно не задано.</returns>
        private bool CheckingIgnoreCombatCondition()
        {
            var check = IgnoreCombatCondition;
            if (check != null)
                return check.IsValid;
            return true;
        }
        #endregion
    }
}
