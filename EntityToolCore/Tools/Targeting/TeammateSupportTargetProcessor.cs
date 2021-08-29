﻿#define DEBUG_CHANGE_TARGET

using AcTp0Tools.Classes.Targeting;
using Astral.Classes;
using EntityCore.Entities;
using EntityCore.Enums;
using EntityCore.Extensions;
using EntityTools;
using EntityTools.Enums;
using EntityTools.Tools.Targeting;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Reflection;

namespace EntityCore.Tools.Targeting
{
    /// <summary>
    /// Класс, реализующий обработку <seealso cref="TeammateSupport"/>
    /// </summary>
    internal class TeammateSupportTargetProcessor : TargetProcessor
    {
        private TeammateSupport _targeter;

        public TeammateSupportTargetProcessor(TeammateSupport protectMember, Predicate<Entity> specialCheck)
        {
            _targeter = protectMember ?? throw new ArgumentException(nameof(protectMember));
            _targeter.PropertyChanged += OnPropertyChanged;
            _specialCheck = specialCheck;
            GetTeammate = Initializer_GetTeammate;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Reset();
        }

        public override void Dispose()
        {
            if (_targeter != null)
            {
                _targeter.PropertyChanged -= OnPropertyChanged;
                _targeter = null;
            }

            _specialCheck = null;
            GetTeammate = Initializer_GetTeammate;
        }

        public override void Reset()
        {
            _teammateCache = null;
            GetTeammate = Initializer_GetTeammate;

            _targetCache = null;
            _targetSelector = null;
            _timeout.ChangeTime(0);
        
            //_label = string.Empty;
        }

        public override bool IsMatch(Entity target)
        {
            var leaderTarget = GetTarget();
            return target?.ContainerId == leaderTarget?.ContainerId;
        }

        public override bool IsTargetMismatchedAndCanBeChanged(Entity target)
        {
            var foe = GetTarget();

            if (foe is null)
                return false;

            return target?.ContainerId != foe.ContainerId;
        }

        public override Entity GetTarget()
        {
            if (ExtendedDebugInfo)
            {
                string currentMethodName = string.Concat(GetType().Name, '.', MethodBase.GetCurrentMethod().Name);

                if (_targetCache != null)
                {
                    if (!_targetCache.IsValid)
                    {
                        _targetCache = null;
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Target invalid. Reset target cache...");
                    }
                    else if (_targetCache.IsDead)
                    {
                        _targetCache = null;
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Target is dead. Reset target cache...");
                    }
                }

                if (_timeout.IsTimedOut)
                {
                    _targetCache = null;

                    var newTeammate = GetTeammate();
                    string debugMsg = string.Concat(currentMethodName, ": Teammate change ",
                        _teammateCache is null ? "NULL " : _teammateCache.Entity.GetDebugString(EntityNameType.InternalName, "TeammateCache", EntityDetail.Pointer|EntityDetail.Health|EntityDetail.HealthPersent|EntityDetail.Distance),
                        " => ",
                        newTeammate is null ? "NULL " : newTeammate.Entity.GetDebugString(EntityNameType.InternalName, _targeter.Teammate.ToString(), EntityDetail.Pointer | EntityDetail.Health | EntityDetail.HealthPersent | EntityDetail.Distance));
                    _teammateCache = newTeammate;
                    ETLogger.WriteLine(LogType.Debug, debugMsg,true);

                    if (_targetSelector is null)
                    {
                        switch (_targeter.FoePreference)
                        {
                            case PreferredFoe.TeammatesTarget:
                                _targetSelector = () => _teammateCache?.Entity.GetTeammatesTarget();
                                break;
                            case PreferredFoe.ClosestToPlayer:
                                _targetSelector = () => _teammateCache?.Entity.GetClosestToPlayerAttacker();
                                break;
                            case PreferredFoe.ClosestToTeammate:
                                _targetSelector = () => _teammateCache?.Entity.GetClosestAttacker();
                                break;
                            case PreferredFoe.Sturdiest:
                                _targetSelector = () => _teammateCache?.Entity.GetSturdiestAttacker();
                                break;
                            case PreferredFoe.Weakest:
                                _targetSelector = () => _teammateCache?.Entity.GetWeakestAttacker();
                                break;
                            case PreferredFoe.MostInjured:
                                _targetSelector = () => _teammateCache?.Entity.GetMostInjuredAttacker();
                                break;
                            default:
                                _targetSelector = () => null;
                                break;
                        }
                    }

                    var newTarget = _targetSelector();
                    
                    debugMsg = string.Concat(currentMethodName, ": Target change ",
                        _teammateCache is null ? "NULL " : _teammateCache.Entity.GetDebugString(EntityNameType.InternalName, "TargetCache", EntityDetail.Pointer | EntityDetail.Health | EntityDetail.HealthPersent | EntityDetail.Distance),
                        " => ",
                        newTeammate is null ? "NULL " : newTeammate.Entity.GetDebugString(EntityNameType.InternalName, _targeter.FoePreference.ToString(), EntityDetail.Pointer | EntityDetail.Health | EntityDetail.HealthPersent | EntityDetail.Distance));
                    _targetCache = newTarget;
                    ETLogger.WriteLine(LogType.Debug, debugMsg, true);

                    _timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
                }
            }
            else
            {
                if (_targetCache != null &&
                        (!_targetCache.IsValid || _targetCache.IsDead))
                    _targetCache = null;

                if (!_timeout.IsTimedOut) return _targetCache;

                _targetCache = null;

                _teammateCache = GetTeammate();

                if (_targetSelector is null)
                {
                    switch (_targeter.FoePreference)
                    {
                        case PreferredFoe.TeammatesTarget:
                            _targetSelector = () => _teammateCache?.Entity.GetTeammatesTarget();
                            break;
                        case PreferredFoe.ClosestToPlayer:
                            _targetSelector = () => _teammateCache?.Entity.GetClosestToPlayerAttacker();
                            break;
                        case PreferredFoe.ClosestToTeammate:
                            _targetSelector = () => _teammateCache?.Entity.GetClosestAttacker();
                            break;
                        case PreferredFoe.Sturdiest:
                            _targetSelector = () => _teammateCache?.Entity.GetSturdiestAttacker();
                            break;
                        case PreferredFoe.Weakest:
                            _targetSelector = () => _teammateCache?.Entity.GetWeakestAttacker();
                            break;
                        default:
                            _targetSelector = () => null;
                            break;
                    }
                }

                _targetCache = _targetSelector();

                _timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
            }

            return _targetCache;
        }
#if DEBUG_CHANGE_TARGET
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object TargetCache => new SimpleEntityWrapper(_targetCache);//new SimpleEntityWrapper(GetTarget());
#endif
        private Entity _targetCache;
        private Func<Entity> _targetSelector;
        private readonly Timeout _timeout = new Timeout(0);

