using Astral.Classes;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Enums;
using EntityTools.States;
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
    public class ChangePluginSetting : Astral.Quester.Classes.Action
    {
        public PluginSettingsCommand Command {get; set;}

        public string Value { get; set; }

        public override string ActionLabel=> $"{GetType().Name} : {Command} => {Value}";

        public override bool NeedToRun => true;

        public override ActionResult Run()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                switch (Command)
                {
                    case PluginSettingsCommand.DisableSlideMonitor:
                        if (bool.TryParse(Value, out bool result))
                        {
                            SlideMonitor.Activate = !result;
                            return ActionResult.Completed;
                        }
                        else return ActionResult.Fail;
                    case PluginSettingsCommand.DisableSpellStuckMonitor:
                        if (bool.TryParse(Value, out result))
                        {
                            SpellStuckMonitor.Activate = !result;
                            return ActionResult.Completed;
                        }
                        else return ActionResult.Fail;
                }
            }
            return ActionResult.Skip;
        }

        public ChangePluginSetting() { }

        protected override bool IntenalConditions => true;
        public override void OnMapDraw(GraphicsNW graph){}
        public override void InternalReset() { }
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => false;
        protected override Vector3 InternalDestination => new Vector3();
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(Value))
                    return new ActionValidity("Value does not set!");

                switch (Command)
                {
                    case PluginSettingsCommand.DisableSlideMonitor:
                        if (!bool.TryParse(Value, out bool result))
                            return new ActionValidity("Value is incorrect!");
                        break;
                    case PluginSettingsCommand.DisableSpellStuckMonitor:
                        if (!bool.TryParse(Value, out result))
                            return new ActionValidity("Value is incorrect!");
                        break;
                }
                return new ActionValidity();
            }
        }
        public override void GatherInfos() { }
    }
}
