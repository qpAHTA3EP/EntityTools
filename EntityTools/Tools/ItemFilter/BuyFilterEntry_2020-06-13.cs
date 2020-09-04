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
    public class BuyFilterEntry : IFilterEntry, IXmlSerializable, INotifyPropertyChanged
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Priority)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KeepNumber)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckEquipmentLevel)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckPlayerLevel)));
                }
            }
        }
        internal bool _checkPlayerLevel = false;

        [Category("Optional")]
        [Description("Экипировать предмет после покупки")]
        public bool PutOnItem
        {
            get => _putOnItem;
            set
            {
                if (!_readOnly)
                {
                    _putOnItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PutOnItem)));
                }
            }
        }
        internal bool _putOnItem = false;
        #endregion

        [XmlIgnore]
        public bool IsReadOnly => _readOnly;
        private bool _readOnly = false;

        public BuyFilterEntry()
        {
            isMatchPredicate = IsMatch_Selector;
            //categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length);
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
            _putOnItem = source._putOnItem;
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

        public bool IsMatch(InventorySlot slot)
        {
            return isMatchPredicate(slot.Item);
        }
        public bool IsMatch(Item item)
        {
            return isMatchPredicate(item);
        }

        public IFilterEntry AsReadOnly()
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

        public event PropertyChangedEventHandler PropertyChanged;

        #region Сопоставления
        /// <summary>
        /// Предикат, выполняющий сопоставление
        /// </summary>
        [NonSerialized]
        Predicate<Item> isMatchPredicate;
        /// <summary>
        /// Подготовленный шаблон, которому должен соответствовать идентификтор предмета
        /// </summary>
        [NonSerialized]
        string pattern;

        /// <summary>
        /// Предикат-прокладка, выполняющий предварительную обработку условий сопоставления
        /// и выбирающий предикат, фактически выполняющий проверку соответствия предмета item текущему ItemFilterExt
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_Selector(Item item)
        {
            //TODO: нужно переработать в связи с усложнением типов списка
            isMatchPredicate = null;
            if (CheckPlayerLevel)
                isMatchPredicate += IsMatch_PlayerLevel;
            if (EntryType == ItemFilterEntryType.Identifier)
            {
                if (StringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if (Pattern[0] == '*')
                    {
                        if (Pattern[Pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = Pattern.Trim('*');
                            isMatchPredicate = IsMatch_Simple_Id_Middle;
                        }
                        else
                        {
                            //SimplePatternPos.End;
                            pattern = Pattern.TrimStart('*');
                            isMatchPredicate = IsMatch_Simple_Id_End;
                        }
                    }
                    else
                    {
                        if (Pattern[Pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = Pattern.TrimEnd('*');
                            isMatchPredicate = IsMatch_Simple_Id_Start;
                        }
                        else
                        {
                            // SimplePatternPos.Full;
                            pattern = Pattern;
                            isMatchPredicate = IsMatch_Simple_Id_Full;
                        }
                    }
                }
                else
                {
                    // Regex
                    isMatchPredicate = IsMatch_Regex_Id;
                    pattern = Pattern;
                }
                return IsMatch_Regex_Id(item);
            }
            else if(_entryType == ItemFilterEntryType.Category)
            {
                pattern = string.Empty;
                BitArray categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length, false); 

                if (StringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if (_pattern[0] == '*')
                    {
                        if (_pattern[_pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = _pattern.Trim('*');
                            foreach (var ctg in Enum.GetValues(typeof(ItemCategory)))
                            {
                                if (ctg.ToString().Contains(pattern))
                                    categories[(int)ctg] = true;
                                else categories[(int)ctg] = false;
                            }
                        }
                        else
                        {
                            //SimplePatternPos.End;
                            pattern = _pattern.TrimStart('*');
                            foreach (var ctg in Enum.GetValues(typeof(ItemCategory)))
                            {
                                if (ctg.ToString().EndsWith(pattern))
                                    categories[(int)ctg] = true;
                                else categories[(int)ctg] = false;
                            }
                        }
                    }
                    else
                    {
                        if (Pattern[Pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = _pattern.TrimEnd('*');
                            foreach (var ctg in Enum.GetValues(typeof(ItemCategory)))
                            {
                                if (ctg.ToString().StartsWith(pattern))
                                    categories[(int)ctg] = true;
                                else categories[(int)ctg] = false;
                            }
                        }
                        else
                        {
                            // SimplePatternPos.Full;
                            pattern = Pattern;
                            if (Enum.TryParse(pattern, out ItemCategory ctg))
                            {
                                categories.SetAll(false);
                                categories[(int)ctg] = true;
                            }
                            else return false;
                        }
                    }
                }
                else
                {
                    // Regex
                    foreach (var ctg in Enum.GetValues(typeof(ItemCategory)))
                    {
                        if (Regex.IsMatch(ctg.ToString(), Pattern))
                            categories[(int)ctg] = true;
                        else categories[(int)ctg] = false;
                    }
                }

                isMatchPredicate = (Item itm) => IsMatch_Category(itm, categories);

                return IsMatch_Regex_Id(item);
            }
            else if(_entryType == ItemFilterEntryType.Type)
            {
                pattern = string.Empty;
                BitArray itemTypes = new BitArray(Enum.GetValues(typeof(ItemType)).Length, false);

                if (StringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if (_pattern[0] == '*')
                    {
                        if (_pattern[_pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = _pattern.Trim('*');
                            foreach (var type in Enum.GetValues(typeof(ItemType)))
                            {
                                if (type.ToString().Contains(pattern))
                                    itemTypes[(int)type] = true;
                                else itemTypes[(int)type] = false;
                            }
                        }
                        else
                        {
                            //SimplePatternPos.End;
                            pattern = _pattern.TrimStart('*');
                            foreach (var type in Enum.GetValues(typeof(ItemType)))
                            {
                                if (type.ToString().EndsWith(pattern))
                                    itemTypes[(int)type] = true;
                                else itemTypes[(int)type] = false;
                            }
                        }
                    }
                    else
                    {
                        if (Pattern[Pattern.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = _pattern.TrimEnd('*');
                            foreach (var type in Enum.GetValues(typeof(ItemType)))
                            {
                                if (type.ToString().StartsWith(pattern))
                                    itemTypes[(int)type] = true;
                                else itemTypes[(int)type] = false;
                            }
                        }
                        else
                        {
                            // SimplePatternPos.Full;
                            pattern = Pattern;
                            if (Enum.TryParse(pattern, out ItemType ctg))
                            {
                                itemTypes.SetAll(false);
                                itemTypes[(int)ctg] = true;
                            }
                            else return false;
                        }
                    }
                }
                else
                {
                    // Regex
                    foreach (var type in Enum.GetValues(typeof(ItemType)))
                    {
                        if (Regex.IsMatch(type.ToString(), Pattern))
                            itemTypes[(int)type] = true;
                        else itemTypes[(int)type] = false;
                    }
                }

                isMatchPredicate = (Item itm) => IsMatch_ItemType(itm, itemTypes);

                return IsMatch_Regex_Id(item);
            }
            throw new InvalidEnumArgumentException($"{nameof(BuyFilterEntry)}: Не удалось вычислить предикат сопоставления для типа '{_entryType}'");
        }

        #region IsMatch_Id
        /// <summary>
        /// Проверка ПОЛНОГО совпадения идентификатора предмета с идентификатором Identifier 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_Simple_Id_Full(Item item)
        {
            return item.ItemDef.InternalName.Equals(pattern);
        }
        /// <summary>
        /// Проверка совпадения НАЧАЛА идентификатора предмета с идентификатором Identifier 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_Simple_Id_Start(Item item)
        {
            return item.ItemDef.InternalName.StartsWith(pattern);
        }
        /// <summary>
        /// Проверка ВХОЖДЕНИЯ идентификатора Identifier в идентификатор предмета
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_Simple_Id_Middle(Item item)
        {
            return item.ItemDef.InternalName.Contains(pattern);
        }
        /// <summary>
        /// Проверка совпадения ОКОНЧАНИЯ идентификатора предмета с идентификатором Identifier 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_Simple_Id_End(Item item)
        {
            return item.ItemDef.InternalName.EndsWith(pattern);
        }
        /// <summary>
        /// Проверка совпадения идентификатора предмета с регулярным выражением, заданным Identifier 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_Regex_Id(Item item)
        {
            return Regex.IsMatch(item.ItemDef.InternalName, pattern);
        }
        #endregion
        #region IsMatch_Category
        /// <summary>
        /// Проверка предмета на наличие заданной категории
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_Category(Item item, BitArray categories)
        {
            return item.ItemDef.Categories.FindIndex((ctg) => categories[(int)ctg]) >= 0;
        }
        #endregion
        #region IsMatch_ItemType
        /// <summary>
        /// Проверка предмета на соответствие типу
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_ItemType(Item item, BitArray itemTypes)
        {
            return itemTypes[(int)item.ItemDef.Type];
        }
        #endregion
        #region IsMatch_PlayerLevel_EquipmentLevel
        static bool IsMatch_PlayerLevel(Item item)
        {
            uint lvl = EntityManager.LocalPlayer.Character.LevelExp;
            return item.ItemDef.UsageRestriction.MinLevel >= lvl
                && lvl <= item.ItemDef.UsageRestriction.MaxLevel;

        }
        #endregion
        #endregion


        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsStartElement())
            {
                //TODO: Распарсить все элементы
                string elementName = reader.Name;
                reader.ReadStartElement(elementName);
                while (reader.ReadState == ReadState.Interactive)
                {
                    if (reader.Name == nameof(EntryType) || reader.Name == nameof(ItemFilterEntry.Type))
                    {
                        string entryType_str = reader.ReadElementContentAsString(nameof(EntryType), "");
                        if (!Enum.TryParse(entryType_str, out ItemFilterEntryType eType))
                            _entryType = eType;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(EntryType)}'", true);
                    }
                    else if (reader.Name == nameof(StringType))
                    {
                        string strType_str = reader.ReadElementContentAsString(nameof(StringType), "");
                        if (!Enum.TryParse(strType_str, out ItemFilterStringType strType))
                            _stringType = strType;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(StringType)}'", true);
                    }
                    else if (reader.Name == nameof(Mode))
                    {
                        string mode_str = reader.ReadElementContentAsString(nameof(Mode), "");
                        if (!Enum.TryParse(mode_str, out ItemFilterMode mode))
                            _mode = mode;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(Mode)}'", true);
                    }
                    else if (reader.Name == nameof(Pattern) || reader.Name == nameof(ItemFilterEntry.Text) || reader.Name == "Identifier")
                        _pattern = reader.ReadElementContentAsString(nameof(Pattern), "");
                    else if (reader.Name == nameof(Priority))
                    {
                        string priority_str = reader.ReadElementContentAsString(nameof(Priority), "");
                        if (!uint.TryParse(priority_str, out uint prt))
                            _priority = prt;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(Priority)}'", true);
                    }
                    else if (reader.Name == nameof(Count))
                    {
                        string cnt_str = reader.ReadElementContentAsString(nameof(Count), "");
                        if (!uint.TryParse(cnt_str, out uint cnt))
                            _count = cnt;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(Count)}'", true);
                    }
                    else if (reader.Name == nameof(KeepNumber))
                    {
                        string keep_str = reader.ReadElementContentAsString(nameof(KeepNumber), "");
                        if (!bool.TryParse(keep_str, out bool keep))
                            _keepNumber = keep;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(KeepNumber)}'", true);
                    }
                    else if (reader.Name == nameof(CheckEquipmentLevel))
                    {
                        string keep_str = reader.ReadElementContentAsString(nameof(CheckEquipmentLevel), "");
                        if (!bool.TryParse(keep_str, out bool keep))
                            _keepNumber = keep;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(CheckEquipmentLevel)}'", true);
                    }
                    else if (reader.Name == nameof(CheckPlayerLevel))
                    {
                        string keep_str = reader.ReadElementContentAsString(nameof(CheckPlayerLevel), "");
                        if (!bool.TryParse(keep_str, out bool keep))
                            _keepNumber = keep;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(CheckPlayerLevel)}'", true);
                    }
                    else if (reader.Name == nameof(PutOnItem))
                    {
                        string keep_str = reader.ReadElementContentAsString(nameof(PutOnItem), "");
                        if (!bool.TryParse(keep_str, out bool keep))
                            _keepNumber = keep;
                        else ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parce '{nameof(PutOnItem)}'", true);
                    }
                    else if (reader.Name == elementName && reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.ReadEndElement();
                        break;
                    }
                    else reader.Skip();
                }
            }
        }

        public void WriteXml(XmlWriter writer)
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
            writer.WriteElementString(nameof(PutOnItem), _putOnItem.ToString());
        }
        #endregion
    }
}
