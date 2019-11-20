#define DEBUG_INSERTINSIGNIA

using System;
using System.Collections.Generic;
using Astral.Logic.Classes.Map;
using EntityTools.Tools.MountInsignias;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.Actions
{
    [Serializable]
    public class InsertInsignia : Astral.Quester.Classes.Action
    {
        #region Inherited
        public override string ActionLabel => GetType().Name;
        public override bool NeedToRun => true;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => true;
        protected override Vector3 InternalDestination => new Vector3();
        protected override ActionValidity InternalValidity => new ActionValidity();
        public override void GatherInfos() { }
        public override void InternalReset() { }
        public override void OnMapDraw(GraphicsNW graph) { }
        #endregion

        //[Description("Not used yet")]
        //[Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        //public ItemFilterCore Mounts { get; set; }

        public override ActionResult Run()
        {
            // freeInsignias - Список всхе неэкипированных инсигний (знаков скакунов)
            List<InventorySlot> freeInsignias = null;

            // Выбираем всех активных коней
            List<InventorySlot> activeMounts = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.MountEquippedActiveSlots).GetItems;
                                             //EntityManager.LocalPlayer.BagsItems.FindAll(slot => slot.BagId == MyNW.Patchables.Enums.InvBagIDs.MountEquippedActiveSlots);
               
            foreach (InventorySlot mount in activeMounts)
            {
#if DEBUG_INSERTINSIGNIA
                int insertedNum = 0;

                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: Check mount '{mount.Item.ItemDef.InternalName}'");
#endif
                // Проверяем наличие свободного места для знака
                foreach (ItemGemSlotDef insgnSlotDef in mount.Item.ItemDef.EffectiveItemGemSlots)
                {
                    // insgnSlotDef - описание слота в скакуне
                    // insgnMountSlot - слот знака в скакуне
                    ItemGemSlot insgnMountSlot = mount.Item.SpecialProps.GetGemSlotByIndex(insgnSlotDef.Index);
                    if (insgnSlotDef != null && insgnSlotDef.IsValid
                        && (insgnMountSlot == null || !insgnMountSlot.IsValid || !insgnMountSlot.SlottedItem.IsValid))
                    {
                        // обнаружен "пустой слот" знака скакуна
#if DEBUG_INSERTINSIGNIA
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: Mount [{mount.Item.ItemDef.InternalName}] has free slot {insgnSlotDef.Index}");
#endif
                        // ищем в сумке подходящие инсигнии (знаки скакуна)
                        if (freeInsignias == null || freeInsignias.Count == 0)
                        {
                            // Ищем в сумке все неэкипированные инсигнии (впервый раз)
                            freeInsignias = EntityManager.LocalPlayer.BagsItems.FindAll(slot =>
                                                                                            slot.Item.ItemDef.Categories.Contains(ItemCategory.Insignia)
                                                                                            /* Вариант 2*/
                                                                                            //slot.Item.ItemDef.Type == ItemType.Gem
                                                                                            //&& (slot.Item.ItemDef.GemType == (uint)InsigniaType.Barbed 
                                                                                            //    || slot.Item.ItemDef.GemType == (uint)InsigniaType.Crescent
                                                                                            //    || slot.Item.ItemDef.GemType == (uint)InsigniaType.Enlightened
                                                                                            //    || slot.Item.ItemDef.GemType == (uint)InsigniaType.Illuminated
                                                                                            //    || slot.Item.ItemDef.GemType == (uint)InsigniaType.Regal)
                                                                                            /* Вариант 3*/
                                                                                            //slot.Item.ItemDef.InternalName.StartsWith("Insignia", System.StringComparison.OrdinalIgnoreCase)
                                                                                        );
                            if (freeInsignias==null || freeInsignias.Count == 0)
                            {
                                // в инвентаре отсутствуют инсигнии
#if DEBUG_INSERTINSIGNIA
                                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: No one insignia found in the Bags");
#endif
                                return ActionResult.Skip;
                            }
                            // сортировка списка "инсигний";
                            freeInsignias.Sort(InsigniaQualityDescendingComparison);
                        }

                        // Ищем первую попавшуюся подходящую инсигнию (знак скакуна)
                        InventorySlot insigniaBagSlot = freeInsignias.Find(insSlot => insSlot.Filled && (insgnSlotDef.Type == (uint)InsigniaType.Universal || insSlot.Item.ItemDef.GemType == insgnSlotDef.Type));
                        // сортировка списка "инсигний" по убыванию качества;
                        freeInsignias.Sort(Actions.InsertInsignia.InsigniaQualityDescendingComparison);

                        // экипируем найденный знак
                        if (insigniaBagSlot != null && insigniaBagSlot.IsValid)
                        {
                            mount.Item.GemThisItem(insigniaBagSlot.Item, insgnSlotDef.Index);
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: Insert '{insigniaBagSlot.Item.ItemDef.InternalName}' at the slot {insgnSlotDef.Index} of [{mount.Item.ItemDef.InternalName}]");
#if DEBUG_INSERTINSIGNIA
                            insertedNum++;
#endif
                            // Удаляем слот сумки, в котором находился знак, из списка
                            // если этот слот стал пустым
                            if (!insigniaBagSlot.Filled)
                                freeInsignias.Remove(insigniaBagSlot);
                        }
                    }
                }
            }

            return ActionResult.Completed;
        }

        /// <summary>
        /// Сравнение инсигний по качеству, которое закодиновано последним символов идентификатора
        /// Insignia_*_R[1-5];
        /// </summary>
        /// <param name="slot1"></param>
        /// <param name="slot2"></param>
        /// <returns></returns>
        internal static int InsigniaQualityDescendingComparison(InventorySlot slot1, InventorySlot slot2)
        {
            char slot1Ends = '\u0000',
                 slot2Ends = '\u0000';
            if (slot1!= null && slot1.IsValid && slot1.Item.IsValid
                && char.IsDigit(slot1Ends = slot1.Item.ItemDef.InternalName[slot1.Item.ItemDef.InternalName.Length - 1]))
            {
                // slot1 - валиден и последний символ идентификатора является числом
                if (slot2!=null && slot2.IsValid && slot2.Item.IsValid
                    && char.IsDigit(slot2Ends= slot2.Item.ItemDef.InternalName[slot2.Item.ItemDef.InternalName.Length - 1]))
                {
                    // slot2 - валиден и последний символ идентификатора является числом
                    return slot2Ends - slot1Ends;
                }
            }
            else
            {
                // slot1 - не валиден или последний символ идентификатора не является числом
                if (slot2 != null && slot2.IsValid && slot2.Item.IsValid
                    && char.IsDigit(slot2Ends = slot2.Item.ItemDef.InternalName[slot2.Item.ItemDef.InternalName.Length - 1]))
                {
                    return slot2Ends - '0';
                }
            }
            // оба slot1 и slot2 являются невалидными. т.е. одинкаовыми
            return 0;
        }
    }
}
