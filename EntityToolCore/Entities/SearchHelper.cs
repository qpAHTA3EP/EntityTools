using System;
using System.Collections.Generic;
using Astral.Quester.Classes;
using MyNW.Classes;
using EntityTools.Extensions;
using EntityTools.Tools.CustomRegions;
using MyNW.Internals;

namespace EntityCore.Entities
{
    public static class SearchHelper
    {
        /// <summary>
        /// Конструирование предиката, для проверки <seealso cref="Entity"/> на соответствие заданным признакам
        /// </summary>
        /// <param name="healthCheck">Если True, искать только <see cref="Entity"/> с количеством здоровья (НР) больше 0 и без флага <see cref="Entity.IsDead"/></param>
        /// <param name="range">Предельное расстояние поиска, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="zRange">Предельная разница высот относительно персонажа, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те <see cref="Entity"/>, которые находятся в одном регионе с игроком</param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        public static Predicate<Entity> Construct_EntityAttributePredicate(bool healthCheck = false,
                                                                           float range = 0, float zRange = 0,
                                                                           bool regionCheck = false,
                                                                           Predicate<Entity> specialCheck = null)
        {
#if false
            if (zRange <= 0)
                zRange = Astral.Controllers.Settings.Get.MaxElevationDifference;  
#endif
            Predicate<Entity> predicate;
            if (specialCheck is null)
            {
                if (healthCheck)
                {
                    if (range > 0)
                    {
                        if (zRange > 0)
                            predicate = (Entity e) => !e.IsDead
                                    && e.Location.Distance3DFromPlayer < range
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;

                        else predicate = (Entity e) => !e.IsDead
                                    && e.Location.Distance3DFromPlayer < range;

                    }
                    else
                    {
                        if (zRange > 0)
                            predicate = (Entity e) => !e.IsDead
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;

                        else predicate = (Entity e) => !e.IsDead;
                    }
                }
                else
                {
                    if (range > 0)
                    {
                        if (zRange > 0)
                            predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;
                        else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range;
                    }
                    else
                    {
                        if (zRange > 0)
                            predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange;
                        else predicate = (Entity e) => true;
                    }
                }
            }
            else
            {
                if (healthCheck)
                {
                    if (range > 0)
                    {
                        if (zRange > 0)
                            predicate = (Entity e) => !e.IsDead
                                    && e.Location.Distance3DFromPlayer < range
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                    && specialCheck(e);

                        else predicate = (Entity e) => !e.IsDead
                                    && e.Location.Distance3DFromPlayer < range
                                    && specialCheck(e);

                    }
                    else
                    {
                        if (zRange > 0)
                            predicate = (Entity e) => !e.IsDead
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                    && specialCheck(e);

                        else predicate = (Entity e) => !e.IsDead
                                    && specialCheck(e);
                    }
                }
                else
                {
                    if (range > 0)
                    {
                        if (zRange > 0)
                            predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                    && specialCheck(e);
                        else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                    && specialCheck(e);
                    }
                    else
                    {
                        if (zRange > 0)
                            predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                    && specialCheck(e);
                        else predicate = (Entity e) => specialCheck(e);
                    }
                }
            }
            return predicate;
        }

