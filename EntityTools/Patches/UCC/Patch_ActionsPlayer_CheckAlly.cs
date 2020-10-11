using System;
using System.Collections.Generic;
using Astral.Logic.UCC.Classes;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Patchables.Enums;

namespace EntityTools.Patches.UCC
{
#if PATCH_ASTRAL
    ///
    internal class Patch_ActionsPlayer_CheckAlly : Patch
    {
        /// <summary>
        /// Подписка на событие изменения MostInjuredAlly
        /// Необходимо для отслеживания MostInjuredAlly в окне плагина
        /// </summary>
        internal static Action<Entity> MostInjuredAllyChanged = null;

        /// <summary>
        /// Доступ к полю Astral.Logic.UCC.Classes.ActionsPlayer.mostInjuredAlly
        /// </summary>
        static readonly StaticFieldAccessor<Entity> mostInjuredAlly = typeof(ActionsPlayer).GetStaticField<Entity>("mostInjuredAlly");

        internal Patch_ActionsPlayer_CheckAlly() : 
            base(typeof(ActionsPlayer).GetMethod("CheckAlly", ReflectionHelper.DefaultFlags), typeof(Patch_ActionsPlayer_CheckAlly).GetMethod(nameof(CheckAlly), ReflectionHelper.DefaultFlags))
        { }

        /// <summary>
        /// Astral.Logic.UCC.Classes.ActionsPlayer.CheckAlly(List<Entity> entities)
        /// </summary>
        /// <param name="entities"></param>
        private static void CheckAlly(List<Entity> entities)
        {
            Entity mostInjured = null;
            float minHP = 100;
            float currentHP = 100;
            //  игнорируем петов 
#if Companion_as_Ally_out_of_Party
            // Нужно включить, чтобы вне группы персонаж "реагировал" на пета
            if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam
                    || EntityManager.LocalPlayer.Player.PlayerMatchEngineData.PlayerMatch.State == PlayerMatchState.IN_PROGRESS) 
#endif
            {
                foreach (Entity entity in entities)
                {
                    if (entity.IsPlayer && !entity.IsDead && !entity.Character.IsNearDeath
                        && entity.RelationToPlayer == EntityRelation.Friend
                        && entity.Location.Distance3DFromPlayer < 70.0
                        && (currentHP = entity.Character.AttribsBasic.HealthPercent) < minHP)
                    {
                        mostInjured = entity;
                        minHP = currentHP;
                    }
                }
            }
#if Companion_as_Ally_out_of_Party
            else
            {
                // Оригинальный код Астрала
                uint companionID = EntityManager.LocalPlayerCompanion.ContainerId; //EntityManager.LocalCompanion.ContainerId;

                List<Entity> list = new List<Entity>(from allies in entities
                                                        where (allies.IsPlayer || (companionID > 0u && allies.ContainerId == companionID)) && !allies.IsDead && !allies.Character.IsNearDeath && allies.RelationToPlayer == EntityRelation.Friend && allies.Location.Distance3DFromPlayer < 70.0
                                                        orderby allies.Character.AttribsBasic.HealthPercent
                                                        select allies);
            }
#endif

            // Уведомление подписчиков, о смене MostInjuredAlly
            MostInjuredAllyChanged?.Invoke(mostInjured); 

            if (mostInjured != null)
            {
                mostInjuredAlly.Value.Pointer = mostInjured.Pointer;
            }
            else mostInjuredAlly.Value.Pointer = IntPtr.Zero;
        }
    } 
#endif
}
