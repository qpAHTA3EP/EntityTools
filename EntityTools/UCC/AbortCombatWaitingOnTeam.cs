using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Quester.FSM.States;
using EntityTools;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using static Astral.Quester.Classes.Condition;

namespace EntityTools.UCC
{
    [Serializable]
    public class AbortCombatWaitingOnTeam : UCCAction
    {
        public AbortCombatWaitingOnTeam()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new AbortCombatWaitingOnTeam
            {
                UiGenID = this.UiGenID,
                Tested = this.Tested,
                Distance = this.Distance,
                MemberCount = this.MemberCount,
                RegionCheck = this.RegionCheck,
                IgnoreCombatTime = this.IgnoreCombatTime,
                IgnoreCombatMinHP = this.IgnoreCombatMinHP
            });
        }

        private UIGen uiGen;
        private string uiGenID = "Team_Maptransferchoice_Waitingonteamlabel";

        [Description("The Identifier of the Ingame user interface element")]
        [Editor(typeof(UiIdEditor), typeof(UITypeEditor))]
        [Category("Required")]
        public string UiGenID
        {
            get { return uiGenID; }
            set
            {
                if (uiGenID != value)
                {
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == value);
                    if (uiGen != null)
                        uiGenID = value;
                }
            }
        }

        [Category("Required")]
        public UiGenCheckType Tested { get; set; } = UiGenCheckType.IsVisible;

        [Description("The maximum Distance from Player to the Team member\n" +
            "Set Distance to zero to disable team member counting")]
        [Category("Team")]
        public float Distance { get; set; } = 30;

        [Description("The minimum team member count that should be closer than the Distance to skip this action")]
        [Category("Team")]
        public int MemberCount { get; set; } = 2;

        [Description("Check team member's Region:\n" +
            "True: Count team member if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting team members")]
        [Category("Team")]
        public bool RegionCheck { get; set; } = true;

        [Description("How many time ignore combat in seconds (0 for infinite)")]
        [Category("Interruption")]
        public int IgnoreCombatTime { get => abordCombat.IgnoreCombatTime; set => abordCombat.IgnoreCombatTime = value; }

        [Description("Minimum health percent to enable combat again")]
        [Category("Interruption")]
        public int IgnoreCombatMinHP { get => abordCombat.IgnoreCombatMinHP; set => abordCombat.IgnoreCombatMinHP = value; }

        public override bool NeedToRun
        {
            get
            {
                if (uiGen == null && !string.IsNullOrEmpty(uiGenID))
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == uiGenID);
                if (uiGen != null && uiGen.IsValid)
                {
                    bool uiGenCheck = false;
                    switch (Tested)
                    {
                        case UiGenCheckType.IsVisible:
                            uiGenCheck = uiGen.IsVisible;
                            break;
                        case UiGenCheckType.IsHidden:
                            uiGenCheck = !uiGen.IsVisible;
                            break;
                    }

                    if (uiGenCheck)
                    {
                        int membersCount = -1;
                        if (Distance > 0)
                            membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                    (member.InternalName != EntityManager.LocalPlayer.InternalName
                                                        && (!RegionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                        && member.Entity.Location.Distance3DFromPlayer < Distance)
                                                    ).Count;
                        return (membersCount == -1 || membersCount >= MemberCount);
                    }
                }
                return false;
            }
        }

        public override bool Run()
        {
            return abordCombat.Run();
        }

        public override string ToString() => GetType().Name;

        [NonSerialized]
        protected AbordCombat abordCombat = new AbordCombat();

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
        #endregion
    }
}
