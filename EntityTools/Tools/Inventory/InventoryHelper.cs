using System;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System.Collections.Generic;
using System.Linq;
using EntityTools.Enums;

namespace EntityTools.Tools.Inventory
{
    public static class InventoryHelper
    {
        /// <summary>
        /// Перечисление слотов инвентаря (сумок) персонажа
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<InventorySlot> EnumerateInventorySlots()
        {
            var player = EntityManager.LocalPlayer;

            var bag = player.GetInventoryBagById(InvBagIDs.Inventory);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if(slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag1);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag2);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag3);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag4);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag5);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag6);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag7);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag8);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.PlayerBag9);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.KeyItems);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            bag = player.GetInventoryBagById(InvBagIDs.Overflow);
            if (bag.FilledSlots > 0U)
            {
                foreach (var slot in bag.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }
        }

        /// <summary>
        /// Перечисление предметов в инвентаре персонажа
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Item> EnumerateInventoryItems()
        {
            foreach (var slot in EnumerateInventorySlots())
            {
                yield return slot.Item;
            }
        }

        public static IEnumerable<InventorySlot> EnumerateBankSlots()
        {
            var player = EntityManager.LocalPlayer;
            var slots = player.GetInventoryBagById(InvBagIDs.Bank);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if(slot.Filled)
                        yield return slot;
                }
            }

            slots = player.GetInventoryBagById(InvBagIDs.Bank1);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            slots = player.GetInventoryBagById(InvBagIDs.Bank2);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            slots = player.GetInventoryBagById(InvBagIDs.Bank4);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            slots = player.GetInventoryBagById(InvBagIDs.Bank5);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            slots = player.GetInventoryBagById(InvBagIDs.Bank6);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            slots = player.GetInventoryBagById(InvBagIDs.Bank7);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            slots = player.GetInventoryBagById(InvBagIDs.Bank8);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }

            slots = player.GetInventoryBagById(InvBagIDs.Bank9);
            if (slots.FilledSlots > 0U)
            {
                foreach (var slot in slots.Slots)
                {
                    if (slot.Filled)
                        yield return slot;
                }
            }
        }

        public static InventorySlot GetRingLeft()
        {
            var slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.AdventuringRings).Slots;
            if (slots.Count >= 0)
            {
                var s = slots[0];
                return s;
            }

            return null;
        }

        public static InventorySlot GetRingRight()
        {
            var slots = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.AdventuringRings).Slots;
            if (slots.Count >= 1)
            {
                var s = slots[1];
                return s;
            }

            return null;
        }

        public static InventorySlot GetEquipmentSlot(SpecificBags bagId)
        {
            var player = EntityManager.LocalPlayer;
            List<InventorySlot> slots = null;
            InventorySlot s = null;
            switch (bagId)
            {
                case SpecificBags.Inventory:
                    s = EnumerateInventorySlots().FirstOrDefault(sl => sl.Filled);
                    break;
                case SpecificBags.Bank:
                    s = EnumerateBankSlots().FirstOrDefault(sl => sl.Filled);
                    break;
                case SpecificBags.Head:
                case SpecificBags.Body:
                case SpecificBags.Arms:
                case SpecificBags.Waist:
                case SpecificBags.Feet:
                case SpecificBags.Shirt:
                case SpecificBags.Paints:
                //case SpecificBags.ArtifactPrimary:
                    slots = player.GetInventoryBagById((InvBagIDs)bagId)?.Slots;
                    if (slots != null && slots.Count > 0)
                    {
                        s = slots[0];
                    }
                    break;
                case SpecificBags.WeaponMain:
                    slots = player.GetInventoryBagById(InvBagIDs.AdventuringHands).Slots;
                    if (slots != null && slots.Count > 0)
                    {
                        s = slots[0];
                        return s.Filled ? s : null;
                    }
                    break;
                case SpecificBags.WeaponSecondary:
                    slots = player.GetInventoryBagById(InvBagIDs.AdventuringHands).Slots;
                    if (slots != null && slots.Count > 1)
                    {
                        return slots[1];
                    }
                    break;
                case SpecificBags.RingLeft:
                    slots = player.GetInventoryBagById(InvBagIDs.AdventuringRings).Slots;
                    if (slots != null && slots.Count > 0)
                    {
                        return slots[0];
                    }
                    break;
                case SpecificBags.RingRight:
                    slots = player.GetInventoryBagById(InvBagIDs.AdventuringRings).Slots;
                    if (slots != null && slots.Count > 1)
                    {
                        return slots[1];
                    }
                    break;
                //case SpecificBags.ArtifactSecondary1:
                //    slots = player.GetInventoryBagById(InvBagIDs.ArtifactSecondary).Slots;
                //    if (slots != null && slots.Count > 0)
                //    {
                //        return slots[0];
                //    }
                //    break;
                //case SpecificBags.ArtifactSecondary2:
                //    slots = player.GetInventoryBagById(InvBagIDs.ArtifactSecondary).Slots;
                //    if (slots != null && slots.Count > 1)
                //    {
                //        return slots[1];
                //    }
                //    break;
                //case SpecificBags.ArtifactSecondary3:
                //    slots = player.GetInventoryBagById(InvBagIDs.ArtifactSecondary).Slots;
                //    if (slots != null && slots.Count > 2)
                //    {
                //        return slots[2];
                //    }
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bagId), bagId, null);
            }

            return (s != null && s.Filled) ? s : null;
        }

        /// <summary>
        /// Проверка возможности экипировать предмет, расположенный в слоте <param name="slot"/>
        /// </summary>
        public static bool IsEquippable(this InventorySlot slot)
        {
            if (slot != null
                && slot.IsValid
                && slot.Filled)
            {
                return slot.Item.IsEquippable();
            }

            return false;
        }

        /// <summary>
        /// Проверка возможности экипировать предмет <param name="item"/>
        /// </summary>
        /// <returns></returns>
        public static bool IsEquippable(this Item item)
        {
            foreach (var bagId in item.ItemDef.RestrictBagIDs)
            {
                switch (bagId)
                {
                    case InvBagIDs.AdventuringHead:
                    case InvBagIDs.AdventuringNeck:
                    case InvBagIDs.AdventuringArmor:
                    case InvBagIDs.AdventuringArms:
                    case InvBagIDs.AdventuringWaist:
                    case InvBagIDs.AdventuringFeet:
                    case InvBagIDs.AdventuringHands:
                    case InvBagIDs.AdventuringShirt:
                    case InvBagIDs.AdventuringTrousers:
                    case InvBagIDs.AdventuringRings:
                        return true;
                    //case InvBagIDs.ArtifactPrimary:
                    //case InvBagIDs.ArtifactSecondary:
                    //    break;
                    //case InvBagIDs.AdventuringRanged:
                    //case InvBagIDs.AdventuringSurges:
                    //    break;
                    //case InvBagIDs.Potions:
                    //    break;

                    //case InvBagIDs.None:
                    //case InvBagIDs.Numeric:
                    //case InvBagIDs.Inventory:
                    //case InvBagIDs.Recipe:
                    //case InvBagIDs.Callout:
                    //case InvBagIDs.Lore:
                    //case InvBagIDs.Tokens:
                    //case InvBagIDs.Titles:
                    //case InvBagIDs.ItemSet:
                    //case InvBagIDs.PlayerBags:
                    //case InvBagIDs.PlayerBag1:
                    //case InvBagIDs.PlayerBag2:
                    //case InvBagIDs.PlayerBag3:
                    //case InvBagIDs.PlayerBag4:
                    //case InvBagIDs.PlayerBag5:
                    //case InvBagIDs.PlayerBag6:
                    //case InvBagIDs.PlayerBag7:
                    //case InvBagIDs.PlayerBag8:
                    //case InvBagIDs.PlayerBag9:
                    //case InvBagIDs.Bank:
                    //case InvBagIDs.Bank1:
                    //case InvBagIDs.Bank2:
                    //case InvBagIDs.Bank3:
                    //case InvBagIDs.Bank4:
                    //case InvBagIDs.Bank5:
                    //case InvBagIDs.Bank6:
                    //case InvBagIDs.Bank7:
                    //case InvBagIDs.Bank8:
                    //case InvBagIDs.Bank9:
                    //case InvBagIDs.HiddenLocationData:
                    //case InvBagIDs.LocationData:
                    //case InvBagIDs.Buyback:
                    //case InvBagIDs.Hidden:
                    //case InvBagIDs.Overflow:
                    //case InvBagIDs.Injuries:
                    //case InvBagIDs.SuperCritterPets:
                    //case InvBagIDs.PetEquipBag:
                    //case InvBagIDs.PostLevelCapRewardPacks:
                    //case InvBagIDs.ItemProgressionEvolve:
                    //case InvBagIDs.DifficultyScalingItems:
                    //case InvBagIDs.Loot:
                    //case InvBagIDs.AdventuringTradeGoods:
                    //case InvBagIDs.CraftingInventory:
                    //case InvBagIDs.CraftingResources:
                    //case InvBagIDs.CraftingArtisans:
                    //case InvBagIDs.Currency:
                    //case InvBagIDs.InactivePets:
                    //case InvBagIDs.FashionHead:
                    //case InvBagIDs.FashionAccessory:
                    //case InvBagIDs.FashionBody:
                    //case InvBagIDs.FashionFeet:
                    //case InvBagIDs.Mounts:
                    //case InvBagIDs.MountStorage:
                    //case InvBagIDs.MountActiveSlots:
                    //case InvBagIDs.MountCostumes:
                    //case InvBagIDs.MountActivePowers:
                    //case InvBagIDs.MountPassivePowers:
                    //case InvBagIDs.MountSpeeds:
                    //case InvBagIDs.MountEquippedActiveSlots:
                    //case InvBagIDs.MountEquippedCostume:
                    //case InvBagIDs.MountEquippedActivePower:
                    //case InvBagIDs.MountEquippedPassivePower:
                    //case InvBagIDs.MountEquippedSpeed:
                    //case InvBagIDs.StoredRewardPacks:
                    //case InvBagIDs.HeavyTokens:
                    //case InvBagIDs.KeyItems:
                    //case InvBagIDs.FashionItems:
                    //case InvBagIDs.TarokkaDeck:
                    //case InvBagIDs.RefinementStoredRewards:
                    //case InvBagIDs.PetActiveBonus:
                    //case InvBagIDs.PetEquippedActiveBonus:
                    //case InvBagIDs.VanityPet:
                    //case InvBagIDs.MountCollars:
                    //case InvBagIDs.PetEnhancementPower:
                    //case InvBagIDs.PetEquippedEnhancementPower:
                    //case InvBagIDs.PetGem:
                    //case InvBagIDs.PetEquip:
                    //case InvBagIDs.HiddenItems:
                    //case InvBagIDs.BingoItems:
                    //case InvBagIDs.Max:
                }
            }

            return false;
        }
    }
}
