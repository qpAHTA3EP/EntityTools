using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
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
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.UCC
{
    [Serializable]
    public class UseItemSpecial : UCCAction
    {
        internal class InventoryBagsCheckedListBoxEditor : CheckedListBoxCommonEditor<InvBagIDs> { }

        public UseItemSpecial()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
            CoolDown = 1000;
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new UseItemSpecial
            {
                itemId = this.itemId,
                itemIdTrimmed = this.itemIdTrimmed,
                patternPos = this.patternPos,
                ItemIdType = this.ItemIdType,
                CheckItemCooldown = this.CheckItemCooldown,
                Bags = this.Bags.Clone()
            });
        }

        [NonSerialized]
        private SimplePatternPos patternPos = SimplePatternPos.None;

        [NonSerialized]
        private string itemId = string.Empty;
        [NonSerialized]
        private string itemIdTrimmed = string.Empty;

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
                        patternPos = CommonTools.GetSimplePatternPos(value, out itemIdTrimmed);                        
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
        [Description("Identificator of the bag where Item would be searched\n" +
            "When selected value is 'None' then item is rearched in the all inventories")]
        [Browsable(false)]
        public InvBagIDs BagId
        {
            get
            {
                if (Bags != null && Bags.Items.Count > 0)
                    return Bags.Items[0];
                else return InvBagIDs.None;
            }
            set
            {
                if (Bags == null)
                {
                    Bags = new CheckedListBoxCommonSelector<InvBagIDs>();
                    Bags.Items.Add(value);
                }
                else
                {
                    if (!Bags.Items.Contains(value))
                        Bags.Items.Add(value);
                }
            }
        }

        [Category("Item")]
        [Description("Identificator of the bags where Item would be searched\n")]
        [Editor(typeof(InventoryBagsCheckedListBoxEditor), typeof(UITypeEditor))]
        public CheckedListBoxCommonSelector<InvBagIDs> Bags { get; set; } = new CheckedListBoxCommonSelector<InvBagIDs>();

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

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }

        [NonSerialized]
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
            else itemSlotChache = null;

            if (Bags != null && Bags.Items.Count > 0)
            {
                InventorySlot iSlot = null;
                foreach (InvBagIDs bagId in Bags.Items)
                {
                    if (bagId != InvBagIDs.None)
                    {
                        if (ItemIdType == ItemFilterStringType.Simple)
                            iSlot = EntityManager.LocalPlayer.GetInventoryBagById(bagId).GetItems.Find(itemIdSimpleComparer);
                        else iSlot = EntityManager.LocalPlayer.GetInventoryBagById(bagId).GetItems.Find(itemIdRegexComparer);

                        if (iSlot != null && iSlot.IsValid && iSlot.Item.Count > 0)
                            return itemSlotChache = iSlot;
                    }
                }
                //if (BagId == InvBagIDs.None)
                //{
                //    if (ItemIdType == ItemFilterStringType.Simple)
                //        return itemSlotChache = EntityManager.LocalPlayer.AllItems.Find(itemIdSimpleComparer);
                //    else return itemSlotChache = EntityManager.LocalPlayer.AllItems.Find(itemIdRegexComparer);
                //}
                //else
                //{
                //    if (ItemIdType == ItemFilterStringType.Simple)
                //        return itemSlotChache = EntityManager.LocalPlayer.GetInventoryBagById(BagId).GetItems.Find(itemIdSimpleComparer);
                //    else return itemSlotChache = EntityManager.LocalPlayer.GetInventoryBagById(BagId).GetItems.Find(itemIdRegexComparer);
                //}
            }

            return null;
        }

        // Функторы сравнения имени предмета в слоте iSlot с шаблоном
        internal bool itemIdSimpleComparer(InventorySlot iSlot)
        {
            switch(patternPos)
            {
                case SimplePatternPos.Start:
                    return iSlot.Item.ItemDef.InternalName.StartsWith(itemIdTrimmed);
                case SimplePatternPos.Middle:
                    return iSlot.Item.ItemDef.InternalName.Contains(itemIdTrimmed);
                case SimplePatternPos.End:
                    return iSlot.Item.ItemDef.InternalName.EndsWith(itemIdTrimmed);
                case SimplePatternPos.Full:
                    return iSlot.Item.ItemDef.InternalName == itemId;
                default:
                    return false;
            }
        }
        internal bool itemIdRegexComparer(InventorySlot iSlot)
        {
            return Regex.IsMatch(iSlot.Item.ItemDef.InternalName, itemId);
        }

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }
        #endregion
    }
}
