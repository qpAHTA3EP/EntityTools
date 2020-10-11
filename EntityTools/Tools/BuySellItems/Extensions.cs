using System;
using System.Collections.Generic;
using System.Linq;
using Astral.Classes.ItemFilter;
using EntityTools.Reflection;
using EntityTools.Tools.BuySellItems;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Inventory = Astral.Logic.NW.Inventory;

namespace EntityTools.Extensions
{
    /// <summary>
    /// Методы-расширения для индексированной сумки
    /// </summary>
    public static class BagsExtension
    {
        /// <summary>
        /// Подсчет числа предметов, соответствующих item2buy
        /// </summary>
        /// <param name="bags"></param>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        public static uint CountItems(this IndexedBags bags, ItemFilterEntryExt item2buy)
        {
            var slots = bags[item2buy];
            uint num = 0;
            if (slots != null && slots.Count > 0)
                num = (uint)slots.Sum(s => s.Item.Count);
            return num;
        }

        /// <summary>
        /// Подсчет числа предметов, соответствующих item2buy
        /// </summary>
        /// <param name="bags"></param>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        public static uint CountItems(this BagsList bags, ItemFilterEntryExt item2buy)
        {
            uint num = 0;
            foreach (var bag in bags.GetSelectedBags())
            {
                num += (uint)bag.GetItems.Sum(s => item2buy.IsMatch(s) ? s.Item.Count : 0);
            }
            return num;
        }

#if false
        /// <summary>
        /// Подсчет количества предметов, заданных данной позицией списка покупок, которые необходимо (до)купить
        /// </summary>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        public static uint NumberOfItem2Buy(this IndexedBags bags, ItemFilterEntryExt item2buy)
        {
            неправильно.если заданы опции 'CheckEquipmentLevel' или 'CheckEquipmentLevel' при расчете должен учитываться предмет, который покупается.
            uint toBuyNum = item2buy.Count;
            if (item2buy.KeepNumber)
            {
                uint haveItemNum = bags.CountItems(item2buy);
                if (haveItemNum < toBuyNum)
                    toBuyNum -= haveItemNum;
                else toBuyNum = 0;
            }
            return toBuyNum;
        }

