using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Extensions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Элемент фильтра предметов
    /// </summary>
    [Serializable]
    public class CommonFilterEntry: IFilterEntry
    {
        #region Данные
        [Description("Тип идентификатора предмета")]
        public ItemFilterEntryType EntryType
        {
            get => _entryType;
            set
            {
                if (!_readOnly && _entryType != value)
                {
                    _entryType = value;
                    isMatchPredicate = IsMatch_Selector;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntryType)));
#else
                    PropertyChanged?.Invoke(this, nameof(EntryType));
#endif
                }
                }
            }
        internal ItemFilterEntryType _entryType = ItemFilterEntryType.Identifier;

        [Description("Способ интерпретации строки, задающей идентификатор предмета:\n\r" +
            "Simple:\tПростой текст, допускающий подстановочный символ '*' в начале и в конце\n\r" +
            "Regex:\tРегулярное выражение")]
        public ItemFilterStringType StringType
        {
            get => _stringType;
            set
            {
                if (!_readOnly && _stringType != value)
                {
                    _stringType = value;
                    isMatchPredicate = IsMatch_Selector;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringType))); 
#else
                    PropertyChanged?.Invoke(this, nameof(StringType));
#endif
                }
            }
        }
        internal ItemFilterStringType _stringType = ItemFilterStringType.Simple;

        /// <summary>
        /// Тип фильтра - разрешающий или запрещающий
        /// Include: предмет, попадающий под фильтр, подлежит обработке
        /// Exclude: предмет, попадающий под фильтр, НЕ подлежит обработке
        /// </summary>
        [Description("Тип фильтра:\n\r" +
            "Include: предмет, попадающий под фильтр, подлежит обработке\n\r" +
            "Exclude: предмет, попадающий под фильтр, НЕ подлежит обработке")]
        public ItemFilterMode Mode
        {
            get => _mode; set
            {
                if (!_readOnly)
                {
                    _mode = value;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mode))); 
#else
                    PropertyChanged?.Invoke(this, nameof(Mode));
#endif
                }
            }
        }
        internal ItemFilterMode _mode = ItemFilterMode.Include;

        [Description("Шаблон с которым производится сопоставление выбранного признака предмета")]
        public string Pattern
        {
            get => _pattern;
            set
            {
                if (!_readOnly && _pattern != value)
                {
                    _pattern = value;
                    isMatchPredicate = IsMatch_Selector;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pattern))); 
#else
                    PropertyChanged?.Invoke(this, nameof(Pattern));
#endif
                }
            }
        }
        internal string _pattern = string.Empty;

        [Description("Номер группы фильтров, в которую входит текущий")]
        public uint Group
        {
            get => _group;
            set
            {
                if(value != _group)
                {
                    _group = value;
                    isMatchPredicate = IsMatch_Selector;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pattern))); 
#else
                    PropertyChanged?.Invoke(this, nameof(Group));
