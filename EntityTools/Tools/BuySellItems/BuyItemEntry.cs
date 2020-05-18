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
                if (_entryType != value)
                {
                    _entryType = value;
                    isMatchPredicate = IsMatch_Selector;
                }
            }
        }
        internal ItemFilterEntryType _entryType = ItemFilterEntryType.Category;

        [Description("Способ интерпретации строки, задающей идентификатор предмета:\n\r" +
            "Simple:\tПростой текст, допускающий подстановочный символ '*' в начале и в конце\n\r" +
            "Regex:\tРегулярное выражение")]
        public ItemFilterStringType StringType
        {
            get => _stringType; set
            {
                if (_stringType != value)
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
        public ItemFilterMode Mode { get => _mode; set => _mode = value; }
        internal ItemFilterMode _mode = ItemFilterMode.Include;

        [Description("Идентификатор предмета или категории")]
        public string Identifier
        {
            get => _identifier; set
            {
                if (_identifier != value)
                {
                    _identifier = value;
                    isMatchPredicate = IsMatch_Selector;
                }
            }
        }
        internal string _identifier = string.Empty;

        public uint Count { get => _count; set => _count = value; }
        internal uint _count = 1;

        [Description("Докупать предметы, чтобы общее количество равнялось 'Count'")]
        public bool KeepNumber { get => _keepNumber; set => _keepNumber = value; }
        internal bool _keepNumber = false;

        [Category("Optional")]
        [Description("Приобретать предмет по 1 ед. (выбор количества не предусмотрен)")]
        public bool BuyByOne { get => _buyBy1; set => _buyBy1 = value; }
        internal bool _buyBy1 = false;

        [Category("Optional")]
        [Description("Покупать экипировку, уроверь которой выше соответствующей экипировки персонажа")]
        public bool CheckEquipmentLevel { get => _checkEquipmentLevel; set => _checkEquipmentLevel = value; }
        internal bool _checkEquipmentLevel = false;

        [Category("Optional")]
        [Description("Покупать экипировку, подходящую персонажу по уровню")]
        public bool CheckPlayerLevel { get => _checkPlayerLevel; set => _checkPlayerLevel = value; }
        internal bool _checkPlayerLevel = false;

        [Category("Optional")]
        [Description("Экипировать предмет после покупки")]
        public bool PutOnItem { get => _putOnItem; set => _putOnItem = value; }
        internal bool _putOnItem = false;

        #endregion

        public ItemFilterEntryExt()
        {
            isMatchPredicate = IsMatch_Selector;
            //categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length);
        }

        public ItemFilterEntryExt(ItemFilterEntry fEntry)
        {
            if (fEntry.Type == ItemFilterType.ItemCatergory)
                _entryType = ItemFilterEntryType.Category;
            else if (fEntry.Type == ItemFilterType.ItemID)
                _entryType = ItemFilterEntryType.Identifier;
            else throw new ArgumentException($"Item type of '{fEntry.Type}' is not supported in {nameof(ItemFilterEntryExt)}");

            _stringType = fEntry.StringType;
            _identifier = fEntry.Text;
            _mode = fEntry.Mode;

            isMatchPredicate = IsMatch_Selector;
            //categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length);
        }

        public bool IsMatch(InventorySlot slot)
        {
            return isMatchPredicate(slot.Item);
        }
        public bool IsMatch(Item item)
        {
            return isMatchPredicate(item);
        }

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

        // Массив флагов сопоставленных категории
        // Флаг взведен, если категория 
        [NonSerialized]
        BitArray categories;

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
            else
            {
                pattern = string.Empty;
                if (categories == null)
                    categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length);
                //categories.SetAll(false);

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

                isMatchPredicate = IsMatch_Category;
                return IsMatch_Regex_Id(item);
            }
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
        private bool IsMatch_Category(Item item)
        {
            return item.ItemDef.Categories.FindIndex((ctg) => categories[(int)ctg]) >= 0;
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
    }
}
