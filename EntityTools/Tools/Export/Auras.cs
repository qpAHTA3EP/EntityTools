using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Tools
{
    [Serializable]
    public class AuraDef : IComparable, IEquatable<AuraDef>
    {
        public string InternalName;
        public string DisplayName;
        public string Description;

        public AuraDef()
        {
            InternalName = string.Empty;
            DisplayName = string.Empty;
            Description = string.Empty;
        }

        public AuraDef(PowerDef def)
        {
            if(def != null && def.IsValid)
            {
                InternalName = def.InternalName;
                DisplayName = def.DisplayName;
                Description = def.Description;
            }
            else
            {
                InternalName = string.Empty;
                DisplayName = string.Empty;
                Description = string.Empty;
            }
        }

        public override string ToString()
        {
            return $"{DisplayName} [{InternalName}]";
        }

        public bool Equals(AuraDef aura)
        {
            return InternalName == aura.InternalName;
        }
        public bool Equals(AttribModNet mod)
        {
            return InternalName == mod.PowerDef.InternalName;
        }

        public override bool Equals(object obj)
        {
            if (obj is AuraDef aura)
                return Equals(aura);
            else if (obj is AttribModNet mod)
                return Equals(mod);
            return false;
        }

        public int CompareTo(object obj)
        {
            if(obj is AuraDef aura)
            {
                return InternalName.CompareTo(aura.InternalName);
            }
            return -1;
        }

        public override int GetHashCode()
        {
            return -676643589 + EqualityComparer<string>.Default.GetHashCode(InternalName);
        }
    }

    [Serializable]
    public class AurasWrapper
    {
        public List<AuraDef> Mods = new List<AuraDef>();

        public AurasWrapper() { }

        public AurasWrapper(Character character)
        {
            Mods = new List<AuraDef>();

            if(character != null && character.IsValid)
            {
                foreach (AttribModNet def in character.Mods) // х64
                    if (def.PowerDef.IsValid)
                            Mods.Add(new AuraDef(def.PowerDef));
            }
        }
    }

}
