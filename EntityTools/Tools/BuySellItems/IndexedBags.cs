using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using System.Collections;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Индексированная сумка
    /// </summary>
    [Serializable]
    public class IndexedBags
    {
        public IndexedBags() { }
        public IndexedBags(IEnumerable<ItemFilterEntryExt> filters)
        {
            AnalizeFilters(filters);
        }
        public IndexedBags(IEnumerable<ItemFilterEntryExt> filters, BagsList bags)
        {
            AnalizeFilters(filters);
            _bags = Reflection.CopyHelper.CreateDeepCopy(bags);
        }
        public IndexedBags(IEnumerable<ItemFilterEntryExt> filters, InvBagIDs[] bagsIds)
        {
            AnalizeFilters(filters);
            _bags = new BagsList(bagsIds);
        }

        /// <summary>
        /// Индекс слотов сумки по категориям
        /// </summary>
        Dictionary<ItemCategory, List<InventorySlot>> categorizedSlots = new Dictionary<ItemCategory, List<InventorySlot>>();

        /// <summary>
        /// Индекс слотов сумки по фильтру
        /// </summary>
        Dictionary<ItemFilterEntryExt, List<InventorySlot>> filteredSlots = new Dictionary<ItemFilterEntryExt, List<InventorySlot>>();

        /// <summary>
        /// Список-заглушка, возвращаемые при отсутствии содержимого в сумке
        /// </summary>
        static readonly List<InventorySlot> emptyList = new List<InventorySlot>();

        /// <summary>
        /// Запрещающий предикат, исключающий из анализа
        /// </summary>
        Predicate<Item> _exclude = null;

        /// <summary>
        /// Фильтр слотов сумки
        /// </summary>
        public List<ItemFilterEntryExt> Filters
        {
            get => _filters;
            set
            {
                if (value != null && value.Count > 0)
                {
                    // Разделение include-фильтра и формирование запрещающего предиката
                    AnalizeFilters(value);
                }
                else 
                {
                    _filters.Clear();
                    _exclude = null;
                }
                // Очистка индексов
                Clear();
            }
        }
        List<ItemFilterEntryExt> _filters = new List<ItemFilterEntryExt>();

        /// <summary>
        /// Выделение include-фильтра и формирование запрещающего предиката
        /// </summary>
        /// <param name="filters"></param>
        private void AnalizeFilters(IEnumerable<ItemFilterEntryExt> filters)
        {
            _filters = new List<ItemFilterEntryExt>();
            _exclude = null;
            foreach (var f in filters)
            {
                if (f.Mode == ItemFilterMode.Include)
                    _filters.Add(f);
                else _exclude += f.IsMatch;
            }
        }

#if BagsOnly_flag
        public bool BagsOnly
        {
            get => _bagsOnly;
            set
            {
                if (_bagsOnly != value)
                {
                    _bagsOnly = value;
                    Clear();
                }
            }
        }
        bool _bagsOnly = false; 
#else
        BagsList Bags
        {
            get => _bags;
        }
        BagsList _bags = new BagsList();
#endif



        public IList<InventorySlot> this[ItemCategory cat]
        {
            get
            {
                if (categorizedSlots.Count == 0)
                    Indexing();


                if (categorizedSlots.ContainsKey(cat))
                    return categorizedSlots[cat].AsReadOnly();
                emptyList.Clear();
                return emptyList.AsReadOnly();
            }
        }

        public IList<InventorySlot> this[ItemFilterEntryExt fItem]
        {
            get
            {
                if (filteredSlots.Count == 0)
                    Indexing();

                if (filteredSlots.ContainsKey(fItem))
                    return filteredSlots[fItem].AsReadOnly();
                emptyList.Clear();
                return emptyList.AsReadOnly();
            }
        }

        /// <summary>
        ///  Очистка индекса
        /// </summary>
        public void Clear()
        {
            categorizedSlots.Clear();
            filteredSlots.Clear();
        }

        /// <summary>
        /// Формирование описания индексированого содержимого сумки
        /// </summary>
        /// <param name="type"></param>
        /// <param name="slots"></param>
        /// <returns></returns>
        public string Description()
        {
            StringBuilder sb = new StringBuilder();
            if (_filters == null || _filters.Count == 0)
            {
                if (categorizedSlots == null || categorizedSlots.Count == 0)
                    Indexing();

                if (categorizedSlots != null && categorizedSlots.Count > 0)
                {
                    foreach (var catSlots in categorizedSlots)
                    {
                        if (catSlots.Value != null && catSlots.Value.Count > 0)
                        {
                            sb.Append("Category [").Append(catSlots.Key.ToString()).AppendLine("] contains:");
                            foreach (var slot in catSlots.Value)
                            {
                                sb.Append('\t').Append(slot.Item.ItemDef.DisplayName).Append('[').Append(slot.Item.ItemDef.InternalName).Append("] {");
                                int catNum = 0;
                                foreach (var cat in slot.Item.ItemDef.Categories)
                                {
                                    if (catNum > 0) sb.Append(", ");
                                    sb.Append(cat);
                                    catNum++;
                                }
                                sb.AppendLine("}");
                            }
                        }
                    }
                }
                else sb.AppendLine("No item matches");
            }
            else
            {
                if (filteredSlots == null || filteredSlots.Count == 0)
                    Indexing();

                if (filteredSlots != null && filteredSlots.Count > 0)
                {
                    foreach (var fltrSlots in filteredSlots)
                    {
                        if (fltrSlots.Value != null && fltrSlots.Value.Count > 0)
                        {
                            sb.Append(fltrSlots.Key.ToString()).AppendLine(" matches to:");
                            foreach (var slot in fltrSlots.Value)
                            {
                                sb.Append('\t').Append(slot.Item.ItemDef.DisplayName).Append('[').Append(slot.Item.ItemDef.InternalName).Append("] {");
                                int catNum = 0;
                                foreach (var cat in slot.Item.ItemDef.Categories)
                                {
                                    if (catNum > 0) sb.Append(", ");
                                    sb.Append(cat);
                                    catNum++;
                                }
                                sb.AppendLine("}");
                            }
                        }
                    }
                }
                else sb.AppendLine("No item matches");
            }
            return sb.ToString();
        }

        #region Индексирование сумки
        /// <summary>
        /// Анализ сумок, заданных в Bags и индексирование их содержимого.
        /// </summary>
        /// <param name="slots"></param>
        private void Indexing()
        {
            Action<IEnumerable<InventorySlot>> indexer = null;
            if (_filters != null && _filters.Count > 0)
                if (_exclude != null)
                    indexer = Indexing_IncludeExcludeFilter;
                else
                    indexer = Indexing_IncludeFilter;
            else if (_exclude != null)
                indexer = Indexing_ExcludeFilter;
            else
                indexer = Indexing_NoFilter;

            int bagsNum = 0;
            if(indexer != null)
            {
                foreach(var bag in _bags.GetSelectedBags())
                {
                    indexer(bag.GetItems);
                    bagsNum++;
                }
            }
        }

        /// <summary>
        /// Индексирование содержимого сумки, с фильтрацией и отсеиванием запрещенных (Excluded)
        /// </summary>
        void Indexing_IncludeExcludeFilter(IEnumerable<InventorySlot> slots)
        {
            // Сканируем все слоты
            foreach (InventorySlot slot in slots)
            {
                // Сопоставляем все слоты с фильтрами
                foreach (ItemFilterEntryExt f in _filters)
                {
                    if (!_exclude(slot.Item) && f.IsMatch(slot.Item))
                    {
                        // Добавляе слот в список, соответствующий фильтру
                        if (filteredSlots.ContainsKey(f))
                            filteredSlots[f].Add(slot);
                        else filteredSlots.Add(f, new List<InventorySlot>() { slot });

                        //Сканируем все категории предмета
                        foreach (ItemCategory cat in slot.Item.ItemDef.Categories)
                        {
                            // Добавляем слот в список, соответствующий категории
                            if (categorizedSlots.ContainsKey(cat))
                                categorizedSlots[cat].Add(slot);
                            else categorizedSlots.Add(cat, new List<InventorySlot>() { slot });
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Индексирование сумки с фильтрацией
        /// </summary>
        /// <param name="slots"></param>
        void Indexing_IncludeFilter(IEnumerable<InventorySlot> slots)
        {
            // Сканируем все слоты
            foreach (InventorySlot slot in slots)
            {
                // Сопоставляем все слоты с фильтрами
                foreach (ItemFilterEntryExt f in _filters)
                {
                    if (f.IsMatch(slot.Item))
                    {
                        // Добавляе слот в список, соответствующий фильтру
                        if (filteredSlots.ContainsKey(f))
                            filteredSlots[f].Add(slot);
                        else filteredSlots.Add(f, new List<InventorySlot>() { slot });

                        //Сканируем все категории предмета
                        foreach (ItemCategory cat in slot.Item.ItemDef.Categories)
                        {
                            // Добавляем слот в список, соответствующий категории
                            if (categorizedSlots.ContainsKey(cat))
                                categorizedSlots[cat].Add(slot);
                            else categorizedSlots.Add(cat, new List<InventorySlot>() { slot });
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Индексирование содержимого сумки с отсеиванием запрещенных предметов
        /// </summary>
        void Indexing_ExcludeFilter(IEnumerable<InventorySlot> slots)
        {
            foreach (InventorySlot slot in slots)
            {
                if (!_exclude(slot.Item))
                {
                    foreach (ItemCategory cat in slot.Item.ItemDef.Categories)
                    {
                        if (categorizedSlots.ContainsKey(cat))
                            categorizedSlots[cat].Add(slot);
                        else categorizedSlots.Add(cat, new List<InventorySlot>() { slot });
                    }
                }
            }
        }
        /// <summary>
        /// Индексирование содержимого сумки без фильтрации
        /// </summary>
        void Indexing_NoFilter(IEnumerable<InventorySlot> slots)
        {
            foreach (InventorySlot slot in slots)
            {
                foreach (ItemCategory cat in slot.Item.ItemDef.Categories)
                {
                    if (categorizedSlots.ContainsKey(cat))
                        categorizedSlots[cat].Add(slot);
                    else categorizedSlots.Add(cat, new List<InventorySlot>() { slot });
                }
            }
        } 
        #endregion
    }
}
