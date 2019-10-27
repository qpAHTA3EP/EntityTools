using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
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
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new UseItemSpecial
            {
                ItemId = this.ItemId
            });
        }

        [Editor(typeof(ItemIdEditor), typeof(UITypeEditor))]
        [Category("Required")]
        public string ItemId { get; set; } = string.Empty;

        [Category("Required")]
        public bool CheckItemCooldown { get; set; } = false;

        public override bool NeedToRun
        {
            get
            {
                if(ItemId.Length > 0)
                {
                    InventorySlot itemSlot = EntityManager.LocalPlayer.BagsItems.Find(iSlot => iSlot.Item.ItemDef.InternalName == ItemId);
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
            InventorySlot itemSlot = EntityManager.LocalPlayer.BagsItems.Find(iSlot => iSlot.Item.ItemDef.InternalName==ItemId);
            if (itemSlot !=null && itemSlot.IsValid && itemSlot.Item.Count > 0)
            {
                itemSlot.Exec();
                Thread.Sleep(500);
                    
                return true;
            }
            return false;
        }

        public override string ToString() => GetType().Name;

        [Browsable(false)]
        public new string ActionName { get; set; }
    }
}
