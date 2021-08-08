using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MyNW.Classes;

namespace EntityTools.Tools
{
    [Serializable]
    public class AuraInfo : IComparable, IEquatable<AuraInfo>
    {
        public string InternalName;
        public string DisplayName;
        public string Description;

        public AuraInfo()
        {
            InternalName = string.Empty;
            DisplayName = string.Empty;
            Description = string.Empty;
        }

        public AuraInfo(PowerDef def)
        {
            if(def != null && def.IsValid)
            {
                InternalName = def.InternalName;
                DisplayName = def.DisplayName;
                Description = Regex.Replace(def.Description, "<[^>]+>", string.Empty);
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

        public bool Equals(AuraInfo aura)
        {
            return InternalName == aura.InternalName;
        }
        public bool Equals(AttribModNet mod)
        {
            return InternalName == mod.PowerDef.InternalName;
        }

        public override bool Equals(object obj)
        {
            if (obj is AuraInfo aura)
                return Equals(aura);
            if (obj is AttribModNet mod)
                return Equals(mod);
            return false;
        }

        public int CompareTo(object obj)
        {
            if(obj is AuraInfo aura)
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
        public List<AuraInfo> Mods = new List<AuraInfo>();

        public AurasWrapper() { }

        public AurasWrapper(Character character)
        {
            Mods = new List<AuraInfo>();

            if(character != null && character.IsValid)
            {
                foreach (var def in character.Mods) // х64
                    if (def.PowerDef.IsValid)
                            Mods.Add(new AuraInfo(def.PowerDef));
            }
        }
    }

}
