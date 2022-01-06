using Astral.Logic.UCC.Classes;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Tools;
using EntityTools.Tools.Inventory;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Threading;
using Timeout = Astral.Classes.Timeout;

namespace EntityCore.UCC.Actions
{
    public class UseItemSpecialEngine : IUccActionEngine
    {
        #region Данные
        private UseItemSpecial @this;
        private string label = string.Empty;
        private string actionIDstr;
        #endregion

        internal UseItemSpecialEngine(UseItemSpecial uis)
        {
            InternalRebase(uis);
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized: {Label()}");
        }
        ~UseItemSpecialEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
                @this = null;
            }

            itemSlotCache = null;
            itemChecker = itemChecker_Initializer;
            Astral.Logic.UCC.API.AfterCallCombat -= ResetCache;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
                switch (e.PropertyName)
                {
                    case nameof(@this.ItemId):
#if false
                        if (@this._itemIdType == ItemFilterStringType.Simple)
                        {
                            // определяем местоположение простого шаблона ItemId в идентификаторе предмета
                            patternPos = @this._itemId.GetSimplePatternPosition(out itemIdPattern);
                        } 
#else
                        itemChecker = itemChecker_Initializer;
#endif
                        break;
                }
                label = string.Empty;
            }
        }

        public bool Rebase(UCCAction action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is UseItemSpecial execPower)
            {
                if (InternalRebase(execPower))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(UseItemSpecial) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(UseItemSpecial execPower)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
                Astral.Logic.UCC.API.AfterCallCombat -= ResetCache;
            }

            @this = execPower;
            @this.PropertyChanged += PropertyChanged;

            Astral.Logic.UCC.API.AfterCallCombat += ResetCache;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            itemSlotCache = null;
            itemChecker = itemChecker_Initializer;

            return true;
        }

        public bool NeedToRun
        {
            get
            {
                if (string.IsNullOrEmpty(@this.ItemId)) return false;

                InventorySlot itemSlot = GetItemSlot();

                if (itemSlot is null) return false;
                
                var item = itemSlot.Item;

                if (!item.CanExecute()) return false;

                var itemDef = item.ItemDef;

                if (@this.CheckItemCooldown)
                {
                    var player = EntityManager.LocalPlayer;
                    var categories = itemDef.Categories;

                    if (categories.Count > 0)
                    {
                        // Проверяем таймеры кулдаунов, категория которых содержится в списке категорий предмета
                        foreach (var timer in player.Character.CooldownTimers)
                        {
                            if (categories.FindIndex(cat => timer.PowerCategory == (uint)cat) >= 0
                                && timer.CooldownLeft > 0)
                                return false;
                        }
                    }
                } 

                // Проверяем возможность использования предмета
                return itemSlot.BagId == InvBagIDs.Potions 
                       || itemDef.CanUseUnequipped
                       || @this.AutoEquip;
            }
        }

        /// <summary>
        /// Выполнение ucc-команды
        /// </summary>
        /// <returns>Во всех случаях возвращаем true, чтобы в выполнять команду повторно в новом ucc-цикле</returns>
        public bool Run()
        {
            InventorySlot itemSlot = GetItemSlot();
            if (itemSlot != null)
            {
                // Проверка возможности использовать предмет
                if (itemSlot.BagId != InvBagIDs.Potions && !itemSlot.Item.ItemDef.CanUseUnequipped)
                {
                    if (@this.AutoEquip)
                    {
                        // Перемещение предметов в новый слот
                        itemSlot = AutoEquip(itemSlot);

                        // 
                        if (itemSlot is null)
                            return true;
                    }
                    else return true;
                }
                itemSlot.Exec();
            }

            return true;
        }

        public Entity UnitRef => EntityManager.LocalPlayer;

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
            {
                var itemId = @this.ItemId;
                label = string.IsNullOrEmpty(itemId) 
                        ? @this.GetType().Name 
                        : $"{@this.GetType().Name} [{itemId}]";
            }
            return label;
        }


        #region Вспомогательные данные и методы
        private InventorySlot itemSlotCache;
        private Predicate<InventorySlot> itemChecker;
        //private Timeout cacheRefreshTimeout = new Timeout(0);
        //private const int CACHE_TIMEOUT = 3000;

        /// <summary>
        /// Получить предмет, соответствующий ItemId.
        /// Используется кэш
        /// </summary>
        /// <returns></returns>
        internal InventorySlot GetItemSlot()
        {
            // Проверяем кэш
            if (/*!cacheRefreshTimeout.IsTimedOut &&*/ itemChecker(itemSlotCache))
                return itemSlotCache;
                
            // Ищем новый слот
            var player = EntityManager.LocalPlayer;
            var bags = @this.Bags;
            if (bags != null && bags.Count > 0)
            {
                // Проверяем сумку Potions
                var iSlot = player.GetInventoryBagById(InvBagIDs.Potions).GetItems.Find(itemChecker);
                if (iSlot != null && iSlot.IsValid && iSlot.Filled && iSlot.Item.Count > 0)
                    return itemSlotCache = iSlot;

                // Ищем в заданных сумках
                foreach (InvBagIDs bagId in bags)
                {
                    if (bagId != InvBagIDs.None 
                        && bagId != InvBagIDs.Potions)
                    {
                        iSlot = player.GetInventoryBagById(bagId).Slots.Find(itemChecker);

                        if (iSlot != null && iSlot.IsValid && iSlot.Filled && iSlot.Item.Count > 0)
                        {
                            //cacheRefreshTimeout.ChangeTime(CACHE_TIMEOUT);
                            return itemSlotCache = iSlot;
                        }
                    }
                }
            }
            else
            {
                // Проверяем сумку Potions
                var iSlot = player.GetInventoryBagById(InvBagIDs.Potions).GetItems.Find(itemChecker);
                if (iSlot != null && iSlot.IsValid && iSlot.Filled && iSlot.Item.Count > 0)
                    return itemSlotCache = iSlot;

                // Не задано ни одной сумки - ищем в сумках персонажа и в Potions
                foreach (InvBagIDs bagId in BagsList.GetPlayerBagsAndPotions())
                {
                    if (bagId != InvBagIDs.None
                        && bagId != InvBagIDs.Potions)
                    {
                        iSlot = player.GetInventoryBagById(bagId).Slots.Find(itemChecker);

                        if (iSlot != null && iSlot.IsValid && iSlot.Filled && iSlot.Item.Count > 0)
                        {
                            //cacheRefreshTimeout.ChangeTime(CACHE_TIMEOUT);
                            return itemSlotCache = iSlot;
                        }
                    }
                }

            }
            //cacheRefreshTimeout.ChangeTime(CACHE_TIMEOUT);
            return null;
        }

        /// <summary>
        /// Метод, инициализирующий предикат, которые проверяет предмета на соответствие заданным параметрам <see cref="UseItemSpecial.ItemId"/> и <see cref="UseItemSpecial.ItemIdType"/>
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        private bool itemChecker_Initializer(InventorySlot slot)
        {
            var itemId = @this.ItemId;
            if (string.IsNullOrEmpty(itemId))
            {
                itemChecker = (_) => false;
            }
            else
            {
                var predicate = itemId.GetComparer(@this.ItemIdType);
                itemChecker = (s) =>
                {
                    if (s != null && s.IsValid && s.Filled)
                        return predicate(s.Item.ItemDef.InternalName);
                    return false;
                };
            }

            return itemChecker(slot);
        }

        /// <summary>
        /// Экипировка предмета из слота <param name="slot"/> в слот, позволяющий его использовать
        /// </summary>
        /// <param name="slot">исходный слот, в котором находится предмет</param>
        /// <returns>Слот, в который экипирован предмет</returns>
        private InventorySlot AutoEquip(InventorySlot slot)
        {
            var item = slot?.Item;
            if (item is null || !item.IsValid)
                return null;

            var itemDef = item.ItemDef;

            if (itemDef.CanUseUnequipped)
                return slot;

            var bags = itemDef.RestrictBagIDs;


            var player = EntityManager.LocalPlayer;

            InventorySlot preferredSlot = null;
            uint minLvl = int.MaxValue;

            // Просматриваем сумки, в которые можно экипировать предмет
            // для поиска наиболее подходящего слота для экипировки предмета
            foreach (var bagId in bags)
            {
                var bag = player.GetInventoryBagById(bagId);

                // просматриваем слоты целевой сумки
#if true
                for (uint i = 0; i < bag.MaxSlots; i++)
                {
                    var s = bag.GetSlotByIndex(i);
                    if (!s.Filled)
                    {
                        // в целевой сумке найден пустой слот
                        // перемещаем целевой предмет в найденный слот
                        if (MoveItems(slot, s.BagId, s.Slot, out var targetSlot))
                        {
                            // Обновляем кэш
                            return itemSlotCache = targetSlot;
                        }
                    }

                    var equippedItem = s.Item;
                    uint lvl = equippedItem.ItemDef.Level;
                    if (lvl < minLvl)
                    {
                        minLvl = lvl;
                        preferredSlot = s;
                    }
                } 
#else
                foreach (var s in bag.Slots)
                {
                    if (!s.Filled)
                    {
                        // в целевой сумке найден пустой слот
                        // перемещаем целевой предмет в найденный слот
                        //return MoveItems(slot, s);
                        if (MoveItems(slot, s.BagId, s.Slot, out var targetSlot))
                        {
                            // Обновляем кэш
                            return itemSlotCache = targetSlot;
                        }
                    }

                    var equippedItem = s.Item;
                    uint lvl = equippedItem.ItemDef.Level;
                    if (lvl < minLvl)
                    {
                        minLvl = lvl;
                        preferredSlot = s;
                    }
                }
#endif
            }

            if (preferredSlot is null || !preferredSlot.IsValid)
                return null;

            //return MoveItems(slot, preferredSlot);
            if (MoveItems(slot, preferredSlot.BagId, preferredSlot.Slot, out preferredSlot))
            {
                // Обновляем кэш
                return itemSlotCache = preferredSlot;
            }

            return null;
        }

        /// <summary>
        /// Метод сброса кэша после начала боя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetCache(object sender, Astral.Logic.UCC.API.AfterCallCombatEventArgs e)
        {
            itemSlotCache = null;
        }
