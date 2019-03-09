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

namespace ScalarAddon
{
    public class ScalarAction : Astral.Quester.Classes.Action
{
    [Editor(typeof(ScalarNamesEditor), typeof(UITypeEditor))]
    [Description("Имя переменной")]
    public string VariableName { get; set; } = String.Empty;

    [Description("Величина сопоставляемая со значением переменной")]
    public int Value { get; set; } = 0;

    public override string ActionLabel => "ScalarAction";

    public override bool NeedToRun => true;

    public override string InternalDisplayName => "ScalarAction";

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
        if (AstralScalars.SetValue(VariableName, Value))
        {
            Astral.Logger.WriteLine(ActionLabel + ':' + VariableName + " set to " + Value);
            return Astral.Quester.Classes.Action.ActionResult.Completed;
        }
        else
        {
            Astral.Logger.WriteLine(ActionLabel + ':' + VariableName + " not set to " + Value);
            return Astral.Quester.Classes.Action.ActionResult.Fail;
        }
    }
}
}