//#define DEBUG_TARGET

using AcTp0Tools;
using Astral.Classes;
using Astral.Logic.Classes.Map;
using EntityCore.Enums;
using EntityCore.Extensions;
using EntityCore.Tools.Targeting;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Patches.Mapper;
using EntityTools.Quester.Actions;
using EntityTools.Tools.Navigation;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using EntityCore.Entities;
using static Astral.Quester.Classes.Action;
// ReSharper disable InconsistentNaming

namespace EntityCore.Quester.Action
{
    public class MoveToTeammateEngine : IQuesterActionEngine
    {
        /// <summary>
        /// ссылка на команду, для которой предоставляется функционал ядра
        /// </summary>
        private MoveToTeammate _action;

        private Timeout _teammateAbsenceTimer;
        private TeammateSupportTargetProcessor _targetProcessor;
        private float _sqrtDistance;
        private float _sqrtAbortCombatDistance;

        #region Данные 
        private string _idStr = string.Empty;
        private string _label = string.Empty;
        #endregion

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Target")]
        public object Target => new SimpleEntityWrapper(_targetProcessor.GetTeammate());

#if DEVELOPER && DEBUG_TARGET
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Team")]
        public object Tank
        {
            get
            {
                var check = GetSpecialTeammateCheck();
                return new SimpleEntityWrapper(check is null 
                    ? PlayerTeamHelper.GetTank()
                    : PlayerTeamHelper.GetTank(check));
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Team")]
        public object Healer
        {
            get
            {
                var check = GetSpecialTeammateCheck();
                return new SimpleEntityWrapper(check is null
                    ? PlayerTeamHelper.GetHealer()
                    : PlayerTeamHelper.GetHealer(check));
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Team")]
        public object Sturdiest
        {
            get
            {
                var check = GetSpecialTeammateCheck();
                return new SimpleEntityWrapper(check is null
                    ? PlayerTeamHelper.GetSturdiest()
                    : PlayerTeamHelper.GetSturdiest(check));
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Team")]
        public object SturdiestDD
        {
            get
            {
                var check = GetSpecialTeammateCheck();
                return new SimpleEntityWrapper(check is null
                    ? PlayerTeamHelper.GetSturdiestDamageDealer()
                    : PlayerTeamHelper.GetSturdiestDamageDealer(check));
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Team")]
        public object Weakest
        {
            get
            {
                var check = GetSpecialTeammateCheck();
                return new SimpleEntityWrapper(check is null
                    ? PlayerTeamHelper.GetWeakest()
                    : PlayerTeamHelper.GetWeakest(check));
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Team")]
        public object WeakestDD
        {
            get
            {
                var check = GetSpecialTeammateCheck();
                return new SimpleEntityWrapper(check is null
                    ? PlayerTeamHelper.GetWeakestDamageDealer()
                    : PlayerTeamHelper.GetWeakestDamageDealer(check));
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Team")]
        public object MostInjured
        {
            get
            {
                var check = GetSpecialTeammateCheck();
                return new SimpleEntityWrapper(check is null
                    ? PlayerTeamHelper.GetMostInjured()
                    : PlayerTeamHelper.GetMostInjured(check));
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Team")]
        public object MostInjuredDD
        {
            get
            {
                var check = GetSpecialTeammateCheck();
                return new SimpleEntityWrapper(check is null
                    ? PlayerTeamHelper.GetMostInjuredDamageDealer()
                    : PlayerTeamHelper.GetMostInjuredDamageDealer(check));
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Engine")]
        public TargetProcessor TargetProcessor => _targetProcessor;
#endif

        internal MoveToTeammateEngine(MoveToTeammate m2t)
        {
            InternalRebase(m2t);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized");
        }
        ~MoveToTeammateEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_action != null)
            {
                _action.Unbind();
                _action = null; 
            }
            _targetProcessor?.Dispose();
        }

        public bool Rebase(Astral.Quester.Classes.Action action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, this._action))
                return true;
            if (action is MoveToTeammate m2t)
            {
                if (InternalRebase(m2t))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }
            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be cast to '" + nameof(MoveToTeammate) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(MoveToTeammate m2t)
        {
            _action?.Unbind();
            _targetProcessor?.Dispose();

            _action = m2t;
            _idStr = string.Concat(_action.GetType().Name, '[', _action.ActionID, ']');

            _targetProcessor = new TeammateSupportTargetProcessor(_action.SupportOptions, GetSpecialTeammateCheck());

            _sqrtDistance = _action.Distance;
            _sqrtDistance *= _sqrtDistance;
            _sqrtAbortCombatDistance = _action.AbortCombatDistance;
            _sqrtAbortCombatDistance *= _sqrtAbortCombatDistance;

            _teammateAbsenceTimer = null;
            _label = string.Empty;

            _action.Bind(this);

            return true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender as Astral.Quester.Classes.Action, e.PropertyName);
        } 
        public void OnPropertyChanged(Astral.Quester.Classes.Action sender, string propertyName)
        {
            if (ReferenceEquals(sender, _action))
            {
                switch (propertyName)
                {
                    case nameof(_action.SupportOptions):
                        _targetProcessor?.Dispose();
                        _targetProcessor = new TeammateSupportTargetProcessor(_action.SupportOptions, GetSpecialTeammateCheck());
                        _label = string.Empty;
                        break;
                    case nameof(_action.SupportOptions.Teammate):
                    case nameof(_action.SupportOptions.FoePreference):
                        _targetProcessor.Reset();
                        _label = string.Empty;
                        break;
                    case nameof(_action.Distance):
                        _sqrtDistance = _action.Distance;
                        _sqrtDistance *= _sqrtDistance;
                        break;
                    case nameof(_action.AbortCombatDistance):
                        _sqrtAbortCombatDistance = _action.AbortCombatDistance;
                        _sqrtAbortCombatDistance *= _sqrtAbortCombatDistance;
                        break;
                    case nameof(_action.CustomRegions):
                        _targetProcessor.SpecialCheck = GetSpecialTeammateCheck();
                        break;
                }
            }
        }

#if MONITORING
        [Category("Engine")] 
#endif
        public bool NeedToRun
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
                        var teammateSearchTime = _action.TeammateSearchTime;
                        if (teammateSearchTime > 0)
                        {
                            if (_teammateAbsenceTimer is null)
                                _teammateAbsenceTimer = new Timeout((int)teammateSearchTime);
                            else if (_teammateAbsenceTimer.IsTimedOut)
                            {
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Teammate[NULL] and TeammateWaitTime is out"));
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Result 'true'"));

                                return true;
                            }
                        }

                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Teammate[NULL]"));
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Result 'False'"));

                        return false;
                    }
                    _teammateAbsenceTimer = null;

                    double sqrtDistance =
                            NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, teammate.Entity.Location);
                    needToRun = sqrtDistance <= _sqrtDistance;

                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", teammate.InternalName,
                            " verification=", needToRun, " {",
                            needToRun ? "Near (" : "Faraway (", Math.Sqrt(sqrtDistance).ToString("N2"), ")}"));

                    if (!needToRun && _action.IgnoreCombat
                        && CheckingIgnoreCombatCondition())
                    {
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, _action.IgnoreCombatMinHP, 5_000);
                        if (_action.AbortCombatDistance > _action.Distance)
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
                    if (teammate is null)
                    {
                        var teammateSearchTime = _action.TeammateSearchTime;
                        if (teammateSearchTime > 0)
                        {
                            if (_teammateAbsenceTimer is null)
                                _teammateAbsenceTimer = new Timeout((int)teammateSearchTime);
                            else if (_teammateAbsenceTimer.IsTimedOut)
                                return true;
                        }

                        return false;
                    }
                    _teammateAbsenceTimer = null;


                    double sqrtDistance =
                            NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, teammate.Entity.Location);
                    needToRun = sqrtDistance <= _sqrtDistance;

                    if (!needToRun && _action.IgnoreCombat)
                    {
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, _action.IgnoreCombatMinHP);
                        if (_action.AbortCombatDistance > _action.Distance)
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CheckCombatShouldBeAborted_and_ChangeTarget, ShouldRemoveAbortCombatCondition);
                    }
                }
                return needToRun;
            }
        }

        public ActionResult Run()
        {
            ActionResult actionResult;

            if (ExtendedDebugInfo)
            {
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run));


                if (!InternalConditions)
                {
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(InternalConditions)} failed. ActionResult={ActionResult.Fail}.");


                    if (_action.IgnoreCombat)
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                    return ActionResult.Fail;
                }

                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Begins"));