        /// <summary>
        /// Подсчет количества предметов, заданных данной позицией списка покупок, которые необходимо (до)купить
        /// </summary>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        public static uint NumberOfItem2Buy(this BagsList bags, ItemFilterEntryExt item2buy)
        {
            неправильно.если заданы опции 'CheckEquipmentLevel' или 'CheckEquipmentLevel' при расчете должен учитываться предмет, который покупается.
            uint toBuyNum = item2buy.Count;
            if (item2buy.KeepNumber)
            {
                uint haveItemNum = bags.CountItems(item2buy);
                if (haveItemNum < toBuyNum)
                    toBuyNum -= haveItemNum;
                else toBuyNum = 0;
            }
            return toBuyNum;
        }
#if ReadOnlyItemFilterEntryExt
        public static uint NumberOfItem2Buy(this KeyValuePair<ReadOnlyItemFilterEntryExt, SlotCache> filterEntryCache)
#else
        public static uint NumberOfItem2Buy(this KeyValuePair<ItemFilterEntryExt, SlotCache> filterEntryCache)
#endif
        {
            неправильно.если заданы опции 'CheckEquipmentLevel' или 'CheckEquipmentLevel' при расчете должен учитываться предмет, который покупается.
            uint toBuyNum = filterEntryCache.Key.Count;
            if (filterEntryCache.Key.KeepNumber)
            {
#if false
                uint haveItemNum = bags.CountItems(item2buy);
                if (haveItemNum < toBuyNum)
                    toBuyNum -= haveItemNum;
                else toBuyNum = 0; 
#else
                if (filterEntryCache.Value.TotalItemsCount < toBuyNum)
                    toBuyNum -= filterEntryCache.Value.TotalItemsCount;
                else toBuyNum = 0;
#endif
            }
            return toBuyNum;
        }
        public static uint NumberOfItem2Buy(this SlotCache slotCache, ItemFilterEntryExt filterEntry)
        {
            неправильно.если заданы опции 'CheckEquipmentLevel' или 'CheckEquipmentLevel' при расчете должен учитываться предмет, который покупается.
            uint toBuyNum = filterEntry.Count;
            if (filterEntry.KeepNumber)
            {
#if false
                uint haveItemNum = bags.CountItems(item2buy);
                if (haveItemNum < toBuyNum)
                    toBuyNum -= haveItemNum;
                else toBuyNum = 0; 
#else
                if (slotCache.TotalItemsCount < toBuyNum)
                    toBuyNum -= slotCache.TotalItemsCount;
                else toBuyNum = 0;
#endif
            }
            return toBuyNum;
        } 
#endif
        /// <summary>
        /// Подсчет в сумках bags количества предметов, соответствующих фильтру filterEntry,
        /// и вычисление количества предметов storeItem, которые необходимо (до)купить
        /// </summary>
        /// <param name="storeItem"></param>
        /// <param name="slotCache"></param>
        /// <param name="filterEntry"></param>
        /// <returns></returns>
        public static uint NumberOfItem2Buy(this StoreItemInfo storeItem, BagsList bags, ItemFilterEntryExt filterEntry)
        {
            uint toBuyNum = filterEntry.Count;

            if (filterEntry.KeepNumber)
            {
                if (filterEntry.CheckEquipmentLevel)
                {
                    //если заданы опциz 'CheckEquipmentLevel',
                    //тогда при подсчете должео производиться сравнение с предметом, который покупается.

                    // Подсчитываем количество предметов, соответствующих filterEntry
                    uint hasNum = 0;
                    if (storeItem.Item.ItemDef.ProgressionDef.IsValid)
                    {
                        // Предмет может быть "обработан" (апгрейжен), поэтому сравнивать по уровню предмета нет смысла
                        foreach (var slot in bags.GetItems())
                        {
                            if (filterEntry.IsMatch(slot))
                            {
                                if (storeItem.Item.IsRecommended(slot.Item))
                                    hasNum += slot.Item.Count;
                                else if (!slot.Item.IsRecommended(storeItem.Item))
                                {
                                    // Если storeItem НЕ лучше чем slot.Item
                                    // и при этом slot.Item НЕ лучше чем storeItem
                                    // значит они одинаковые
                                    hasNum += slot.Item.Count;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Предмет не может быть "обработан" (апгрейжен)
                        // сравниваем по уровню предмета
                        foreach (var slot in bags.GetItems())
                        {
                            if (filterEntry.IsMatch(slot)
                                && storeItem.Item.ItemDef.Level >= slot.Item.ItemDef.Level)
                                hasNum += slot.Item.Count;
                        }
                        if (hasNum < toBuyNum)
                            toBuyNum -= hasNum;
                        else toBuyNum = 0;
                    }
                }
                else
                {
                    uint hasItems = bags.CountItems(filterEntry);
                    if (hasItems < toBuyNum)
                        toBuyNum -= hasItems;
                    else toBuyNum = 0;
                }
            }
            return toBuyNum;
        }

        /// <summary>
        /// Подсчет в slotCache количества предметов, соответствующих фильтру filterEntry
        /// и вычисление количества предметов storeItem, которые необходимо (до)купить
        /// </summary>
        /// <param name="storeItem"></param>
        /// <param name="slotCache"></param>
        /// <param name="filterEntry"></param>
        /// <returns></returns>
        public static uint NumberOfItem2Buy(this StoreItemInfo storeItem,  SlotCache slotCache, ItemFilterEntryExt filterEntry)
        {
            uint toBuyNum = filterEntry.Count;

            if (filterEntry.KeepNumber && slotCache.Slots.Count > 0)
            {
                if (filterEntry.CheckEquipmentLevel)
                {
                    //если заданы опциz 'CheckEquipmentLevel',
                    //тогда при подсчете должео производиться сравнение с предметом, который покупается.

                    // Подсчитываем количество предметов, соответствующих filterEntry
                    uint hasNum = 0;
                    if (storeItem.Item.ItemDef.ProgressionDef.IsValid)
                    {
                        // Предмет может быть "обработан" (апгрейжен), поэтому сравнивать по уровню предмета нет смысла
                        foreach (var slot in slotCache.Slots)
                        {
                            if (filterEntry.IsMatch(slot))
                            {
                                if (storeItem.Item.IsRecommended(slot.Item))
                                    hasNum += slot.Item.Count;
                                else if (!slot.Item.IsRecommended(storeItem.Item))
                                {
                                    // Если storeItem НЕ лучше чем slot.Item
                                    // и при этом slot.Item НЕ лучше чем storeItem
                                    // значит они одинаковые
                                    hasNum += slot.Item.Count;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Предмет не может быть "обработан" (апгрейжен)
                        // сравниваем по уровню предмета
                        foreach (var slot in slotCache.Slots)
                            if (filterEntry.IsMatch(slot)
                                && slot.Item.ItemDef.Level >= storeItem.Item.ItemDef.Level)
                                    hasNum += slot.Item.Count;
                    }
                    if (hasNum < toBuyNum)
                        toBuyNum -= hasNum;
                    else toBuyNum = 0;
                }
                else
                {
                    if (slotCache.TotalItemsCount < toBuyNum)
                        toBuyNum -= slotCache.TotalItemsCount;
                    else toBuyNum = 0;
                }
            }
            return toBuyNum;
        }


        /// <summary>
        /// Проверка соответствия уровню персонажа
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool FitsPlayerLevel(this StoreItemInfo slot)
        {
            uint plLvl = EntityManager.LocalPlayer.Character.LevelExp;
            uint minLvl = slot.Item.ItemDef.UsageRestriction.MinLevel;

            if (minLvl == 0 || minLvl >= plLvl)
            {
                uint maxLvl = slot.Item.ItemDef.UsageRestriction.MaxLevel;
                return maxLvl == 0 || plLvl <= maxLvl;
            }
            return false;
        }

        /// <summary>
        /// Сопоставления уровня предмета storeItem с имеющимися в сумках bags
        /// </summary>
        /// <param name="bags"></param>
        /// <param name="storeItem"></param>
        /// <returns></returns>
        public static bool ContainsBetterItemEquipmentLevel(this BagsList bags, StoreItemInfo storeItem)
        {
            var restrictBags = storeItem.Item.ItemDef.RestrictBagIDs;
            if (restrictBags.Count > 0)
            {
                bool isEquitable = false;
                //BagsList restrictBagsList = new BagsList(BagsList.Equipments);
                // Проверяем возможность экипировать предмет
                // Предмет может быть экипирован, если RestrictBagIDs содержит хотя бы 1 элемент из BagsList.FullInventory
#if false
                foreach (var bagId in storeItem.Item.ItemDef.RestrictBagIDs)
                    if (BagsList.FullInventory.Contains(bagId))
                    {
                        isEquitable = true;
                        //restrictBagsList[bagId] = true;
                    } 
#else
                var equipBags = storeItem.Item.ItemDef.RestrictBagIDs.FindAll(bagId => BagsList.IsEquipmentsBag(bagId));
                isEquitable = equipBags.Count > 0;
#endif

                uint maxItemLevel = 0;
                if (isEquitable)
                {
                    // Предмет может быть экипирован
                    // поэтому сканируем сумки на наличие предметов, экипируемых в те же слоты (RestrictBagIDs)
                    foreach (var bag in bags.GetSelectedBags())
                    {
                        foreach (var bagSlot in bag.GetItems)
                        {
                            uint bagItemLvl = bagSlot.Item.ItemDef.Level;
                            if (bagItemLvl > maxItemLevel
                                // && bagSlot.Item.ItemDef.RestrictBagIDs.Count > 0
                                && bagSlot.Item.ItemDef.RestrictBagIDs.ContainsAny(equipBags))//Any((bagId) => equipBags.Contains(bagId)))
                                maxItemLevel = bagItemLvl;
                        }
                    }
                    return storeItem.Item.ItemDef.Level > maxItemLevel;
                }
            }
            return false;
        }

        /// <summary>
        /// Сопоставление предмета в магазине со всеми предметами в сумке.
        /// Покупаемый предмет должен быть лучше всех предметов в сумке, соответствующих фильтру
        /// </summary>
        /// <param name="filterEntryCache"></param>
        /// <param name="storeItem"></param>
        /// <returns></returns>
        public static bool HasWorseThen<KeyType>(this KeyValuePair<KeyType, SlotCache> filterEntryCache, StoreItemInfo storeItem)
        {
            if(storeItem.Item.ItemDef.ProgressionDef.IsValid)
            {
                // Предмет может быть "обработан" (апгрейжен), поэтому сравнивать по уровню предмета нет смысла
                if(filterEntryCache.Value.Slots.Count > 0)
                {
                    foreach (var slot in filterEntryCache.Value.Slots)
                        if (!storeItem.Item.IsRecommended(slot.Item))
                            return false;
                }
            }
            else
            {
                // Предмет не быть "обработан" (апгрейжен)
                // сравниваем по уровню предмета
                return filterEntryCache.Value.MaxItemLevel <= storeItem.Item.ItemDef.Level;
            }
            return true;
        }

        /// <summary>
        /// Сопоставление предмета в магазине с предметами в сумке
        /// Покупаемый предмет должен быть лучше всех предметов в сумке, соответствующих фильтру
        /// </summary>
        /// <param name="slotCache"></param>
        /// <param name="storeItem"></param>
        /// <returns></returns>
        public static bool HasWorseThen(this SlotCache slotCache, StoreItemInfo storeItem)
        {
            if (storeItem.Item.ItemDef.ProgressionDef.IsValid)
            {
                // Предмет может быть "обработан" (апгрейжен), поэтому сравнивать по уровню предмета нет смысла
                if (slotCache.Slots.Count > 0)
                {
                    foreach (var slot in slotCache.Slots)
                        if (!storeItem.Item.IsRecommended(slot.Item))
                            return false;
                }
            }
            else
            {
                // Предмет не может быть "обработан" (апгрейжен)
                // сравниваем по уровню предмета
                return slotCache.MaxItemLevel <= storeItem.Item.ItemDef.Level;
            }
            return true;
        }

        /// <summary>
        /// Поиск предмета в сумке
        /// </summary>
        /// <param name="bags"></param>
        /// <param name="storeItem"></param>
        /// <returns></returns>
        public static InventorySlot Find(this BagsList bags, ItemDef itemDef)
        {
            string itemInternalName = itemDef.InternalName;
            foreach (var bag in bags.GetSelectedBags())
            {
#if false
                foreach (var bagSlot in bag.GetItems)
                {
                    if (bagSlot.Item.ItemDef.InternalName == itemInternalName)
                        return bagSlot;
                } 
#else
                InventorySlot slot = bag.GetItems.Find(s => s.Item.Count > 0 && s.Item.ItemDef.InternalName == itemInternalName);
                if (slot != null)
                    return slot;
#endif
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool GetBagsItems(this ItemFilterCore filter, out List<InventorySlot> list, bool checkSellable = false)
        {
            list = new List<InventorySlot>();

#if false
            var predicate = typeof(ItemFilterCore).GetFunction<Item, bool>("\u0001");

            if (predicate != null)
            {
                List<InventorySlot> slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Inventory).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag1).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag2).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag3).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag4).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag5).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag6).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag7).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag8).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag9).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Overflow).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));

                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.CraftingResources).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.CraftingInventory).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.FashionItems).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(filter)(s.Item)));
            }
