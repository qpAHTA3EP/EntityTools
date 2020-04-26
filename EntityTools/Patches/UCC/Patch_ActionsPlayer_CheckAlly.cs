using Astral.Logic.UCC.Classes;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityTools.Reflection;

namespace EntityTools.Patches
{
#if DEVELOPER
    ///
    internal class Patch_ActionsPlayer_CheckAlly : Patch
    {
        //mostInjuredAlly;
        /// <summary>
        /// Доступ к полю Astral.Logic.UCC.Classes.ActionsPlayer.mostInjuredAlly
        /// </summary>
        static readonly StaticFielsAccessor<Entity> mostInjuredAlly = typeof(Astral.Logic.UCC.Classes.ActionsPlayer).GetStaticField<Entity>("mostInjuredAlly");

        internal Patch_ActionsPlayer_CheckAlly() : 
            base(typeof(Astral.Logic.UCC.Classes.ActionsPlayer).GetMethod("CheckAlly", ReflectionHelper.DefaultFlags), typeof(Patch_ActionsPlayer_CheckAlly).GetMethod(nameof(CheckAlly), ReflectionHelper.DefaultFlags))
        { }

        /// <summary>
        /// Astral.Logic.UCC.Classes.ActionsPlayer.CheckAlly(List<Entity> entities)
        /// </summary>
        /// <param name="entities"></param>
        private static void CheckAlly(List<Entity> entities)
        {
            Entity mostInjured = null;

            //  игнорируем петов 
#if false
            if (EntityManager.LocalPlayer.PlayerTeam.IsInTeam
                    || EntityManager.LocalPlayer.Player.PlayerMatchEngineData.PlayerMatch.State == PlayerMatchState.IN_PROGRESS) 
#endif
            {
                foreach (Entity entity in entities)
                {
                    if (entity.IsPlayer && !entity.IsDead && !entity.Character.IsNearDeath
                        && entity.RelationToPlayer == EntityRelation.Friend
                        && entity.Location.Distance3DFromPlayer < 70.0)
                    {
                        mostInjured = entity;
                    }
                }
            }

#if false
            // Оригинальный код Астрала
            uint companionID = EntityManager.LocalPlayerCompanion.ContainerId; //EntityManager.LocalCompanion.ContainerId;

            List<Entity> list = new List<Entity>(from allies in entities
                                                    where (allies.IsPlayer || (companionID > 0u && allies.ContainerId == companionID)) && !allies.IsDead && !allies.Character.IsNearDeath && allies.RelationToPlayer == EntityRelation.Friend && allies.Location.Distance3DFromPlayer < 70.0
                                                    orderby allies.Character.AttribsBasic.HealthPercent
                                                    select allies);
            if (list.Count > 0/* && General.GetInLosEntities(list).Count > 0*/)
            {
                mostInjuredAlly.Value.Pointer = list[0].Pointer;
                return;
            }
            mostInjuredAlly.Value.Pointer = IntPtr.Zero; 
#endif
            if (mostInjured is null)
                mostInjuredAlly.Value.Pointer = mostInjured.Pointer;
        }
    } 
#endif
}
