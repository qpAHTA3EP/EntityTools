using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityPlugin.Tools
{
    [Serializable]
    public class Aura
    {
        public string InternalName;
        public string DisplayName;
        public string Description;

        public Aura()
        {
            InternalName = string.Empty;
            DisplayName = string.Empty;
            Description = string.Empty;
        }

        public Aura(PowerDef def)
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
    }

    [Serializable]
    public class AurasWrapper
    {
        public List<Aura> Mods;
        public List<Aura> Buffs;

        public AurasWrapper()
        {
            Mods = new List<Aura>();
            Buffs = new List<Aura>();
        }

        public AurasWrapper(Character character)
        {
            Mods = new List<Aura>();
            Buffs = new List<Aura>();

            if(character != null && character.IsValid)
            {
                foreach (AttribModNet def in character.Mods)
                    if (def.PowerDef.IsValid)
                        Mods.Add(new Aura(def.PowerDef));

                // Исключено в Astral64
                //foreach(AttribModNet def in character.Buffs)
                //    if(def.PowerDef.IsValid)
                //        Mods.Add(new Aura(def.PowerDef));
            }
        }
    }

}
