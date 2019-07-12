using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityPlugin.Tools
{
    class Enchanting
    {
        void GemItem()
        {
            foreach (InventorySlot slot in EntityManager.LocalPlayer.BagsItems)
            {
                slot.Item.GemThisItem();
            }
        }

        void UnGemItem()
        {
            foreach (InventorySlot slot in EntityManager.LocalPlayer.BagsItems)
            {
                slot.Item.
                ItemGemSlot gSlot = slot.Item.SpecialProps.ItemGemSlots;
                gSlot.SlottedItem.
            }
        }

    }
}
