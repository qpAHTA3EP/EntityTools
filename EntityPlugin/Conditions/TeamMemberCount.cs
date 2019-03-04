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
        public TeamMembersCount()
        {
            MemberCount = 3;
            Distance = 50;
            Sign = Relation.Inferior;
            DistanceSign = Relation.Inferior;
        }

        public override void Reset()
        {
        }

        [Description("Threshold value of the Distance from Player to the Team member for comparison by 'DistanceSign'")]
        public float Distance { get; set; }

        [Description("The comparison type for 'Distance'")]
        public Relation DistanceSign { get; set; }

        [Description("Threshold value of the Team members for comparison by 'Sign'")]
        public int MemberCount { get; set; }

        [Description("The comparison type for 'MemberCount'")]
        public Relation Sign { get; set; }

        public override bool IsValid
        {
            get
            {
                int membersCount = 0;
                switch (DistanceSign)
                {
                    case Relation.Inferior:
                        {
                            membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                            (   member.InternalName != EntityManager.LocalPlayer.InternalName && 
                                                member.Entity.Location.Distance3DFromPlayer < Distance)
                                            ).Count;                            

                            break;
                        }
                    case Relation.Superior:
                        {
                            membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                            (   member.InternalName != EntityManager.LocalPlayer.InternalName && 
                                                member.Entity.Location.Distance3DFromPlayer > Distance)
                                            ).Count;
                            break;
                        }
                    case Relation.Equal:
                        {
                            membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                            (   member.InternalName != EntityManager.LocalPlayer.InternalName && 
                                                member.Entity.Location.Distance3DFromPlayer == Distance)
                                            ).Count;
                            break;
                        }
                    case Relation.NotEqual:
                        {
                            membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                            (   member.InternalName != EntityManager.LocalPlayer.InternalName && 
                                                member.Entity.Location.Distance3DFromPlayer != Distance)
                                            ).Count;
                            break;
                        }
                }
                bool result;
                switch (Sign)
                {
                    case Condition.Relation.Equal:
                        result = ((ulong)membersCount == (ulong)((long)MemberCount));
                        break;
                    case Condition.Relation.NotEqual:
                        result = ((ulong)membersCount != (ulong)((long)MemberCount));
                        break;
                    case Condition.Relation.Inferior:
                        result = ((ulong)membersCount < (ulong)((long)MemberCount));
                        break;
                    case Condition.Relation.Superior:
                        result = ((ulong)membersCount > (ulong)((long)MemberCount));
                        break;
                    default:
                        result = false;
                        break;
                }
                return result;
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
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam)
                {
                    int memsCount = 0;
                    StringBuilder strBldr = new StringBuilder();
                    strBldr.AppendLine();

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                        {
                            strBldr.Append($"Distance to [{member.InternalName}] is ").AppendFormat("{0:0.##}", member.Entity.Location.Distance3DFromPlayer).AppendLine();
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


                    //switch (DistanceSign)
                    //{
                    //    case Relation.Inferior:
                    //        {
                    //            memsCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) => 
                    //                    (   member.InternalName != EntityManager.LocalPlayer.InternalName && 
                    //                        member.Entity.Location.Distance3DFromPlayer < Distance )    
                    //                    ).Count;
                    //            break;
                    //        }
                    //    case Relation.Superior:
                    //        {
                    //            memsCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) => 
                    //                    (   member.InternalName != EntityManager.LocalPlayer.InternalName && 
                    //                        member.Entity.Location.Distance3DFromPlayer > Distance)
                    //                    ).Count;
                    //            break;
                    //        }
                    //    case Relation.Equal:
                    //        {
                    //            memsCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                    //                    (   member.InternalName != EntityManager.LocalPlayer.InternalName &&
                    //                        member.Entity.Location.Distance3DFromPlayer == Distance)
                    //                    ).Count;
                    //            break;
                    //        }
                    //    case Relation.NotEqual:
                    //        {
                    //            memsCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                    //                    (   member.InternalName != EntityManager.LocalPlayer.InternalName &&
                    //                        member.Entity.Location.Distance3DFromPlayer != Distance)
                    //                    ).Count;
                    //            break;
                    //        }
                    //}

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
