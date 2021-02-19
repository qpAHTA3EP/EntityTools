using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using EntityTools.Tools.BuySellItems;

namespace EntityCore.UCC.Actions
{
    public class UseItemSpecialEngine : IUCCActionEngine
    {
        #region Данные
        private UseItemSpecial @this;

        private string label = string.Empty;
        #endregion

        internal UseItemSpecialEngine(UseItemSpecial uis)
        {
            @this = uis;
            @this.Engine = this;
            @this.PropertyChanged += PropertyChanged;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                switch (e.PropertyName)
                {
                    case "ItemId":
                        if (@this._itemIdType == ItemFilterStringType.Simple)
                        {
                            // определяем местоположение простого шаблона ItemId в идентификаторе предмета
                            patternPos = @this._itemId.GetSimplePatternPosition(out itemIdPattern);
                        }
                        label = string.Empty;
                        break;
                }
            }
        }

        public bool NeedToRun
        {
            get
            {
                if (@this._itemId.Length > 0)
                {
                    InventorySlot itemSlot = GetItemSlot();

                    if (itemSlot != null && itemSlot.IsValid && itemSlot.Item.Count > 0)
                    {
                        if (@this._checkItemCooldown && itemSlot.Item.ItemDef.Categories.Count > 0)
                        {
                            // Проверяем таймеры кулдаунов, категория которых содержится в списке категорий итема
                            foreach (CooldownTimer timer in EntityManager.LocalPlayer.Character.CooldownTimers)
                            {
                                if (itemSlot.Item.ItemDef.Categories.FindIndex((ItemCategory cat) => timer.PowerCategory == (uint)cat) >= 0
                                    && timer.CooldownLeft > 0)
                                    return false;
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        public bool Run()
        {
            InventorySlot itemSlot = GetItemSlot();
            if (itemSlot != null && itemSlot.IsValid && itemSlot.Item.Count > 0)
            {
                itemSlot.Exec();
                //Thread.Sleep(500);

                return true;
            }
            return false;
        }

        public Entity UnitRef => throw new NotImplementedException();

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
            {
                if (string.IsNullOrEmpty(@this._itemId))
                    label = @this.GetType().Name;
                else label = $"{@this.GetType().Name} [{@this._itemId}]";
            }
            return label;
        }

        #region Вспомогательные данные и методы
        private SimplePatternPos patternPos = SimplePatternPos.None;
        private string itemIdPattern = string.Empty;
        private InventorySlot itemSlotChache = null;

        /// <summary>
        /// Получить предмет, соответствующий ItemId
        /// </summary>
        /// <returns></returns>
        internal InventorySlot GetItemSlot()
        {
            Predicate<InventorySlot> checkSlot = (@this._itemIdType == ItemFilterStringType.Simple)
                                                    ? new Predicate<InventorySlot>(itemIdSimpleComparer) 
                                                    : new Predicate<InventorySlot>(itemIdRegexComparer);

            if (itemSlotChache != null && itemSlotChache.IsValid && itemSlotChache.Item.Count > 0 && checkSlot(itemSlotChache))
                return itemSlotChache;
            else itemSlotChache = null;

            if (@this._bags != null)
            {
                if(patternPos == SimplePatternPos.None)
                    patternPos = @this._itemId.GetSimplePatternPosition(out itemIdPattern);

                int bagsNum = 0;
                foreach (InvBagIDs bagId in @this._bags)
                {
                    if (bagId != InvBagIDs.None)
                    {
                        bagsNum++;
                        InventorySlot iSlot = EntityManager.LocalPlayer.GetInventoryBagById(bagId).GetItems.Find(checkSlot);

                        if (iSlot != null && iSlot.IsValid && iSlot.Item.Count > 0
                            && (iSlot.BagId == InvBagIDs.Potions || iSlot.Item.ItemDef.CanUseUnequipped))
                            return itemSlotChache = iSlot;
                    }
                }

                if(bagsNum == 0)
                {
                    // Не задано ни одной сумки - ищем в сумках персонажа и в Potions
                    foreach (InvBagIDs bagId in BagsList.GetPlayerBagsAndPotions())
                    {
                        if (bagId != InvBagIDs.None)
                        {
                            bagsNum++;
                            InventorySlot iSlot = EntityManager.LocalPlayer.GetInventoryBagById(bagId).GetItems.Find(checkSlot);

                            if (iSlot != null && iSlot.IsValid && iSlot.Item.Count > 0
                                && (iSlot.BagId == InvBagIDs.Potions || iSlot.Item.ItemDef.CanUseUnequipped))
                                return itemSlotChache = iSlot;
                        }
                    }
                }
            }

            return null;
        }

        // Функторы сравнения имени предмета в слоте iSlot с шаблоном
        internal bool itemIdSimpleComparer(InventorySlot iSlot)
        {
            switch (patternPos)
            {
                case SimplePatternPos.Start:
                    return iSlot.Item.ItemDef.InternalName.StartsWith(itemIdPattern);
                case SimplePatternPos.Middle:
                    return iSlot.Item.ItemDef.InternalName.Contains(itemIdPattern);
                case SimplePatternPos.End:
                    return iSlot.Item.ItemDef.InternalName.EndsWith(itemIdPattern);
                case SimplePatternPos.Full:
                    return iSlot.Item.ItemDef.InternalName == @this._itemId;
                default:
                    return false;
            }
        }
        internal bool itemIdRegexComparer(InventorySlot iSlot)
        {
            return Regex.IsMatch(iSlot.Item.ItemDef.InternalName, @this._itemId);
        }
        #endregion
    }
    }
