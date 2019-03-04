using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityPlugin.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного члена группы (но не игрока)
    /// в регионе CustomRegion, заданным в CustomRegionName
    /// </summary>

    [Serializable]
    public class TeamMemberCountInCustomRegion : Condition
    {
        [Description("The relation of the character's location to the CustomRegion")]
        public Presence Tested { get; set; }

        [Editor(typeof(CustomRegionEditor), typeof(UITypeEditor))]
        public string CustomRegionName { get; set; }

        [Description("Threshold value of the Team members for comparison by 'Sign'")]
        public int MemberCount { get; set; }

        [Description("The comparison type for 'MemberCount'")]
        public Relation Sign { get; set; }

        public TeamMemberCountInCustomRegion()
        {
            MemberCount = 3;
            Sign = Relation.Inferior;

            Tested = Presence.Equal;

            CustomRegionName = string.Empty;
            foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions)
            {
                if (customRegion.IsIn)
                {
                    CustomRegionName = customRegion.Name;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name} [{CustomRegionName}] {Sign} to {MemberCount}";
        }

        public override bool IsValid
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam && !string.IsNullOrEmpty(CustomRegionName))
                {
                    uint memCount = 0;
                    CustomRegion customRegion = Astral.Quester.API.CurrentProfile.CustomRegions.Find(CR => CR.Name == CustomRegionName);

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        if (member.InternalName != EntityManager.LocalPlayer.InternalName && Tools.IsInCustomRegion(member.Entity, customRegion))
                            memCount++;
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

                    CustomRegion customRegion = Astral.Quester.API.CurrentProfile.CustomRegions.Find(CR => CR.Name == CustomRegionName);

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                        {
                            if (Tools.IsInCustomRegion(member.Entity, customRegion))
                            {
                                strBldr.AppendLine($"[{member.InternalName}] is in [{customRegion.Name}]");
                                memsCount++;
                            }
                            else
                                strBldr.AppendLine($"{member.InternalName} is not in [{customRegion.Name}]");
                        }
                    }

                    if (Tested == Presence.Equal)
                    {
                        strBldr.Insert(0, $"Total {memsCount} TeamMember are in CustomRegion [{CustomRegionName}]:");
                    }
                    else
                    {
                        memsCount = EntityManager.LocalPlayer.PlayerTeam.Team.MembersCount - 1 - memsCount;
                        strBldr.Insert(0, $"Total {memsCount} TeamMember are not in CustomRegion [{CustomRegionName}]:");
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
