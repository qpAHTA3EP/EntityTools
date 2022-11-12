#define DEBUG_INSERTINSIGNIA

using Astral.Logic.Classes.Map;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class InsertInsignia : Action, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        #region Интерфейс Action
        public override bool NeedToRun => true;

        public override ActionResult Run()
        {
            // freeInsignias - Список всхе неэкипированных инсигний (знаков скакунов)
            List<InventorySlot> freeInsignias = null;

            // Выбираем всех активных коней
            List<InventorySlot> activeMounts = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.MountEquippedActiveSlots).GetItems;
            int insertedNum = 0;

            foreach (InventorySlot mount in activeMounts)
            {
#if DEBUG_INSERTINSIGNIA

                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Check mount '{mount.Item.ItemDef.InternalName}'");
#endif
                // Проверяем наличие свободного места для знака
                foreach (ItemGemSlotDef insgnSlotDef in mount.Item.ItemDef.EffectiveItemGemSlots)
                {
                    // insgnSlotDef - описание слота в скакуне
                    // insgnMountSlot - слот знака в скакуне
                    if (insgnSlotDef.IsValid)
                    {
                        ItemGemSlot insgnMountSlot = mount.Item.SpecialProps.GetGemSlotByIndex(insgnSlotDef.Index);
                        if (insgnMountSlot == null || !insgnMountSlot.IsValid || !insgnMountSlot.SlottedItem.IsValid)
                        {
                            // обнаружен "пустой слот" знака скакуна
#if DEBUG_INSERTINSIGNIA
                            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Mount [{mount.Item.ItemDef.InternalName}] has free slot {insgnSlotDef.Index}");
#endif
                            // ищем в сумке подходящие инсигнии (знаки скакуна)
                            if (freeInsignias == null || freeInsignias.Count == 0)
                            {
                                // Ищем в сумке все неэкипированные инсигнии (впервый раз)
                                freeInsignias = EntityManager.LocalPlayer.BagsItems.FindAll(slot => slot.Item.ItemDef.Categories.Contains(ItemCategory.Insignia)
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
                                if (freeInsignias.Count == 0)
                                {
                                    // в инвентаре отсутствуют инсигнии
#if DEBUG_INSERTINSIGNIA
                                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: No one insignia found in the Bags");
#endif
                                    return ActionResult.Skip;
                                }
                                // сортировка списка "инсигний";
                                freeInsignias.Sort(InsigniaQualityDescendingComparison);
                            }

                            // Ищем первую попавшуюся подходящую инсигнию (знак скакуна)
                            InventorySlot insigniaBagSlot = freeInsignias.Find(insSlot => insSlot.Filled && (insgnSlotDef.Type == (uint)InsigniaType.Universal || insSlot.Item.ItemDef.GemType == insgnSlotDef.Type));
                            // сортировка списка "инсигний" по убыванию качества;
                            freeInsignias.Sort(InsigniaQualityDescendingComparison);

                            // экипируем найденный знак
                            if (insigniaBagSlot != null && insigniaBagSlot.IsValid)
                            {
                                mount.Item.GemThisItem(insigniaBagSlot.Item, insgnSlotDef.Index);
                                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Insert '{insigniaBagSlot.Item.ItemDef.InternalName}' at the slot {insgnSlotDef.Index} of [{mount.Item.ItemDef.InternalName}]");

                                insertedNum++;

                                // Удаляем слот сумки, в котором находился знак, из списка
                                // если этот слот стал пустым
                                if (!insigniaBagSlot.Filled)
                                    freeInsignias.Remove(insigniaBagSlot);
                            }
                        }
                    }
                }
            }

            return insertedNum > 0 ? ActionResult.Completed : ActionResult.Skip;
        }

        public override string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(label))
                    label = actionIDstr;
                return label;
            }
        }
        private string label;
        private string actionIDstr;

        public override string InternalDisplayName => string.Empty;

        protected override bool IntenalConditions => true;

        protected override ActionValidity InternalValidity => new ActionValidity();

        public override bool UseHotSpots => false;

        protected override Vector3 InternalDestination => Vector3.Empty;

        public override void InternalReset()
        {
            actionIDstr = string.Concat(actionIDstr, '[', ActionID, ']');
        }

        public override void GatherInfos() { }

        public override void OnMapDraw(GraphicsNW graphics) { } 
        #endregion

        #region Вспомогательные функции
        /// <summary>
        /// Сравнение инсигний по качеству
        /// </summary>
        /// <param name="slot1"></param>
        /// <param name="slot2"></param>
        /// <returns></returns>
        internal static int InsigniaQualityDescendingComparison(InventorySlot slot1, InventorySlot slot2)
        {

            if (slot1 != null && slot1.IsValid && slot1.Item.IsValid && slot1.Item.Count > 0)
            {
                if (slot2 != null && slot2.IsValid && slot2.Item.IsValid && slot2.Item.Count > 0)
                    return (int)slot2.Item.ItemDef.Quality - (int)slot1.Item.ItemDef.Quality;
                return -1;
            }
            else
            {
                if (slot2 != null && slot2.IsValid && slot2.Item.IsValid && slot2.Item.Count > 0)
                    return 1;
            }
            // оба slot1 и slot2 являются невалидными. т.е. одинкаовыми
            return 0;
        }
        #endregion
    }
}
