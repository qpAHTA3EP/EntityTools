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

        protected override bool IntenalConditions => true;

        protected override Vector3 InternalDestination => Vector3.Empty;

        protected override ActionValidity InternalValidity => new ActionValidity();

        public override void GatherInfos(){}
        public override void InternalReset(){}
        public override void OnMapDraw(GraphicsNW graph){}

        public override ActionResult Run()
        {
            //Определение номера инстанса в PossibleMapChoice.InstanceIndex
            //return Memory.MMemory.Read<uint>(base.Pointer + 40);
#if false
            if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam)
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsLeader)
                    return ActionResult.Skip;

                TeamMember player = EntityManager.LocalPlayer.PlayerTeam.Team.Members.Find(mem => mem.InternalName == EntityManager.LocalPlayer.InternalName);
                TeamMember leader = EntityManager.LocalPlayer.PlayerTeam.Team.Members.Find(mem => mem.EntityId == EntityManager.LocalPlayer.PlayerTeam.Team.Leader.EntityId);

                if (player != null && player.IsValid && leader != null && leader.IsValid)
                {
                    if (leader.MapName != player.MapName)
                        return ActionResult.Fail;

                    Instances.ChangeInstanceResult changeInstanceResult = CommonTools.ChangeInstance(leader.MapInstanceNumber);

                    if (changeInstanceResult == Instances.ChangeInstanceResult.Combat
                        || changeInstanceResult == Instances.ChangeInstanceResult.CantChange)
                    {
                        return ActionResult.Running;
                    }
                    if (changeInstanceResult == Instances.ChangeInstanceResult.Success)
                    {
                        //Astral.Classes.Timeout SearchTimeout = new Astral.Classes.Timeout(7000);
                        //while (Core.GetNearesNodetPosition(EntityManager.LocalPlayer.Location, false).Distance3DFromPlayer > 100.0)
                        //{
                        //    if (SearchTimeout.IsTimedOut)
                        //    {
                        //        Astral.Logger.WriteLine("Respawn point too far away the path, stop bot.");
                        //        Roles.ToggleRole(false);
                        //        return Action.ActionResult.Fail;
                        //    }
                        //    Thread.Sleep(500);
                        //}
                        return ActionResult.Completed;
                    }
                }
            } 
#else
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
#endif
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

#if false
            if (tmPlayer != null && tmLeader != null
                    && tmPlayer.MapName == tmLeader.MapName)

            {
                var playerEntity = tmPlayer.Entity;
                var leaderEntity = tmLeader.Entity;
                //BUG Если персонажи в разных инстансах, то tmLeader.Entity - не валидна
                return playerEntity.IAICombatTeamID == leaderEntity.IAICombatTeamID
                       && playerEntity.RegionInternalName == leaderEntity.RegionInternalName;
            }

            return false; 
#else
            return tmPlayer != null 
                   && tmPlayer.IsValid
                   && tmLeader != null
                   && tmLeader.IsValid
                   && tmPlayer.MapName == tmLeader.MapName;
#endif
        }
    }
}