        /// <summary>
        /// Конструирование предиката, для проверки <seealso cref="Entity"/> на соответствие заданным признакам
        /// </summary>
        /// <param name="healthCheck">Если True, искать только <see cref="Entity"/> с количеством здоровья (НР) больше 0 и без флага <see cref="Entity.IsDead"/></param>
        /// <param name="range">Предельное расстояние поиска, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="zRange">Предельная разница высот относительно персонажа, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те <see cref="Entity"/>, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegions">Если задано, то проверяется нахождение в области, заданной сочетанием <seealso cref="CustomRegion"/>'ов</param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns></returns>
        public static Predicate<Entity> Construct_EntityAttributePredicate(bool healthCheck = false,
                                                                           float range = 0, float zRange = 0,
                                                                           bool regionCheck = false,
                                                                           List<CustomRegion> customRegions = null,
                                                                           Predicate<Entity> specialCheck = null)
        {
#if false
            if (zRange <= 0)
                zRange = Astral.Controllers.Settings.Get.MaxElevationDifference;  
#endif

            Predicate<Entity> predicate;
            if (customRegions?.Count > 0)
            {
                if (specialCheck is null)
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Cover(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && customRegions.Cover(e);

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Cover(e);

                            else predicate = (Entity e) => !e.IsDead;
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Cover(e);
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && customRegions.Cover(e);
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                        && customRegions.Cover(e);
                            else predicate = (Entity e) => customRegions.Cover(e);
                        }
                    }
                }
                else
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Cover(e)
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && specialCheck(e);

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Cover(e)
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && specialCheck(e);
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Cover(e)
                                        && specialCheck(e);
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && customRegions.Cover(e)
                                        && specialCheck(e);
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                        && customRegions.Cover(e)
                                        && specialCheck(e);
                            else predicate = (Entity e) => specialCheck(e)
                                        && customRegions.Cover(e);
                        }
                    }
                }
            }
            else
            {
                if (specialCheck is null)
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range;

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;

                            else predicate = (Entity e) => !e.IsDead;
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range;
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange;
                            else predicate = (Entity e) => true;
                        }
                    }
                }
                else
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && specialCheck(e);

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && specialCheck(e);
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && specialCheck(e);
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                        && specialCheck(e);
                            else predicate = (Entity e) => specialCheck(e);
                        }
                    }
                }
            }
            return predicate;
        }

        /// <summary>
        /// Конструирование предиката, для проверки <seealso cref="Entity"/> на соответствие заданным признакам
        /// </summary>
        /// <param name="healthCheck">Если True, искать только <see cref="Entity"/> с количеством здоровья (НР) больше 0 и без флага <see cref="Entity.IsDead"/></param>
        /// <param name="range">Предельное расстояние поиска, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="zRange">Предельная разница высот относительно персонажа, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="regionCheck">Флаг, указывающий на необходимость выполнять проверку совпадения <seealso cref="Entity.RegionInternalName"/> у игрока и <see cref="Entity"/></param>
        /// <param name="customRegions">Сочетание <seealso cref="CustomRegion"/> в пределах которого производится поиск <seealso cref="Entity"/></param>
        /// <param name="outsideCustomRegions">Если задано, то проверяется нахождение за пределами области, заданной <param name="customRegions"/></param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns></returns>
        public static Predicate<Entity> Construct_EntityAttributePredicate(bool healthCheck = false,
                                                                           float range = 0, float zRange = 0,
                                                                           bool regionCheck = false,
                                                                           CustomRegionCollection customRegions = null,
                                                                           bool outsideCustomRegions = false,
                                                                           Predicate<Entity> specialCheck = null)
        {
#if false
            if (zRange <= 0)
                zRange = Astral.Controllers.Settings.Get.MaxElevationDifference; 
#endif

            Predicate<Entity> predicate;
            if (customRegions?.Count > 0)
            {
                //TODO удалить аргумент outsideCustomRegions
                if (outsideCustomRegions)
                {
                    if (specialCheck is null)
                    {
                        if (healthCheck)
                        {
                            if (range > 0)
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => !e.IsDead
                                            && e.Location.Distance3DFromPlayer < range
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Outside(e);

                                else predicate = (Entity e) => !e.IsDead
                                            && e.Location.Distance3DFromPlayer < range
                                            && customRegions.Outside(e);

                            }
                            else
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => !e.IsDead
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Outside(e);

                                else predicate = (Entity e) => !e.IsDead
                                            && customRegions.Outside(e);
                            }
                        }
                        else
                        {
                            if (range > 0)
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Outside(e);
                                else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                            && customRegions.Outside(e);
                            }
                            else
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                            && customRegions.Outside(e);
                                else predicate = (Entity e) => customRegions.Outside(e);
                            }
                        }
                    }
                    else
                    {
                        if (healthCheck)
                        {
                            if (range > 0)
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => !e.IsDead
                                            && e.Location.Distance3DFromPlayer < range
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Outside(e)
                                            && specialCheck(e);

                                else predicate = (Entity e) => !e.IsDead
                                            && e.Location.Distance3DFromPlayer < range
                                            && customRegions.Outside(e)
                                            && specialCheck(e);

                            }
                            else
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => !e.IsDead
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Outside(e)
                                            && specialCheck(e);

                                else predicate = (Entity e) => !e.IsDead
                                            && customRegions.Outside(e)
                                            && specialCheck(e);
                            }
                        }
                        else
                        {
                            if (range > 0)
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Outside(e)
                                            && specialCheck(e);
                                else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                            && customRegions.Outside(e)
                                            && specialCheck(e);
                            }
                            else
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                            && customRegions.Outside(e)
                                            && specialCheck(e);
                                else predicate = (Entity e) => specialCheck(e)
                                            && customRegions.Outside(e);
                            }
                        }
                    }
                }
                else
                {
                    if (specialCheck is null)
                    {
                        if (healthCheck)
                        {
                            if (range > 0)
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => !e.IsDead
                                            && e.Location.Distance3DFromPlayer < range
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Within(e);

                                else predicate = (Entity e) => !e.IsDead
                                            && e.Location.Distance3DFromPlayer < range
                                            && customRegions.Within(e);

                            }
                            else
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => !e.IsDead
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Within(e);

                                else predicate = (Entity e) => !e.IsDead
                                            && customRegions.Within(e);
                            }
                        }
                        else
                        {
                            if (range > 0)
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Within(e);
                                else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                            && customRegions.Within(e);
                            }
                            else
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                            && customRegions.Within(e);
                                else predicate = (Entity e) => customRegions.Within(e);
                            }
                        }
                    }
                    else
                    {
                        if (healthCheck)
                        {
                            if (range > 0)
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => !e.IsDead
                                            && e.Location.Distance3DFromPlayer < range
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Within(e)
                                            && specialCheck(e);

                                else predicate = (Entity e) => !e.IsDead
                                            && e.Location.Distance3DFromPlayer < range
                                            && customRegions.Within(e)
                                            && specialCheck(e);

                            }
                            else
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => !e.IsDead
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Within(e)
                                            && specialCheck(e);

                                else predicate = (Entity e) => !e.IsDead
                                            && customRegions.Within(e)
                                            && specialCheck(e);
                            }
                        }
                        else
                        {
                            if (range > 0)
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                            && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                            && customRegions.Within(e)
                                            && specialCheck(e);
                                else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                            && customRegions.Within(e)
                                            && specialCheck(e);
                            }
                            else
                            {
                                if (zRange > 0)
                                    predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                            && customRegions.Within(e)
                                            && specialCheck(e);
                                else predicate = (Entity e) => specialCheck(e)
                                            && customRegions.Within(e);
                            }
                        }
                    }
                }
            }
            else
            {
                if (specialCheck is null)
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range;

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;

                            else predicate = (Entity e) => !e.IsDead;
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range;
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange;
                            else predicate = (Entity e) => true;
                        }
                    }
                }
                else
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && specialCheck(e);

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && specialCheck(e);
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && specialCheck(e);
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                        && specialCheck(e);
                            else predicate = (Entity e) => specialCheck(e);
                        }
                    }
                }
            }
            return predicate;
        }

        /// <summary>
        /// Конструирование команды для добавления в список <paramref name="resultList"/> элемента <seealso cref="Entity"/>, отвечающего дополнительным признакам
        /// </summary>
        /// <param name="resultList">Пополняемый список</param>
        /// <param name="healthCheck">Если True, искать только <see cref="Entity"/> с количеством здоровья (НР) больше 0 и без флага <see cref="Entity.IsDead"/></param>
        /// <param name="range">Предельное расстояние поиска, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="zRange">Предельная разница высот относительно персонажа, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="regionCheck">Флаг, указывающий на необходимость выполнять проверку совпадения <seealso cref="Entity.RegionInternalName"/> у игрока и <see cref="Entity"/></param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        public static Action<Entity> Construct_EntityAgregator(LinkedList<Entity> resultList,
                                                              bool healthCheck = false,
                                                              float range = 0, float zRange = 0,
                                                              bool regionCheck = false,
                                                              Predicate<Entity> specialCheck = null)
        {
            if (resultList is null)
                return null;
            Action<Entity> evalueateAction;
            if (specialCheck is null)
            {
                if (healthCheck)
                {
                    if (range > 0)
                    {
                        if (zRange > 0)
                            evalueateAction = (Entity e) =>
                            {
                                if (!e.IsDead
                                   && e.Location.Distance3DFromPlayer < range
                                   && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange)
                                    resultList.AddLast(e);
                            };

                        else evalueateAction = (Entity e) =>
                        {
                            if (!e.IsDead
                                && e.Location.Distance3DFromPlayer < range)
                                resultList.AddLast(e);
                        };
                    }
                    else
                    {
                        if (zRange > 0)
                            evalueateAction = (Entity e) =>
                            {
                                if (!e.IsDead
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange)
                                    resultList.AddLast(e);
                            };

                        else evalueateAction = (Entity e) =>
                        {
                            if (!e.IsDead)
                                resultList.AddLast(e);
                        };
                    }
                }
                else
                {
                    if (range > 0)
                    {
                        if (zRange > 0)
                            evalueateAction = (Entity e) =>
                            {
                                if (e.Location.Distance3DFromPlayer < range
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange)
                                    resultList.AddLast(e);
                            };
                        else evalueateAction = (Entity e) =>
                        {
                            if (e.Location.Distance3DFromPlayer < range)
                                resultList.AddLast(e);
                        };
                    }
                    else
                    {
                        if (zRange > 0)
                            evalueateAction = (Entity e) =>
                            {
                                if (Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange)
                                    resultList.AddLast(e);
                            };
                        else evalueateAction = (Entity e) => resultList.AddLast(e);
                    }
                }
            }
            else
            {
                if (healthCheck)
                {
                    if (range > 0)
                        if (zRange > 0)
                            evalueateAction = (Entity e) =>
                            {
                                if (!e.IsDead
                                    && e.Location.Distance3DFromPlayer < range
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                    && specialCheck(e))
                                    resultList.AddLast(e);
                            };

                        else evalueateAction = (Entity e) =>
                        {
                            if (!e.IsDead
                                && e.Location.Distance3DFromPlayer < range
                                && specialCheck(e))
                                resultList.AddLast(e);
                        };

                    else if (zRange > 0)
                        evalueateAction = (Entity e) =>
                        {
                            if (!e.IsDead
                                && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                && specialCheck(e))
                                resultList.AddLast(e);
                        };

                    else evalueateAction = (Entity e) =>
                    {
                        if (!e.IsDead
                            && specialCheck(e))
                            resultList.AddLast(e);
                    };
                }
                else
                {
                    if (range > 0)
                        if (zRange > 0)
                            evalueateAction = (Entity e) =>
                            {
                                if (e.Location.Distance3DFromPlayer < range
                                    && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                    && specialCheck(e))
                                    resultList.AddLast(e);
                            };
                        else evalueateAction = (Entity e) =>
                        {
                            if (e.Location.Distance3DFromPlayer < range
                                && specialCheck(e))
                                resultList.AddLast(e);
                        };
                    else if (zRange > 0)
                        evalueateAction = (Entity e) =>
                        {
                            if (Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                && specialCheck(e))
                                resultList.AddLast(e);
                        };
                    else evalueateAction = (Entity e) =>
                    {
                        if (specialCheck(e))
                            resultList.AddLast(e);
                    };
                }
            }
            return evalueateAction;
        }

