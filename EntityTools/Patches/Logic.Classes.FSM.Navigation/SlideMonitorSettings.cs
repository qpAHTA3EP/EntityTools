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
        /// Интервал проверки залипания умений (милисекунд)
        /// </summary>
        [Bindable(true)]
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
        /// Интервал проверки залипания умений (милисекунд)
        /// </summary>
        [Bindable(true)]
        public int Filter
        {
            get => filter;
            set
            {
                if (filter != value)
                {
                    filter = Math.Max(value, 40);
                    NotifyPropertyChanged(nameof(Filter));
                }
            }
        }
        //[NonSerialized]
        private int filter = 60;
    }
}
