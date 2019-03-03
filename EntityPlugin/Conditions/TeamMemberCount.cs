using System;
using System.ComponentModel;
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

        public float Distance { get; set; }

        [Description("Distance comparison type to the closest Entity")]
        public Relation DistanceSign { get; set; }

        public int MemberCount { get; set; }

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
                    int membersCount = 0;

                    switch (DistanceSign)
                    {
                        case Relation.Inferior:
                            {
                                membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) => 
                                        (   member.InternalName != EntityManager.LocalPlayer.InternalName && 
                                            member.Entity.Location.Distance3DFromPlayer < Distance )    
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

                    return $"{membersCount} TeamMember has Distance from Player {DistanceSign} to {Distance}";
                }
                else
                {
                    return "Player is not in a party";
                }
            }
        }
    }
}
