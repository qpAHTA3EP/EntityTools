using System;
using System.ComponentModel;
using Astral.Quester.Classes;
using EntityTools.Patches;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class TeamLeaderMapInstance : Condition
    {
#if PATCH_ASTRAL
        static TeamLeaderMapInstance()
        {
            // Пременение патча на этапе десериализации (до инициализации плагина)
            ETPatcher.Apply();
        }
#endif
        [Description("There is a bug: the bot does not detect correctly the current number of the map's instance \n" +
            "if the player changed the map's instance while it is in the party, \n" +
            "Therefore this condition has the false result in this case.")]
        public Presence Tested { get; set; }

        public override bool IsValid
        {
            get
            {
                bool mapInstanceEquals = false;
                var playerTeam = EntityManager.LocalPlayer.PlayerTeam;
                if (playerTeam != null 
                    && playerTeam.IsInTeam
                    && playerTeam.Team.MembersCount > 1)
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

        public override void Reset() { }

        public override string ToString()
        {
            return GetType().Name;
        }

        public override string TestInfos
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam
                    && EntityManager.LocalPlayer.PlayerTeam.Team.MembersCount > 1)
                {
                    if (EntityManager.LocalPlayer.PlayerTeam.IsLeader)
                        return "Player is TeamLeader";
                    TeamMember player = EntityManager.LocalPlayer.PlayerTeam.Team.Members.Find(mem => mem.InternalName == EntityManager.LocalPlayer.InternalName);
                    TeamMember leader = EntityManager.LocalPlayer.PlayerTeam.Team.Members.Find(mem => mem.EntityId == EntityManager.LocalPlayer.PlayerTeam.Team.Leader.EntityId);

                    if (player != null && player.IsValid && leader != null && leader.IsValid)
                    {
                        if (leader.MapName != player.MapName)
                            return "Player and TeamLeader is on the different maps";
                        return $"Player's instance is [{player.MapInstanceNumber}] and TeamLeader's is [{leader.MapInstanceNumber}] on the map {{{leader.MapName}}}";
                    }

                    return "Internal error in Player of TeamLeader identification";
                }
                return "Player is not in Team";
            }
        }
    }
}
