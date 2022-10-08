using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Astral.Classes.ItemFilter;
using ACTP0Tools.Reflection;
using MyNW.Classes;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools.Inventory
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
            _bags = CopyHelper.CreateDeepCopy(bags);
        }
        public IndexedBags(IEnumerable<ItemFilterEntryExt> filters, InvBagIDs[] bagsIds)
        {
            AnalizeFilters(filters);
            _bags = new BagsList(bagsIds);
        }

        /// <summary>
        /// Индекс слотов сумки по категориям
        /// </summary>
        public IEnumerable<KeyValuePair<ItemCategory, SlotCache>> CategorizedSlots
        {
            get
            {
                if (_categorizedSlots.Count == 0)
                    Indexing();

                foreach (var d in _categorizedSlots)
                    yield return new KeyValuePair<ItemCategory, SlotCache>(d.Key, new SlotCache(d.Value, true));
            }
        }
        Dictionary<ItemCategory, SlotCache> _categorizedSlots = new Dictionary<ItemCategory, SlotCache>();

        /// <summary>
        /// Индекс слотов сумки по типам
        /// </summary>
        public IEnumerable<KeyValuePair<ItemType, SlotCache>> TypedSlots
        {
            get
            {
                if (_typedSlots.Count == 0)
                    Indexing();

                foreach (var d in _typedSlots)
                    yield return new KeyValuePair<ItemType, SlotCache>(d.Key, new SlotCache(d.Value, true));
            }
        }
        Dictionary<ItemType, SlotCache> _typedSlots = new Dictionary<ItemType, SlotCache>();

        /// <summary>
        /// Индекс слотов сумки по фильтру
        /// </summary>
#if ReadOnlyItemFilterEntryExt
        public IEnumerable<KeyValuePair<ReadOnlyItemFilterEntryExt, SlotCache>> FilteredSlots
        {
            get
            {
                if (_filteredSlots.Count == 0)
                    Indexing();

                foreach (var d in _filteredSlots)
                    yield return new KeyValuePair<ReadOnlyItemFilterEntryExt, SlotCache>(d.Key.AsReadOnly(), new SlotCache(d.Value, true));
            }
        } 
#else
        public IEnumerable<KeyValuePair<ItemFilterEntryExt, SlotCache>> FilteredSlots
        {
            get
            {
                if (_filteredSlots.Count == 0)
                    Indexing();

                foreach (var d in _filteredSlots)
                    yield return new KeyValuePair<ItemFilterEntryExt, SlotCache>(d.Key.AsReadOnly(), new SlotCache(d.Value, true));
            }
        }
#endif
        Dictionary<ItemFilterEntryExt, SlotCache> _filteredSlots = new Dictionary<ItemFilterEntryExt, SlotCache>();

#if ItemStats
        /// <summary>
        /// Получение статистики по предметам, имеющимся в сумке
        /// соответствуеющей заданному фильтру
        /// </summary>
        public ItemStats GetItemStats(ItemFilterEntryExt fEntry)
        {
            if (_filteredItemStats.Count == 0 && _filters.Count > 0)
                Indexing();

            if (_filteredItemStats.ContainsKey(fEntry))
                return _filteredItemStats[fEntry].Clone();
            return new ItemStats();
        }
        Dictionary<ItemFilterEntryExt, ItemStats> _filteredItemStats = new Dictionary<ItemFilterEntryExt, ItemStats>();

        /// <summary>
        /// Получение статистики по предметам, имеющимся в сумке
        /// соответствуеющей заданной категории
        /// </summary>
        public ItemStats GetItemStats(ItemCategory iCat)
        {
            if (_categorizedItemStats.Count == 0)
                Indexing();

            if (_categorizedItemStats.ContainsKey(iCat))
                return _categorizedItemStats[iCat].Clone();
            return new ItemStats();
        }
        Dictionary<ItemCategory, ItemStats> _categorizedItemStats = new Dictionary<ItemCategory, ItemStats>(); 
