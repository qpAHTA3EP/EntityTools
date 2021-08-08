using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки монитора <seealso cref="Forms.MissionMonitorForm"/>
    /// </summary>
    [Serializable]
    public class MissionMonitorSettings : PluginSettingsBase
    {
        /// <summary>
        /// Настройки периода времени между проверками обновления свойств <seealso cref="MyNW.Classes.Mission"/>
        /// </summary>
        [Description("Настройки периода времени между проверками обновления свойств (мс).\n" +
                     "Минимальное значение - 500 мс.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public int UpdateTimeout 
        { 
            get => _timeout;
            set
            {
                if (value < 500)
                    _timeout = 500;
                else _timeout = value;
                NotifyPropertyChanged();
            }
        }
        private int _timeout = 1_000;

        /// <summary>
        /// Настройки отображения свойств <seealso cref="MyNW.Classes.Mission"/> в окне монитора <seealso cref="Forms.MissionMonitorForm"/>
        /// </summary>
        [Description("Настройки отображения свойств Mission в мониторе")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MissionTreeNodeSettings Mission { get; set; } = new MissionTreeNodeSettings();

        /// <summary>
        /// Настройки отображения свойств <seealso cref="MyNW.Classes.MissionDef"/> в окне монитора <seealso cref="Forms.MissionMonitorForm"/>
        /// </summary>
        [Description("Настройки отображения свойств MissionDef в мониторе")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MissionDefTreeNodeSettings MissionDef { get; set; } = new MissionDefTreeNodeSettings();

        public override string ToString()
        {
            return nameof(MissionMonitorSettings);
        }
    }
}
