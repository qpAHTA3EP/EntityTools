using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Tools;
using EntityTools.Tools.Inventory;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Timeout = Astral.Classes.Timeout;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class UseItemSpecial : UCCAction, INotifyPropertyChanged
    {
        #region Опции команды
#if DEVELOPER
        [Editor(typeof(ItemIdEditor), typeof(UITypeEditor))]
        [Category("Item")]
#else
        [Browsable(false)]
#endif
        public string ItemId
        {
            get => _itemId;
            set
            {
                if (_itemId != value)
                {
                    _itemId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _itemId = string.Empty;

#if DEVELOPER
        [Description("Type of and ItemId:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Item")]
#else
        [Browsable(false)]
#endif
        public ItemFilterStringType ItemIdType
        {
            get => _itemIdType; set
            {
                if (_itemIdType != value)
                {
                    _itemIdType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _itemIdType = ItemFilterStringType.Simple;


#if DEVELOPER
        [Category("Item")]
        [Description("Identificator of the bags where Item would be searched")]
        [Editor(typeof(BagsListEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public BagsList Bags
        {
            get => _bags; set
            {
                if (_bags != value)
                {
                    _bags = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private BagsList _bags = BagsList.GetPlayerBagsAndPotions();

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool CheckItemCooldown
        {
            get => _checkItemCooldown; set
            {
                if (_checkItemCooldown != value)
                {
                    _checkItemCooldown = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _checkItemCooldown;

#if DEVELOPER
        [Category("Optional")]
        [Description("Equip an item into the needed slot automatically if it can't be used from the bag")]
#else
        [Browsable(false)]
#endif
        public bool AutoEquip
        {
            get => _autoEquip; set
            {
                if (_autoEquip != value)
                {
                    _autoEquip = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _autoEquip;

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }
        #endregion
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(ItemId))
                itemChecker = itemChecker_Initializer;
            _label = string.Empty;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UseItemSpecial()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
            CoolDown = 18000;
            itemChecker = itemChecker_Initializer;
            Astral.Logic.UCC.API.AfterCallCombat += ResetCache;
        }
        #endregion




        public override UCCAction Clone()
        {
            return BaseClone(new UseItemSpecial {
                _itemId = _itemId,
                _itemIdType = _itemIdType,
                _checkItemCooldown = _checkItemCooldown,
                _bags = _bags.Clone()
            });
        }




        public override bool NeedToRun
        {
            get
            {
                if (string.IsNullOrEmpty(ItemId)) return false;

                InventorySlot itemSlot = GetItemSlot();

                if (itemSlot is null) return false;

                var item = itemSlot.Item;

                if (!item.CanExecute()) return false;

                var itemDef = item.ItemDef;

                if (CheckItemCooldown)
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
                       || AutoEquip;
            }
        }

        /// <summary>
        /// Выполнение ucc-команды
        /// </summary>
        /// <returns>Во всех случаях возвращаем true, чтобы в выполнять команду повторно в новом ucc-цикле</returns>
        public override bool Run()
        {
            InventorySlot itemSlot = GetItemSlot();
            if (itemSlot != null)
            {
                // Проверка возможности использовать предмет
                if (itemSlot.BagId != InvBagIDs.Potions && !itemSlot.Item.ItemDef.CanUseUnequipped)
                {
                    if (AutoEquip)
                    {
                        // Перемещение предметов в новый слот
                        itemSlot = AutoEquipSlot(itemSlot);

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

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                var itemId = ItemId;
                _label = string.IsNullOrEmpty(itemId)
                        ? GetType().Name
                        : $"{GetType().Name} [{itemId}]";
            }
            return _label;
        }
        private string _label = string.Empty;




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
            var bags = Bags;
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
            var itemId = ItemId;
            if (string.IsNullOrEmpty(itemId))
            {
                itemChecker = (_) => false;
            }
            else
            {
                var predicate = itemId.GetComparer(ItemIdType);
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
        private InventorySlot AutoEquipSlot(InventorySlot slot)
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
