using EntityTools.Enums;
using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки службы SlideMonitor
    /// Выключающей залипшие умения
    /// </summary>
    [Serializable]
    public class SlideMonitorSettings : PluginSettingsBase
    {
        /// <summary>
        /// Активация или деактивация службы
        /// </summary>
        [Bindable(true)]
        public SlideMonitorState State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    NotifyPropertyChanged(nameof(State));
                }
            }
        }
        private SlideMonitorState state = SlideMonitorState.Disabled;

        /// <summary>
        /// Период обновления расстояния до целевой путевой точки
        /// </summary>
        [Bindable(true)]
        [Description("Период обновления расстояния до целевой путевой точки")]
        public int Timeout
        {
            get => timeout;
            set
            {
                if (timeout != value)
                {
                    timeout = Math.Max(value, 100);
                    NotifyPropertyChanged(nameof(Timeout));
                }
            }
        }
        //[NonSerialized]
        private int timeout = 250;

        /// <summary>
        /// Расстояние до целевой путевой точки при 'скольжении' (на лодке или ледяной поверхности)
        /// </summary>
        [Bindable(true)]
        [Description("Расстояние до целевой путевой точки при 'скольжении' (на лодке или ледяной поверхности)")]
        public int BoatFilter
        {
            get => boatFilter;
            set
            {
                if (boatFilter != value)
                {
                    boatFilter = Math.Max(value, 40);
                    NotifyPropertyChanged(nameof(BoatFilter));
                }
            }
        }
        //[NonSerialized]
        private int boatFilter = 60;

        /// <summary>
        /// Расстояние до целевой путевой точки при 'скольжении' (на лодке или ледяной поверхности)
        /// </summary>
        [Bindable(true)]
        [Description("Расстояние до целевой путевой точки при езде на 'Адской машине'")]
        public int InfernalMachineFilter
        {
            get => infernalMachineFilter;
            set
            {
                if (infernalMachineFilter != value)
                {
                    infernalMachineFilter = Math.Max(value, 40);
                    NotifyPropertyChanged(nameof(InfernalMachineFilter));
                }
            }
        }
        //[NonSerialized]
        private int infernalMachineFilter = 400;

        public override string ToString()
        {
            return state.ToString();
        }
    }
}
