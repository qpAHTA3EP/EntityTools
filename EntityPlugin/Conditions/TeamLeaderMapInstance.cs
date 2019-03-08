using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityPlugin.Conditions
{
    [Serializable]
    public class TeamLeaderMapInstance : Condition
    {
        public Condition.Presence Tested { get; set; }

        public override bool IsValid
        {
            get
            {
                bool mapInstanceEquals = false;

                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam)
                {
                    if (EntityManager.LocalPlayer.PlayerTeam.IsLeader)
                        mapInstanceEquals = true;
                    else
                    {
                        TeamMember player = EntityManager.LocalPlayer.PlayerTeam.Team.Members.Find(mem => mem.InternalName == EntityManager.LocalPlayer.InternalName);
                        TeamMember leader = EntityManager.LocalPlayer.PlayerTeam.Team.Members.Find(mem => mem.EntityId == EntityManager.LocalPlayer.PlayerTeam.Team.Leader.EntityId);

                        if (player != null && player.IsValid && leader != null && leader.IsValid)
                            mapInstanceEquals = (leader.MapName == player.MapName && player.MapInstanceNumber == leader.MapInstanceNumber);
                    }                    
                }
                return (mapInstanceEquals && Tested == Presence.Equal) || (!mapInstanceEquals && Tested == Presence.NotEquel);
            }
        }

        public override void Reset()
        {
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public override string TestInfos
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam)
                {
                    if (EntityManager.LocalPlayer.PlayerTeam.IsLeader)
                        return "Player is TeamLeader";
                    else
                    {
                        TeamMember player = EntityManager.LocalPlayer.PlayerTeam.Team.Members.Find(mem => mem.InternalName == EntityManager.LocalPlayer.InternalName);
                        TeamMember leader = EntityManager.LocalPlayer.PlayerTeam.Team.Members.Find(mem => mem.EntityId == EntityManager.LocalPlayer.PlayerTeam.Team.Leader.EntityId);

                        if (player != null && player.IsValid && leader != null && leader.IsValid)
                        {
                            if (leader.MapName != player.MapName)
                                return "Player and TeamLeader is on the different maps";
                            else
                            {
                                return $"Player's instance is [{player.MapInstanceNumber}] and TeamLeader's is [{leader.MapInstanceNumber}] on the map {{{leader.MapName}}}";
                            }
                        }
                        else
                            return "Internal error in Player of TeamLeader identification";
                    }
                }
                return "Player is not in Team";
            }
        }
    }
}
