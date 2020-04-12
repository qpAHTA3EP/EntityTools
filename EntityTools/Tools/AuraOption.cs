using Astral.Classes.ItemFilter;
using EntityTools.Extensions;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Sign = Astral.Logic.UCC.Ressources.Enums.Sign;

namespace EntityTools.Tools
{
    [Serializable]
    public class AuraOption
    {
#if DEVELOPER
        [Description("An Identifier of the Aura which is checked on the the Entity")]
        [Editor(typeof(AuraIdEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif   
        public string AuraName
        {
            get => _auraId;
            set
            {
                if (_auraId != value)
                {
                    _auraId = value;
                    patternPos = value.GetSimplePatternPosition(out auraPattern);
                }
            }
        }
        internal string _auraId;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public ItemFilterStringType AuraNameType { get; set; } = ItemFilterStringType.Simple;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public int Stacks
        {
            get => _stacks; set
            {
                _stacks = value;
            }
        }
        internal int _stacks = 0;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public Sign Sign
        {
            get => _sign; set
            {
                _sign = value;
            }
        }
        internal Sign _sign = Sign.Superior;

        private string auraPattern;
        private SimplePatternPos patternPos = SimplePatternPos.None;

        [XmlIgnore]
        [Browsable(false)]
        public Predicate<Entity> Checker
        {
            get
            {
                if (!string.IsNullOrEmpty(_auraId))
                    if (Sign == Sign.Superior)
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
            foreach (var mod in e.Character.Mods)
            {
                if (mod.PowerDef.InternalName.CompareToSimplePattern(patternPos, auraPattern))
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
            int num = e.Character.Mods.Count(m => m.PowerDef.InternalName.CompareToSimplePattern(patternPos, auraPattern));
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
            if (string.IsNullOrEmpty(_auraId))
                return "Empty";
            else return $"{_auraId} {Sign} {Stacks}";
        }
    }
}
