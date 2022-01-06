using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MyNW.Classes;

namespace EntityTools.Tools.Export
{
    [Serializable]
    public class AuraInfo : IComparable, IEquatable<AuraInfo>
    {
        public string InternalName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public AuraInfo() { }

        public AuraInfo(PowerDef def)
        {
            if (def != null && def.IsValid)
            {
                InternalName = def.InternalName;
                DisplayName = def.DisplayName;
                Description = Regex.Replace(def.Description, "<[^>]+>", string.Empty);
            }
        }

        public override string ToString()
        {
            return $"{DisplayName} [{InternalName}]";
        }

        public bool Equals(AuraInfo aura)
        {
            return string.Equals(InternalName, aura?.InternalName, StringComparison.Ordinal);
        }
        public bool Equals(AttribModNet mod)
        {
            return string.Equals(InternalName, mod?.PowerDef.InternalName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case AuraInfo aura:
                    return Equals(aura);
                case AttribModNet mod:
                    return Equals(mod);
                default:
                    return false;
            }
        }

        public int CompareTo(object obj)
        {
            if(obj is AuraInfo aura)
            {
                return string.Compare(InternalName, aura.InternalName, StringComparison.Ordinal);
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
        public List<AuraInfo> Mods { get; private set; } = new List<AuraInfo>();

        public AurasWrapper() { }

        public AurasWrapper(Character character)
        {
            if(character != null && character.IsValid)
            {
                var source = character.Mods;
                Mods = new List<AuraInfo>(source.Count);
                foreach (var def in source)
                {
                    var pDef = def.PowerDef;
                    if (pDef.IsValid)
                        Mods.Add(new AuraInfo(pDef));
                }
            }
            else Mods = new List<AuraInfo>();
        }
    }

}
