using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityTools.Editors;
using MyNW.Classes;
using MyNW.Internals;
using EntityTools.Extentions;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class TeamMembersCount : Condition
    {
        [XmlIgnore]
        private List<string> customRegionNames = null;
        [XmlIgnore]
        private List<CustomRegion> customRegions = null;

        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Location")]
        public List<string> CustomRegionNames
        {
            get => customRegionNames;
            set
            {
                if (customRegionNames != value)
                {
                    customRegions = CustomRegionExtentions.GetCustomRegions(value);
                    customRegionNames = value;
                }
            }
        }

        [Description("The Check of the Team member's location relative to the custom regions\n" +
            "Equals: Count only members located WITHIN the CustomRegions\n" +
            "NotEquals: Count only members located OUTSIDE the CustomRegions")]
        [Category("Location")]
        public Presence CustomRegionCheck { get; set; } = Presence.Equal;

        [Description("The Value which is compared by 'DistanceSign' with the distance between Player and Team member")]
        [Category("Distance")]
        public float Distance { get; set; } = 0;

        [Description("Option specifies the comparison of the distance to the group member")]
        [Category("Distance")]
        public Relation DistanceSign { get; set; } = Relation.Superior;

        [Description("The Value which is compared by 'Sign' with the counted Team members")]
        [Category("Members")]
        public int MemberCount { get; set; } = 3;

        [Description("Option specifies the comparison of 'MemberCount' and the counted Team members")]
        [Category("Members")]
        public Relation Sign { get; set; } = Relation.Inferior;

        [Description("The Check of the Team member's Region (not CustomRegion)):\n" +
            "True: Count Team member if it is located in the same Region as Player\n" +
            "False: Does not consider the region when counting Team members")]
        [Category("Members")]
        public bool RegionCheck { get; set; } = false;

        [Description("Time between searches of the TeamMembers (ms)")]
        public int SearchTimeInterval { get; set; } = 500;

        [NonSerialized]
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(500);

        /// <summary>
        /// Кэшированное число членов группы, удовлетворяющих критериям
        /// </summary>
        [NonSerialized]
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

                        if (customRegionNames != null && (customRegions == null || customRegions.Count != customRegionNames.Count))
                            customRegions = CustomRegionExtentions.GetCustomRegions(customRegionNames); 

                        switch (DistanceSign)
                        {
                            case Relation.Inferior:
                                {
                                    membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                        member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer < Distance
                                                        && (customRegions == null || customRegions.Find((CustomRegion cr) => member.Entity.Within(cr)) != null))
                                                    ).Count;

                                    break;
                                }
                            case Relation.Superior:
                                {
                                    membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                        member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer > Distance
                                                        && (customRegions == null || customRegions.Find((CustomRegion cr) => member.Entity.Within(cr)) != null))
                                                    ).Count;
                                    break;
                                }
                            case Relation.Equal:
                                {
                                    membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                        member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer == Distance
                                                        && (customRegions == null || customRegions.Find((CustomRegion cr) => member.Entity.Within(cr)) != null))
                                                    ).Count;
                                    break;
                                }
                            case Relation.NotEqual:
                                {
                                    membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                        member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer != Distance
                                                        && (customRegions == null || customRegions.Find((CustomRegion cr) => member.Entity.Within(cr)) != null))
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
            //return $"{GetType().Name} {Sign} to {MemberCount} which Distance {DistanceSign} to {Distance}";
            return $"{GetType().Name} {Sign} to {MemberCount}";
        }

        public override string TestInfos
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam?.IsInTeam == true
                    && EntityManager.LocalPlayer.PlayerTeam?.Team?.MembersCount > 1)
                {
                    if (customRegionNames != null && (customRegions == null || customRegions.Count != customRegionNames.Count))
                        customRegions = CustomRegionExtentions.GetCustomRegions(customRegionNames);

                    int memsCount = 0;
                    StringBuilder strBldr = new StringBuilder();
                    strBldr.AppendLine();

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        /* if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                         * Эквивалентно следующей строке: */
                        if (member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId)
                        {
                            strBldr.AppendLine(member.InternalName);
                            strBldr.AppendFormat("\tDistance: {0:0.##}", member.Entity.Location.Distance3DFromPlayer).AppendLine();

                            if (RegionCheck)
                            {
                                strBldr.Append($"\tRegionCheck[").Append(member.Entity.RegionInternalName).Append("]: ");
                                if(member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                    strBldr.AppendLine("succeeded");
                                else strBldr.AppendLine("fail");
                            }

                            StringBuilder strBldr2 = new StringBuilder();
                            bool match = false;

                            if (customRegions != null && customRegions.Count > 0)
                            {
                                foreach (CustomRegion cr in customRegions)
                                    if (member.Entity.Within(cr))
                                    {
                                        match = true;
                                        if (strBldr2.Length > 0)
                                            strBldr2.Append(", ");
                                        strBldr2.Append($"[{cr.Name}]");
                                    }
                                strBldr.Append("\tCustomRegions: ").Append(strBldr2).AppendLine();
                            }

                            switch (DistanceSign)
                            {
                                case Relation.Inferior:
                                    if (member.Entity.Location.Distance3DFromPlayer < Distance)
                                    {
                                        if (CustomRegionCheck == Presence.Equal && match)
                                            memsCount++;
                                        if (CustomRegionCheck == Presence.NotEquel && !match)
                                            memsCount++;
                                    }
                                    break;
                                case Relation.Superior:
                                    if (member.Entity.Location.Distance3DFromPlayer > Distance)
                                    {
                                        if (CustomRegionCheck == Presence.Equal && match)
                                            memsCount++;
                                        if (CustomRegionCheck == Presence.NotEquel && !match)
                                            memsCount++;
                                    }
                                    break;
                                case Relation.Equal:
                                    if (member.Entity.Location.Distance3DFromPlayer == Distance)
                                    {
                                        if (CustomRegionCheck == Presence.Equal && match)
                                            memsCount++;
                                        if (CustomRegionCheck == Presence.NotEquel && !match)
                                            memsCount++;
                                    }
                                    break;
                                case Relation.NotEqual:
                                    if (member.Entity.Location.Distance3DFromPlayer != Distance)
                                    {
                                        if (CustomRegionCheck == Presence.Equal && match)
                                            memsCount++;
                                        if (CustomRegionCheck == Presence.NotEquel && !match)
                                            memsCount++;
                                    }
                                    break;
                            }
                             
                        }
                    }
                    return strBldr.Insert(0, $"Total {memsCount} Team members matches to the conditions.").ToString();
                }
                else
                {
                    return "Player is not in a party";
                }
            }
        }

        public TeamMembersCount() { }
        public override void Reset() { }
    }
}
