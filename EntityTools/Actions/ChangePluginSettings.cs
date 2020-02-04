using Astral;
using Astral.Classes;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Enums;
using EntityTools.Daemons;
using EntityTools.Tools.Entities;
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
                            //SlideMonitor.Activate = !result;
                            Logger.WriteLine(Logger.LogType.Debug, "SlideMonitor was removed. This command is deprecated");
                            return ActionResult.Skip;
                        }
                        else return ActionResult.Fail;
                    //case PluginSettingsCommand.DisableSpellStuckMonitor:
                    //    if (bool.TryParse(Value, out result))
                    //    {
                    //        SpellStuckMonitor.Activate = !result;
                    //        return ActionResult.Completed;
                    //    }
                    //    else return ActionResult.Fail;
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        if (bool.TryParse(Value, out result))
                        {
                            UnstuckSpells.Active = !result;
                            return ActionResult.Completed;
                        }
                        else return ActionResult.Fail;
                    case PluginSettingsCommand.EntityCacheTime:
                        if (int.TryParse(Value, out int timer))
                        {
                            if(timer >= 1)
                                EntityCache.ChacheTime = timer;
                        }
                        return ActionResult.Fail;
                    case PluginSettingsCommand.EntityCacheCombatTime:
                        if (int.TryParse(Value, out timer))
                        {
                            if (timer >= 1)
                                EntityCache.CombatChacheTime = timer;
                        }
                        return ActionResult.Fail;
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
                        /*if (!bool.TryParse(Value, out bool result))
                            return new ActionValidity("Value is incorrect!\n" +
                                "The boolean is required.");
                        break;*/
                        return new ActionValidity("SlideMonitor was removed. This command is deprecated");
                    //case PluginSettingsCommand.DisableSpellStuckMonitor:
                    //    if (!bool.TryParse(Value, out result))
                    //        return new ActionValidity("Value is incorrect!\n" +
                    //            "The boolean is required.");
                    //    break;
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        if (!bool.TryParse(Value, out bool result))
                            return new ActionValidity("Value is incorrect!\n" +
                                "The boolean is required.");
                        break;
                    case PluginSettingsCommand.EntityCacheTime:
                        if (!int.TryParse(Value, out int timer) || timer < 1)
                            return new ActionValidity("Value is incorrect!\n" +
                                "The positive integer greter 1 is required.");
                        break;
                    case PluginSettingsCommand.EntityCacheCombatTime:
                        if (!int.TryParse(Value, out timer))
                            return new ActionValidity("Value is incorrect!\n" +
                                "The positive integer greter 1 is required.");
                        break;
                }
                return new ActionValidity();
            }
        }
        public override void GatherInfos() { }
    }
}
