using Astral.Classes;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace EntityTools.Actions
{
    [Serializable]
    public class ChangePlutionSettings : Astral.Quester.Classes.Action
    {
        public ChangePlutionSettings() { }

        public override string ActionLabel
        {
            get
            {
                return string.Empty;
            }
        }

        public override bool NeedToRun
        {
            get
            {
                return false;
            }
        }

        public override ActionResult Run()
        {

            return ActionResult.Skip;
        }

        protected override bool IntenalConditions
        {
            get
            {
                return false;
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
                return new Vector3();
            }
        }
        protected override ActionValidity InternalValidity
        {
            get
            {
                return new ActionValidity();
            }
        }
        public override void GatherInfos() { }
    }
}
