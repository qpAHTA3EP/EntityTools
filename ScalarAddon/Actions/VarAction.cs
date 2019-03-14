using System;
using System.Collections.Generic;
using Astral.Forms;
using MyNW.Classes;
using System.ComponentModel;
using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using System.Drawing.Design;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using AstralVars.Classes;

namespace AstralVars
{
    public class VarAction : Astral.Quester.Classes.Action
    {
        [Editor(typeof(variableUiEditor), typeof(UITypeEditor))]
        [Description("Идентификатор переменной")]
        public Variable VariableName { get; set; }

        [Description("Значение переменной")]
        public string Equation { get; set; } = string.Empty;

        public override string ActionLabel => GetType().Name;

        public override bool NeedToRun => true;

        public override string InternalDisplayName => String.Empty;

        public override bool UseHotSpots => false;

        protected override bool IntenalConditions => true;

        protected override Vector3 InternalDestination => new Vector3();

        protected override ActionValidity InternalValidity => new ActionValidity();

        public override void GatherInfos()
        {
        }

        public override void InternalReset()
        {
        }

        public override void OnMapDraw(GraphicsNW graph)
        {
        }

        public override ActionResult Run()
        {
            //if (VariablesAddon.Variables.Set(Name, Value))
            //{
            //    Astral.Logger.WriteLine(ActionLabel + ':' + Name + " set to " + Value);
            //    return Astral.Quester.Classes.Action.ActionResult.Completed;
            //}
            //else
            //{
            //    Astral.Logger.WriteLine(ActionLabel + ':' + Name + " not set to " + Value);
            //    return Astral.Quester.Classes.Action.ActionResult.Fail;
            //}
            return Astral.Quester.Classes.Action.ActionResult.Completed;
        }
    }
}