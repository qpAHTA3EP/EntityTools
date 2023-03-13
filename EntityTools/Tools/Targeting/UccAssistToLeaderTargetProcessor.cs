using AcTp0Tools.Classes.UCC;
using EntityCore.Entities;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Actions.TargetSelectors;
using MyNW.Classes;
using System;
using Astral.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityCore.UCC.Classes
{
    /// <summary>
    /// Класс, реализующий обработку <seealso cref="AssistToLeader"/>
    /// </summary>
    internal class UccAssistToLeaderTargetProcessor : UccTargetProcessor
    {
        private ChangeTarget action;
        private AssistToLeader selector;

        public UccAssistToLeaderTargetProcessor(ChangeTarget changeTargetAction, AssistToLeader targetSelector)
        {
            action = changeTargetAction ?? throw new ArgumentException(nameof(changeTargetAction));
            selector = targetSelector ?? throw new ArgumentException(nameof(targetSelector));

            selector.PropertyChanged += OnPropertyChanged;
            action.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
            _label = string.Empty;
            _targetCache = null;
            _timeout.ChangeTime(0);
        }

        public override bool IsMatch(Entity target)
        {
            var leaderTarget = GetTarget();
            return target?.ContainerId == leaderTarget?.ContainerId;
        }

        public override bool IsTargetMismatchedAndCanBeChanged(Entity target)
        {
            var leaderTarget = GetTarget();

            if (leaderTarget is null)
                return false;

            return target?.ContainerId != leaderTarget.ContainerId;
        }

        public override Entity GetTarget()
        {
            if (_timeout.IsTimedOut)
            {
                _targetCache = null;

                var leader = GetTeamLeader();

                if (leader != null && leader.Character.HasTarget)
                {
                    var currentTarget = leader.Character.CurrentTarget;
                    if (currentTarget.RelationToPlayer == EntityRelation.Foe)
                        _targetCache = currentTarget;
                }
                _timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
            }

            return _targetCache;
        }
        private Entity _targetCache;
        private readonly Timeout _timeout = new Timeout(0);


        private Entity GetTeamLeader()
        {
            var player = EntityManager.LocalPlayer;

            var team = player.PlayerTeam;

            if (!team.IsInTeam)
                return null;
            if (team.IsLeader)
                return null;

            var leader = team.Team.Leader.Entity;

            if (!leader.IsValid || !leader.InCombat)
                return null;

            var range = action.Range;
            if (range <= 0)
                range = int.MaxValue;
            if (leader.CombatDistance3 < range)
                return leader;

            return null;
        }

        public override string Label()
        {
            if(string.IsNullOrEmpty(_label))
                _label = $"{action.GetType().Name} to TeamLeader's Target";
            return _label;
        }

        private string _label;
    }

}