                var teammate = _targetProcessor.GetTeammate();
                if (teammate is null)
                {
                    actionResult = ActionResult.Running;

                    var teammateSearchTime = _action.TeammateSearchTime;
                    if (teammateSearchTime > 0)
                    {
                        if (_teammateAbsenceTimer is null)
                        {
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Teammate[NULL]. Activate TeammateWaitTimer and continue..."));
                            _teammateAbsenceTimer = new Timeout((int)teammateSearchTime);
                        }
                        if (_teammateAbsenceTimer.IsTimedOut)
                        {
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : Teammate[NULL] and TeammateWaitTime is out. Stop"));
                            actionResult = ActionResult.Fail;
                        }
                    }
                }
                else
                {
                    Attack_Target(_targetProcessor.GetTarget());

                    actionResult = _action.StopOnApproached
                        ? ActionResult.Completed
                        : ActionResult.Running; 
                }

                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " : ActionResult = ", actionResult));
            }
            else
            {

                if (!InternalConditions)
                    return ActionResult.Fail;

                var teammate = _targetProcessor.GetTeammate();
                if (teammate is null)
                {
                    actionResult = ActionResult.Running;
                    var teammateSearchTime = _action.TeammateSearchTime;
                    if (teammateSearchTime > 0)
                    {
                        if (_teammateAbsenceTimer is null)
                            _teammateAbsenceTimer = new Timeout((int)teammateSearchTime);
                        actionResult = _teammateAbsenceTimer.IsTimedOut
                            ? ActionResult.Completed
                            : ActionResult.Running; 
                    }
                }
                else
                {
                    Attack_Target(_targetProcessor.GetTarget());

                    actionResult = _action.StopOnApproached
                        ? ActionResult.Completed
                        : ActionResult.Running;
                }
            }
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
                    string entityStr = target.GetDebugString(EntityNameType.InternalName, _action.SupportOptions.foePreference.ToString(), EntityDetail.Pointer | EntityDetail.RelationToPlayer);

                    //Astral.Quester.API.IgnoreCombat = false;
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false, _action.IgnoreCombatMinHP);

                    if (_action.AbortCombatDistance > _action.Distance)
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
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false, _action.IgnoreCombatMinHP);

                    if (_action.AbortCombatDistance > _action.Distance)
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
                        string.Concat(currentMethodName, ": Player withing ", nameof(_action.AbortCombatDistance),
                            " (", teammate.Entity.CombatDistance3.ToString("N2"), " to ", teammate.InternalName, "). Continue..."));

                    var target = _targetProcessor.GetTarget();
                    if (target != null)
                    {
                        ETLogger.WriteLine(LogType.Debug,
                            string.Concat(currentMethodName, ": ChangeTarget to ", target.GetDebugString(EntityNameType.InternalName, _action.SupportOptions.foePreference.ToString(), EntityDetail.Pointer)));
                        combatTarget = target;
                    }

                    return false;
                }

                ETLogger.WriteLine(LogType.Debug,
                    string.Concat(currentMethodName, ": Player outside ", nameof(_action.AbortCombatDistance),
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
            return _action.Completed;
        }
        #endregion

        [Browsable(false)]
        public string ActionLabel
        {
            get
            {
                var teammate = _targetProcessor.GetTeammate();
                return teammate is null
                    ? $"{_action.GetType().Name} : {_action.SupportOptions.teammate}"
                    : $"{_action.GetType().Name} : {_action.SupportOptions.teammate} ({teammate.InternalName})";
            }
        }

        [Category("Engine")]
        public bool InternalConditions => _action.TeammateSearchTime == 0 
                                          || _teammateAbsenceTimer is null 
                                          || !_teammateAbsenceTimer.IsTimedOut;

        public ActionValidity InternalValidity => new ActionValidity();

        [Browsable(false)]
        public bool UseHotSpots => false;

        [Category("Engine")]
        public Vector3 InternalDestination
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
                        if (NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, location) >= _sqrtDistance)
                            return location.Clone();
                    }
                }
                return Vector3.Empty;
            }
        }

        public void InternalReset()
        {
            _targetProcessor.Reset();
            _teammateAbsenceTimer = null;

            AstralAccessors.Logic.NW.Combats.RemoveAbortCombatCondition();

            if (ExtendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(_idStr, '.', nameof(InternalReset)));
        }

        public void GatherInfos()
        {
#if false
            if (_action.HotSpots.Count == 0)
                _action.HotSpots.Add(EntityManager.LocalPlayer.Location.Clone());

            _label = string.Empty; 
#endif
        }

        public void OnMapDraw(GraphicsNW graphics)
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
                    distance = _action.Distance,
                    abortCombatDistance = _action.AbortCombatDistance,
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
                      y = pos.Y,
                      distance = _action.Distance,
                      abortCombatDistance= _action.AbortCombatDistance;
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
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugMoveToTeammate && logConf.Active;
            }
        }
        
        private Predicate<Entity> GetSpecialTeammateCheck()
        {
            Predicate<Entity> specialCheck = null;

            var crSet = _action.CustomRegions;
            if (crSet.Count > 0)
            {
                specialCheck = (ett) =>
                    crSet.Within(ett);
            }

            return specialCheck;
        }

        /// <summary>
        /// Проверка условия отключения боя <see cref="MoveToTeammate.IgnoreCombatCondition"/>
        /// </summary>
        /// <returns>Результат проверки <see cref="MoveToTeammate.IgnoreCombatCondition"/> либо True, если оно не задано.</returns>
        private bool CheckingIgnoreCombatCondition()
        {
            var check = _action.IgnoreCombatCondition;
            if (check != null)
                return check.IsValid;
            return true;
        }
        #endregion
    }
}