#else

            if (AstralAccessors.ItemFilter.IsMatch != null)
            {
                // Формируем предикат для проверки предметов на соответствие
                Predicate<Item> predicate;
                if (checkSellable)
                {
                    predicate = item => {
                        return Inventory.CanSell(item)
                               && AstralAccessors.ItemFilter.IsMatch(filter)(item);
                    };
                }
                else predicate = item => AstralAccessors.ItemFilter.IsMatch(filter)(item);

                List<InventorySlot> slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Inventory).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag1).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag2).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag3).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag4).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag5).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag6).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag7).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag8).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag9).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Overflow).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));

                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.CraftingResources).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.CraftingInventory).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.FashionItems).GetItems;
                if (slots != null && slots.Count > 0)
                    list.AddRange(slots.FindAll(s => predicate(s.Item)));
            }
#endif


            return list.Count > 0;
        }

        public static bool GetItems(this BagsList bags, ItemFilterCore filter, out List<InventorySlot> list, bool checkSellable = false)
        {
            list = new List<InventorySlot>();
#if false
            var predicate = typeof(ItemFilterCore).GetFunction<Item, bool>("\u0001");

            if (predicate != null)
#else
            if (AstralAccessors.ItemFilter.IsMatch != null)
            {
                // Формируем предикат для проверки предметов на соответствие
                Predicate<Item> predicate;
                if(checkSellable)
                {
                    predicate = item => {
                        return Inventory.CanSell(item)
                               && AstralAccessors.ItemFilter.IsMatch(filter)(item);
                    };
                }
                else predicate = item => AstralAccessors.ItemFilter.IsMatch(filter)(item);
#endif

                // Сканируем все выбранные сумки и добавляем подходящие предметы в список
                foreach (var bag in bags.GetSelectedBags())
                {
                    List<InventorySlot> slots = bag.GetItems;
                    if (slots != null && slots.Count > 0)
                        list.AddRange(slots.FindAll(s => predicate(s.Item)));
                }
            }

            return list.Count > 0;
        }
    }

}
