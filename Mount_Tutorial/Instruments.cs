using Astral.Classes;
using Astral.Logic.NW;
using MyNW.Classes;
using MyNW.Internals;
using System.Collections.Generic;
using static Astral.Quester.Classes.Action;

namespace Mount_Tutorial
{
    internal static  class Instruments
    {
        internal static ActionResult MainMethod()
        {
            if (Missions.MissionIsCompletedByPath("Nw_Pe_M9_Mount_Tutorial/Horse_Store")
                && Missions.MissionIsCompletedByPath("Nw_Pe_M9_Mount_Tutorial/Horse_Equip")
                && !Missions.MissionIsCompletedByPath("Nw_Pe_M9_Mount_Tutorial"))
            {
                if (Game.IsCursorModeEnabled)
                {
                    Game.ToggleCursorMode(false);
                    System.Threading.Thread.Sleep(2000);
                }

                Timeout timer = new Timeout(5000);
                GameCommands.Execute("mounts");
                while (!UIManager.GetUIGenByName("Playerstatus_Mounts_Subtab_Activemounts").IsVisible
                    && !timer.IsTimedOut)
                        System.Threading.Thread.Sleep(1000);
                if (UIManager.GetUIGenByName("Playerstatus_Mounts_Subtab_Activemounts").IsVisible)
                {
                    GameCommands.Execute("gensendmessage Playerstatus_Mounts_Subtab_Activemounts Clicked");

                    InventorySlot insgSl = null;
                    timer.Reset();
                    while (!timer.IsTimedOut)
                    {
                        System.Threading.Thread.Sleep(1000);
                        insgSl = InsertInsignia();
                        if (insgSl == null)
                            continue;
                        else return ActionResult.Completed;
                    }
                    if (insgSl == null)
                        return ActionResult.Fail;
                    else return ActionResult.Completed;
                }
            }
            return ActionResult.Skip;
        }

        internal static InventorySlot InsertInsignia(List<InventorySlot> freeInsignias = null)
        {
            // freeInsignias - Список всех неэкипированных инсигний (знаков скакунов)            

            // Выбираем всех активных коней
            List<InventorySlot> activeMounts = EntityManager.LocalPlayer.GetInventoryBagById(MyNW.Patchables.Enums.InvBagIDs.MountEquippedActiveSlots).GetItems;

            foreach (InventorySlot mount in activeMounts)
            {
                // Проверяем наличие свободного места для знака
                foreach (ItemGemSlotDef insgnSlotDef in mount.Item.ItemDef.EffectiveItemGemSlots)
                {
                    // insgnSlotDef - описание слота
                    // insgnSlot - слот знака
                    ItemGemSlot insgnSlot = mount.Item?.SpecialProps?.GetGemSlotByIndex(insgnSlotDef.Index);
                    if (insgnSlotDef != null && insgnSlotDef?.IsValid==true
                        && (insgnSlot == null || !insgnSlot.IsValid || !(insgnSlot.SlottedItem?.IsValid==true)))
                    {
                        // обнаружен "пустой слот" знака скакуна

                        // ищем подходящие инсигнии (знаки скакуна)
                        if (freeInsignias == null || freeInsignias.Count == 0)
                        {
                            // Ищем все неэкипированные инсигнии (впервый раз)
                            freeInsignias = EntityManager.LocalPlayer?.BagsItems?.FindAll(slot => slot.Item?.ItemDef?.InternalName?.StartsWith("Insignia")==true);
                            if (freeInsignias == null || freeInsignias.Count == 0)
                            {
                                // в инвентаре отсутствуют инсигнии
                                return null;
                            }
                        }

                        // Ищем первую попавшуюся подходящую инсигнию (знак скакуна)
                        InventorySlot insignia = freeInsignias.Find(insgn => (insgnSlotDef.Type == 8126464u || insgn.Item?.ItemDef?.GemType == insgnSlotDef.Type));

                        // экипируем найденный знак
                        if (insignia != null && insignia.IsValid)
                        {
                            mount.Item.GemThisItem(insignia.Item, insgnSlotDef.Index);
#if DEBUG
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"Insert '{insignia.Item?.ItemDef?.InternalName}' at the slot {insgnSlotDef.Index} of [{mount.Item?.ItemDef?.InternalName}]");
#endif
                            return insignia;
                        }
                    }

                }
            }

            return null;
        }

    }
}
