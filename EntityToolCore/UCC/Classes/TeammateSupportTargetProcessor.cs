#define DEBUG_CHANGE_TARGET

using AcTp0Tools.Classes.UCC;
using Astral.Classes;
using EntityCore.Enums;
using EntityCore.Extensions;
using EntityCore.Tools;
using EntityTools;
using EntityTools.Enums;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Actions.TargetSelectors;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Reflection;
using EntityCore.Entities;

namespace EntityCore.UCC.Classes
{
    /// <summary>
    /// Класс, реализующий обработку <seealso cref="TeammateSupport"/>
    /// </summary>
    internal class TeammateSupportTargetProcessor : UccTargetProcessor
    {
        private ChangeTarget action;
        private TeammateSupport selector;

        public TeammateSupportTargetProcessor(ChangeTarget changeTargetAction, TeammateSupport protectMember)
        {
            action = changeTargetAction ?? throw new ArgumentException(nameof(changeTargetAction));
            selector = protectMember ?? throw new ArgumentException(nameof(protectMember));

            selector.PropertyChanged += OnPropertyChanged;
            action.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Reset();
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
            _teammateCache = null;
            _teammateSelector = null;

            _targetCache = null;
            _targetSelector = null;
            _timeout.ChangeTime(0);
        
            _label = string.Empty;
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
            bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.UccActions.DebugChangeTarget;

            if (extendedDebugInfo)
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

                    //var player = EntityManager.LocalPlayer;
                    //if (_teammateCache is null ||
                    //    !_teammateCache.IsValid ||
                    //    _teammateCache.MapName != player.MapState.MapName ||
                    //    _teammateCache.Entity.IAICombatTeamID != player.IAICombatTeamID ||
                    //    // должно быть равнозначно следующей проверке:
                    //    //_teammateCache.Entity.PlayerTeam.TeamId != player.PlayerTeam.TeamId ||
                    //    _teammateCache.Entity.RegionInternalName != player.RegionInternalName)
                    //{
                    //    _teammateCache = GetTeammate();
                    //}

                    var newTeammate = GetTeammate();
                    //if (!ReferenceEquals(_teammateCache, newTeammate))
                    {
                        string debugMsg = string.Concat(currentMethodName, ": Teammate change ",
                            _teammateCache is null ? "NULL " : _teammateCache.Entity.GetDebugString(EntityNameType.InternalName, "TeammateCache", EntityDetail.Pointer|EntityDetail.Health|EntityDetail.HealthPersent|EntityDetail.Distance),
                            " => ",
                            newTeammate is null ? "NULL " : newTeammate.Entity.GetDebugString(EntityNameType.InternalName, selector.Teammate.ToString(), EntityDetail.Pointer | EntityDetail.Health | EntityDetail.HealthPersent | EntityDetail.Distance));
                        _teammateCache = newTeammate;
                        ETLogger.WriteLine(LogType.Debug, debugMsg,true);
                    } 

                    if (_targetSelector is null)
                    {
                        switch (selector.FoePreference)
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
                    {
                        string debugMsg = string.Concat(currentMethodName, ": Target change ",
                            _teammateCache is null ? "NULL " : _teammateCache.Entity.GetDebugString(EntityNameType.InternalName, "TargetCache", EntityDetail.Pointer | EntityDetail.Health | EntityDetail.HealthPersent | EntityDetail.Distance),
                            " => ",
                            newTeammate is null ? "NULL " : newTeammate.Entity.GetDebugString(EntityNameType.InternalName, selector.FoePreference.ToString(), EntityDetail.Pointer | EntityDetail.Health | EntityDetail.HealthPersent | EntityDetail.Distance));
                        _targetCache = newTarget;
                        ETLogger.WriteLine(LogType.Debug, debugMsg, true);
                    }

                    _timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
                }
            }
            else
            {
                if (_targetCache != null &&
                        (!_targetCache.IsValid || _targetCache.IsDead))
                    _targetCache = null;

                if (_timeout.IsTimedOut)
                {
                    _targetCache = null;

                    //var player = EntityManager.LocalPlayer;
                    //if (_teammateCache is null ||
                    //    !_teammateCache.IsValid ||
                    //    _teammateCache.MapName != player.MapState.MapName ||
                    //    _teammateCache.Entity.IAICombatTeamID != player.IAICombatTeamID ||
                    //    // должно быть равнозначно следующей проверке:
                    //    //_teammateCache.Entity.PlayerTeam.TeamId != player.PlayerTeam.TeamId ||
                    //    _teammateCache.Entity.RegionInternalName != player.RegionInternalName)
                    //{
                    //    _teammateCache = GetTeammate();
                    //}
                    _teammateCache = GetTeammate();

                    if (_targetSelector is null)
                    {
                        switch (selector.FoePreference)
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
        private TeamMember GetTeammate()
        {
            if(_teammateSelector is null)
            {
                var squareRange = action.Range * action.Range;

                switch (selector.Teammate)
                {
                    case Teammates.Leader:
                        _teammateSelector = squareRange > 0 
                            ? () => PlayerTeamHelper.GetLeader(squareRange)
                            : new Func<TeamMember>(PlayerTeamHelper.GetLeader);
                        break;
                    case Teammates.Tank:
                        _teammateSelector = squareRange > 0
                            ? () => PlayerTeamHelper.GetTank(squareRange)
                            : _teammateSelector = PlayerTeamHelper.GetTank;
                        break;
                    case Teammates.Healer:
                        _teammateSelector = squareRange > 0
                            ? () => PlayerTeamHelper.GetHealer(squareRange)
                            : _teammateSelector = PlayerTeamHelper.GetHealer;
                        break;
                    case Teammates.Sturdiest:
                        _teammateSelector = squareRange > 0
                            ? () => PlayerTeamHelper.GetSturdiest(squareRange)
                            : _teammateSelector = PlayerTeamHelper.GetSturdiest;
                        break;
                    case Teammates.SturdiestDD:
                        _teammateSelector = squareRange > 0
                            ? () => PlayerTeamHelper.GetSturdiestDamageDealer(squareRange)
                            : _teammateSelector = PlayerTeamHelper.GetSturdiestDamageDealer;
                        break;
                    case Teammates.Weakest:
                        _teammateSelector = squareRange > 0
                            ? () => PlayerTeamHelper.GetWeakest(squareRange)
                            : _teammateSelector = PlayerTeamHelper.GetWeakest;
                        break;
                    case Teammates.WeakestDD:
                        _teammateSelector = squareRange > 0
                            ? () => PlayerTeamHelper.GetWeakestDamageDealer(squareRange)
                            : _teammateSelector = PlayerTeamHelper.GetWeakestDamageDealer;
                        break;
                    case Teammates.MostInjured:
                        _teammateSelector = squareRange > 0
                            ? () => PlayerTeamHelper.GetMostInjured(squareRange)
                            : _teammateSelector = PlayerTeamHelper.GetMostInjured;
                        break;
                    case Teammates.MostInjuredDD:
                        _teammateSelector = squareRange > 0
                            ? () => PlayerTeamHelper.GetMostInjuredDamageDealer(squareRange)
                            : _teammateSelector = PlayerTeamHelper.GetMostInjuredDamageDealer;
                        break;
                    default:
                        _teammateSelector = () => null;
                        break;
                } 
            }

            return _teammateSelector();
        }
        private Func<TeamMember> _teammateSelector;
#if DEBUG_CHANGE_TARGET
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object TeammateCache => new SimpleEntityWrapper(_teammateCache);//new SimpleEntityWrapper(GetTeammate());
#endif
        private TeamMember _teammateCache;

        public override string ToString() => Label();

        public override string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (action is null)
                    _label = GetType().Name;
                else
                {
                    if (selector is null)
                        _label = action.GetType().Name;
                    else
                    {
                        switch (selector.FoePreference)
                        {
                            case PreferredFoe.TeammatesTarget:
                                _label = $"Assist {selector.Teammate}";
                                break;
                            case PreferredFoe.ClosestToPlayer:
                                _label = $"{action.GetType().Name} : Protect {selector.Teammate} from {selector.FoePreference}";
                                break;
                        }
                    }
                }
            }
            return _label;
        }
        private string _label;
    }
}
