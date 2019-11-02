using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Threading;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Quester.FSM.States;
using Astral.Quester.UIEditors;
using EntityTools;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.UCC
{
    public class UseItemSpecial : UCCAction
    {
        public UseItemSpecial()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
            CoolDown = 1000;
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new UseItemSpecial
            {
                ItemId = this.ItemId,
                CheckItemCooldown = this.CheckItemCooldown
            });
        }

        /// <summary>
        /// Тип местоположения простого шаблона в имени предмета
        /// </summary>
        private enum PatternPos { None, Full, Start, End, Middle }
        private PatternPos patternPos = PatternPos.None;

        private string itemId;
        private string itemIdTrimmed;
        [Editor(typeof(ItemIdEditor), typeof(UITypeEditor))]
        [Category("Item")]
        public string ItemId
        {
            get => itemId;
            set
            {
                if (itemId != value )
                {
                    if (ItemIdType == ItemFilterStringType.Simple)
                    {
                        // определяем местоположение простого шаблона ItemId в идентификаторе предмета
                        if (value[0] == '*')
                        {
                            if (value[value.Length - 1] == '*')
                            {
                                patternPos = PatternPos.Middle;
                                itemIdTrimmed = value.Trim('*');
                            }
                            else
                            {
                                patternPos = PatternPos.End;
                                itemIdTrimmed = value.TrimStart('*');
                            }
                        }
                        else
                        {
                            if (value[value.Length - 1] == '*')
                            {
                                patternPos = PatternPos.Middle;
                                itemIdTrimmed = value.TrimEnd('*');
                            }
                            else
                            {
                                patternPos = PatternPos.End;
                                itemIdTrimmed = value;
                            }
                        }
                    }
                    itemId = value;
                }
            }
        }

        [Description("Type of and ItemId:\n" +
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Item")]
        public ItemFilterStringType ItemIdType { get; set; } = ItemFilterStringType.Simple;

        [Category("Item")]
        public bool CheckItemCooldown { get; set; } = false;

        [Category("Item")]
        [Description("Identificator of the bag where Item wold be searched\n" +
            "When selected value is 'None' then item is rearched in the all inventories")]
        public InvBagIDs BagId { get; set; } = InvBagIDs.PlayerBags;

        public override bool NeedToRun
        {
            get
            {
                if (ItemId.Length > 0)
                {
                    InventorySlot itemSlot = GetItemSlot();

                    if (itemSlot != null && itemSlot.IsValid && itemSlot.Item.Count > 0)
                    {
                        if (CheckItemCooldown && itemSlot.Item.ItemDef.Categories.Count > 0)
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

        public override bool Run()
        {
            InventorySlot itemSlot = GetItemSlot();
            if (itemSlot != null && itemSlot.IsValid && itemSlot.Item.Count > 0)
            {
                itemSlot.Exec();
                Thread.Sleep(500);

                return true;
            }
            return false;
        }

        public override string ToString() => GetType().Name + " [" + ItemId + ']';

        [Browsable(false)]
        public new string ActionName { get; set; }

        private InventorySlot itemSlotChache = null;

        /// <summary>
        /// Получить предмет, соответствующий ItemId
        /// </summary>
        /// <returns></returns>
        internal InventorySlot GetItemSlot()
        {
            if (itemSlotChache != null && itemSlotChache.IsValid && itemSlotChache.Item.Count > 0
                && ((ItemIdType == ItemFilterStringType.Simple && itemIdSimpleComparer(itemSlotChache))
                    || (ItemIdType == ItemFilterStringType.Regex && itemIdRegexComparer(itemSlotChache))))
                return itemSlotChache;

            if (BagId == InvBagIDs.None)
            {
                if (ItemIdType == ItemFilterStringType.Simple)
                    return itemSlotChache = EntityManager.LocalPlayer.AllItems.Find(itemIdSimpleComparer);
                else return itemSlotChache = EntityManager.LocalPlayer.AllItems.Find(itemIdRegexComparer);
            }
            else
            {
                if (ItemIdType == ItemFilterStringType.Simple)
                    return itemSlotChache = EntityManager.LocalPlayer.GetInventoryBagById(BagId).GetItems.Find(itemIdSimpleComparer);
                else return itemSlotChache = EntityManager.LocalPlayer.GetInventoryBagById(BagId).GetItems.Find(itemIdSimpleComparer);
            }
        }

        // Функторы сравнения имени предмета в слоте iSlot с шаблоном
        internal bool itemIdSimpleComparer(InventorySlot iSlot)
        {
            switch(patternPos)
            {
                case PatternPos.Start:
                    return iSlot.Item.ItemDef.InternalName.StartsWith(itemIdTrimmed);
                case PatternPos.Middle:
                    return iSlot.Item.ItemDef.InternalName.Contains(itemIdTrimmed);
                case PatternPos.End:
                    return iSlot.Item.ItemDef.InternalName.EndsWith(itemIdTrimmed);
                case PatternPos.Full:
                    return iSlot.Item.ItemDef.InternalName == itemId;
                default:
                    return false;
            }
        }
        internal bool itemIdRegexComparer(InventorySlot iSlot)
        {
            return Regex.IsMatch(iSlot.Item.ItemDef.InternalName, itemId);
        }
    }
}
