using EntityTools.Daemons;
using System;

namespace EntityTools.Settings
{
    /// <summary>
    /// Настройки демона UnstuckSpell
    /// </summary>
    [Serializable]
    public class UnstuckSpellsDaemonSettings : PluginSettingsBase
    {
        [NonSerialized]
        private bool active = true;
        public bool Active
        {
            get => active;
            set
            {
                if (active != value)
                {
                    base.NotifyPropertyChanged(nameof(Active));
                    active = value;
                }
            }
        }
    }
}
