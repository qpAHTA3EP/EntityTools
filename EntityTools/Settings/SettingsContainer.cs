using EntityTools.Settings;
using EntityTools.Daemons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools
{
    [Serializable]
    public class SettingsContainer
    {
        /// <summary>
        /// Настройки UnstuckSpellTask
        /// </summary>
        public UnstuckSpellsDaemonSettings UnstuckSpells { get; set; } = new UnstuckSpellsDaemonSettings();

        /// <summary>
        /// Настройки Mapper'a
        /// </summary>
        public MapperSettings Mapper { get; set; } = new MapperSettings();
    }
}
