using System;
using System.ComponentModel;
using System.Text;
using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityPlugin.Conditions
{
    [Serializable]
    public class TeamMembersCount : Condition
    {
        [Description("Threshold value of the Distance from Player to the Team member for comparison by 'DistanceSign'")]
        [Category("Tested")]
        public float Distance { get; set; }

        [Description("The comparison type for 'Distance'")]
        [Category("Tested")]
        public Relation DistanceSign { get; set; }

        [Description("Threshold value of the Team members for comparison by 'Sign'")]
        [Category("Members")]
        public int MemberCount { get; set; }

        [Description("The comparison type for 'MemberCount'")]
        [Category("Members")]
        public Relation Sign { get; set; }

        [Description("Check TeamMember's Region:\n" +
            "True: Count TeamMember if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting TeamMembers")]
        [Category("Tested")]
        public bool RegionCheck { get; set; }

        public TeamMembersCount()
        {
            MemberCount = 3;
            Distance = 50;
            Sign = Relation.Inferior;
            DistanceSign = Relation.Inferior;
            RegionCheck = false;
        }

        public override void Reset()
        {
        }

        public override bool IsValid
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam?.IsInTeam == true
                    && EntityManager.LocalPlayer.PlayerTeam?.Team?.MembersCount > 1)
                {
                    int membersCount = 0;
                    switch (DistanceSign)
                    {
                        case Relation.Inferior:
                            {
                                membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                    && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                    && member.Entity.Location.Distance3DFromPlayer < Distance)
                                                ).Count;

                                break;
                            }
                        case Relation.Superior:
                            {
                                membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                    && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                    && member.Entity.Location.Distance3DFromPlayer > Distance)
                                                ).Count;
                                break;
                            }
                        case Relation.Equal:
                            {
                                membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                    && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                    && member.Entity.Location.Distance3DFromPlayer == Distance)
                                                ).Count;
                                break;
                            }
                        case Relation.NotEqual:
                            {
                                membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                    && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                    && member.Entity.Location.Distance3DFromPlayer != Distance)
                                                ).Count;
                                break;
                            }
                    }
                    bool result;
                    switch (Sign)
                    {
                        case Condition.Relation.Equal:
                            result = membersCount == MemberCount;
                            break;
                        case Condition.Relation.NotEqual:
                            result = membersCount != MemberCount;
                            break;
                        case Condition.Relation.Inferior:
                            result = membersCount < MemberCount;
                            break;
                        case Condition.Relation.Superior:
                            result = membersCount > MemberCount;
                            break;
                        default:
                            result = false;
                            break;
                    }
                    return result;
                }
                else return false;
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name} {Sign} to {MemberCount} which Distance {DistanceSign} to {Distance}";
        }

        public override string TestInfos
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam?.IsInTeam == true
                    && EntityManager.LocalPlayer.PlayerTeam?.Team?.MembersCount > 1)
                {
                    int memsCount = 0;
                    StringBuilder strBldr = new StringBuilder();
                    strBldr.AppendLine();

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                        {
                            strBldr.Append($"Distance to [{member.InternalName}] is ").AppendFormat("{0:0.##}", member.Entity.Location.Distance3DFromPlayer);
                            if (RegionCheck)
                            {
                                strBldr.Append($" (RegionCheck[{member.Entity.RegionInternalName}] ");
                                if(member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                    strBldr.Append("succeeded)");
                                else strBldr.Append("fail)");
                            }
                            strBldr.AppendLine();

                            switch (DistanceSign)
                            {
                                case Relation.Inferior:
                                    if(member.Entity.Location.Distance3DFromPlayer < Distance)
                                        memsCount++;
                                    break;
                                case Relation.Superior:
                                    if (member.Entity.Location.Distance3DFromPlayer > Distance)
                                        memsCount++;
                                    break;
                                case Relation.Equal:
                                    if (member.Entity.Location.Distance3DFromPlayer == Distance)
                                        memsCount++;
                                    break;
                                case Relation.NotEqual:
                                    if (member.Entity.Location.Distance3DFromPlayer != Distance)
                                        memsCount++;
                                    break;
                            }
                             
                        }
                    }
                    return strBldr.Insert(0, $"Total {memsCount} TeamMember has Distance from Player {DistanceSign} to {Distance}:").ToString();
                }
                else
                {
                    return "Player is not in a party";
                }
            }
        }
    }
}
