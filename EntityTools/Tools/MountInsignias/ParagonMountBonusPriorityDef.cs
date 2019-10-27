using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EntityTools.Tools.MountInsignias
{
    [Serializable]
    public class ParagonMountBonusPriorityDef
    {
        private PlayerClassParagonType clPrgType = PlayerClassParagonType.Unknown;
        public PlayerClassParagonType ClassParagonType
        {
            get => clPrgType;
            set
            {
                if(clPrgType != value)
                {
                    clPrgType = value;
                    dispName = string.Empty;
                }
            }
        }
        [XmlIgnore]
        private string dispName = string.Empty;
        [XmlIgnore]
        public string DisplayName
        {
            get
            {
                if (ClassParagonType == PlayerClassParagonType.Unknown)
                    return dispName = PlayerClassParagonType.Unknown.ToString();

                if (!string.IsNullOrEmpty(dispName))
                    return dispName;

                CharacterPath paragon = Game.CharacterPaths.Find(p => p.Name == ClassParagonType.ToString() );
                if(paragon == null || !paragon.IsValid)
                    return dispName = PlayerClassParagonType.Unknown.ToString();

                if (paragon.RequiredClasses.Count > 0)
                    return dispName  = $"{paragon.RequiredClasses[0].Class.DisplayName}: {paragon.DisplayName}";
                else
                    return dispName = paragon.DisplayName;
            }
        }

        public BindingList<MountBonusPriorityDef> MountBonusPriorityList { get; set; } = new BindingList<MountBonusPriorityDef>();

        public ParagonMountBonusPriorityDef() { }
        public ParagonMountBonusPriorityDef(PlayerClassParagonType type)
        {
            ClassParagonType = type;
        }
    }
}
