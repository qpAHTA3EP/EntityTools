using System;
using System.ComponentModel;
using System.Drawing.Design;
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
        public Presence Tested { get; set; }

        [Editor(typeof(CustomRegionEditor), typeof(UITypeEditor))]
        public string CustomRegionName { get; set; }

        public int MemberCount { get; set; }

        public Relation Sign { get; set; }

        public TeamMemberCountInCustomRegion()
        {
            MemberCount = 3;
            Sign = Relation.Inferior;

            Tested = Presence.Equal;
            foreach (CustomRegion customRegion in Astral.Quester.Core.Profile.CustomRegions)
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
            return "CustomRegion " + Tested + " " + CustomRegionName;
        }

        public override bool IsValid
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam)
                {
                    uint memCount = 0;
                    CustomRegion customRegion = Astral.Quester.Core.Profile.CustomRegions.Find(CR => CR.Name == CustomRegionName);

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
                    uint memCount = 0;
                    CustomRegion customRegion = Astral.Quester.Core.Profile.CustomRegions.Find(CR => CR.Name == CustomRegionName);

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        if (member.InternalName != EntityManager.LocalPlayer.InternalName && Tools.IsInCustomRegion(member.Entity, customRegion))
                            memCount++;
                    }

                    if (Tested == Presence.NotEquel)
                        memCount = EntityManager.LocalPlayer.PlayerTeam.Team.MembersCount - memCount;


                    return $"{memCount} TeamMembers are {Tested} in CustomRegion '{CustomRegionName}'";
                }
                return "Player is not in a party";
            }
        }

        public override void Reset()
        {
        }
    }
}
