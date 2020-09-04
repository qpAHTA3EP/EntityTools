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

namespace EntityTools.Tools.ItemFilter
{
    /// <summary>
    /// Элемент фильтра предметов
    /// </summary>
    [Serializable]
    public class BuyFilterEntry : CommonFilterEntry
    {
        #region Данные
#if Derived_from_CommonFilterEntry
        // Эти опкции унаследованы от CommonFilterEntry
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntryType)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringType)));
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
            get => _mode;
            set
            {
                if (!_readOnly)
                {
                    _mode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mode)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pattern)));
                }
            }
        }
        internal string _pattern = string.Empty; 
#endif

        /// <summary>
        /// Приоритет (0 - максимальный)
        /// Покупка предметов с меньшим приоритетом запрещена (например 1), 
        /// если не удалось приобрести хотя бы один предмет с более высоким приоритетом (например 0).
        /// Покупка предметов с меньшим приоритетом производится, 
        /// если предметы с большим приоритетом не требуется покупать (например, они есть в наличии в нужном количестве)
        /// </summary>
        [Description("Приоритет покупки (0 - максимальный)\n\r" +
            "Если не удалось приобрести хотя бы один предмет с более высоким приоритетом (например 0),\n\r" +
            "тогда покупка предметов с меньшим приоритетом не производится (например 1)")]
        public uint Priority
        {
            get => _priority;
            set
            {
                if (!_readOnly)
                {
                    _priority = value;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Priority))); 
#else
                    PropertyChanged?.Invoke(this, nameof(Priority));
#endif
                }
            }
        }
        internal uint _priority = 0;

        public uint Count
        {
            get => _count;
            set
            {
                if (!_readOnly)
                {
                    _count = value;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count))); 
#else
                    PropertyChanged?.Invoke(this, nameof(Priority));
#endif
                }
            }
        }
        internal uint _count = 1;

        [Description("Докупать предметы, чтобы общее количество равнялось 'Count'")]
        public bool KeepNumber
        {
            get => _keepNumber; set
            {
                if (!_readOnly)
                {
                    _keepNumber = value;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KeepNumber))); 
#else
                    PropertyChanged?.Invoke(this, nameof(KeepNumber));
#endif
                }
            }
        }
        internal bool _keepNumber = false;

        [Category("Optional")]
        [Description("Покупать экипировку, уроверь которой выше соответствующей экипировки персонажа")]
        public bool CheckEquipmentLevel
        {
            get => _checkEquipmentLevel;
            set
            {
                if (!_readOnly)
                {
                    _checkEquipmentLevel = value;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckEquipmentLevel))); 
#else
                    PropertyChanged?.Invoke(this, nameof(CheckEquipmentLevel));
#endif
                }
            }
        }
        internal bool _checkEquipmentLevel = false;

        [Category("Optional")]
        [Description("Покупать экипировку, подходящую персонажу по уровню")]
        public bool CheckPlayerLevel
        {
            get => _checkPlayerLevel;
            set
            {
                if (!_readOnly)
                {
                    _checkPlayerLevel = value;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckPlayerLevel))); 
#else
                    PropertyChanged?.Invoke(this, nameof(CheckPlayerLevel));
#endif
                }
            }
        }
        internal bool _checkPlayerLevel = false;

        [Category("Optional")]
        [Description("Экипировать предмет после покупки")]
        public bool Wear
        {
            get => _wear;
            set
            {
                if (!_readOnly)
                {
                    _wear = value;
#if event_PropertyChanged
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wear)));
#else
                    PropertyChanged?.Invoke(this, nameof(Wear));
#endif
                }
            }
        }
        internal bool _wear = false;
        #endregion

#if Derived_from_CommonFilterEntry
        [XmlIgnore]
        public bool IsReadOnly => _readOnly;
        private bool _readOnly = false; 
#endif
#if event_PropertyChanged
        public override event PropertyChangedEventHandler PropertyChanged;
