using Astral.Classes.ItemFilter;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Sign = Astral.Logic.UCC.Ressources.Enums.Sign;

namespace EntityTools.Tools
{
    [Serializable]
    public class AuraOption
    {
        [Description("An Identifier of the Aura which is checked on the the Entity")]
        [Editor(typeof(AuraIdEditor), typeof(UITypeEditor))]
        public string AuraName
        {
            get => auraId;
            set
            {
                if(auraId != value)
                {
                    patternPos = CommonTools.GetSimplePatternPos(value, out auraPattern);
                    auraId = value;
                }
            }
        }

        public ItemFilterStringType AuraNameType { get; set; } = ItemFilterStringType.Simple;

        public int Stacks { get; set; } = 0;

        public Sign Sign { get; set; } = Sign.Superior;

        [NonSerialized]
        private string auraId;
        [NonSerialized]
        private string auraPattern;
        [NonSerialized]
        private SimplePatternPos patternPos = SimplePatternPos.None;

        [XmlIgnore]
        [Browsable(false)]
        public Predicate<Entity> Checker
        {
            get
            {
                if (!string.IsNullOrEmpty(auraId))
                    if(Sign == Sign.Superior)
                        switch (AuraNameType)
                        {
                            case ItemFilterStringType.Simple:
                                return AuraCheck_SimpleSuperrior;
                            case ItemFilterStringType.Regex:
                                return AuraCheck_RegexSuperrior;
                        }
                    else switch (AuraNameType)
                        {
                            case ItemFilterStringType.Simple:
                                return AuraCheck_Simple;
                            case ItemFilterStringType.Regex:
                                return AuraCheck_Regex;
                        }
                return null;
            }
        }

        #region Методы_сравнения
        private bool AuraCheck_SimpleSuperrior(Entity e)
        {
            int num = 0;
            foreach(var mod in e.Character.Mods)
            {
                if(CommonTools.SimpleMaskTextComparer(mod.PowerDef.InternalName, patternPos, auraPattern))
                {
                    num++;
                    if (num > Stacks)
                        return true;
                }
            }
            return num > Stacks;
        }

        private bool AuraCheck_Simple(Entity e)
        {
            int num = e.Character.Mods.Count(m => CommonTools.SimpleMaskTextComparer(m.PowerDef.InternalName, patternPos, auraPattern));
            switch (Sign)
            {
                case Sign.Equal:
                    return num == Stacks;
                case Sign.NotEqual:
                    return num != Stacks;
                case Sign.Inferior:
                    return num < Stacks;
                case Sign.Superior:
                    return num > Stacks;
            }
            return false;
        }

        private bool AuraCheck_RegexSuperrior(Entity e)
        {
            int num = 0;
            foreach (var mod in e.Character.Mods)
            {
                if (Regex.IsMatch(mod.PowerDef.InternalName, auraPattern))
                {
                    num++;
                    if (num > Stacks)
                        return true;
                }
            }
            return num > Stacks;
        }

        private bool AuraCheck_Regex(Entity e)
        {
            int num = e.Character.Mods.Count(m => Regex.IsMatch(m.PowerDef.InternalName, auraPattern));
            switch (Sign)
            {
                case Sign.Equal:
                    return num == Stacks;
                case Sign.NotEqual:
                    return num != Stacks;
                case Sign.Inferior:
                    return num < Stacks;
                case Sign.Superior:
                    return num > Stacks;
            }
            return false;
        }
        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(auraId))
                return "Empty";
            else return $"{auraId} {Sign} {Stacks}";
        }
    }
}
