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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Элемент фильтра предметов
    /// </summary>
    [Serializable]
    public class ItemFilterEntryExt
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
                if(!_readOnly)
                    _mode = value;
            }
        }
        internal ItemFilterMode _mode = ItemFilterMode.Include;

        [Description("Идентификатор предмета или категории")]
        public string Identifier
        {
            get => _identifier;
            set
            {
                if (!_readOnly && _identifier != value)
                {
                    _identifier = value;
                    isMatchPredicate = IsMatch_Selector;
                }
            }
        }
        internal string _identifier = string.Empty;

        /// <summary>
        /// Приоритет (0 - максимальный)
        /// Покупка предметов с меньшим приоритетом запрещена (например 1), 
        /// если не удалось приобрести хотя бы один предмет с более высоким приоритетом (например 0).
        /// Покупка предметов с меньшим приоритетом производится, 
        /// если предметы с большим приоритетом не требуется покупать (например, они есть в наличии в нужном количестве)
        /// </summary>
        [Description("Приоритет покупки (0 - максимальный)")]
        public uint Priority
        {
            get => _priority;
            set
            {
                if (!_readOnly)
                    _priority = value;
            }
        }
        internal uint _priority = 0;

        public uint Count
        {
            get => _count;
            set
            {
                if(!_readOnly)
                    _count = value;
            }
        }
        internal uint _count = 1;

        [Description("Докупать предметы, чтобы общее количество равнялось 'Count'")]
        public bool KeepNumber
        {
            get => _keepNumber; set
            {
                if(!_readOnly)
                    _keepNumber = value;
            }
        }
        internal bool _keepNumber = false;

#if false
        // Максимальное количество предметов, которые можно купить за одну транзакцию (стак)
        // задано в поле StoreItemInfo.Item.ItemDef.MayBuyInBulk
        // 0 - одна штука
        [Category("Optional")]
        [Description("Приобретать предмет по 1 ед. (выбор количества не предусмотрен)")]
        public bool BuyByOne
        {
            get => _buyBy1; set
            {
                if (!_readOnly) _buyBy1 = value;
            }
        }
        internal bool _buyBy1 = false; 