        /// <summary>
        /// Выбор члена группы, заданного <seealso cref="TeammateSupport.Teammate"/>
        /// </summary>
        /// <returns></returns>
        public TeamMember Initializer_GetTeammate()
        {
            Func <TeamMember> teammateSelector;
            
            switch (_targeter.Teammate)
            {
                case Teammates.Leader:
                    if (_specialCheck != null)
                        teammateSelector = () => PlayerTeamHelper.GetLeader(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetLeader;
                    break;
                case Teammates.Tank:
                    if(_specialCheck != null)
                        teammateSelector = () => PlayerTeamHelper.GetTank(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetTank;
                    break;
                case Teammates.Healer:
                    if (_specialCheck != null) 
                        teammateSelector = () => PlayerTeamHelper.GetHealer(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetHealer;
                    break;
                case Teammates.Sturdiest:
                    if (_specialCheck != null) 
                        teammateSelector = () => PlayerTeamHelper.GetSturdiest(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetSturdiest;
                    break;
                case Teammates.SturdiestDD:
                    if (_specialCheck != null) 
                        teammateSelector = () => PlayerTeamHelper.GetSturdiestDamageDealer(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetSturdiestDamageDealer;
                    break;
                case Teammates.Weakest:
                    if (_specialCheck != null) 
                        teammateSelector = () => PlayerTeamHelper.GetWeakest(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetWeakest;
                    break;
                case Teammates.WeakestDD:
                    if (_specialCheck != null) 
                        teammateSelector = () => PlayerTeamHelper.GetWeakestDamageDealer(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetWeakestDamageDealer;
                    break;
                case Teammates.MostInjured:
                    if (_specialCheck != null) 
                        teammateSelector = () => PlayerTeamHelper.GetMostInjured(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetMostInjured;
                    break;
                case Teammates.MostInjuredDD:
                    if (_specialCheck != null) 
                        teammateSelector = () => PlayerTeamHelper.GetMostInjuredDamageDealer(SpecialCheck);
                    else teammateSelector = PlayerTeamHelper.GetMostInjuredDamageDealer;
                    break;
                default:
                    teammateSelector = () => null;
                    break;
            }

            GetTeammate = teammateSelector;

            return GetTeammate();
        }
        public Func<TeamMember> GetTeammate { get; private set; }

#if DEBUG_CHANGE_TARGET
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object TeammateCache => new SimpleEntityWrapper(_teammateCache);
#endif
        private TeamMember _teammateCache;

        public override string ToString() => Label();

        public override string Label()
        {
#if false
            if (string.IsNullOrEmpty(_label))
            {

                if (_targeter is null)
                    _label = nameof(TeammateSupport);
                else
                {
                    _label = _targeter.FoePreference == PreferredFoe.TeammatesTarget
                        ? $"Assist {_targeter.Teammate}"
                        : $"Protect '{_targeter.Teammate}' from '{_targeter.FoePreference}' Foe";
                }
            }
            return _label; 
#else
            if (_targeter is null)
                return nameof(TeammateSupport);
            if (_targeter.FoePreference == PreferredFoe.TeammatesTarget)
                return $"Assist {_targeter.Teammate}";
            return $"Protect '{_targeter.Teammate}' from '{_targeter.FoePreference}' Foe";
#endif
        }
        //private string _label;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// </summary>        
        [Browsable(false)]
        public override Predicate<Entity> SpecialCheck
        {
            get => _specialCheck;
            set
            {
                _specialCheck = value;
                GetTeammate = Initializer_GetTeammate;
            }
        }

        private Predicate<Entity> _specialCheck;

        /// <summary>
        /// Флаг настроек вывода расширенной отладочной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.EntityTools.Config.Logger;
                return logConf.Active && (logConf.UccActions.DebugChangeTarget || logConf.QuesterActions.DebugMoveToTeammate);
            }
        }
    }
}