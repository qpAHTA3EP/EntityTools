using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Conditions
{
    [Serializable]
    public class TeamMembersCount2 : Condition
    {
        [XmlIgnore]
        private List<string> customRegionNames = null;
        [XmlIgnore]
        private List<CustomRegion> customRegions = null;

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Location")]
        public List<string> CustomRegionNames
        {
            get => customRegionNames;
            set
            {
                if (customRegionNames != value)
                {
                    if (value != null
                        && value.Count > 0)
                        customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                    value.Exists((string regName) => regName == cr.Name));

                    customRegionNames = value;
                }
            }
        }

        [Description("The relation of the character's location to the CustomRegion\n" +
            "Equals: Count only members located IN the CustomRegions\n" +
            "NotEquals: Count only members located OUT the CustomRegions")]
        [Category("Location")]
        public Presence CustomRegionRelation { get; set; } = Presence.Equal;

        [Description("Threshold value of the Distance from Player to the Team member for comparison by 'DistanceSign'")]
        [Category("Distance")]
        public float Distance { get; set; } = 0;

        [Description("The comparison type for 'Distance'")]
        [Category("Distance")]
        public Relation DistanceSign { get; set; } = Relation.Superior;

        [Description("Threshold value of the Team members for comparison by 'Sign'")]
        [Category("Members")]
        public int MemberCount { get; set; } = 3;

        [Description("The comparison type for 'MemberCount'")]
        [Category("Members")]
        public Relation Sign { get; set; } = Relation.Inferior;

        [Description("Check TeamMember's Region:\n" +
            "True: Count TeamMember if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting TeamMembers")]
        [Category("Members")]
        public bool RegionCheck { get; set; } = false;

        [XmlIgnore]
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(500);

        [Description("Time between searches of the TeamMembers (ms)")]
        public int SearchTimeInterval { get; set; } = 500;

        /// <summary>
        /// Кэшированное число членов группы, удовлетворяющих критериям
        /// </summary>
        [XmlIgnore]
        int membersCount = 0;

        public override bool IsValid
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam?.IsInTeam == true
                    && EntityManager.LocalPlayer.PlayerTeam?.Team?.MembersCount > 1)
                {
                    if (timeout.IsTimedOut)
                    {
                        membersCount = 0;


                        switch (DistanceSign)
                        {
                            case Relation.Inferior:
                                {
                                    membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer < Distance
                                                        && (customRegions == null || customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(member.Entity, cr)) != null))
                                                    ).Count;

                                    break;
                                }
                            case Relation.Superior:
                                {
                                    membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer > Distance
                                                        && (customRegions == null || customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(member.Entity, cr)) != null))
                                                    ).Count;
                                    break;
                                }
                            case Relation.Equal:
                                {
                                    membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer == Distance
                                                        && (customRegions == null || customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(member.Entity, cr)) != null))
                                                    ).Count;
                                    break;
                                }
                            case Relation.NotEqual:
                                {
                                    membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer != Distance
                                                        && (customRegions == null || customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(member.Entity, cr)) != null))
                                                    ).Count;
                                    break;
                                }
                        }
                        timeout.ChangeTime(SearchTimeInterval);
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

                            StringBuilder strBldr2 = new StringBuilder();
                            bool match = false;

                            foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions) //customRegions)
                            {
                                if (CommonTools.IsInCustomRegion(member.Entity, customRegion))
                                {
                                    match = true;
                                    if (strBldr2.Length > 0)
                                        strBldr2.Append(", ");
                                    strBldr2.Append($"[{customRegion.Name}]");
                                }
                            }
                            strBldr.Append($"[{member.InternalName}] is in CustomRegions: ").Append(strBldr2);



                            switch (DistanceSign)
                            {
                                case Relation.Inferior:
                                    if (member.Entity.Location.Distance3DFromPlayer < Distance)
                                    {
                                        if (CustomRegionRelation == Presence.Equal && match)
                                            memsCount++;
                                        if (CustomRegionRelation == Presence.NotEquel && !match)
                                            memsCount++;
                                    }
                                    break;
                                case Relation.Superior:
                                    if (member.Entity.Location.Distance3DFromPlayer > Distance)
                                    {
                                        if (CustomRegionRelation == Presence.Equal && match)
                                            memsCount++;
                                        if (CustomRegionRelation == Presence.NotEquel && !match)
                                            memsCount++;
                                    }
                                    break;
                                case Relation.Equal:
                                    if (member.Entity.Location.Distance3DFromPlayer == Distance)
                                    {
                                        if (CustomRegionRelation == Presence.Equal && match)
                                            memsCount++;
                                        if (CustomRegionRelation == Presence.NotEquel && !match)
                                            memsCount++;
                                    }
                                    break;
                                case Relation.NotEqual:
                                    if (member.Entity.Location.Distance3DFromPlayer != Distance)
                                    {
                                        if (CustomRegionRelation == Presence.Equal && match)
                                            memsCount++;
                                        if (CustomRegionRelation == Presence.NotEquel && !match)
                                            memsCount++;
                                    }
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

        public TeamMembersCount2() { }
        public override void Reset() { }
    }
}