#endif

        [Category("Optional")]
        [Description("Покупать экипировку, уроверь которой выше соответствующей экипировки персонажа")]
        public bool CheckEquipmentLevel
        {
            get => _checkEquipmentLevel; set
            {
                if(!_readOnly) _checkEquipmentLevel = value;
            }
        }
        internal bool _checkEquipmentLevel = false;

        [Category("Optional")]
        [Description("Покупать экипировку, подходящую персонажу по уровню")]
        public bool CheckPlayerLevel
        {
            get => _checkPlayerLevel; set
            {
                if(!_readOnly) _checkPlayerLevel = value;
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
                if (!_readOnly) _putOnItem = value;
            }
        }
        internal bool _putOnItem = false;
        #endregion

        [XmlIgnore]
        public bool IsReadOnly => _readOnly;
        private bool _readOnly = false;

        public ItemFilterEntryExt()
        {
            isMatchPredicate = IsMatch_Selector;
            //categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length);
#if ReadOnlyItemFilterEntryExt
            readOnlyClone = new ReadOnlyItemFilterEntryExt(this);
#endif
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="source"></param>
        /// <param name="readOnly"></param>
        public ItemFilterEntryExt(ItemFilterEntryExt source, bool readOnly = false)
        {
            _entryType = source._entryType;
            _stringType = source.StringType;
            _identifier = source._identifier;
            _mode = source._mode;
            _putOnItem = source._putOnItem;
            _keepNumber = source._keepNumber;
            _count = source._count;
            _checkEquipmentLevel = source._checkEquipmentLevel;
            _checkPlayerLevel = source._checkPlayerLevel;

            _readOnly = readOnly;

            isMatchPredicate = IsMatch_Selector;
            //categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length);

#if ReadOnlyItemFilterEntryExt
            readOnlyClone = new ReadOnlyItemFilterEntryExt(this); 
#endif
        }
        public ItemFilterEntryExt(ItemFilterEntry fEntry, bool readOnly = false)
        {
            if (fEntry.Type == ItemFilterType.ItemCatergory)
                _entryType = ItemFilterEntryType.Category;
            else if (fEntry.Type == ItemFilterType.ItemID)
                _entryType = ItemFilterEntryType.Identifier;
            else throw new ArgumentException($"Item type of '{fEntry.Type}' is not supported in {nameof(ItemFilterEntryExt)}");

            _stringType = fEntry.StringType;
            _identifier = fEntry.Text;
            _mode = fEntry.Mode;
            _readOnly = readOnly;

            isMatchPredicate = IsMatch_Selector;
            //categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length);

#if ReadOnlyItemFilterEntryExt
            readOnlyClone = new ReadOnlyItemFilterEntryExt(this); 
#endif
        }

        public bool IsMatch(InventorySlot slot)
        {
            return isMatchPredicate(slot.Item);
        }
        public bool IsMatch(Item item)
        {
            return isMatchPredicate(item);
        }

#if ReadOnlyItemFilterEntryExt
        public ReadOnlyItemFilterEntryExt AsReadOnly()
        {
            return readOnlyClone;
        }
        ReadOnlyItemFilterEntryExt readOnlyClone; 
#else
        public ItemFilterEntryExt AsReadOnly()
        {
            return new ItemFilterEntryExt(this, true);
        }
#endif

#if moved_to_IndexedBags
        Функционал перемещен в 
        public uint CountItemInBag(List<InventorySlot> slots = null/*, bool bagsItemsOnly = true*/)
        {
            if (slots is null)
                slots = EntityManager.LocalPlayer.BagsItems;
            uint count = 0;
            //List<InventorySlot> slots = bagsItemsOnly ? localPlayer.BagsItems : localPlayer.AllItems;
            foreach (InventorySlot slot in slots)
            {
                if (isMatchPredicate(slot.Item))
                    count += slot.Item.Count;
            }
            return count;
        } 
#endif

        public override string ToString()
        {
            //return $"{GetType().Name}{{{_mode}; {_entryType}; {_stringType}; {_count}; '{_identifier}'}}";
            return string.Concat(GetType().Name, " {",
                _mode, "; ",
                _entryType, "; ",
                _stringType, "; ",
                _count, "; ",
                '\'', _identifier, '\'',
                (_keepNumber)?"; Keep":string.Empty,
                (_checkPlayerLevel)? "; PlLvl": string.Empty,
                (_checkEquipmentLevel)? "; EqLvl": string.Empty, '}');
        }

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

#if false
        // Массив флагов сопоставленных категории
        // Флаг взведен, если категория 
        [NonSerialized]
        BitArray categories;
        // Массив флагов сопоставленных типам предметов
        // Флаг взведен, если категория 
        [NonSerialized]
        BitArray itemTypes; 
#endif

        /// <summary>
        /// Предикат-прокладка, выполняющий предварительную обработку условий сопоставления
        /// и выбирающий предикат, фактически выполняющий проверку соответствия предмета item текущему ItemFilterExt
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsMatch_Selector(Item item)
        {
            isMatchPredicate = null;
            if (CheckPlayerLevel)
                isMatchPredicate += IsMatch_PlayerLevel;
            if (EntryType == ItemFilterEntryType.Identifier)
            {
                if (StringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if (Identifier[0] == '*')
                    {
                        if (Identifier[Identifier.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = Identifier.Trim('*');
                            isMatchPredicate = IsMatch_Simple_Id_Middle;
                        }
                        else
                        {
                            //SimplePatternPos.End;
                            pattern = Identifier.TrimStart('*');
                            isMatchPredicate = IsMatch_Simple_Id_End;
                        }
                    }
                    else
                    {
                        if (Identifier[Identifier.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = Identifier.TrimEnd('*');
                            isMatchPredicate = IsMatch_Simple_Id_Start;
                        }
                        else
                        {
                            // SimplePatternPos.Full;
                            pattern = Identifier;
                            isMatchPredicate = IsMatch_Simple_Id_Full;
                        }
                    }
                }
                else
                {
                    // Regex
                    isMatchPredicate = IsMatch_Regex_Id;
                    pattern = Identifier;
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
                    if (_identifier[0] == '*')
                    {
                        if (_identifier[_identifier.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = _identifier.Trim('*');
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
                            pattern = _identifier.TrimStart('*');
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
                        if (Identifier[Identifier.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = _identifier.TrimEnd('*');
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
                            pattern = Identifier;
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
                        if (Regex.IsMatch(ctg.ToString(), Identifier))
                            categories[(int)ctg] = true;
                        else categories[(int)ctg] = false;
                    }
                }

                isMatchPredicate = (Item itm) => IsMatch_Category(itm, categories);

                return IsMatch_Regex_Id(item);
            }
            else if(_entryType == ItemFilterEntryType.ItemType)
            {
                pattern = string.Empty;
                BitArray itemTypes = new BitArray(Enum.GetValues(typeof(ItemType)).Length, false);

                if (StringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if (_identifier[0] == '*')
                    {
                        if (_identifier[_identifier.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
                            pattern = _identifier.Trim('*');
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
                            pattern = _identifier.TrimStart('*');
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
                        if (Identifier[Identifier.Length - 1] == '*')
                        {
                            // SimplePatternPos.Start;
                            pattern = _identifier.TrimEnd('*');
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
                            pattern = Identifier;
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
                        if (Regex.IsMatch(type.ToString(), Identifier))
                            itemTypes[(int)type] = true;
                        else itemTypes[(int)type] = false;
                    }
                }

                isMatchPredicate = (Item itm) => IsMatch_ItemType(itm, itemTypes);

                return IsMatch_Regex_Id(item);
            }
            throw new InvalidEnumArgumentException($"{nameof(ItemFilterEntryExt)}: Не удалось вычислить предикат сопоставления для типа '{_entryType}'");
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
#if false
        static bool IsMatch_EquipmentLevel(Item item)
        {
            uint equipLvl = 0;
            foreach (ItemCategory cat in item.ItemDef.Categories)
            {
                if (cat)
            }

            return item.ItemDef.Level > equipLvl;
        } 
#endif
        #endregion
        #endregion

#if ReadOnlyItemFilterEntryExt
        /// <summary>
        /// Обертка для ItemFilterEntryExt, предоставляющая доступ только для чтения
        /// </summary>
        public class ReadOnlyItemFilterEntryExt
        {
            ItemFilterEntryExt itemFilterEntry = null;

        #region Данные
            [Description("Тип идентификатора предмета")]
            public ItemFilterEntryType EntryType => itemFilterEntry._entryType;

            [Description("Способ интерпретации строки, задающей идентификатор предмета:\n\r" +
                "Simple:\tПростой текст, допускающий подстановочный символ '*' в начале и в конце\n\r" +
                "Regex:\tРегулярное выражение")]
            public ItemFilterStringType StringType => itemFilterEntry._stringType;

            /// <summary>
            /// Тип фильтра - разрешающий или запрещающий
            /// Include: предмет, попадающий под фильтр, подлежит обработке
            /// Exclude: предмет, попадающий под фильтр, НЕ подлежит обработке
            /// </summary>
            [Description("Тип фильтра:\n\r" +
                "Include: предмет, попадающий под фильтр, подлежит обработке\n\r" +
                "Exclude: предмет, попадающий под фильтр, НЕ подлежит обработке")]
            public ItemFilterMode Mode => itemFilterEntry._mode;

            [Description("Идентификатор предмета или категории")]
            public string Identifier => itemFilterEntry._identifier;

            public uint Count => itemFilterEntry._count;

            [Description("Докупать предметы, чтобы общее количество равнялось 'Count'")]
            public bool KeepNumber => itemFilterEntry._keepNumber;

            [Category("Optional")]
            [Description("Приобретать предмет по 1 ед. (выбор количества не предусмотрен)")]
            public bool BuyByOne => itemFilterEntry._buyBy1;

            [Category("Optional")]
            [Description("Покупать экипировку, уроверь которой выше соответствующей экипировки персонажа")]
            public bool CheckEquipmentLevel => itemFilterEntry._checkEquipmentLevel;

            [Category("Optional")]
            [Description("Покупать экипировку, подходящую персонажу по уровню")]
            public bool CheckPlayerLevel => itemFilterEntry._checkPlayerLevel;

            [Category("Optional")]
            [Description("Экипировать предмет после покупки")]
            public bool PutOnItem => itemFilterEntry._putOnItem;
        #endregion

            public ReadOnlyItemFilterEntryExt(ItemFilterEntryExt fEntry)
            {
                if (fEntry is null)
                    throw new ArgumentNullException();

                itemFilterEntry = fEntry;
            }

            public bool IsMatch(InventorySlot slot) => itemFilterEntry.isMatchPredicate(slot.Item);
            public bool IsMatch(Item item) => itemFilterEntry.isMatchPredicate(item);
        }
#endif
    } 
}
