using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.States
{
    [Serializable]
    /// <summary>
    /// Настройки UnstuckSpellTask
    /// </summary>
    public class UnstuckSpellTaskSettings : Tools.PluginSettingsBase
    {
        public bool Active { get => UnstuckSpellTask.Active; set => UnstuckSpellTask.Active = value; }
    }
}
