using Astral.Classes.ItemFilter;
using EntityTools.Reflection;
using EntityTools.Tools.BuySellItems;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                num = (uint)slots.Sum((s) => s.Item.Count);
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

        /// <summary>
        /// Подсчет количества предметов, заданных данной позицией списка покупок, которые необходимо (до)купить
        /// </summary>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        public static uint NumberOfItem2Buy(this IndexedBags bags, ItemFilterEntryExt item2buy)
        {
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
        public static bool IsBetterEquipmentLevel(this BagsList bags, StoreItemInfo storeItem)
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
                isEquitable = storeItem.Item.ItemDef.RestrictBagIDs.FindIndex((bagId) => BagsList.IsEquipmentsBag(bagId)) >= 0;
#endif

                uint maxItemLevel = storeItem.Item.ItemDef.Level;
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
                                && bagSlot.Item.ItemDef.RestrictBagIDs.ContainsAny(restrictBags))
                                maxItemLevel = bagItemLvl;
                        }
                    }
                    return storeItem.Item.ItemDef.Level > maxItemLevel;
                }
            }
            return false;
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
                InventorySlot slot = bag.GetItems.Find((s) => s.Item.Count > 0 && s.Item.ItemDef.InternalName == itemInternalName);
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
        public static bool GetBagsItems(this ItemFilterCore filter, out List<InventorySlot> list)
        {
            list = new List<InventorySlot>();

            var predicate = typeof(ItemFilterCore).GetFunction<Item, bool>("\u0001");

            if (predicate != null)
            {
                List<InventorySlot> slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Inventory).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag1).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag2).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag3).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag4).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag5).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag6).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag7).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag8).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.PlayerBag9).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Overflow).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));

                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.CraftingResources).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.CraftingInventory).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.FashionItems).GetItems;
                if (slots != null && slots.Count > 0)
                    foreach (var filterEntry in filter.Entries)
                        list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
            }

            return list.Count > 0;
        }

        public static bool GetItems(this BagsList bags, ItemFilterCore filter, out List<InventorySlot> list)
        {
            list = new List<InventorySlot>();

            var predicate = typeof(ItemFilterCore).GetFunction<Item, bool>("\u0001");

            if (predicate != null)
            {
                foreach (var bag in bags.GetSelectedBags())
                {
                    List<InventorySlot> slots = bag.GetItems;
                    if (slots != null && slots.Count > 0)
                        foreach (var filterEntry in filter.Entries)
                            list.AddRange(slots.FindAll(s => predicate(filterEntry)(s.Item)));
                }
            }

            return list.Count > 0;
        }
    }

}
