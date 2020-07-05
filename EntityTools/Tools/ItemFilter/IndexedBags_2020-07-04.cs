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
using System.Collections.ObjectModel;
using static EntityTools.Tools.BuySellItems.BuyFilterEntry;
using EntityTools.Reflection;
using System.Xml.Serialization;
using System.ComponentModel;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Индексированная сумка
    /// Реализация логики KeyedCollection и перечисления
    /// </summary>
    public partial class IndexedBags<TFilterEntry> : KeyedCollection<ItemFilterGroup<TFilterEntry>, SlotCache<TFilterEntry>>, IEnumerable<SlotCache<TFilterEntry>>
    {
        protected override ItemFilterGroup<TFilterEntry> GetKeyForItem(SlotCache<TFilterEntry> slotCache)
        {
            return slotCache.FilterGroup;
        }

        new public SlotCache<TFilterEntry> this[ItemFilterGroup<TFilterEntry> key]
        {
            get
            {
                if (base.Count == 0 && _filterCore.IsValid)
                    Indexing();

                if (base.Contains(key))
                    return base[key].AsReadOnly();

                return emptySlotCache;
            }
        }

        new public IEnumerator<SlotCache<TFilterEntry>> GetEnumerator()
        {
            if (base.Count == 0)
                Indexing();

            return base.GetEnumerator();
        }
    }

    /// <summary>
    /// Индексированная сумка
    /// Реализация логики индексирования
    /// </summary>
    /// <typeparam name="TFilterEntry"></typeparam>
    public partial class IndexedBags<TFilterEntry> where TFilterEntry : IFilterEntry
    {
        public IndexedBags() { }
        public IndexedBags(ItemFilterCoreExt<TFilterEntry> filters)
        {
            _filterCore = CopyHelper.CreateDeepCopy(filters);
        }
        public IndexedBags(ItemFilterCoreExt<TFilterEntry> filters, BagsList bags)
        {
            _filterCore = CopyHelper.CreateDeepCopy(filters);
            _bags = CopyHelper.CreateDeepCopy(bags);
        }
        public IndexedBags(ItemFilterCoreExt<TFilterEntry> filters, InvBagIDs[] bagsIds)
        {
            _filterCore = CopyHelper.CreateDeepCopy(filters);
            _bags = new BagsList(bagsIds);
        }

        /// <summary>
        /// Список фильтров
        /// </summary>
        public ItemFilterCoreExt<TFilterEntry> Filters
        {
            get => _filterCore;
            set
            {
                if(_filterCore != value)
                {
                    _filterCore = value;
                    Clear();
                }
            }
        }
        public ItemFilterCoreExt<TFilterEntry> _filterCore;

        /// <summary>
        /// Список-заглушка, возвращаемые при отсутствии содержимого в сумке
        /// </summary>
        static readonly SlotCache<TFilterEntry> emptySlotCache = new SlotCache<TFilterEntry>((ItemFilterGroup<TFilterEntry>)Activator.CreateInstance(typeof(ItemFilterGroup<TFilterEntry>)), true);

        /// <summary>
        /// Список индексированных сумок
        /// </summary>
        BagsList Bags
        {
            get => _bags;
        }
        BagsList _bags = new BagsList();

        /// <summary>
        /// Формирование описания индексированого содержимого сумки
        /// </summary>
        /// <param name="type"></param>
        /// <param name="slots"></param>
        /// <returns></returns>
        public string Description()
        {
            StringBuilder sb = new StringBuilder();

            {
                if (_filterCore.IsValid && (base.Count == 0))
                    Indexing();

                if (base.Count > 0)
                {
                    foreach(var filterGroup in _filterCore.Groups)
                    {
                        if(base.Contains(filterGroup))
                        {
                            var cachedSlots = base[filterGroup];
                            if(cachedSlots.Count > 0)
                            {
                                sb.Append(cachedSlots.FilterGroup?.ToString() ?? "'NullFilterGroup'").AppendLine(" matches to:");
                                foreach (var slot in cachedSlots)
                                {
                                    sb.Append('\t').Append(slot.Item.ItemDef.DisplayName).Append('[').Append(slot.Item.ItemDef.InternalName).Append(']');
                                    var categories = slot.Item.ItemDef.Categories;
                                    if (categories != null && categories.Count > 0)
                                    {
                                        int catNum = 0;
                                        sb.Append(" {");
                                        foreach (var cat in slot.Item.ItemDef.Categories)
                                        {
                                            if (catNum > 0) sb.Append(", ");
                                            sb.Append(cat);
                                            catNum++;
                                        }
                                        sb.AppendLine("}");
                                    }
                                    else sb.AppendLine();
                                }
                                sb.Append("MaxItemLevel: ").AppendLine(cachedSlots.MaxItemLevel.ToString());
                                sb.Append("TotalItemsCount: ").AppendLine(cachedSlots.TotalItemsCount.ToString());
                                sb.AppendLine(string.Empty);
                            }
                        }
                    }

#if disabled_20200704_1115
                    // cachedSlots получаются упорядочены по Хэшу, а не по номеру группы
                    using (var enumer = base.GetEnumerator())
                    {
                        while (enumer.MoveNext())
                        {
                            var cachedSlots = enumer.Current;
                            if (cachedSlots.Count > 0)
                            {
                                sb.Append(cachedSlots.FilterGroup?.ToString() ?? "'NullFilterGroup'").AppendLine(" matches to:");
                                foreach (var slot in cachedSlots)
                                {
                                    sb.Append('\t').Append(slot.Item.ItemDef.DisplayName).Append('[').Append(slot.Item.ItemDef.InternalName).Append(']');
                                    var categories = slot.Item.ItemDef.Categories;
                                    if (categories != null && categories.Count > 0)
                                    {
                                        int catNum = 0;
                                        sb.Append(" {");
                                        foreach (var cat in slot.Item.ItemDef.Categories)
                                        {
                                            if (catNum > 0) sb.Append(", ");
                                            sb.Append(cat);
                                            catNum++;
                                        }
                                        sb.AppendLine("}");
                                    }
                                    else sb.AppendLine();
                                }
                                sb.Append("MaxItemLevel: ").AppendLine(cachedSlots.MaxItemLevel.ToString());
                                sb.Append("TotalItemsCount: ").AppendLine(cachedSlots.TotalItemsCount.ToString());
                                sb.AppendLine(string.Empty);
                            }
                        }
                    }

#endif                
                }
                else sb.AppendLine("No item matches");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Анализ сумок, заданных в Bags и индексирование их содержимого.
        /// </summary>
        /// <param name="slots"></param>
        private void Indexing()
        {
            base.ClearItems();
            if (_filterCore.IsValid)
            {
                // Перебираем сумки
                foreach (var bag in _bags.GetSelectedBags())
                {
                    // Перебираем содержимое сумок
                    foreach (var slot in bag.GetItems)
                    {
#if disabled_20200624_2200
                        // Прроверяем слот на соответствие запрещающему фильтру
                        if (!_filters.IsForbidden(slot)) 
#endif
                        {
                            // Проверяем слот на соответствие разрешающим фильтрам
                            foreach (var grp in _filterCore.Groups)
                            {
                                if(grp.IsMatch(slot))
                                {
                                    // Помещаем слот в индексированную коллекцию
                                    if (base.Contains(grp))
                                        base[grp].Add(slot);
                                    else base.Add(new SlotCache<TFilterEntry>(grp, slot));
                                }
                            }
                        }
                    }
                }
            }
        }    
    }
}
