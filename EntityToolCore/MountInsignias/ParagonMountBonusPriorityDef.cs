using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EntityTools.Enums;

namespace EntityCore.MountInsignias
{
    [Serializable]
    public class ParagonMountBonusPriorityDef
    {
        private PlayerParagonType clPrgType = PlayerParagonType.Unknown;
        public PlayerParagonType ParagonType
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
                if (ParagonType == PlayerParagonType.Unknown)
                    return dispName = PlayerParagonType.Unknown.ToString();

                if (!string.IsNullOrEmpty(dispName))
                    return dispName;

                CharacterPath paragon = Game.CharacterPaths.Find(p => p.Name == ParagonType.ToString() );
                if(paragon == null || !paragon.IsValid)
                    return dispName = PlayerParagonType.Unknown.ToString();

                if (paragon.RequiredClasses.Count > 0)
                    return dispName  = $"{paragon.RequiredClasses[0].Class.DisplayName}: {paragon.DisplayName}";
                else
                    return dispName = paragon.DisplayName;
            }
        }

        public BindingList<MountBonusPriorityDef> MountBonusPriorityList { get; set; } = new BindingList<MountBonusPriorityDef>();

        public ParagonMountBonusPriorityDef() { }
        public ParagonMountBonusPriorityDef(PlayerParagonType type)
        {
            ParagonType = type;
        }
    }
}