#endif

        /// <summary>
        /// Список-заглушка, возвращаемые при отсутствии содержимого в сумке
        /// </summary>
        static readonly ReadOnlyCollection<InventorySlot> emptyList = new ReadOnlyCollection<InventorySlot>(new List<InventorySlot>());
        static readonly SlotCache emptySlotCache = new SlotCache(true);

        /// <summary>
        /// Запрещающий предикат, исключающий из анализа
        /// </summary>
        Predicate<Item> _exclude;

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
            if (_filters is null)
                _filters = new List<ItemFilterEntryExt>();
            else _filters.Clear();

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

        public SlotCache this[ItemCategory cat]
        {
            get
            {
                if (_categorizedSlots.Count == 0)
                    Indexing();

                if (_categorizedSlots.ContainsKey(cat))
                    return _categorizedSlots[cat].AsReadOnly();

                return emptySlotCache;
            }
        }

        public SlotCache this[ItemFilterEntryExt fItem]
        {
            get
            {
                if (_filteredSlots.Count == 0 && _filters.Count > 0)
                    Indexing();

                if (_filteredSlots.ContainsKey(fItem))
                    return _filteredSlots[fItem].AsReadOnly();

                return emptySlotCache;
            }
        }

        public SlotCache this[ItemType type]
        {
            get
            {
                if (_typedSlots.Count == 0)
                    Indexing();

                if (_typedSlots.ContainsKey(type))
                    return _typedSlots[type].AsReadOnly();

                return emptySlotCache;
            }
        }
        /// <summary>
        ///  Очистка индекса
        /// </summary>
        public void Clear()
        {
            _categorizedSlots.Clear();
            _filteredSlots.Clear();
#if ItemStats
            _filteredItemStats.Clear(); 
#endif
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
                if (_categorizedSlots == null || _categorizedSlots.Count == 0)
                    Indexing();

                if (_categorizedSlots != null && _categorizedSlots.Count > 0)
                {
                    foreach (var catSlots in _categorizedSlots)
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
                                sb.Append("\tMaxItemLevel: ").AppendLine(catSlots.Value.MaxItemLevel.ToString());
                                sb.Append("\tTotalItemsCount: ").AppendLine(catSlots.Value.TotalItemsCount.ToString());
                            }
                        }
                    }
                }
                else sb.AppendLine("No item matches");
            }
            else
            {
                if (_filteredSlots == null || _filteredSlots.Count == 0)
                    Indexing();

                if (_filteredSlots != null && _filteredSlots.Count > 0)
                {
                    foreach (var fltrSlots in _filteredSlots)
                    {
                        if (fltrSlots.Value != null && fltrSlots.Value.Count > 0)
                        {
                            sb.Append(fltrSlots.Key).AppendLine(" matches to:");
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
                                sb.Append("\tMaxItemLevel: ").AppendLine(fltrSlots.Value.MaxItemLevel.ToString());
                                sb.Append("\tTotalItemsCount: ").AppendLine(fltrSlots.Value.TotalItemsCount.ToString());
                            }
                        }
                    }
                }
                else sb.AppendLine("No item matches");
            }
            return sb.ToString();
        }

        #region Индексирование сумкок
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
                        if (_filteredSlots.ContainsKey(f))
                            _filteredSlots[f].Add(slot);
                        else _filteredSlots.Add(f, new SlotCache(slot));

                        // Добавляе слот в список, соответствующий типу
                        ItemType itemType = slot.Item.ItemDef.Type;
                        if (_typedSlots.ContainsKey(itemType))
                            _typedSlots[itemType].Add(slot);
                        else _typedSlots.Add(itemType, new SlotCache(slot));

                        //Сканируем все категории предмета
                        foreach (ItemCategory cat in slot.Item.ItemDef.Categories)
                        {
                            // Добавляем слот в список, соответствующий категории
                            if (_categorizedSlots.ContainsKey(cat))
                                _categorizedSlots[cat].Add(slot);
                            else _categorizedSlots.Add(cat, new SlotCache(slot));

#if ItemStats
                            //обновляем статистику по категориям
                            if (_categorizedItemStats.ContainsKey(cat))
                            {
                                var stats = _categorizedItemStats[cat];
                                if (stats.MaxItemLevel < slot.Item.ItemDef.Level)
                                    stats.MaxItemLevel = slot.Item.ItemDef.Level;
                                stats.TotalItemsCount += slot.Item.Count;
                            }
                            else _categorizedItemStats.Add(cat, new ItemStats(slot.Item.ItemDef.Level, slot.Item.Count));

#endif
                        }

#if ItemStats
                        //обновляем статистику по фильтрам
                        if (_filteredItemStats.ContainsKey(f))
                        {
                            var stats = _filteredItemStats[f];
                            if (stats.MaxItemLevel < slot.Item.ItemDef.Level)
                                stats.MaxItemLevel = slot.Item.ItemDef.Level;

                            stats.TotalItemsCount += slot.Item.Count;
                        }
                        else _filteredItemStats.Add(f, new ItemStats(slot.Item.ItemDef.Level, slot.Item.Count)); 
