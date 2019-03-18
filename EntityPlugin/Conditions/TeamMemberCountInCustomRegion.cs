using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityPlugin.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного члена группы (но не игрока)
    /// в регионе CustomRegion, заданным в CustomRegionNames
    /// </summary>

    [Serializable]
    public class TeamMemberCountInCustomRegions : Condition
    {
        [Description("The relation of the character's location to the CustomRegion")]
        public Presence Tested { get; set; }

        //[Editor(typeof(CustomRegionEditor), typeof(UITypeEditor))]
        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        public List<string> CustomRegionNames { get; set; }

        [Description("Threshold value of the Team members for comparison by 'Sign'")]
        public uint MemberCount { get; set; }

        [Description("The comparison type for 'MemberCount'")]
        public Relation Sign { get; set; }

        public TeamMemberCountInCustomRegions()
        {
            MemberCount = 0;
            Sign = Relation.Superior;

            Tested = Presence.Equal;

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
            //switch (CustomRegionNames.Count)
            //{
            //    case 0:
            //        break;
            //    case 1:
            //        strBldr.Append($": [{CustomRegionNames[0]}]");
            //        break;
            //    default:
            //        strBldr.Append($": [{CustomRegionNames[0]}] and {CustomRegionNames.Count-1} other");
            //        break;
            //}

            strBldr.Append($" {Sign} to {MemberCount}");

            return strBldr.ToString();
        }

        public override bool IsValid
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam && CustomRegionNames.Count > 0)
                {
                    uint memCount = 0;
                    List <CustomRegion> customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) => 
                                                                                    CustomRegionNames.Exists((string regName) => regName == cr.Name));

                    
                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                        {
                            foreach (CustomRegion customRegion in customRegions)
                            {
                                if (SelectionTools.IsInCustomRegion(member.Entity, customRegion))
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
                    //StringBuilder strBldr3 = new StringBuilder();
                    //switch (CustomRegionNames.Count)
                    //{
                    //    case 0:
                    //        break;
                    //    case 1:
                    //        strBldr3.Append($"[{CustomRegionNames[0]}]");
                    //        break;
                    //    default:
                    //        strBldr3.Append($"[{CustomRegionNames[0]}] and {CustomRegionNames.Count - 1} other");
                    //        break;
                    //}

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                        {
                            StringBuilder strBldr2 = new StringBuilder();
                            foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions)
                            {
                                if ( CustomRegionNames.Exists((string regName) => regName == customRegion.Name) &&
                                        SelectionTools.IsInCustomRegion(member.Entity, customRegion) )
                                {
                                    if (strBldr2.Length > 0)
                                        strBldr2.Append(", ");
                                    strBldr2.Append($"[{customRegion.Name}]");
                                }
                            }

                            if (strBldr2.Length > 0)
                            {
                                strBldr.AppendLine($"[{member.Entity.InternalName}] is in CustomRegion(s): ").Append(strBldr2);
                                memsCount++;
                            }
                        }
                    }

                    if (Tested == Presence.Equal)
                    {
                        //strBldr.Insert(0, $"Total {memsCount} TeamMember are in CustomRegions {{{strBldr3}}}:");
                        strBldr.Insert(0, $"Total {memsCount} TeamMember are in {CustomRegionNames.Count} CustomRegion(s):");
                    }
                    else
                    {
                        memsCount = EntityManager.LocalPlayer.PlayerTeam.Team.MembersCount - 1 - memsCount;
                        //strBldr.Insert(0, $"Total {memsCount} TeamMember are not in CustomRegions {{{strBldr3}}}");
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
