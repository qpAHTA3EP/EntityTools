using System;
using System.ComponentModel;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки службы UnstuckSpell
    /// Выключающей залипшие умения
    /// </summary>
    [Serializable]
    public class UnstuckSpellsSettings : PluginSettingsBase
    {
        /// <summary>
        /// Активация или деактивация умения
        /// </summary>
        [Bindable(true)]
        public bool Active
        {
            get => active;
            set
            {
                if (active != value)
                {
                    active = value;
                    NotifyPropertyChanged(nameof(Active));
                }
            }
        }
        //[NonSerialized]
        private bool active = true;

        /// <summary>
        /// Интервал проверки залипания умений (милисекунд)
        /// </summary>
        [Bindable(true)]
        public int CheckInterval
        {
            get => checkInterval;
            set
            {
                if (checkInterval != value)
                {
                    checkInterval = Math.Max(value, 500);
                    NotifyPropertyChanged(nameof(CheckInterval));
                }
            }
        }
        //[NonSerialized]
        private int checkInterval = 500;

        public override string ToString()
        {
            return active ? "Active" : "Disabled";
        }
    }
}
