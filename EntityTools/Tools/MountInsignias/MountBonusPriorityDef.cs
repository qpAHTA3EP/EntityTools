using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EntityTools.Tools.MountInsignias
{
    /// <summary>
    /// Определение элемента списка приоритета Бонусоов скакунов
    /// </summary>
    [Serializable]
    public class MountBonusPriorityDef
    {
        public int Number { get; set; } = 0;

        public string InternalName
        {
            get => Bonus.InternalName;
            set
            {
                if(Bonus.InternalName != value)
                {
                    // Bonus = MountBonusDef.GetBonusByInternalName(value);
                }
            }
        }
        [XmlIgnore]
        public MountBonusDef Bonus { get; set; } = new MountBonusDef();

        [XmlIgnore]
        public string Name { get => Bonus.Name; }
        [XmlIgnore]
        public string Description { get => Bonus.Description; }
    }
}