#if false
        public static void Astral.Logic.NW.Potions.EquipPotion()
        {
            InventorySlot betterPotionInBags = Potions.BetterPotionInBags;
            if (betterPotionInBags.IsValid)
            {
                uint num = uint.MaxValue;
                InventoryBag inventoryBagById = Class1.LocalPlayer.GetInventoryBagById(InvBagIDs.Potions);
                uint num2 = 0U;
                for (uint num3 = 0U; num3 < inventoryBagById.MaxSlots; num3 += 1U)
                {
                    InventorySlot slotByIndex = inventoryBagById.GetSlotByIndex(num3);
                    if (!slotByIndex.Filled)
                    {
                        num2 = num3;
                    IL_95:
                        betterPotionInBags.MoveAll(InvBagIDs.Potions, num2);
                        uint level = betterPotionInBags.Item.ItemDef.Level;
                        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(3000);
                        while (inventoryBagById.GetSlotByIndex(num2).Item.ItemDef.Level != level)
                        {
                            if (timeout.IsTimedOut)
                            {
                                return;
                            }
                            Thread.Sleep(100);
                        }
                        return;
                    }
                    if (slotByIndex.Item.ItemDef.Level < num && slotByIndex.Item.ItemDef.InternalName.StartsWith("Potion_Heal"))
                    {
                        num2 = num3;
                        num = slotByIndex.Item.ItemDef.Level;
                    }
                }
                goto IL_95;
            }
        } 
