#define DEBUG_INSERTINSIGNIA

using System;
using System.Collections.Generic;
using Astral.Logic.Classes.Map;
using EntityTools.Quester.Actions;
using EntityCore.MountInsignias;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using static Astral.Quester.Classes.Action;
using Astral.Quester.Classes;
using System.ComponentModel;
using EntityTools.Core.Interfaces;
using EntityTools;
using EntityTools.Quester;

namespace EntityCore.Quester.Action
{
    internal class InsertInsigniaEngine : IQuesterActionEngine
    {
        private InsertInsignia @this = null;
        private string label;
        private string actionIDstr;

        internal InsertInsigniaEngine(InsertInsignia ii)
        {
            @this = ii;

            @this.Bind(this);
            @this.PropertyChanged += OnPropertyChanged;

            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized: {ActionLabel}");
        }
        ~InsertInsigniaEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.Unbind();
                @this = null;
            }
        }

        public bool Rebase(Astral.Quester.Classes.Action action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is InsertInsignia ii)
            {
                InternalRebase(ii);
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} reinitialized");
                return true;
            }
            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be cast to '" + nameof(InsertInsignia) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(InsertInsignia ii)
        {
            // Убираем привязку к старой команде
            @this?.Unbind();

            @this = ii;
            @this.PropertyChanged += OnPropertyChanged;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

            @this.Bind(this);

            return true;
        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) { }
        public void OnPropertyChanged(Astral.Quester.Classes.Action sender, string propertyName) { }

        public bool NeedToRun => true;

        public ActionResult Run()
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

                ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}: Check mount '{mount.Item.ItemDef.InternalName}'");
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
                            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}: Mount [{mount.Item.ItemDef.InternalName}] has free slot {insgnSlotDef.Index}");
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
                                    ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}: No one insignia found in the Bags");
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
                                ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}: Insert '{insigniaBagSlot.Item.ItemDef.InternalName}' at the slot {insgnSlotDef.Index} of [{mount.Item.ItemDef.InternalName}]");
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
            }

            return ActionResult.Completed;
        }

        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(label))
                    label = @this.GetType().Name;
                return label;
            }
        }

        public bool InternalConditions => true;

        public ActionValidity InternalValidity => new ActionValidity();

        public bool UseHotSpots => false;

        public Vector3 InternalDestination => Vector3.Empty;

        public void InternalReset() { }

        public void GatherInfos() { }

        public void OnMapDraw(GraphicsNW graphics) { }

        #region Вспомогательные функции
        /// <summary>
        /// Сравнение инсигний по качеству
        /// </summary>
        /// <param name="slot1"></param>
        /// <param name="slot2"></param>
        /// <returns></returns>
        internal static int InsigniaQualityDescendingComparison(InventorySlot slot1, InventorySlot slot2)
        {
            //char slot1Ends = '\u0000',
            //     slot2Ends = '\u0000';
            //if (slot1!= null && slot1.IsValid && slot1.Item.IsValid
            //    && char.IsDigit(slot1Ends = slot1.Item.ItemDef.InternalName[slot1.Item.ItemDef.InternalName.Length - 1]))
            //{
            //    // slot1 - валиден и последний символ идентификатора является числом
            //    if (slot2!=null && slot2.IsValid && slot2.Item.IsValid
            //        && char.IsDigit(slot2Ends= slot2.Item.ItemDef.InternalName[slot2.Item.ItemDef.InternalName.Length - 1]))
            //    {
            //        // slot2 - валиден и последний символ идентификатора является числом
            //        return slot2Ends - slot1Ends;
            //    }
            //}
            //else
            //{
            //    // slot1 - не валиден или последний символ идентификатора не является числом
            //    if (slot2 != null && slot2.IsValid && slot2.Item.IsValid
            //        && char.IsDigit(slot2Ends = slot2.Item.ItemDef.InternalName[slot2.Item.ItemDef.InternalName.Length - 1]))
            //    {
            //        return slot2Ends - '0';
            //    }
            //}
            //// оба slot1 и slot2 являются невалидными. т.е. одинкаовыми

            if (slot1 != null && slot1.IsValid && slot1.Item.IsValid && slot1.Item.Count > 0)
            {
                if (slot2 != null && slot2.IsValid && slot2.Item.IsValid && slot2.Item.Count > 0)
                    return (int)slot2.Item.ItemDef.Quality - (int)slot1.Item.ItemDef.Quality;
                else return -1;
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
