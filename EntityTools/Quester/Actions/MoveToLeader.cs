using Astral.Classes;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using DevExpress.XtraEditors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class MoveToLeader : Astral.Quester.Classes.Action
    {
        [Description("Distance to the Team Leader by which it is necessary to approach")]
        [Category("Interruptions")]
        public float Distance { get; set; } = 30;

        [Description("Enable IgnoreCombat profile value while playing action")]
        [Category("Interruptions")]
        public bool IgnoreCombat { get; set; } = true;

        [Description("True: Complite an action when the Team Leader is closer than 'Distance'\n" +
                     "False: Follow an Team Leader regardless of its distance")]
        [Category("Interruptions")]
        public bool StopOnApproached { get; set; } = false;

        //[Description("Check player is the member of the Team:\n" +
        //    "True: If the player is not a team member then an action stops ('Position' is ignored).\n" +
        //    "False: If the player is not a team member the player move to the 'Position'")]
        //[Category("Entity")]
        //public bool PartyCheck { get; set; }

        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        public Vector3 Position { get; set; } = new Vector3();

        public MoveToLeader() { }

        public override string ActionLabel
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam
                    && !EntityManager.LocalPlayer.PlayerTeam.IsLeader)
                        return $"{GetType().Name}: {EntityManager.LocalPlayer.PlayerTeam.Team.Leader.InternalName}";
                return GetType().Name;
            }
        }

        public override bool NeedToRun
        {
            get
            {
                if (InternalDestination.Distance3DFromPlayer > Distance )
                {
                    if(IgnoreCombat)
                        Astral.Quester.API.IgnoreCombat = true;
                    return false;
                }
                else
                {
                    if (IgnoreCombat)
                        Astral.Quester.API.IgnoreCombat = false;
                    return true;
                }
            }
        }

        public override ActionResult Run()
        {
            if (InternalDestination.Distance3DFromPlayer <= Distance)
            {
                if (StopOnApproached)
                    return ActionResult.Completed;
                else return ActionResult.Running;
            }
            else
            {
                if (IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = true;
                return ActionResult.Running;
            }
        }

        protected override bool IntenalConditions
        {
            get
            {
                return Position.IsValid
                        || (EntityManager.LocalPlayer.PlayerTeam.IsInTeam
                            && !EntityManager.LocalPlayer.PlayerTeam.IsLeader);
            }
        }
        public override void OnMapDraw(GraphicsNW graph)
        {
            graph.drawFillEllipse(InternalDestination, new Size(10, 10), Brushes.Beige);
        }
        public override void InternalReset() { }
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => true;
        protected override Vector3 InternalDestination
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam
                    && !EntityManager.LocalPlayer.PlayerTeam.IsLeader)
                {
                    return EntityManager.LocalPlayer.PlayerTeam.Team?.Leader?.Entity.Location.Clone();
                }
                return Position.Clone();
            }
        }
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (Position.IsValid)
                    return new ActionValidity();
                else return new ActionValidity($"Property '{nameof(Position)}' is not set.");
            }
        }
        public override void GatherInfos()
        {
            XtraMessageBox.Show(/*Form.ActiveForm, */"Place the character on the default waypoint and press OK");
            Position = EntityManager.LocalPlayer.Location.Clone();
        }
    }
}