#endif

#if false
        /// <summary>
        /// Функция перемещения предмета из sourceSlot в targetSlot
        /// Возвращает targetSlot, если перемещение было успешно
        /// или null
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="targetSlot"></param>
        /// <returns></returns>
        InventorySlot MoveItems(InventorySlot sourceSlot, InventorySlot targetSlot)
        {
            sourceSlot.MoveAll(targetSlot.BagId, targetSlot.Slot);

            // Ждем, когда игра переместит предмет в слот targetSlot
            var timeout = new Timeout(1000);
            while (!itemChecker(targetSlot))
            {
                if (timeout.IsTimedOut)
                {
                    // перемещение не удалось
                    return null;
                }
                Thread.Sleep(100);
            }
            return targetSlot;
        } 
#endif
        /// <summary>
        /// Функция перемещения предмета из sourceSlot в targetSlot
        /// Возвращает targetSlot, если перемещение было успешно
        /// или null
        /// </summary>
        /// <param name="sourceSlot">Слот с предметом, который должен быть экипирован</param>
        /// <param name="bagId">Сумка в которую нужно экипировать предмет</param>
        /// <param name="targetSlotInd">Номер слота, в который должен быть экипирован предмет</param>
        /// <param name="equippedSlot">Слот в который экипирован предмет</param>
        /// <returns>true, если перемещение выполнено успешно</returns>
        bool MoveItems(InventorySlot sourceSlot, InvBagIDs bagId, uint targetSlotInd, out InventorySlot equippedSlot)
        {
            equippedSlot = null;
            if (bagId == InvBagIDs.None)
                return false;

            var bag = EntityManager.LocalPlayer.GetInventoryBagById(bagId);

            if (targetSlotInd >= bag.MaxSlots)
                return false;

            var item = sourceSlot.Item;
            var itemId = item.Id;
            var itemName = item.ItemDef.InternalName;

            sourceSlot.MoveAll(bagId, targetSlotInd);

            // Ждем, когда игра переместит предмет в слот targetSlotInd
            InventorySlot targetSlot;
            Item targetItem;
            var timeout = new Timeout(1000);
            while ((targetSlot = bag.GetSlotByIndex(targetSlotInd)) == null
                   || !targetSlot.IsValid
                   || !(targetItem = targetSlot.Item).IsValid
                   || targetItem.Id != itemId
                   || targetItem.ItemDef.InternalName != itemName)
            {
                if (timeout.IsTimedOut)
                {
                    // перемещение не удалось
                    return false;
                }
                Thread.Sleep(100);
            }

            equippedSlot = targetSlot;
            return true;
        }
        #endregion
    }
}
