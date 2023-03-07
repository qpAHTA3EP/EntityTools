//#define DEBUG_TARGET

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Infrastructure;
using Astral.Logic.Classes.Map;
using EntityTools.Editors;
using EntityTools.Editors.TestEditors;
using EntityTools.Enums;
using EntityTools.Quester.Mapper;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Extensions;
using EntityTools.Tools.Navigation;
using EntityTools.Tools.Targeting;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Action = Astral.Quester.Classes.Action;
using Timeout = Astral.Classes.Timeout;

// ReSharper disable InconsistentNaming
namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class MoveToTeammate : Action, INotifyPropertyChanged
    {
        #region Данные 
        private Timeout teammateAbsenceTimer;
        private TeammateSupportTargetProcessor _targetProcessor;
        private float _sqrtDistance;
        private float _sqrtAbortCombatDistance;
        private string _idStr = string.Empty;
        #endregion

        #region General
        [Description("The selection of the supported teammate")]
        [Category("General")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TeammateSupport SupportOptions
        {
            get => supportOptions;
            set
            {
                supportOptions = value;
                _targetProcessor?.Dispose();
                _targetProcessor = new TeammateSupportTargetProcessor(SupportOptions, GetSpecialTeammateCheck());
                NotifyPropertyChanged();
            }
        }
        private TeammateSupport supportOptions = new TeammateSupport();

        [XmlIgnore]
        [Editor(typeof(TargetSelectorTestEditor), typeof(UITypeEditor))]
        [Description("Test the Teammate searching.")]
        [Category("General")]
        public string TestSearch => @"Push button '...' =>";
        #endregion


        #region ManageCombatOptions
        [Description("Distance to the Teammate by which it is necessary to approach.\n" +
                     "Keep in mind that the distance below 5 is too small to display on the Mapper")]
        [Category("Manage Combat Options")]
        [DisplayName("CombatDistance")]

        public float Distance
        {
            get => distance; set
            {
                if (Math.Abs(distance - value) > 0.1f)
                {
                    distance = value;
                    _sqrtDistance = distance;
                    _sqrtDistance *= _sqrtDistance;
                    NotifyPropertyChanged();
                }
            }
        }
        private float distance = 30;

        [Description("Enable '" + nameof(IgnoreCombat) + "' profile value while playing action")]
        [Category("Manage Combat Options")]
        public bool IgnoreCombat
        {
            get => ignoreCombat; set
            {
                if (ignoreCombat != value)
                {
                    ignoreCombat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool ignoreCombat = true;


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

        [Description("The battle is aborted outside '" + nameof(AbortCombatDistance) + "' radius from the Teammate.\n" +
                     "The combat is restored within the '" + nameof(Distance) + "' radius.\n" +
                     "However, this is not performed if the value less than '" + nameof(Distance) + "' or '" + nameof(IgnoreCombat) + "' is False.")]
        [Category("Manage Combat Options")]
        public uint AbortCombatDistance
        {
            get => abortCombatDistance; set
            {
                if (abortCombatDistance != value)
                {
                    abortCombatDistance = value;
                    _sqrtAbortCombatDistance = abortCombatDistance;
                    _sqrtAbortCombatDistance *= _sqrtAbortCombatDistance;
                    NotifyPropertyChanged();
                }
            }
        }
        private uint abortCombatDistance;
        #endregion


        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public CustomRegionCollection CustomRegions
        {
            get => customRegions;
            set
            {
                if (customRegions != value)
                {
                    customRegions = value;
                    _targetProcessor.SpecialCheck = GetSpecialTeammateCheck();
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection customRegions = new CustomRegionCollection();


        #region Interruptions
        [Description("True: Complete an action when the Teammate is closer than 'Distance'\n" +
                     "False: Follow an Teammate regardless of its distance")]
        [Category("Interruptions")]
        public bool StopOnApproached
        {
            get => stopOnApproached; set
            {
                if (stopOnApproached != value)
                {
                    stopOnApproached = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool stopOnApproached;

        [Description("The command is interrupted upon teammate search timer reaching zero (ms).\n" +
                     "Set zero value to infinite search.")]
        [Category("Interruptions")]
        public uint TeammateSearchTime
        {
            get => teammateSearchTime; 
            set
            {
                if (teammateSearchTime != value)
                {
                    teammateSearchTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private uint teammateSearchTime = 10_000; 
        #endregion


        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void InternalResetOnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            switch (propertyName)
            {
                case nameof(SupportOptions):
                    _targetProcessor?.Dispose();
                    _targetProcessor = new TeammateSupportTargetProcessor(SupportOptions, GetSpecialTeammateCheck());
                    break;
                case nameof(SupportOptions.Teammate):
                case nameof(SupportOptions.FoePreference):
                    _targetProcessor.Reset();
                    break;
                case nameof(Distance):
                    _sqrtDistance = Distance;
                    _sqrtDistance *= _sqrtDistance;
                    break;
                case nameof(AbortCombatDistance):
                    _sqrtAbortCombatDistance = AbortCombatDistance;
                    _sqrtAbortCombatDistance *= _sqrtAbortCombatDistance;
                    break;
                case nameof(CustomRegions):
                    _targetProcessor.SpecialCheck = GetSpecialTeammateCheck();
                    break;
            }
        }
        #endregion

        


        [XmlIgnore]
        public override bool NeedToRun
        {
            get
            {
                bool needToRun;
                var teammate = _targetProcessor.GetTeammate();

                if (ExtendedDebugInfo)
                {
                    string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun));

                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Begins"));


                    if (teammate is null)
                    {
                        if (teammateAbsenceTimer?.IsTimedOut == true)
                        {
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Teammate[NULL] and TeammateWaitTime is out"));
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Result 'true'"));

                            return true;
                        }

                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Teammate[NULL]"));
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Result 'False'"));

                        return false;
                    }

                    double sqrtDistance =
                            NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, teammate.Entity.Location);
                    needToRun = sqrtDistance <= _sqrtDistance;

                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", teammate.InternalName,
                            " verification=", needToRun, " {",
                            needToRun ? "Near (" : "Faraway (", Math.Sqrt(sqrtDistance).ToString("N2"), ")}"));

                    if (!needToRun && IgnoreCombat
                        && CheckingIgnoreCombatCondition())
                    {
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, IgnoreCombatMinHP, 5_000);
                        if (AbortCombatDistance > Distance)
                        {
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CheckCombatShouldBeAborted_and_ChangeTarget, ShouldRemoveAbortCombatCondition);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Disable combat and set abort combat condition"));
                        }
                        else ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Disable combat"));
                    }

                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Result '", needToRun, '\''));
                }
                else
                {
                    if (teammateAbsenceTimer?.IsTimedOut == true)
                        return true;

                    double sqrtDistance =
                            NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, teammate.Entity.Location);
                    needToRun = sqrtDistance <= _sqrtDistance;

                    if (!needToRun && IgnoreCombat)
                    {
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, IgnoreCombatMinHP);
                        if (AbortCombatDistance > Distance)
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CheckCombatShouldBeAborted_and_ChangeTarget, ShouldRemoveAbortCombatCondition);
                    }
                }
                if (!needToRun)
                    StartSearchTimer();
                return needToRun;
            }
        }

        public override ActionResult Run()
        {
            ActionResult actionResult;

            if (ExtendedDebugInfo)
            {
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run));


                if (!IntenalConditions)
                {
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(IntenalConditions)} failed. ActionResult={ActionResult.Fail}.");

                    if (IgnoreCombat)
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                    return ActionResult.Fail;
                }

                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Begins"));

                var teammate = _targetProcessor.GetTeammate();
                if (teammate is null)
                {
                    actionResult = ActionResult.Running;
                    if (teammateAbsenceTimer?.IsTimedOut == true)
                    {
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Teammate[NULL] and TeammateWaitTime is out. Stop"));
                        actionResult = ActionResult.Fail;
                    }
                }
                else
                {
                    Attack_Target(_targetProcessor.GetTarget());

                    actionResult = StopOnApproached
                        ? ActionResult.Completed
                        : ActionResult.Running;
                }

                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : ActionResult = ", actionResult));
            }
            else
            {

                if (!IntenalConditions)
                    return ActionResult.Fail;

                var teammate = _targetProcessor.GetTeammate();
                if (teammate is null)
                {

                    actionResult = teammateAbsenceTimer?.IsTimedOut == true
                        ? ActionResult.Fail
                        : ActionResult.Running;
                }
                else
                {
                    Attack_Target(_targetProcessor.GetTarget());

                    actionResult = StopOnApproached
                        ? ActionResult.Completed
                        : ActionResult.Running;
                }
            }
            ResetSearchTimer();
            return actionResult;
        }

        #region обработка AbortCombatDistance
        /// <summary>
        /// Нападение на сущность <paramref name="target"/> в зависимости от настроек команды
        /// </summary>
        private void Attack_Target(Entity target)
        {
            if (ExtendedDebugInfo)
            {
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Attack_Target));
                if (target is null)
                {
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity is NULL. Break"));
                    return;
                }

                if (target.RelationToPlayer == EntityRelation.Foe
                    && target.IsLineOfSight()
                    && !target.DoNotDraw
                    && !target.IsUntargetable)
                {
                    // target враждебно и может быть атаковано
                    string entityStr = target.GetDebugString(EntityNameType.InternalName, SupportOptions.foePreference.ToString(), EntityDetail.Pointer | EntityDetail.RelationToPlayer);

                    //Astral.Quester.API.IgnoreCombat = false;
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false, IgnoreCombatMinHP);

                    if (AbortCombatDistance > Distance)
                    {
                        // устанавливаем прерывание боя
                        AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CheckCombatShouldBeAborted_and_ChangeTarget, ShouldRemoveAbortCombatCondition);
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Set abort combat condition, engage combat and attack ", entityStr, " at the distance ", target.CombatDistance3.ToString("N2")));
                    }
                    else ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat and attack ", entityStr, " at the distance ", target.CombatDistance3.ToString("N2")));

                    // атакуем target
                    Astral.Logic.NW.Combats.CombatUnit(target);
                }
            }
            else
            {
                if (target is null)
                    return;

                if (target.RelationToPlayer == EntityRelation.Foe
                    && target.IsLineOfSight()
                    && !target.DoNotDraw
                    && !target.IsUntargetable)
                {
                    // target враждебно и может быть атаковано

                    //Astral.Quester.API.IgnoreCombat = false;
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false, IgnoreCombatMinHP);

                    if (AbortCombatDistance > Distance)
                    {
                        // устанавливаем прерывание боя
                        AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CheckCombatShouldBeAborted_and_ChangeTarget, ShouldRemoveAbortCombatCondition);
                    }

                    // атакуем target
                    Astral.Logic.NW.Combats.CombatUnit(target);
                }
            }
        }

        /// <summary>
        /// Делегат выполняющий два действия:
        /// 1) определяющий необходимость прерывания боя путем сопоставления <see cref="MoveToTeammate.AbortCombatDistance"/> с расстоянием между игроком и членом группы, заданным <see cref="MoveToTeammate.SupportOptions"/>
        /// Бой прерывается при удалении персонажа от указанного члена группы путем возврата true.
        /// 2) Заменяет <param name="combatTarget"/> на цель, заданную <see cref="MoveToTeammate.SupportOptions"/>
        /// </summary>
        /// <returns>true, если нужно прервать бой</returns>
        private bool CheckCombatShouldBeAborted_and_ChangeTarget(ref Entity combatTarget)
        {
            var teammate = _targetProcessor.GetTeammate();
            if (teammate is null)
            {
                // член группы не идентифицируется - бой продолжается
                return false;
            }

            if (ExtendedDebugInfo)
            {
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(CheckCombatShouldBeAborted_and_ChangeTarget));

                var player = EntityManager.LocalPlayer;

                // Бой не должен прерываться, если  HP < IgnoreCombatMinHP
                var healthPercent = player.Character.AttribsBasic.HealthPercent;
                var ignoreCombatMinHP = AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP;
                if (healthPercent < ignoreCombatMinHP)
                {
                    ETLogger.WriteLine(LogType.Debug,
                        string.Concat(currentMethodName, ": Player health (", healthPercent, "%) below IgnoreCombatMinHP (", ignoreCombatMinHP, "%). Continue..."));
                    return false;
                }

                var sqrtDist = NavigationHelper.SquareDistance3D(player.Location, teammate.Entity.Location);

                if (sqrtDist <= _sqrtAbortCombatDistance)
                {
                    ETLogger.WriteLine(LogType.Debug,
                        string.Concat(currentMethodName, ": Player withing ", nameof(AbortCombatDistance),
                            " (", teammate.Entity.CombatDistance3.ToString("N2"), " to ", teammate.InternalName, "). Continue..."));

                    var target = _targetProcessor.GetTarget();
                    if (target != null)
                    {
                        ETLogger.WriteLine(LogType.Debug,
                            string.Concat(currentMethodName, ": ChangeTarget to ", target.GetDebugString(EntityNameType.InternalName, SupportOptions.foePreference.ToString(), EntityDetail.Pointer)));
                        combatTarget = target;
                    }

                    return false;
                }

                ETLogger.WriteLine(LogType.Debug,
                    string.Concat(currentMethodName, ": Player outside ", nameof(AbortCombatDistance),
                        " (", teammate.Entity.CombatDistance3.ToString("N2"), " to ", teammate.InternalName, "). Combat have to be aborted"));
                return true;
            }
            else
            {
                // проверка необходимости прерывания боя без вывода отладочной информации
                var player = EntityManager.LocalPlayer;
                if (player.Character.AttribsBasic.HealthPercent <
                      AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP) return false;

                var sqrtDist = NavigationHelper.SquareDistance3D(player.Location, teammate.Entity.Location);

                if (sqrtDist <= _sqrtAbortCombatDistance)
                {
                    var target = _targetProcessor.GetTarget();
                    if (target != null)
                        combatTarget = target;

                    return false;
                }

                return true;
            }
        }
        private bool ShouldRemoveAbortCombatCondition()
        {
            return Completed;
        }
        #endregion





        [XmlIgnore]
        [Browsable(false)]
        public override string ActionLabel => $"{GetType().Name} : {SupportOptions.teammate}";

        public override string InternalDisplayName => string.Empty;

        [XmlIgnore]
        [Category("Engine")]
        protected override bool IntenalConditions => TeammateSearchTime == 0
                                          || teammateAbsenceTimer is null
                                          || !teammateAbsenceTimer.IsTimedOut;

        [XmlIgnore]
        [Category("Engine")]
        protected override ActionValidity InternalValidity => new ActionValidity();

        [XmlIgnore]
        [Browsable(false)]
        public override bool UseHotSpots => false;

        [XmlIgnore]
        [Category("Engine")]
        protected override Vector3 InternalDestination
        {
            get
            {
                var teammate = _targetProcessor.GetTeammate();
                if (teammate != null)
                {
                    var tmEntity = teammate.Entity;
                    if (tmEntity.IsValid)
                    {
                        var location = tmEntity.Location;
                        var playerLocation = EntityManager.LocalPlayer.Location;
                        if (NavigationHelper.SquareDistance3D(playerLocation, location) >= _sqrtDistance)
                            return location.Clone();
                        return playerLocation.Clone();
                    }
                }
                return Vector3.Empty;
            }
        }

        public override void InternalReset()
        {
            _targetProcessor.Reset();

            ResetSearchTimer();
            AstralAccessors.Logic.NW.Combats.RemoveAbortCombatCondition();

            if (ExtendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(_idStr, '.', nameof(InternalReset)));
        }

        public override void GatherInfos()
        {
#if false
            if (HotSpots.Count == 0)
                HotSpots.Add(EntityManager.LocalPlayer.Location.Clone());

            _label = string.Empty; 
#endif
        }

        public override void OnMapDraw(GraphicsNW graphics)
        {
            var teammate = _targetProcessor.GetTeammate();
            if (teammate is null || !teammate.IsValid)
                return;
            var emEntity = teammate.Entity;
            if (!emEntity.IsValid)
                return;

            var pos = emEntity.Location;
            if (!pos.IsValid)
                return;

            if (graphics is MapperGraphics mapGraphics)
            {
                float x = pos.X,
                    y = pos.Y,
                    diaD = distance * 2,
                    diaACD = abortCombatDistance * 2;

                mapGraphics.FillRhombCentered(Brushes.Yellow, pos, 16, 16);
                if (distance > 11)
                {
                    mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaD, true);
                    if (abortCombatDistance > distance)
                        mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaACD, true);
                }
            }
            else
            {
                float x = pos.X,
                      y = pos.Y;
                List<Vector3> coords = new List<Vector3> {
                    new Vector3(x, y - 5, 0),
                    new Vector3(x - 4.33f, y + 2.5f, 0),
                    new Vector3(x + 4.33f, y + 2.5f, 0)
                };
                graphics.drawFillPolygon(coords, Brushes.Yellow);

                int diaD = (int)(distance * 2.0f * graphics.Zoom);
                int diaACD = (int)(abortCombatDistance * 2.0f * graphics.Zoom);
                if (distance > 5)
                {
                    graphics.drawEllipse(pos, new Size(diaD, diaD), Pens.Yellow);

                    if (abortCombatDistance > distance)
                    {
                        graphics.drawEllipse(pos, new Size(diaACD, diaACD), Pens.Orange);
                    }
                }
            }
        }

        #region Вспомогательные инструменты
        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной ниформации
        /// </summary>
        [XmlIgnore]
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugMoveToTeammate && logConf.Active;
            }
        }

        /// <summary>
        /// Обновление таймера остановки поиска 
        /// </summary>
        private void ResetSearchTimer()
        {
            int time = (int)TeammateSearchTime;
            if (time > 0)
            {
                if (teammateAbsenceTimer != null)
                    teammateAbsenceTimer.ChangeTime(time);
                else teammateAbsenceTimer = new Timeout(time);
            }
        }
        /// <summary>
        /// Запуск таймера остановки поиска.
        /// Если тайме запущен, то отсчет продолжается
        /// </summary>
        private void StartSearchTimer()
        {
            var searchTime = (int)TeammateSearchTime;
            if (searchTime > 0)
            {
                if (teammateAbsenceTimer is null)
                    teammateAbsenceTimer = new Timeout(searchTime);
            }
        }
        private Predicate<Entity> GetSpecialTeammateCheck()
        {
            Predicate<Entity> specialCheck = null;

            var crSet = CustomRegions;
            if (crSet.Count > 0)
            {
                specialCheck = crSet.Within;
            }

            return specialCheck;
        }

        /// <summary>
        /// Проверка условия отключения боя <see cref="MoveToTeammate.IgnoreCombatCondition"/>
        /// </summary>
        /// <returns>Результат проверки <see cref="MoveToTeammate.IgnoreCombatCondition"/> либо True, если оно не задано.</returns>
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
