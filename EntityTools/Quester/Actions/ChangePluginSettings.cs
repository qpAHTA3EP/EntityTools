using Infrastructure.Reflection;
using Astral.Logic.Classes.Map;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.ComponentModel;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class ChangePluginSetting : Action
    {
        private PluginSettingsCommand command;

        public PluginSettingsCommand Command
        {
            get => command;
            set
            {
                if (command != value)
                {
                    command = value;
                    Value = string.Empty;
                }
            }
        }

        [TypeConverter(typeof(ChangePluginSettingValueEditor))]
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
                        if (Enum.TryParse(Value, out SlideMonitorState slideState))
                        {
                            EntityTools.Config.SlideMonitor.State = slideState;
                            return ActionResult.Completed;
                        }
                        else return ActionResult.Fail;
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        if (bool.TryParse(Value, out boolResult))
                        {
                            EntityTools.Config.UnstuckSpells.Active = !boolResult;
                            return ActionResult.Completed;
                        }
                        else return ActionResult.Fail;
                    case PluginSettingsCommand.EntityCacheTime:
                        if (int.TryParse(Value, out int timer))
                        {
                            if (timer >= 1)
                                EntityTools.Config.EntityCache.GlobalCacheTime = timer;
                        }
                        return ActionResult.Fail;
                    case PluginSettingsCommand.EntityCacheCombatTime:
                        if (int.TryParse(Value, out timer))
                        {
                            if (timer >= 1)
                                EntityTools.Config.EntityCache.CombatCacheTime = timer;
                        }
                        return ActionResult.Fail;
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
                        if (!Enum.TryParse(Value, out SlideMonitorState slideState))
                            return new ActionValidity("Value is incorrect!\n" +
                                "The boolean is required.");
                        break;
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        if (!bool.TryParse(Value, out _))
                            return new ActionValidity("Value is incorrect!\n" +
                                "The boolean is required.");
                        break;
                    case PluginSettingsCommand.EntityCacheTime:
                        if (!int.TryParse(Value, out intResult) || intResult < 1)
                            return new ActionValidity("Value is incorrect!\n" +
                                "The positive integer greater then 1 is required.");
                        break;
                    case PluginSettingsCommand.EntityCacheCombatTime:
                        if (!int.TryParse(Value, out intResult) || intResult < 1)
                            return new ActionValidity("Value is incorrect!\n" +
                                "The positive integer greater then 1 is required.");
                        break;
                }
                return Empty.ActionValidity;
            }
        }
        public override void GatherInfos() { }

        public override string ToString()
        {
            return $"{GetType().Name} : {Command} => {Value}";
        }
    }
}
