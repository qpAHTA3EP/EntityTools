using EntityTools.Enums;
using EntityTools.Quester.Actions;
using System;
using System.ComponentModel;

namespace EntityTools.Editors
{
    internal class ChangePluginSettingValueEditor : StringConverter
    {
        /// <summary>
        /// Будем предоставлять выбор из списка
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// ... и только из списка
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            // false - можно вводить вручную
            // true - только выбор из списка
            if (context.Instance is ChangePluginSetting changePluginSetting)
            {
                switch (changePluginSetting.Command)
                {
                    case PluginSettingsCommand.DisableSlideMonitor:
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (context.Instance is ChangePluginSetting changePluginSetting)
            {
                switch (changePluginSetting.Command)
                {
                    case PluginSettingsCommand.DisableSlideMonitor:
                        if (value is SlideMonitorState)
                            return true;
                        return Enum.TryParse(value.ToString(), out SlideMonitorState _);
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        if (value is bool)
                            return true;
                        return Enum.TryParse(value.ToString(), out bool _);
                    case PluginSettingsCommand.EntityCacheCombatTime:
                    case PluginSettingsCommand.EntityCacheTime:
                        return Enum.TryParse(value.ToString(), out int _);
                }
            }
            return false;
        }

        /// <summary>
        /// А вот и список
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // возвращаем список строк из настроек программы
            // (базы данных, интернет и т.д.)
            if (context.Instance is ChangePluginSetting changePluginSetting)
            {
                switch (changePluginSetting.Command)
                {
                    case PluginSettingsCommand.DisableSlideMonitor:
                        return new StandardValuesCollection(Enum.GetValues(typeof(SlideMonitorState)));
                    case PluginSettingsCommand.DisableUnstuckSpell:
                        return new StandardValuesCollection(new [] { true, false });
                }
            }

            return new StandardValuesCollection(Array.Empty<object>());
        }
    }
}
