using System;
using Astral;
using Astral.Logic.Classes.Map;
using EntityTools.Enums;
using MyNW.Classes;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class ChangePluginSetting : Action
    {
        public PluginSettingsCommand Command {get; set;}

        public string Value { get; set; }

        public override bool NeedToRun => true;

        public override ActionResult Run()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                bool boolResult;
                switch (Command)
                {
                    case PluginSettingsCommand.DisableSlideMonitor:
#if true
                        if (Enum.TryParse(Value, out SlideMonitorState slideState))
                        {
                            EntityTools.Config.SlideMonitor.State = slideState;
                            return ActionResult.Completed;
                        }
                        else return ActionResult.Fail;
#else
                            Logger.WriteLine(Logger.LogType.Debug, "SlideMonitor was removed. This command is deprecated");
                            return ActionResult.Skip; 
#endif
                    //case PluginSettingsCommand.DisableSpellStuckMonitor:
                    //    if (bool.TryParse(Value, out result))
                    //    {
                    //        SpellStuckMonitor.Activate = !result;
                    //        return ActionResult.Completed;
                    //    }
                    //    else return ActionResult.Fail;
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        if (bool.TryParse(Value, out boolResult))
                        {
                            EntityTools.Config.UnstuckSpells.Active = !boolResult;
                            return ActionResult.Completed;
                        }
                        else return ActionResult.Fail;
                    case PluginSettingsCommand.EntityCacheTime:
                        //if (int.TryParse(Value, out int timer))
                        //{
                        //    if(timer >= 1)
                        //        EntityCache.ChacheTime = timer;
                        //}
                        //return ActionResult.Fail;
                        throw new NotImplementedException("PluginSettingsCommand.EntityCacheTime");                        
                    case PluginSettingsCommand.EntityCacheCombatTime:
                        //if (int.TryParse(Value, out timer))
                        //{
                        //    if (timer >= 1)
                        //        EntityCache.CombatChacheTime = timer;
                        //}
                        //return ActionResult.Fail;
                        throw new NotImplementedException("PluginSettingsCommand.EntityCacheCombatTime");
                }
            }
            return ActionResult.Skip;
        }

        public override string ActionLabel=> string.Empty;
        protected override bool IntenalConditions => true;
        public override void OnMapDraw(GraphicsNW graph){}
        public override void InternalReset() { }
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => false;
        protected override Vector3 InternalDestination => Vector3.Empty;
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(Value))
                    return new ActionValidity("Value does not set!");
                int intResult = -1;
                switch (Command)
                {
                    case PluginSettingsCommand.DisableSlideMonitor:
#if true                        
                        if (!Enum.TryParse(Value, out SlideMonitorState slideState))
                            return new ActionValidity("Value is incorrect!\n" +
                                "The boolean is required.");
                        break;
#else
                        return new ActionValidity("SlideMonitor was removed. This command is deprecated"); 
#endif
                    //case PluginSettingsCommand.DisableSpellStuckMonitor:
                    //    if (!bool.TryParse(Value, out result))
                    //        return new ActionValidity("Value is incorrect!\n" +
                    //            "The boolean is required.");
                    //    break;
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        if (!bool.TryParse(Value, out _))
                            return new ActionValidity("Value is incorrect!\n" +
                                "The boolean is required.");
                        break;
                    case PluginSettingsCommand.EntityCacheTime:
                        if (!int.TryParse(Value, out intResult) || intResult < 1)
                            return new ActionValidity("Value is incorrect!\n" +
                                "The positive integer greter 1 is required.");
                        break;
                    case PluginSettingsCommand.EntityCacheCombatTime:
                        if (!int.TryParse(Value, out intResult) || intResult < 1)
                            return new ActionValidity("Value is incorrect!\n" +
                                "The positive integer greter 1 is required.");
                        break;
                }
                return new ActionValidity();
            }
        }
        public override void GatherInfos() { }

        public override string ToString()
        {
            return $"{GetType().Name} : {Command} => {Value}";
        }
    }
}
