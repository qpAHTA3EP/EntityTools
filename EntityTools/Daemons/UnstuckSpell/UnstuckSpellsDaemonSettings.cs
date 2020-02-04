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
        public bool Active
        {
            get => UnstuckSpells.Active;
            set
            {
                if (UnstuckSpells.Active != value)
                {
                    base.NotifyPropertyChanged(nameof(Active));
                    UnstuckSpells.Active = value;
                }
            }
        }
    }
}
