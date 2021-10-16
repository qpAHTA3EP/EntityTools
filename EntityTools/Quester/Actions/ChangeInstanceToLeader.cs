using System;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class ChangeInstanceToLeader : Action
    {
        public override string ActionLabel => GetType().Name;

        public override bool NeedToRun => true;

        public override string InternalDisplayName => string.Empty;

        public override bool UseHotSpots => false;

        protected override bool IntenalConditions
        {
            get
            {
                var team = EntityManager.LocalPlayer.PlayerTeam;
                return team.IsInTeam && !team.IsLeader;
            }
        }

        protected override Vector3 InternalDestination => Vector3.Empty;

        protected override ActionValidity InternalValidity => new ActionValidity();

        public override void GatherInfos(){}
        public override void InternalReset(){}
        public override void OnMapDraw(GraphicsNW graph) {}

        public override ActionResult Run()
        {
            //Определение номера инстанса в PossibleMapChoice.InstanceIndex
            //return Memory.MMemory.Read<uint>(base.Pointer + 40);

            var team = EntityManager.LocalPlayer.PlayerTeam;

            if (!team.IsInTeam) 
                return ActionResult.Fail;
            
            if (team.IsLeader)
                return ActionResult.Skip;

            if (GetTeamMembers(out TeamMember tmPlayer, out TeamMember tmLeader))
            {
                if (tmPlayer.MapInstanceNumber == tmLeader.MapInstanceNumber)
                    return ActionResult.Skip;

                var changeInstanceResult = CommonTools.ChangeInstance(tmLeader.MapInstanceNumber);

                if (changeInstanceResult == Instances.ChangeInstanceResult.Combat
                    || changeInstanceResult == Instances.ChangeInstanceResult.CantChange)
                {
                    return ActionResult.Running;
                }
                if (changeInstanceResult == Instances.ChangeInstanceResult.Success)
                {
                    return ActionResult.Completed;
                }
            }
            return ActionResult.Fail;
        }

        private bool GetTeamMembers(out TeamMember tmPlayer, out TeamMember tmLeader)
        {
            tmPlayer = null;
            tmLeader = null;

            var team = EntityManager.LocalPlayer.PlayerTeam;

            var playerId = EntityManager.LocalPlayer.ContainerId;
            var leaderId = team.Team.Leader.EntityId;

            foreach (var member in team.Team.Members)
            {
                var ettId = member.EntityId;
                if (ettId == playerId)
                {
                    tmPlayer = member;
                    continue;
                }

                if (ettId == leaderId)
                {
                    tmLeader = member;
                }
            }

            return tmPlayer != null 
                   && tmPlayer.IsValid
                   && tmLeader != null
                   && tmLeader.IsValid
                   && string.Equals(tmPlayer.MapName, tmLeader.MapName, StringComparison.Ordinal);
        }
    }
}