using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;

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
            FilterProcessing(filters);
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
                    FilterProcessing(value);
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
        private void FilterProcessing(IEnumerable<ItemFilterEntryExt> filters)
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

        
        public bool BagsOnly
        {
            get => _bagsOnly;
            set
            {
                if(_bagsOnly != value)
                {
                    _bagsOnly = value;
                    Clear();
                }
            }
        }
        bool _bagsOnly = false;

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
        public string Description(IList<InventorySlot> slots = null)
        {
            StringBuilder sb = new StringBuilder();
            if (_filters == null || _filters.Count == 0)
            {
                if (categorizedSlots == null || categorizedSlots.Count == 0)
                    Indexing(slots);

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
                    Indexing(slots);

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
        /// Анализ slot и добавление к индексу.
        /// Если slots не задан, тогда индексируется сумка персонажа и экипировка (в зависимости от флага BagsOnly)
        /// </summary>
        /// <param name="slots"></param>
        private void Indexing(IEnumerable<InventorySlot> slots = null)
        {
            if (slots is null)
            {
                if (_filters != null && _filters.Count > 0)
                    if (_exclude != null)
                    {
                        Indexing_IncludeExcludeFilter(EntityManager.LocalPlayer.BagsItems);
                        if (!_bagsOnly) Indexing_IncludeExcludeFilter(EntityManager.LocalPlayer.EquippedItem);
                    }
                    else
                    {
                        Indexing_IncludeFilter(EntityManager.LocalPlayer.BagsItems);
                        if (!_bagsOnly) Indexing_IncludeFilter(EntityManager.LocalPlayer.EquippedItem);
                    }
                else if (_exclude != null)
                {
                    Indexing_ExcludeFilter(EntityManager.LocalPlayer.BagsItems);
                    if (!_bagsOnly) Indexing_ExcludeFilter(EntityManager.LocalPlayer.EquippedItem);
                }
                else
                {
                    Indexing_NoFilter(EntityManager.LocalPlayer.BagsItems);
                    if (!_bagsOnly) Indexing_NoFilter(EntityManager.LocalPlayer.EquippedItem);
                }
            }
            else
            {
                if (_filters != null && _filters.Count > 0)
                    if (_exclude != null)
                        Indexing_IncludeExcludeFilter(slots);
                    else Indexing_IncludeFilter(slots);
                else if (_exclude != null)
                    Indexing_ExcludeFilter(slots);
                else Indexing_NoFilter(slots);
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
