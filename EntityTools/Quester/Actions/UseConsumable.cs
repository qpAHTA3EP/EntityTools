﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Threading;
using Astral.Logic.Classes.Map;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Action = Astral.Quester.Classes.Action;

namespace EntityPlugin.Actions
{
    public class UseConsumable : Astral.Quester.Classes.Action
    {
        public override string ActionLabel
        {
            get
            {
                return $"{GetType().Name} [{ConsumableID}]";
            }
        }

        public override bool NeedToRun => true;

        public override string InternalDisplayName => GetType().Name;

        public override bool UseHotSpots => false;

        protected override bool IntenalConditions => true;

        protected override Vector3 InternalDestination => new Vector3();

        protected override ActionValidity InternalValidity
        {
            get
            {
                if (ConsumableID == string.Empty)
                {
                    return new ActionValidity("ConsumableID property not set.");
                }
                return new ActionValidity();
            }
        }

        public override void GatherInfos()
        {
        }

        public override void InternalReset()
        {
        }

        public override void OnMapDraw(GraphicsNW graph)
        {
        }

        [Description("ID (an internal name) of the Consumable (regex)")]
        /// Редактор ItemIdEditor является закрытым членом
        [Editor(typeof(ItemIdEditor), typeof(UITypeEditor))]
        public string ConsumableID { get; set; }

        public override ActionResult Run()
        {
            if (!EntityManager.LocalPlayer.IsValid)
                return ActionResult.Skip;

            LocalPlayerEntity character = EntityManager.LocalPlayer;
            if (!character.IsValid)
                return ActionResult.Skip;

            foreach (InventorySlot inventorySlot in character.GetInventoryBagById(InvBagIDs.Potions).GetItems)
            {
                if (Regex.IsMatch(inventorySlot.Item.ItemDef.InternalName, ConsumableID))
                {
                    inventorySlot.Exec();
                    Thread.Sleep(500);
                    return ActionResult.Completed;
                }
            }
            return ActionResult.Fail;
        }
    }
}
