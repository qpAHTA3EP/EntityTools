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

namespace EntityTools.Tools.BuyItems
{
    [Serializable]
    public class ItemEntry
    {
        #region Данные
        public BuyEntryType EntryType
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
        private BuyEntryType _entryType = BuyEntryType.Category;

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
        private ItemFilterStringType _stringType = ItemFilterStringType.Simple;

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
        private string _identifier = string.Empty;

        public uint Count { get => _count; set => _count = value; }
        private uint _count = 0;

        [Description("Докупать предметы, чтобы общее количество равнялось 'Count'")]
        public bool OverallNumber { get => _overallNumber; set => _overallNumber = value; }
        private bool _overallNumber = false; 
        #endregion

        public ItemEntry()
        {
            isMatchPredicate = IsMatch_Selector;
            categories = new BitArray(Enum.GetValues(typeof(ItemCategory)).Length);
        }

        public bool IsMatch(InventorySlot slot)
        {
            return isMatchPredicate(slot.Item);
        }
        public bool IsMatch(Item item)
        {
            return isMatchPredicate(item);
        }
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

        #region Сопоставление
        [NonSerialized]
        Predicate<Item> isMatchPredicate;
        [NonSerialized]
        string pattern;
        [NonSerialized]
        // Массив флагов сопоставленных категории
        // Флаг взведен, если категория 
        BitArray categories;

        private bool IsMatch_Selector(Item item)
        {
            if (EntryType == BuyEntryType.Identifier)
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

                    return isMatchPredicate(item);
                }
                else
                {
                    // Regex
                    isMatchPredicate = IsMatch_Regex_Id;
                    return IsMatch_Regex_Id(item);
                }
            }
            else
            {
                pattern = string.Empty;
                //categories.SetAll(false);

                if (StringType == ItemFilterStringType.Simple)
                {
                    // Simple
                    if (Identifier[0] == '*')
                    {
                        if (Identifier[Identifier.Length - 1] == '*')
                        {
                            // SimplePatternPos.Middle;
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
        private bool IsMatch_Simple_Id_Full(Item item)
        {
            return item.ItemDef.InternalName.Equals(pattern);
        }
        private bool IsMatch_Simple_Id_Start(Item item)
        {
            return item.ItemDef.InternalName.StartsWith(pattern);
        }
        private bool IsMatch_Simple_Id_Middle(Item item)
        {
            return item.ItemDef.InternalName.Contains(pattern);
        }
        private bool IsMatch_Simple_Id_End(Item item)
        {
            return item.ItemDef.InternalName.EndsWith(pattern);
        }
        private bool IsMatch_Regex_Id(Item item)
        {
            return Regex.IsMatch(item.ItemDef.InternalName, pattern);
        }
        #endregion
        #region IsMatch_Category
        private bool IsMatch_Category(Item item)
        {
            return item.ItemDef.Categories.FindIndex((ctg) => categories[(int)ctg]) >= 0;
        }
        #endregion
        #endregion
    }
}
