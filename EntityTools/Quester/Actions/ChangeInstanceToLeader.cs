using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using EntityTools.Tools;
using MyNW;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class ChangeInstanceToLeader : Astral.Quester.Classes.Action
    {
        public ChangeInstanceToLeader()
        {
            //MaxRunningTime = 1;
        }

        public override string ActionLabel => GetType().Name;

        public override bool NeedToRun
        {
            get
            {
                return true;
                //return EntityManager.LocalPlayer.PlayerTeam.IsInTeam &&
                        //!EntityManager.LocalPlayer.PlayerTeam.IsLeader;// &&
                        //EntityManager.LocalPlayer.PlayerTeam.Team.Leader.MapName == EntityManager.LocalPlayer.MapState.MapName &&
                        //EntityManager.LocalPlayer.PlayerTeam.Team.Leader.MapInstanceNumber != ;
                
                //Определение номера инстанса в PossibleMapChoice.InstanceIndex
                //return Memory.MMemory.Read<uint>(base.Pointer + 40);
            }
        }

        public override string InternalDisplayName => string.Empty;

        public override bool UseHotSpots => false;

        protected override bool IntenalConditions => true;

        protected override Vector3 InternalDestination => new Vector3();

        protected override ActionValidity InternalValidity => new ActionValidity();

        public override void GatherInfos()
        {
        }

        public override void InternalReset()
        {
        }

        public override void OnMapDraw(GraphicsNW graph)
        {
        }

        public override ActionResult Run()
        {
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
                        return Action.ActionResult.Running;
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
                        return Action.ActionResult.Completed;
                    }
                }
            }
            return Action.ActionResult.Fail;
        }
    }
}