#endif
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
                        if (_filteredSlots.ContainsKey(f))
                            _filteredSlots[f].Add(slot);
                        else _filteredSlots.Add(f, new SlotCache(slot));

                        // Добавляе слот в список, соответствующий типу
                        ItemType itemType = slot.Item.ItemDef.Type;
                        if (_typedSlots.ContainsKey(itemType))
                            _typedSlots[itemType].Add(slot);
                        else _typedSlots.Add(itemType, new SlotCache(slot));

                        //Сканируем все категории предмета
                        foreach (ItemCategory cat in slot.Item.ItemDef.Categories)
                        {
                            // Добавляем слот в список, соответствующий категории
                            if (_categorizedSlots.ContainsKey(cat))
                                _categorizedSlots[cat].Add(slot);
                            else _categorizedSlots.Add(cat, new SlotCache(slot));

#if ItemStats
                            //обновляем статистику по категориям
                            if (_categorizedItemStats.ContainsKey(cat))
                            {
                                var stats = _categorizedItemStats[cat];
                                if (stats.MaxItemLevel < slot.Item.ItemDef.Level)
                                    stats.MaxItemLevel = slot.Item.ItemDef.Level;
                                stats.TotalItemsCount += slot.Item.Count;
                            }
                            else _categorizedItemStats.Add(cat, new ItemStats(slot.Item.ItemDef.Level, slot.Item.Count)); 
#endif
                        }

#if ItemStats
                        //обновляем статистику по фильтрам
                        if (_filteredItemStats.ContainsKey(f))
                        {
                            var stats = _filteredItemStats[f];
                            if (stats.MaxItemLevel < slot.Item.ItemDef.Level)
                                stats.MaxItemLevel = slot.Item.ItemDef.Level;

                            stats.TotalItemsCount += slot.Item.Count;
                        }
                        else _filteredItemStats.Add(f, new ItemStats(slot.Item.ItemDef.Level, slot.Item.Count)); 
#endif
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
                    // Добавляе слот в список, соответствующий типу
                    ItemType itemType = slot.Item.ItemDef.Type;
                    if (_typedSlots.ContainsKey(itemType))
                        _typedSlots[itemType].Add(slot);
                    else _typedSlots.Add(itemType, new SlotCache(slot));

                    //Сканируем все категории предмета
                    foreach (ItemCategory cat in slot.Item.ItemDef.Categories)
                    {
                        // Добавляем слот в список, соответствующий категории
                        if (_categorizedSlots.ContainsKey(cat))
                            _categorizedSlots[cat].Add(slot);
                        else _categorizedSlots.Add(cat, new SlotCache(slot));

#if ItemStats
                        //обновляем статистику по категориям
                        if (_categorizedItemStats.ContainsKey(cat))
                        {
                            var stats = _categorizedItemStats[cat];
                            if (stats.MaxItemLevel < slot.Item.ItemDef.Level)
                                stats.MaxItemLevel = slot.Item.ItemDef.Level;
                            stats.TotalItemsCount += slot.Item.Count;
                        }
                        else _categorizedItemStats.Add(cat, new ItemStats(slot.Item.ItemDef.Level, slot.Item.Count)); 
#endif
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
                // Добавляе слот в список, соответствующий типу
                ItemType itemType = slot.Item.ItemDef.Type;
                if (_typedSlots.ContainsKey(itemType))
                    _typedSlots[itemType].Add(slot);
                else _typedSlots.Add(itemType, new SlotCache(slot));

                //Сканируем все категории предмета
                foreach (ItemCategory cat in slot.Item.ItemDef.Categories)
                {
                    // Добавляем слот в список, соответствующий категории
                    if (_categorizedSlots.ContainsKey(cat))
                        _categorizedSlots[cat].Add(slot);
                    else _categorizedSlots.Add(cat, new SlotCache( slot ));

#if ItemStats
                    //обновляем статистику по категориям
                    if (_categorizedItemStats.ContainsKey(cat))
                    {
                        var stats = _categorizedItemStats[cat];
                        if (stats.MaxItemLevel < slot.Item.ItemDef.Level)
                            stats.MaxItemLevel = slot.Item.ItemDef.Level;
                        stats.TotalItemsCount += slot.Item.Count;
                    }
                    else _categorizedItemStats.Add(cat, new ItemStats(slot.Item.ItemDef.Level, slot.Item.Count)); 
#endif
                }
            }
        } 
        #endregion
    }
}
