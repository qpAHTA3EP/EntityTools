using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;

namespace EntityTools.Tools.Entities
{
    public static class CachedSearch
    {
        /// <summary>
        /// Объявление типа делегата д
        /// </summary>
        /// <param name="e"></param>
        internal delegate void OptionCheck(Entity e);

        /// <summary>
        /// Кэш Entities
        /// </summary>
        private static EntityCache EntityCache = new EntityCache();

        /// <summary>
        /// Поиск ближайшего Entity из entities, соответствующего условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="pattern">Шаблон, которому должен соответствовать идентификатор (имя) Entity, заданные параметном </param>
        /// <param name="matchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) Entity, с которым сопостовляется entPattern</param>
        /// <param name="healthCheck">Если True, искать только Entity с количеством здоровья (НР) больше 0 и без флага IsDead</param>
        /// <param name="range">Предельное расстояние поиска, дальше которого Утешен игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те Entity, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegions">Список CustomRegion'ов, в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <param name="interactable">Если True, искать только Entity с которыми можно взаимодействать</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
            bool healthCheck = false, float range = 0, bool regionCheck = false, List<CustomRegion> customRegions = null, 
            Predicate<Entity> specialCheck = null)
        {
            if (EntityCache.TryGetValue(out EntityCacheRecord cache, pattern, matchType, nameType, setType))
            {
                // конструируем функтор для дополнительных проверок Entity и поиска ближайшего
                float closestDistance = float.MaxValue;
                Entity closestEntity = null;
                Action<Entity> optionCheck;

                if (customRegions == null)
                {
                    if (specialCheck == null)
                        optionCheck = (Entity e) =>
                            {
                                float eDistance = e.CombatDistance2;
                                if (eDistance < range
                                    && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                    && (!healthCheck || !e.IsDead)
                                    && e.CombatDistance2 < closestDistance)
                                {
                                    closestEntity = e;
                                    closestDistance = eDistance;
                                }
                            };
                    else optionCheck = (Entity e) =>
                            {
                                float eDistance = e.CombatDistance2;
                                if (eDistance < range
                                    && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                    && (!healthCheck || !e.IsDead)
                                    && specialCheck(e)
                                    && e.CombatDistance2 < closestDistance)
                                {
                                    closestEntity = e;
                                    closestDistance = eDistance;
                                }
                            };
                }
                else
                {
                    if (specialCheck == null)
                        optionCheck = (Entity e) =>
                        {
                            float eDistance = e.CombatDistance2;
                            if (eDistance < range
                                && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(e, cr)) != null
                                && e.CombatDistance2 < closestDistance)
                            {
                                closestEntity = e;
                                closestDistance = eDistance;
                            }
                        };
                    else optionCheck = (Entity e) =>
                    {
                        float eDistance = e.CombatDistance2;
                        if (eDistance < range
                            && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (!healthCheck || !e.IsDead)
                            && customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(e, cr)) != null
                            && specialCheck(e)
                            && e.CombatDistance2 < closestDistance)
                        {
                            closestEntity = e;
                            closestDistance = eDistance;
                        }
                    };
                }

                // Проверяем Entities
                cache.Entities.ForEach(optionCheck);

                if (closestEntity == null)
                    return new Entity(IntPtr.Zero);
                else return closestEntity;
            }
            else return new Entity(IntPtr.Zero);
        }
    }
}