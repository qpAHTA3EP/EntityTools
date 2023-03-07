#define DEBUG_INTERACTENTITIES
//#define PROFILING

using Infrastructure;
using AStar;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.UIEditors;
using EntityTools.Forms;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Entities;
using EntityTools.Tools.Extensions;
using EntityTools.Tools.Navigation;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;
// ReSharper disable InconsistentNaming

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class InteractEntities : Action, INotifyPropertyChanged, IEntityDescriptor
    {
#if DEBUG && PROFILING
        public static int RunCount = 0;
        public static void ResetWatch()
        {
            RunCount = 0;
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntities::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (RunCount > 0)
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntities: RunCount: {RunCount}");
        }
#endif

        #region Entity
        [Description("ID of the Entity for the search")]
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
                    NotifyPropertyChanged();
                }
            }
        }
        private string _entityId = string.Empty;

        [Description("Type of the EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType
        {
            get => _entityIdType;
            set
            {
                if (_entityIdType != value)
                {
                    _entityIdType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType
        {
            get => _entityNameType;
            set
            {
                if (_entityNameType != value)
                {
                    _entityNameType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

        [Description("A subset of entities that are searched for a target:\n" +
                     "Contacts: Only interactable Entities\n" +
                     "Complete: All possible Entities")]
        [Category("Entity")]
        public EntitySetType EntitySetType
        {
            get => _entitySetType; set
            {
                if (_entitySetType != value)
                {
                    _entitySetType = value;
                    NotifyPropertyChanged();
                }
            }

        }
        private EntitySetType _entitySetType = EntitySetType.Contacts;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the entity searching.")]
        [Category("Entity")]
        public string TestSearch => @"Push button '...' =>";
        #endregion


        #region EntitySearchOptions
        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity Search Options")]
        public bool HealthCheck
        {
            get => _healthCheck; 
            set
            {
                if (_healthCheck != value)
                {
                    _healthCheck = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _healthCheck = true;

        [Description("True: Do not change the target Entity while it is alive or until the Bot within " + nameof(InteractDistance) + " of it.\n" +
                    "False: Constantly scan an area and target the nearest Entity.")]
        [Category("Entity Search Options")]
        public bool HoldTargetEntity
        {
            get => _holdTargetEntity; 
            set
            {
                if (_holdTargetEntity != value)
                {
                    _holdTargetEntity = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _holdTargetEntity = true;

        [Description("Check if Entity is moving:\n" +
            "True: Only standing Entities are detected\n" +
            "False: Both moving and stationary Entities are detected")]
        [Category("Entity Search Options")]
        public bool SkipMoving
        {
            get => _skipMoving; 
            set
            {
                if (_skipMoving != value)
                {
                    _skipMoving = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _skipMoving;
        #endregion


        #region SearchArea
        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected;\n" +
            "False: Entity's Region does not checked during search.")]
        [Category("Search Area")]
        public bool RegionCheck
        {
            get => _regionCheck; 
            set
            {
                if (_regionCheck != value)
                {
                    _regionCheck = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _regionCheck;

        [Description("The maximum distance from the character within which the Entity is searched.\n" +
                     "The value equals 0(zero) disables distance checking.")]
        [Category("Search Area")]
        public float ReactionRange
        {
            get => _reactionRange; 
            set
            {
                if (Math.Abs(_reactionRange - value) > 0.1)
                {
                    _reactionRange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionRange = 150;

        [Description("The maximum Z-coordiante difference with Player within which the Entity is searched.\n" +
                     "The default value is 0, which disables Z-coordiante checking.")]
        [Category("Search Area")]
        public float ReactionZRange
        {
            get => _reactionZRange; 
            set
            {
                if (Math.Abs(_reactionZRange - value) > 0.1f)
                {
                    _reactionZRange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionZRange;

        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Search Area")]
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
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();

        [Description("Checking the Player is in the area, defined by 'CustomRegions'")]
        [Category("Search Area")]
        public bool CustomRegionsPlayerCheck
        {
            get => customRegionsPlayerCheck; 
            set
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


        #region Manage Combat Options
        [Description("Ignore the enemies while approaching the target Entity.\n" +
                     "The minimum value is 5.")]
        [Category("Manage Combat Options")]
        public float CombatDistance
        {
            get => _combatDistance; 
            set
            {
                if (value < 5)
                    value = 5;

                if (Math.Abs(_combatDistance - value) > 0.1)
                {
                    _combatDistance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _combatDistance = 30;

        [Description("Ignore the enemies while approaching the " + nameof(Entity) + ".")]
        [Category("Manage Combat Options")]
        public bool IgnoreCombat
        {
            get => _ignoreCombat;
            set
            {
                if (_ignoreCombat == value) 
                    return;
                _ignoreCombat = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ignoreCombat;

        [Description("Sets the ucc option '" + nameof(IgnoreCombatMinHP) + "' when disabling combat mode.\n" +
                     "Options ignored if the value is -1.")]
        [Category("Manage Combat Options")]
        public int IgnoreCombatMinHP
        {
            get => _ignoreCombatMinHp; 
            set
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
                     "The combat is restored within the '" + nameof(InteractDistance) + "' radius.\n" +
                     "However, this is not performed if the value less than '" + nameof(InteractDistance) + "' or '" + nameof(IgnoreCombat) + "' is False.")]
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
        #endregion


        #region Interaction
        [Category("Interaction")]
        [Description("Only one interaction with Entity is possible in 'InteractingTimeout' period")]
        public bool InteractOnce
        {
            get => _interactOnce; set
            {
                if (_interactOnce != value)
                {
                    _interactOnce = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _interactOnce;

        [Category("Interaction")]
        [Description("Interaction timeout (sec) if InteractitOnce flag is set.")]
        [DisplayName("InteractionTimeout")]
        public int InteractingTimeout
        {
            get => _interactingTimeout; 
            set
            {
                if (value < 0)
                    value = 0;

                if (_interactingTimeout != value)
                {
                    _interactingTimeout = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _interactingTimeout = 60;

        [Category("Interaction")]
        [Description("Time to interact (ms)")]
        public int InteractTime
        {
            get => _interactTime;
            set
            {
                if (_interactTime != value)
                {
                    _interactTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _interactTime = 2000;

        [Category("Interaction")]
        [Description("The distance to the Entity within which the interaction is possible.\n" +
                     "Minimum value is 5.")]
        public int InteractDistance
        {
            get => _interactDistance;
            set
            {
                if (value < 5)
                    value = 5;

                if (_interactDistance != value)
                {
                    _interactDistance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _interactDistance = 5;

        [Category("Interaction")]
        [Description("Answers in dialog while interact with Entity")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        public List<string> Dialogs
        {
            get => _dialogs; set
            {
                if (_dialogs != value)
                {
                    _dialogs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private List<string> _dialogs = new List<string>(); 
        #endregion


        #region Interruptions

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
                    NotifyPropertyChanged();
                }
            }
        }
        private int _entitySearchTime;
        #endregion

        [Description("Reset current HotSpot after interaction and move to the closest one")]
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


        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        #region Данные
        private readonly TempBlackList<uint> blackList = new TempBlackList<uint>();
        private Entity targetEntity;
        private Entity closestEntity;
        private readonly Astral.Classes.Timeout internalCacheTimer = new Astral.Classes.Timeout(0);
        private Astral.Classes.Timeout entityAbsenceTimer;
        private string _label = string.Empty;
        private string _idStr = string.Empty;
        #endregion

        


        #region Интерфейс Action
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
                    // targetEntity не валидный - сбрасываем
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
                        if (AbortCombatDistance > InteractDistance)
                        {
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: Engage combat and set abort combat condition");
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
                Interact_Entity(closestEntity);
            else if (entityKey.Validate(targetEntity))
                Interact_Entity(targetEntity);

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult={ActionResult.Running}");

            //  таймера остановки поиска
            ResetEntitySearchTimer();

            if (ResetCurrentHotSpot)
                CurrentHotSpotIndex = -1;
            return ActionResult.Running;
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
                bool distanceResult = distance <= InteractDistance;
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
        /// Взаимодействие с сущностью <paramref name="entity"/>
        /// </summary>
        private void Interact_Entity(Entity entity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = string.Empty,
                   entityDebugId = string.Empty;
            if (extendedDebugInfo)
            {
                currentMethodName = $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Interact_Entity)}";
                entityDebugId = entity.GetDebugString(EntityNameType, "Entity", EntityDetail.Pointer);
            }

            if (entity is null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity is NULL. Break");
                return;
            }

            var player = EntityManager.LocalPlayer.Player;

            _moved = false;
            _combat = false;
            try
            {
#if DEBUG && PROFILING
                RunCount++;
#endif
                var interactDistance = Math.Max(InteractDistance, 5);
                var ignoreCombat = IgnoreCombat;

                if (entity.Location.Distance3DFromPlayer < interactDistance)
                {
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Approaching {entityDebugId} for interaction");

                    //TODO: Заменить EntityForInteraction собственной функцией перемещения и взаимодействия
                    if (Approach.EntityForInteraction(targetEntity, BreakInteraction)
                        && targetEntity.SmartInteract(interactDistance, InteractTime))
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Interaction with {entityDebugId} succeeded");

                        var dialogs = Dialogs;
                        if (dialogs.Count > 0)
                        {
                            Astral.Classes.Timeout timer = new Astral.Classes.Timeout(5000);
                            while (player.InteractInfo.ContactDialog.Options.Count == 0)
                            {
                                if (timer.IsTimedOut)
                                {
                                    return;
                                }
                                Thread.Sleep(100);
                            }
                            Thread.Sleep(500);
                            foreach (string key in dialogs)
                            {
                                player.InteractInfo.ContactDialog.SelectOptionByKey(key);
                                Thread.Sleep(1000);
                            }
                        }
                        player.InteractInfo.ContactDialog.Close();
                        return;
                    }
                    else if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Interaction failed");
                    if (ignoreCombat && targetEntity.Location.Distance3DFromPlayer <= CombatDistance)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Engage combat");

                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);

                        if (AbortCombatDistance > InteractDistance)
                        {
                            // устанавливаем прерывание боя
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: Set abort combat condition and engage combat at the distance {entity.CombatDistance3:N2} from {entityDebugId}");
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Engage combat at the distance {entity.CombatDistance3:N2} from {entityDebugId}");

                        return;
                    }
                    if (_combat)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Player in combat...");

                        return;
                    }
                    if (_moved && SkipMoving)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {entityDebugId} moved, skip...");
                        else ETLogger.WriteLine("Entity moved, skip...", true);
                    }
                }
            }
            finally
            {
                // В случае неудачного интеракта из-за боя InteractOnce не должно помещать в черный список до повторной попытки
                if (!_combat && InteractOnce || SkipMoving && _moved)
                {
                    PushToBlackList(targetEntity);
                    targetEntity = null;
                }
                if (ResetCurrentHotSpot)
                    CurrentHotSpotIndex = -1;
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
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(CombatShouldBeAborted));

                // Бой не должен прерываться, если  HP < IgnoreCombatMinHP
                var healthPercent = EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent;
                var ignoreCombatMinHP = AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP;
                if (healthPercent < ignoreCombatMinHP)
                {
                    ETLogger.WriteLine(LogType.Debug,
                        string.Concat(currentMethodName, ": Player health (", healthPercent, "%) below IgnoreCombatMinHP (", ignoreCombatMinHP, "%). Continue..."));
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
                                string.Concat(currentMethodName, ": Player withing ", nameof(AbortCombatDistance),
                                    " (", dist.ToString("N2"), " to ", entityStr, "). Continue..."));
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
                            string.Concat(currentMethodName, ": ", entityStr, " is dead. Combat have to be aborted"));
                        return true;
                    }

                    double dist = targetEntity.Location.Distance3DFromPlayer;
                    if (dist >= abortCombatDistance)
                    {
                        // расстояние до targetEntity больше дистанции прерывания боя => прерываем бой
                        ETLogger.WriteLine(LogType.Debug,
                            string.Concat(currentMethodName, ": Player outside ", nameof(AbortCombatDistance),
                                " (", dist.ToString("N2"), " to ", entityStr, "). Combat have to be aborted"));
                        return true;
                    }

                    // targetEntity - живое и расстояние до него меньше дистанции прерывания боя => ghjljk;ftv ,jq
                    ETLogger.WriteLine(LogType.Debug,
                        string.Concat(currentMethodName, ": Player withing ", nameof(AbortCombatDistance),
                            " (", dist.ToString("N2"), " to ", entityStr, "). Continue..."));
                    return false;
                }

                // targetEntity и closestEntity невалидны => прерываем бой
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity[INVALID]. Combat have to be aborted"));
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

                if (entityAbsenceTimer?.IsTimedOut == true)
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
                    if (targetEntity.Location.Distance3DFromPlayer > InteractDistance)
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

            _key = null;
            _label = string.Empty;
            _specialCheck = null;

            _idStr = $"{GetType().Name}[{ActionID}]";

            internalCacheTimer.ChangeTime(0);
            entityAbsenceTimer = null;

            ResetEntitySearchTimer();
        }

        public override void GatherInfos()
        {
            var player = EntityManager.LocalPlayer;
            if (player.IsValid && !player.IsLoading)
            {
                if (HotSpots.Count == 0)
                {
                    var pos = player.Location.Clone();
                    Node node = AstralAccessors.Quester.Core.CurrentProfile.CurrentMesh.ClosestNode(pos.X, pos.Y, pos.Z, out double distance, false);
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
            if (EntityKey.Validate(targetEntity))
                graphics.drawFillEllipse(targetEntity.Location, new Size(10, 10), Brushes.Beige);
        } 
        #endregion




        #region Вспомогательный инструменты
        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugInteractEntities && logConf.Active;
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
                    _key = new EntityCacheRecordKey(EntityID, EntityIdType, EntityNameType, EntitySetType);
                return _key;
            }
        }
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в черном списке <see cref="blackList"/>,
        /// а также в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// </summary>        
        private Predicate<Entity> SpecialEntityCheck
        {
            get
            {
                if (_specialCheck is null)
                {
                    var customRegionNames = CustomRegionNames;
                    if (customRegionNames.Count > 0)
                        _specialCheck = SearchHelper.Construct_EntityAttributePredicate(HealthCheck,
                                                                ReactionRange, ReactionZRange,
                                                                RegionCheck,
                                                                customRegionNames,
                                                                false,
                                                                CheckBlacklist);
                    else _specialCheck = CheckBlacklist;
                }
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;

        private bool CheckBlacklistAndCustomRegions(Entity e)
        {
            return !blackList.Contains(e.ContainerId) && CustomRegionNames.Within(e);
        }
        private bool CheckBlacklist(Entity e)
        {
            return !blackList.Contains(e.ContainerId);
        }

        private void PushToBlackList(Entity ent)
        {
            if (ent != null && ent.IsValid)
            {
                blackList.Add(ent.ContainerId, InteractingTimeout);
                if(ExtendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr}::PushToBlackList: Entity[{ent.ContainerId:X8}]");
            }
        }

        /// <summary>
        /// Метод, используемый для прерывания взаимодействия с <see cref="targetEntity"/>
        /// </summary>
        /// <returns></returns>
        private Approach.BreakInfos BreakInteraction()
        {
            if (Attackers.InCombat)
            {
                _combat = true;
                return Approach.BreakInfos.ApproachFail;
            }
            if (SkipMoving && targetEntity.Location.Distance3D(_initialPos) > 3.0)
            {
                _moved = true;
                return Approach.BreakInfos.ApproachFail;
            }
            return Approach.BreakInfos.Continue;
        }
        private Vector3 _initialPos = Vector3.Empty;
        private bool _combat;
        private bool _moved;

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
                else entityAbsenceTimer = new Astral.Classes.Timeout(entitySearchTime);
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
                    entityAbsenceTimer = new Astral.Classes.Timeout(entitySearchTime);
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