#endif

        public BuyFilterEntry()
        {
            isMatchPredicate = IsMatch_Selector;
        }
        public BuyFilterEntry(bool readOnly)
        {
            isMatchPredicate = IsMatch_Selector;
            _readOnly = readOnly;
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="source"></param>
        /// <param name="readOnly"></param>
        public BuyFilterEntry(BuyFilterEntry source, bool readOnly = false)
        {
            _entryType = source._entryType;
            _stringType = source.StringType;
            _pattern = source._pattern;
            _mode = source._mode;
            _wear = source._wear;
            _keepNumber = source._keepNumber;
            _count = source._count;
            _checkEquipmentLevel = source._checkEquipmentLevel;
            _checkPlayerLevel = source._checkPlayerLevel;
            _priority = source._priority;

            _readOnly = readOnly;

            isMatchPredicate = IsMatch_Selector;
        }
        public BuyFilterEntry(ItemFilterEntry fEntry, bool readOnly = false)
        {
#if changed_20200607_2022
            if (fEntry.Type == ItemFilterType.ItemCatergory)
                _entryType = ItemFilterEntryType.Category;
            else if (fEntry.Type == ItemFilterType.ItemID)
                _entryType = ItemFilterEntryType.Identifier;
            else throw new ArgumentException($"Item type of '{fEntry.Type}' is not supported in {nameof(BuyFilterEntry)}");
#else
            _entryType = (ItemFilterEntryType)fEntry.Type;
#endif
            _stringType = fEntry.StringType;
            _pattern = fEntry.Text;
            _mode = fEntry.Mode;
            _readOnly = readOnly;

            isMatchPredicate = IsMatch_Selector;
        }

        public override IFilterEntry Clone()
        {
            return new BuyFilterEntry(this);
        }

        public override bool IsMatch(InventorySlot slot)
        {
            return isMatchPredicate(slot.Item);
        }
        public override bool IsMatch(Item item)
        {
            return isMatchPredicate(item);
        }

        public override IFilterEntry AsReadOnly()
        {
            return new BuyFilterEntry(this, true);
        }

        public override string ToString()
        {
            return string.Concat(GetType().Name, " {",
                "Pr:", _priority, "; ",
                _mode, "; ",
                _entryType, "; ",
                _stringType, "; ",
                _count, "; ",
                '\'', _pattern, '\'',
                (_keepNumber)?"; Keep":string.Empty,
                (_checkPlayerLevel)? "; PlLvl": string.Empty,
                (_checkEquipmentLevel)? "; EqLvl": string.Empty, '}');
        }

        #region Сопоставления
#if Derived_from_CommonFilterEntry
        /// <summary>
        /// Предикат, выполняющий сопоставление
        /// </summary>
        [NonSerialized]
        Predicate<Item> isMatchPredicate; 
#endif


        /// <summary>
        /// Предикат-прокладка, выполняющий предварительную обработку условий сопоставления
        /// и выбирающий предикат, фактически выполняющий проверку соответствия предмета item текущему <see cref="BuyFilterEntry"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsMatch_Selector(Item item)
        {
            isMatchPredicate = null;
            if (CheckPlayerLevel)
                isMatchPredicate += IsMatch_PlayerLevel;
            Predicate<Item> predicate = null;
            switch (_entryType)
            {
                case ItemFilterEntryType.Category:
                    if (GetBitArray<ItemCategory>(_pattern, _stringType, out BitArray categories) > 0)
                        isMatchPredicate = (Item itm) => itm.ItemDef.Categories.FindIndex((ctg) => categories[(int)ctg]) >= 0;
                    break;
                case ItemFilterEntryType.Flag:
                    if (GetItemFlagPredicate(_pattern, _stringType, out predicate))
                        isMatchPredicate = predicate;
                    break;
                case ItemFilterEntryType.Identifier:
                    isMatchPredicate = StringToPatternComparer.Get(_pattern, _stringType, (Item itm) => itm.ItemDef.InternalName);
                    break;
                case ItemFilterEntryType.Name:
                    isMatchPredicate = StringToPatternComparer.Get(_pattern, _stringType, (Item itm) => itm.DisplayName);
                    break;
                case ItemFilterEntryType.Quality:
                    if (GetBitArray<ItemQuality>(_pattern, _stringType, out BitArray itemQualities) > 0)
                        isMatchPredicate = (Item itm) => itemQualities[(int)itm.ItemDef.Quality];
                    break;
                case ItemFilterEntryType.Type:
                    if (GetBitArray<ItemType>(_pattern, _stringType, out BitArray itemTypes) > 0)
                        isMatchPredicate = predicate = (Item itm) => itemTypes[(int)itm.ItemDef.Type];
                    break;
            }

            if (isMatchPredicate != null)
                return isMatchPredicate(item);
            else
            {
                ETLogger.WriteLine(LogType.Error, $"Unable construct match predicate for the '{_pattern}'[{_entryType}]");
                isMatchPredicate = (Item itm) =>
                {
                    ETLogger.WriteLine(LogType.Error, $"Invalid match predicate for the '{_pattern}'[{_entryType}]. Fail test of '{itm.DisplayName}'[{itm.ItemDef.InternalName}]");
                    return false;
                };
                return false;
            }
        }
        static bool IsMatch_PlayerLevel(Item item)
        {
            uint lvl = EntityManager.LocalPlayer.Character.LevelExp;
            return item.ItemDef.UsageRestriction.MinLevel >= lvl
                && lvl <= item.ItemDef.UsageRestriction.MaxLevel;

        }
        #endregion


        #region IXmlSerializable
        public override XmlSchema GetSchema()
        {
            return null;
        }

#if false
        // Определено в базовом классе CommonFilterEntry
        public override void ReadXml(XmlReader reader)
        {
            //TODO: Изменить метод по аналогии с CommonFilterEntry.ReadXml через переопределение ReadFilterEntryFields
            if (reader.IsStartElement())
            {
                string elementName = reader.Name;
                reader.ReadStartElement(elementName);
                ReadInnerXml(reader, elementName);
            }
        } 
#endif

        protected override bool ReadFilterEntryFields(XmlReader reader, string elementName)
        {
            bool entryTypeOk = false;
            bool stringTypeOk = false;
            bool patternOk = false;

            while (reader.ReadState == ReadState.Interactive)
            {
                string innerElemName = reader.Name;
                if (innerElemName == nameof(EntryType))
                {
                    string entryType_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (Enum.TryParse(entryType_str, out ItemFilterEntryType eType))
                    {
                        _entryType = eType;
                        entryTypeOk = true;
                    }
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(ItemFilterEntry.Type))
                {
                    string entryType_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (Enum.TryParse(entryType_str, out ItemFilterType eType))
                    {
                        switch (eType)
                        {
                            case ItemFilterType.ItemCatergory:
                                _entryType = ItemFilterEntryType.Category;
                                entryTypeOk = true;
                                break;
                            case ItemFilterType.ItemFlag:
                                _entryType = ItemFilterEntryType.Flag;
                                entryTypeOk = true;
                                break;
                            case ItemFilterType.ItemID:
                                _entryType = ItemFilterEntryType.Identifier;
                                entryTypeOk = true;
                                break;
                            case ItemFilterType.ItemName:
                                _entryType = ItemFilterEntryType.Name;
                                entryTypeOk = true;
                                break;
                            case ItemFilterType.ItemQuality:
                                _entryType = ItemFilterEntryType.Quality;
                                entryTypeOk = true;
                                break;
                            case ItemFilterType.ItemType:
                                _entryType = ItemFilterEntryType.Type;
                                entryTypeOk = true;
                                break;
                            case ItemFilterType.Loot:
                                throw new XmlException($"'{ItemFilterType.Loot}' value of '{nameof(ItemFilterType)}' does not supported in {nameof(BuyFilterEntry)}");
                        }
                        entryTypeOk = true;
                    }
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(StringType))
                {
                    string strType_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (Enum.TryParse(strType_str, out ItemFilterStringType strType))
                    {
                        _stringType = strType;
                        stringTypeOk = true;
                    }
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(Mode))
                {
                    string mode_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (Enum.TryParse(mode_str, out ItemFilterMode mode))
                    {
                        _mode = mode;
                        if (_mode == ItemFilterMode.Exclude)
                            throw new XmlException($"'{ItemFilterMode.Exclude}' value of '{nameof(ItemFilterMode)}' does not suppoted in {nameof(BuyFilterEntry)}");
                    }
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(Pattern) || innerElemName == nameof(ItemFilterEntry.Text) || innerElemName == "Identifier")
                {
                    _pattern = reader.ReadElementContentAsString(innerElemName, "");
                    patternOk = true;
                }
                else if (innerElemName == nameof(Priority))
                {
                    string priority_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (uint.TryParse(priority_str, out uint prt))
                        _priority = prt;
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(Count))
                {
                    string cnt_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (uint.TryParse(cnt_str, out uint cnt))
                        _count = cnt;
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(KeepNumber))
                {
                    string keep_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (bool.TryParse(keep_str, out bool keep))
                        _keepNumber = keep;
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(CheckEquipmentLevel))
                {
                    string eqLvl_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (bool.TryParse(eqLvl_str, out bool eqLvl))
                        _checkEquipmentLevel = eqLvl;
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(CheckPlayerLevel))
                {
                    string plLvl_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (bool.TryParse(plLvl_str, out bool plLvl))
                        _checkPlayerLevel = plLvl;
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == nameof(Wear) || innerElemName == "PutOnItem")
                {
                    string wear_str = reader.ReadElementContentAsString(innerElemName, "");
                    if (bool.TryParse(wear_str, out bool wear))
                        _wear = wear;
                    else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{innerElemName}'", true);
                }
                else if (innerElemName == elementName && reader.NodeType == XmlNodeType.EndElement)
                    reader.ReadEndElement();
                else reader.Skip();
            }

            return entryTypeOk && stringTypeOk && patternOk;
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(EntryType), _entryType.ToString());
            writer.WriteElementString(nameof(StringType), _stringType.ToString());
            writer.WriteElementString(nameof(Mode), _mode.ToString());
            writer.WriteElementString(nameof(Pattern), _pattern);
            writer.WriteElementString(nameof(Priority), _priority.ToString());
            writer.WriteElementString(nameof(Count), _count.ToString());
            writer.WriteElementString(nameof(KeepNumber), _keepNumber.ToString());
            writer.WriteElementString(nameof(CheckEquipmentLevel), _checkEquipmentLevel.ToString());
            writer.WriteElementString(nameof(CheckPlayerLevel), _checkPlayerLevel.ToString());
            writer.WriteElementString(nameof(Wear), _wear.ToString());
        }
        #endregion

        public override bool Equals(IFilterEntry other)
        {
            bool result = base.Equals(other);
            if (other is BuyFilterEntry buyFilter)
                result = result && _priority == buyFilter._priority;
            return result;
        }
    }
}