#if false
        public static Action<Entity> Construct_Agregator(Pair<double, Entity> closestEntity, bool healthCheck = false,
                                                         float range = 0, float zRange = 0,
                                                         bool regionCheck = false,
                                                         CustomRegionCollection customRegions = null,
                                                         Predicate<Entity> specialCheck = null)
        {
            if (closestEntity is null)
                return null;
            closestEntity.First = double.MaxValue;

            if (zRange <= 0)
                zRange = Astral.Controllers.Settings.Get.MaxElevationDifference;

            Action<Entity> predicate;
            if (customRegions?.Count > 0)
            {
                if (specialCheck is null)
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) =>
                                {
                                    double dist = e.Location.Distance3DFromPlayer;
                                    if (!e.IsDead
                                       && e.Location.Distance3DFromPlayer < range
                                       && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                       && customRegions.Within(e)
                                       && )
                                        closestEntity
                                };
                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && customRegions.Within(e);

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Within(e);

                            else predicate = (Entity e) => !e.IsDead;
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Within(e);
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && customRegions.Within(e);
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                        && customRegions.Within(e);
                            else predicate = (Entity e) => customRegions.Within(e);
                        }
                    }
                }
                else
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Within(e)
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && specialCheck(e);

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Within(e)
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && specialCheck(e);
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && customRegions.Within(e)
                                        && specialCheck(e);
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && customRegions.Within(e)
                                        && specialCheck(e);
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                        && customRegions.Within(e)
                                        && specialCheck(e);
                            else predicate = (Entity e) => specialCheck(e)
                                        && customRegions.Within(e);
                        }
                    }
                }
            }
            else
            {
                if (specialCheck is null)
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range;

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;

                            else predicate = (Entity e) => !e.IsDead;
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange;
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range;
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange;
                            else predicate = (Entity e) => true;
                        }
                    }
                }
                else
                {
                    if (healthCheck)
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && e.Location.Distance3DFromPlayer < range
                                        && specialCheck(e);

                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => !e.IsDead
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);

                            else predicate = (Entity e) => !e.IsDead
                                        && specialCheck(e);
                        }
                    }
                    else
                    {
                        if (range > 0)
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && Math.Abs(EntityManager.LocalPlayer.Z - e.Z) < zRange
                                        && specialCheck(e);
                            else predicate = (Entity e) => e.Location.Distance3DFromPlayer < range
                                        && specialCheck(e);
                        }
                        else
                        {
                            if (zRange > 0)
                                predicate = (Entity e) => Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange
                                        && specialCheck(e);
                            else predicate = (Entity e) => specialCheck(e);
                        }
                    }
                }
            }
            return predicate;
        } 
#endif
    }

}
