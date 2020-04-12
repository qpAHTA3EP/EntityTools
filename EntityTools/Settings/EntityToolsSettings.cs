using EntityTools.Settings;
using EntityTools.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityTools.Logger;

namespace EntityTools
{
    [Serializable]
    public class EntityToolsSettings
    {
        /// <summary>
        /// Настройки UnstuckSpellTask
        /// </summary>
        public UnstuckSpellsSettings UnstuckSpells { get; set; } = new UnstuckSpellsSettings();

        /// <summary>
        /// Настройки Mapper'a
        /// </summary>
        public MapperSettings Mapper { get; set; } = new MapperSettings();

        /// <summary>
        /// Настройки EntityToolsLogger
        /// </summary>
        public EntityToolLoggerSettings Logger { get; set; } = new EntityToolLoggerSettings();

        /// <summary>
        /// Настройки EntityCache
        /// </summary>
        public EntityCacheSettings EntityCache { get; set; } = new EntityCacheSettings();
    }
}