#endif
                }
            }
        }
        internal uint _group;
        #endregion

        [XmlIgnore]
        public bool IsReadOnly => _readOnly;
        protected bool _readOnly = false;

        public CommonFilterEntry()
        {
            isMatchPredicate = IsMatch_Selector;
        }
        public CommonFilterEntry(bool readOnly)
        {
            isMatchPredicate = IsMatch_Selector;
            _readOnly = readOnly;
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="source"></param>
        /// <param name="readOnly"></param>
        public CommonFilterEntry(CommonFilterEntry source, bool readOnly = false)
        {
            _entryType = source._entryType;
            _stringType = source.StringType;
            _pattern = source._pattern;
            _mode = source._mode;
#if BoundingRestrictionType
            _boundingRestriction = source._boundingRestriction;
#endif
            _readOnly = readOnly;

            isMatchPredicate = IsMatch_Selector;
        }
        public CommonFilterEntry(ItemFilterEntry fEntry, bool readOnly = false)
        {
            _entryType = (ItemFilterEntryType)fEntry.Type;
            _stringType = fEntry.StringType;
            _pattern = fEntry.Text;
            _mode = fEntry.Mode;
#if BoundingRestrictionType
            _boundingRestriction = BoundingRestrictionType.None;
#endif
            _readOnly = readOnly;

            isMatchPredicate = IsMatch_Selector;
        }

        public virtual IFilterEntry Clone()
        {
            return new CommonFilterEntry(this);
        }

        public virtual bool IsMatch(InventorySlot slot)
        {
            return isMatchPredicate(slot.Item);
        }
        public virtual bool IsMatch(Item item)
        {
            return isMatchPredicate(item);
        }

        public virtual IFilterEntry AsReadOnly()
        {
            return new CommonFilterEntry(this, true);
        }

        public override string ToString()
        {
            return string.Concat(GetType().Name, " {",
                _mode, "; ",
                _entryType, "; ",
                _stringType, "; ", 
                _pattern, '}');
        }

#if event_PropertyChanged
        public virtual event PropertyChangedEventHandler PropertyChanged;
#else
        public Action<object, string> PropertyChanged { get; set; } 
#endif

        #region Сопоставления
        /// <summary>
        /// Предикат, выполняющий сопоставление
        /// </summary>
        [NonSerialized]
        protected Predicate<Item> isMatchPredicate;

        /// <summary>
        /// Предикат-прокладка, выполняющий предварительную обработку условий сопоставления
        /// и выбирающий предикат, фактически выполняющий проверку соответствия предмета item текущему ItemFilterExt
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool IsMatch_Selector(Item item)
        {
            isMatchPredicate = null;
#if BoundingRestrictionType
            switch (_boundingRestriction)
            {
                case BoundingRestrictionType.Unbounded:
                    isMatchPredicate = (Item itm) => (itm.Flags & (uint)BoundingRestrictionType.Bounded) == 0u; ;//IsMatch_Unbounded;
                    break;
                case BoundingRestrictionType.Bounded:
                    isMatchPredicate = (Item itm) => (item.Flags & (uint)BoundingRestrictionType.Bounded) > 0u;//IsMatch_Bounded;
                    break;
                case BoundingRestrictionType.AccountBounded:
                    isMatchPredicate = (Item itm) => (itm.Flags & (uint)BoundingRestrictionType.CharacterBounded) > 0u; //IsMatch_AccountBounded;
                    break;
                case BoundingRestrictionType.CharacterBounded:
                    isMatchPredicate = (Item itm) => (itm.Flags & (uint)BoundingRestrictionType.AccountBounded) > 0u; ;// IsMatch_CharacterBounded;
                    break;
            } 
#endif
            Predicate<Item> predicate = null;
            switch (_entryType)
            {
                case ItemFilterEntryType.Category:
                    if (GetBitArray<ItemCategory>(_pattern, _stringType, out BitArray categories) > 0)
                    {
                        predicate = (Item itm) => itm.ItemDef.Categories.FindIndex((ctg) => categories[(int)ctg]) >= 0;

#if BoundingRestrictionType
                        if (isMatchPredicate != null)
                        {
                            isMatchPredicate += predicate;
                        }
                        else 
#endif
                        isMatchPredicate = predicate;
                    }
                    break;
                case ItemFilterEntryType.Flag:
#if false
                    if (GetItemFlags(_pattern, _stringType, out ItemFlagsExt flags) > 0)
                    {
                        Predicate<Item> predicate = (Item itm) =>
                        {
                            var itemFlags = itm.ItemDef.Flags;
                            return (itemFlags == (uint)ItemFlagsExt.Unbound || (itemFlags & (uint)flags) >= 0);
                        };
                        isMatchPredicate = predicate;
                    } 
#if BoundingRestrictionType
                        if (isMatchPredicate != null)
                        {
                            isMatchPredicate += predicate;
                        }
                        else 
#endif
#else
                    if (GetItemFlagPredicate(_pattern, _stringType, out predicate))
                    { 
                        isMatchPredicate = predicate;
                    } 
#endif
                    break;
                case ItemFilterEntryType.Identifier:
                    {
                        predicate = StringToPatternComparer.Get(_pattern, _stringType, (Item itm) => itm.ItemDef.InternalName);

#if BoundingRestrictionType
                        if (isMatchPredicate != null)
                        {
                            isMatchPredicate += predicate;
                        }
                        else 
#endif
                        isMatchPredicate = predicate;
                    }
                    break;
                case ItemFilterEntryType.Name:
                    {
                        predicate = StringToPatternComparer.Get(_pattern, _stringType, (Item itm) => itm.DisplayName);

#if BoundingRestrictionType
                        if (isMatchPredicate != null)
                        {
                            isMatchPredicate += predicate;
                        }
                        else 
#endif
                        isMatchPredicate = predicate;
                    }
                    break;
                case ItemFilterEntryType.Quality:
                    if (GetBitArray<ItemQuality>(_pattern, _stringType, out BitArray itemQualities) > 0)
                    {
                        predicate = (Item itm) => itemQualities[(int)itm.ItemDef.Quality];

#if BoundingRestrictionType
                        if (isMatchPredicate != null)
                        {
                            isMatchPredicate += predicate;
                        }
                        else 
#endif
                        isMatchPredicate = predicate;
                    }
                    break;
                case ItemFilterEntryType.Type:
                    if (GetBitArray<ItemType>(_pattern, _stringType, out BitArray itemTypes) > 0)
                    {
                        predicate = (Item itm) => itemTypes[(int)itm.ItemDef.Type];

#if BoundingRestrictionType
                        if (isMatchPredicate != null)
                        {
                            isMatchPredicate += predicate;
                        }
                        else  
#endif
                        isMatchPredicate = predicate;
                    }
                    break;
            }

            if (isMatchPredicate != null)
                return isMatchPredicate(item);
            else
            {
#if BoundingRestrictionType
                ETLogger.WriteLine(LogType.Error, $"Unable construct match predicate for the '{pattern}'[{_entryType}; {_boundingRestriction}]");
                isMatchPredicate = (Item itm) =>
                {
                    ETLogger.WriteLine(LogType.Error, $"Invalid match predicate for the '{pattern}'[{_entryType}; {_boundingRestriction}]. Fail test of '{itm.DisplayName}'[{itm.ItemDef.InternalName}]");
                    return false;
                };
#else
                ETLogger.WriteLine(LogType.Error, $"Unable construct match predicate for the '{_pattern}'[{_entryType}]");
                isMatchPredicate = (Item itm) =>
                {
                    ETLogger.WriteLine(LogType.Error, $"Invalid match predicate for the '{_pattern}'[{_entryType}]. Fail test of '{itm.DisplayName}'[{itm.ItemDef.InternalName}]");
                    return false;
                };
#endif
                return false;
            }
        }

        /// <summary>
        /// Формирование битового массива, в котором отмечены все элементы перечисления,
        /// соответствующие шаблону <paramref name="pattern"/> и <paramref name="stringType"/>
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления</typeparam>
        /// <param name="pattern"></param>
        /// <param name="stringType"></param>
        /// <returns></returns>
        internal static int GetBitArray<TEnum>(string pattern, ItemFilterStringType stringType, out BitArray bitSet) where TEnum : Enum
        {
            var enumValues = Enum.GetValues(typeof(TEnum));
            bitSet = new BitArray(enumValues.Length, false);
            int selecteEnumNumber = 0;
            if (!string.IsNullOrEmpty(pattern))
            {
                if (stringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if (pattern[0] == '*')
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = pattern.Trim('*');
                            foreach (var @enum in enumValues)
                            {
                                if (@enum.ToString().Contains(pattern))
                                {
                                    bitSet[(int)@enum] = true;
                                    selecteEnumNumber++;
                                }
                                else bitSet[(int)@enum] = false;
                            }
                        }
                        else
                        {
                            //SimplePatternPos.End;
                            pattern = pattern.TrimStart('*');
                            foreach (var @enum in enumValues)
                            {
                                if (@enum.ToString().EndsWith(pattern))
                                {
                                    bitSet[(int)@enum] = true;
                                    selecteEnumNumber++;
                                }
                                else bitSet[(int)@enum] = false;
                            }
                        }
                    }
                    else
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = pattern.TrimEnd('*');
                            foreach (var @enum in enumValues)
                            {
                                if (@enum.ToString().StartsWith(pattern))
                                {
                                    bitSet[(int)@enum] = true;
                                    selecteEnumNumber++;
                                }
                                else bitSet[(int)@enum] = false;
                            }
                        }
                        else
                        {
                            // SimplePatternPos.Full;
#if Enum_TryParce
                            if (Enum.TryParse(pattern, out TEnum @enum))
                            {
                                bitSet.SetAll(false);
                                bitSet[(int)@enum] = true;
                                selecteEnumNumber++;
                            }
                            else return 0; 
#else
                            foreach(var @enum in enumValues)
                            {
                                if (@enum.ToString().Equals(pattern))
                                {
                                    bitSet[(int)@enum] = true;
                                    selecteEnumNumber++;
                                }
                                else bitSet[(int)@enum] = false;
                            }
#endif
                        }
                    }
                }
                else
                {
                    // Regex
                    foreach (var @enum in enumValues)
                    {
                        if (Regex.IsMatch(@enum.ToString(), pattern))
                        {
                            bitSet[(int)@enum] = true;
                            selecteEnumNumber++;
                        }
                        else bitSet[(int)@enum] = false;
                    }
                }
            }
            return selecteEnumNumber;
        }

        /// <summary>
        /// Создание битовой маской, в которой отмечены все элементы перечисления <seealso cref="ItemFlagsExt"/>,
        /// соответствующие шаблону <paramref name="pattern"/> и <paramref name="stringType"/>
        /// Элементы перечисления должны задавать комбинаацию битов
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="stringType"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        internal static int GetItemFlags(string pattern, ItemFilterStringType stringType, out ItemFlagsExt flags)
        {
            var flagValues = Enum.GetValues(typeof(ItemFlagsExt));
            flags = 0;

            int selecteEnumNumber = 0;
            if (!string.IsNullOrEmpty(pattern))
            {
                if (stringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if(pattern == "*" || pattern == "**")
                    {
                        // pattern == '*' || '**'
                        foreach (ItemFlagsExt f in flagValues)
                            flags |= f;
                        return flagValues.Length;
                    }
                    else if (pattern[0] == '*')
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = pattern.Trim('*');
                            foreach (ItemFlagsExt f in flagValues)
                            {
                                if (f.ToString().Contains(pattern))
                                {
                                    flags |= f;
                                    selecteEnumNumber++;
                                }
                            }
                        }
                        else
                        {
                            //SimplePatternPos.End;
                            pattern = pattern.TrimStart('*');
                            foreach (ItemFlagsExt f in flagValues)
                            {
                                if (f.ToString().EndsWith(pattern))
                                {
                                    flags |= f;
                                    selecteEnumNumber++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = pattern.TrimEnd('*');
                            foreach (ItemFlagsExt f in flagValues)
                            {
                                if (f.ToString().StartsWith(pattern))
                                {
                                    flags |= f;
                                    selecteEnumNumber++;
                                }
                            }
                        }
                        else
                        {
                            // SimplePatternPos.Full;
                            if (Enum.TryParse(pattern, out ItemFlagsExt f))
                            {
                                flags = f;
                                selecteEnumNumber++;
                            }
                            else return 0; 
                        }
                    }
                }
                else
                {
                    // Regex
                    foreach (ItemFlagsExt f in flagValues)
                    {
                        if (Regex.IsMatch(f.ToString(), pattern))
                        {
                            flags |= f;
                            selecteEnumNumber++;
                        }
                    }
                }
            }
            return selecteEnumNumber;
        }
        internal static bool GetItemFlagPredicate(string pattern, ItemFilterStringType stringType, out Predicate<Item> predicate)
        {
            uint flagBitSet = 0;
            var flagValues = Enum.GetValues(typeof(ItemFlagsExt));
            bool checkUnbound = false;
            int selecteEnumNumber = 0;

            // Анализ pattern
            if (!string.IsNullOrEmpty(pattern))
            {
                if (stringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if (pattern == "*" || pattern == "**")
                    {
                        // pattern == '*' || '**'
                        // результат всегда True
                        predicate = (Item itm) => true;
                    }
                    else if (pattern[0] == '*')
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = pattern.Trim('*');
                            checkUnbound = nameof(ItemFlagsExt.Unbound).Contains(pattern);
                            foreach (ItemFlagsExt f in flagValues)
                            {
                                if (f.ToString().Contains(pattern))
                                {
                                    flagBitSet |= (uint)f;
                                    selecteEnumNumber++;
                                }
                            }
                        }
                        else
                        {
                            //SimplePatternPos.End;
                            pattern = pattern.TrimStart('*');
                            checkUnbound = nameof(ItemFlagsExt.Unbound).EndsWith(pattern);
                            foreach (ItemFlagsExt f in flagValues)
                            {
                                if (f.ToString().EndsWith(pattern))
                                {
                                    flagBitSet |= (uint)f;
                                    selecteEnumNumber++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = pattern.TrimEnd('*');
                            checkUnbound = nameof(ItemFlagsExt.Unbound).StartsWith(pattern);
                            foreach (ItemFlagsExt f in flagValues)
                            {
                                if (f.ToString().StartsWith(pattern))
                                {
                                    flagBitSet |= (uint)f;
                                    selecteEnumNumber++;
                                }
                            }
                        }
                        else
                        {
                            // SimplePatternPos.Full;
                            if (Enum.TryParse(pattern, out ItemFlagsExt flag))
                            {
                                if (checkUnbound = flag == ItemFlagsExt.Unbound)
                                {
                                    uint uAnyBounds = (uint)ItemFlagsExt.AnyBounds;
                                    predicate = (Item itm) => (itm.Flags & uAnyBounds) == 0;
                                }
                                else
                                {
                                    uint uFlag = (uint)flag;
                                    predicate = (Item itm) => (itm.Flags & uFlag) == uFlag;
                                }
                                selecteEnumNumber++;
                            }
                        }
                    }
                }
                else
                {
                    // Regex
                    checkUnbound = Regex.IsMatch(nameof(ItemFlagsExt.Unbound), pattern);
                    foreach (ItemFlagsExt f in flagValues)
                    {
                        if (Regex.IsMatch(f.ToString(), pattern))
                        {
                            flagBitSet |= (uint)f;
                            selecteEnumNumber++;
                        }
                    }
                }
            }
            // конструирование предиката
            if (selecteEnumNumber > 0)
            {
                if (checkUnbound)
                {
                    uint uAnyBounds = (uint)ItemFlagsExt.AnyBounds;
                    predicate = (Item itm) => (itm.Flags & uAnyBounds) == 0 || (itm.Flags & flagBitSet) > 0;
                }
                else predicate = (Item itm) => (itm.Flags & flagBitSet) > 0;
            }
            else predicate = (Item itm) => false;
            return predicate != null;
        }
        #endregion

        #region IXmlSerializable
        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            if (reader.IsStartElement())
            {
                string elementName = reader.Name;
                reader.ReadStartElement(elementName);
                while (reader.ReadState == ReadState.Interactive)
                {
                    string innerElemName = reader.Name;
                    if (innerElemName == nameof(EntryType))
                    {
                        string entryType_str = reader.ReadElementContentAsString(innerElemName, "");
                        if (Enum.TryParse(entryType_str, out ItemFilterEntryType eType))
                            _entryType = eType;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                    }
                    else if(innerElemName == nameof(ItemFilterEntry.Type))
                    {
                        string entryType_str = reader.ReadElementContentAsString(innerElemName, "");
                        if (Enum.TryParse(entryType_str, out ItemFilterType eType))
                        {
                            switch(eType)
                            {
                                case ItemFilterType.ItemCatergory:
                                    _entryType = ItemFilterEntryType.Category;
                                    break;
                                case ItemFilterType.ItemFlag:
                                    _entryType = ItemFilterEntryType.Flag;
                                    break;
                                case ItemFilterType.ItemID:
                                    _entryType = ItemFilterEntryType.Identifier;
                                    break;
                                case ItemFilterType.ItemName:
                                    _entryType = ItemFilterEntryType.Name;
                                    break;
                                case ItemFilterType.ItemQuality:
                                    _entryType = ItemFilterEntryType.Quality;
                                    break;
                                case ItemFilterType.ItemType:
                                    _entryType = ItemFilterEntryType.Type;
                                    break;
                                case ItemFilterType.Loot:
                                    _entryType = ItemFilterEntryType.Identifier;
                                    break;
                            }
                        }
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                    }
                    else if (innerElemName == nameof(StringType))
                    {
                        string strType_str = reader.ReadElementContentAsString(innerElemName, "");
                        if (Enum.TryParse(strType_str, out ItemFilterStringType strType))
                            _stringType = strType;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                    }
                    else if (innerElemName == nameof(Mode))
                    {
                        string mode_str = reader.ReadElementContentAsString(innerElemName, "");
                        if (Enum.TryParse(mode_str, out ItemFilterMode mode))
                            _mode = mode;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                    }
                    else if (innerElemName == nameof(Pattern) || innerElemName == nameof(ItemFilterEntry.Text) || innerElemName == "Identifier")
                        _pattern = reader.ReadElementContentAsString(innerElemName, "");
                    else if (innerElemName == elementName && reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.ReadEndElement();
                        break;
                    }
                    else reader.Skip();
                }
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(EntryType), _entryType.ToString());
            writer.WriteElementString(nameof(StringType), _stringType.ToString());
            writer.WriteElementString(nameof(Mode), _mode.ToString());
            writer.WriteElementString(nameof(Pattern), _pattern);
        }
        #endregion

        public virtual bool Equals(IFilterEntry other)
        {
            return _entryType == other.EntryType && _mode == other.Mode
                    && _stringType == other.StringType && _pattern == other.Pattern;
        }
    }
}
