using System;
using System.Collections.Generic;
using Astral.Logic.UCC.Classes;
using AcTp0Tools.Reflection;
using MyNW.Classes;
using MyNW.Patchables.Enums;

namespace EntityTools.Patches.UCC
{
#if PATCH_ASTRAL
    ///
    internal class Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly : Patch
    {
        /// <summary>
        /// Доступ к полю <see cref="Astral.Logic.UCC.Classes.ActionsPlayer.mostInjuredAlly"/>
        /// </summary>
        static readonly StaticFieldAccessor<Entity> mostInjuredAlly = typeof(ActionsPlayer).GetStaticField<Entity>("mostInjuredAlly");

        internal Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly()
        {
            if (NeedInjection)
            {
                methodToReplace = typeof(ActionsPlayer).GetMethod("CheckAlly", ReflectionHelper.DefaultFlags);
                methodToInject = typeof(Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly).GetMethod(nameof(CheckAlly), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjection => true;

        /// <summary>
        /// Переопределение функции <seealso cref="Astral.Logic.UCC.Classes.ActionsPlayer.CheckAlly(List{Entity})"/>
        /// </summary>
        private static void CheckAlly(List<Entity> entities)
        {
            Entity mostInjured = null;
            float minHP = float.MaxValue;
            //  игнорируем петов 
            foreach (Entity entity in entities)
            {
                float currentHP;
                if (entity.IsPlayer && !entity.IsDead && !entity.Character.IsNearDeath
                    && entity.RelationToPlayer == EntityRelation.Friend
                    && entity.Location.Distance3DFromPlayer < 70.0
                    && (currentHP = entity.Character.AttribsBasic.HealthPercent) < minHP)
                {
                    mostInjured = entity;
                    minHP = currentHP;
                }
            }

            mostInjuredAlly.Value.Pointer = mostInjured?.Pointer ?? IntPtr.Zero;
        }
    } 
#endif
}
