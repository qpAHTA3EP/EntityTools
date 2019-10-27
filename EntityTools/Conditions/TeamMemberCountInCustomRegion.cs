using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного члена группы (но не игрока)
    /// в регионе CustomRegion, заданным в CustomRegionNames
    /// </summary>

    [Serializable]
    public class TeamMembersCountInCustomRegions : Condition
    {
        [Description("The relation of the character's location to the CustomRegion")]
        [Category("Location")]
        public Presence Tested { get; set; }

        //[Editor(typeof(CustomRegionEditor), typeof(UITypeEditor))]
        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Location")]
        public List<string> CustomRegionNames { get; set; }

        [Description("Threshold value of the Team members for comparison by 'Sign'")]
        [Category("Members")]
        public uint MemberCount { get; set; }

        [Description("The comparison type for 'MemberCount'")]
        [Category("Members")]
        public Relation Sign { get; set; }

        [Description("Check TeamMember's Region (not the CustomRegion):\n" +
            "True: Count TeamMember if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting TeamMembers")]
        [Category("Location")]
        public bool RegionCheck { get; set; }

        public TeamMembersCountInCustomRegions()
        {
            MemberCount = 0;
            Sign = Relation.Superior;

            Tested = Presence.Equal;

            RegionCheck = false;

            CustomRegionNames = new List<string>();
            foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions)
            {
                if (customRegion.IsIn)
                {
                    CustomRegionNames.Add(customRegion.Name);
                    break;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder strBldr = new StringBuilder(GetType().Name);

            strBldr.Append($" {Sign} to {MemberCount}");

            return strBldr.ToString();
        }

        public override bool IsValid
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam)
                {
                    if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam && CustomRegionNames.Count > 0)
                    {
                        uint memCount = 0;
                        List<CustomRegion> customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                                                       CustomRegionNames.Exists((string regName) => regName == cr.Name));


                        foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                        {
                            if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                            {
                                foreach (CustomRegion customRegion in customRegions)
                                {
                                    if ((!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                        && CommonTools.IsInCustomRegion(member.Entity, customRegion))
                                    {
                                        memCount++;
                                        break;
                                    }
                                }
                            }
                        }

                        if (Tested == Presence.NotEquel)
                            memCount = EntityManager.LocalPlayer.PlayerTeam.Team.MembersCount - memCount;

                        switch (Sign)
                        {
                            case Relation.Equal:
                                return memCount == MemberCount;
                            case Relation.NotEqual:
                                return memCount != MemberCount;
                            case Relation.Inferior:
                                return memCount < MemberCount;
                            case Relation.Superior:
                                return memCount > MemberCount;
                        }
                    }
                }
                return false;
            }
        }

        public override string TestInfos
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam)
                {
                    uint memsCount = 0;

                    StringBuilder strBldr = new StringBuilder();
                    strBldr.AppendLine();


                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                        {
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

                            if (Tested == Presence.Equal && match)
                                memsCount++;
                            if (Tested == Presence.NotEquel && !match)
                                memsCount++;

                            strBldr.Append($"[{member.InternalName}] is in CustomRegions: ").Append(strBldr2);
                            if (RegionCheck)
                            {
                                strBldr.Append($" (RegionCheck[{member.Entity.RegionInternalName}] ");
                                if (member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                    strBldr.Append("succeeded)");
                                else strBldr.Append("fail)");
                            }
                            strBldr.AppendLine();
                        }
                    }

                    if (Tested == Presence.Equal)
                    {
                        strBldr.Insert(0, $"Total {memsCount} TeamMember are in {CustomRegionNames.Count} CustomRegion(s):");
                    }
                    else
                    {
                        memsCount = EntityManager.LocalPlayer.PlayerTeam.Team.MembersCount - 1 - memsCount;
                        strBldr.Insert(0, $"Total {memsCount} TeamMember are not in {CustomRegionNames.Count} CustomRegion(s):");
                    }

                    return strBldr.ToString();
                }
                return "Player is not in a party";
            }
        }

        public override void Reset()
        {
        }
    }
}
