using EntityTools.Settings;
using EntityTools.Services;
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
        public UnstuckSpellsSettings UnstuckSpells { get; set; } = new UnstuckSpellsSettings();

        /// <summary>
        /// Настройки Mapper'a
        /// </summary>
        public MapperSettings Mapper { get; set; } = new MapperSettings();
    }
}
