using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using EntityTools.Editors;
using MyNW.Classes;
using AstralSign = Astral.Logic.UCC.Ressources.Enums.Sign;

namespace EntityTools.Tools
{
    [Serializable]
    public class AuraOption
    {
        Predicate<string> _idComparer;

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
                _auraId = value;
                _idComparer = _auraId.GetComparer(_auraNameType);
                _label = string.Empty;
            }
        }
        internal string _auraId;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public ItemFilterStringType AuraNameType
        {
            get => _auraNameType;
            set
            {
                _auraNameType = value;
                _idComparer = _auraId.GetComparer(_auraNameType);
            }
        }
        private ItemFilterStringType _auraNameType = ItemFilterStringType.Simple;
#if !DEVELOPER
        [Browsable(false)]
#endif
        public int Stacks
        {
            get => _stacks;
            set
            {
                _stacks = value;
                _label = string.Empty;
            }
        }
        internal int _stacks;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public AstralSign Sign
        {
            get => _sign;
            set
            {
                _sign = value;
                _checker = null;
                _label = string.Empty;
            }
        }
        internal AstralSign _sign = AstralSign.Superior;

        /// <summary>
        /// Предикат, проверяющий наличие заданной ауры на <seealso cref="Entity"/>
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public Predicate<Entity> IsMatch
        {
            get
            {
                if (!string.IsNullOrEmpty(_auraId))
                {
                    if (_checker is null)
                    {
                        if (_idComparer is null)
                            _idComparer = _auraId.GetComparer(_auraNameType);
                        switch (Sign)
                        {
                            case AstralSign.Inferior:
                                _checker = AuraCheck_Inferior;
                                break;
                            case AstralSign.Superior:
                                _checker = AuraCheck_Superior;
                                break;
                            case AstralSign.Equal:
                                _checker = AuraCheck_Equal;
                                break;
                            case AstralSign.NotEqual:
                                _checker = AuraCheck_NotEqual;
                                break;
                        }
                    }
                    return _checker;
                }
                return null;
            }
        }
        Predicate<Entity> _checker;

        #region Методы_сравнения
        private bool AuraCheck_Superior(Entity e)
        {
            int num = 0;
            foreach (var mod in e.Character.Mods)
            {
                if (_idComparer(mod.PowerDef.InternalName))
                {
                    num++;
                    if (num > _stacks)
                        return true;
                }
            }
            return num > Stacks;
        }
        private bool AuraCheck_Inferior(Entity e)
        {
            int num = 0;
            foreach (var mod in e.Character.Mods)
            {
                if (_idComparer(mod.PowerDef.InternalName))
                {
                    num++;
                    if (num >= _stacks)
                        return false;
                }
            }
            return num < _stacks;
        }
        private bool AuraCheck_Equal(Entity e)
        {
            int num = 0;
            foreach (var mod in e.Character.Mods)
            {
                if (_idComparer(mod.PowerDef.InternalName))
                {
                    num++;
                    if (num > _stacks)
                        return false;
                }
            }
            return num == _stacks;
        }
        private bool AuraCheck_NotEqual(Entity e)
        {
            int num = 0;
            foreach (var mod in e.Character.Mods)
            {
                if (_idComparer(mod.PowerDef.InternalName))
                    num++;
            }
            return num != _stacks;
        }
        #endregion

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_label))
            {
                if (string.IsNullOrEmpty(_auraId))
                    _label = "Empty";
                else _label = $"{_auraId} {_sign} {_stacks}"; 
            }
            return _label;
        }
        string _label;
    }
}
