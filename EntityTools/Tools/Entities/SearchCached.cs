﻿using Astral;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using EntityCore.Enums;
using EntityCore.Tools;
using EntityCore.Extentions;

namespace EntityCore.Entities
{
    internal static class SearchCached
    {
#if DEBUG && PROFILING
        static readonly long interval = 10000;

        private static Stopwatch stopwatch = new Stopwatch();
        private static int Count = 0;
        private static int WorseTryNumber = 0;
        private static TimeSpan MinTime = TimeSpan.MaxValue;
        private static TimeSpan MaxTime = TimeSpan.MinValue;
        private static Dictionary<long, long> frequencyDistribution = new Dictionary<long, long>();


        internal static void ResetWatch()
        {
            Count = 0;
            WorseTryNumber = 0;
            MinTime = TimeSpan.MaxValue;
            MaxTime = TimeSpan.MinValue;
            stopwatch.Reset();
            frequencyDistribution.Clear();
            Logger.WriteLine(Logger.LogType.Debug, $"SearchCached::ResetWatch()");
        }

        internal static void LogWatch()
        {
            if (Count > 0)
            {
                double avrgTime = (double)stopwatch.ElapsedMilliseconds / (double)Count;
                double avrgTicks = (double)stopwatch.ElapsedTicks / (double)Count;
                Logger.WriteLine(Logger.LogType.Debug, $"SearchCached:\tCount: {Count}, TotalTime: {stopwatch.Elapsed}({stopwatch.ElapsedMilliseconds.ToString("N0")} ms)");
                Logger.WriteLine(Logger.LogType.Debug, $"SearchCached:\tMinTime: {MinTime.TotalMilliseconds.ToString("N3")} ms ({MinTime.Ticks.ToString("N0")} ticks)");
                Logger.WriteLine(Logger.LogType.Debug, $"SearchCached:\tMaxTime: {MaxTime.TotalMilliseconds.ToString("N3")} ms ({MaxTime.Ticks.ToString("N0")} ticks)");
                Logger.WriteLine(Logger.LogType.Debug, $"SearchCached:\tAverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                if (frequencyDistribution.Count > 0)
                {
                    Logger.WriteLine(Logger.LogType.Debug, $"SearchCached::FrequencyDistribution:");
                    var list = frequencyDistribution.ToList();
                    list.Sort((KeyValuePair<long, long> l, KeyValuePair<long, long> r) => { return (int)l.Key - (int)r.Key; } );
                    foreach (var i in list)
                        Logger.WriteLine(Logger.LogType.Debug, $"\t\t{((double)i.Key * (double)interval / 10000d).ToString("N3")} := {i.Value.ToString("N0")}");
                }
            }
            else Logger.WriteLine(Logger.LogType.Debug, $"SearchCached: Count: 0");
        }
#endif

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
        /// Поиск всех Entity, соответствующих условиям
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
        internal static LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
            bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null, 
            Predicate<Entity> specialCheck = null)
        {
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            // конструируем функтор для дополнительных проверок Entity и поиска ближайшего
            float closestDistance = (range == 0) ? float.MaxValue : range;
                LinkedList<Entity> entities = new LinkedList<Entity>();
                Action<Entity> evaluateAction;

                // Конструируем функтор для поиска Entity в соответствии с доп. условиями
                if (customRegions != null && customRegions.Count > 0)
                {
                    if (specialCheck == null)
                        evaluateAction = (Entity e) =>
                        {
                            if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && (range == 0 || e.Location.Distance3DFromPlayer < range)
                                && (zRange == 0 || Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < range)
                                && customRegions.Find((CustomRegion cr) => e.Within(cr)) != null)
                                entities.AddLast(e);
                        };
                    else evaluateAction = (Entity e) =>
                    {
                        if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (!healthCheck || !e.IsDead)
                            && (range == 0 || e.Location.Distance3DFromPlayer < range)
                            && (zRange == 0 || Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < range)
                            && customRegions.Find((CustomRegion cr) => e.Within(cr)) != null
                            && specialCheck(e))
                            entities.AddLast(e);
                    };
                }
                else
                {
                    if (specialCheck == null)
                        evaluateAction = (Entity e) =>
                        {
                            if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && (range == 0 || e.Location.Distance3DFromPlayer < range)
                                && (zRange == 0 || Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < range))
                                entities.AddLast(e);
                        };
                    else evaluateAction = (Entity e) =>
                    {
                        if((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && (range == 0 || e.Location.Distance3DFromPlayer < range)
                                && (zRange == 0 || Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < range)
                                && specialCheck(e))
                            entities.AddLast(e);
                    };
                }

                // Проверяем наличие кэша
                if (EntityCache.TryGetValue(out EntityCacheRecord cachedEntities, pattern, matchType, nameType, setType))
                {
                    // Проверяем Entities применяя функтор ко всем Entity, удовлетворяющим шаблону
                    // Функтор evaluateAction заполняет entities
                    cachedEntities.Processing(evaluateAction);
                }
                else
                {
                    // Кэш не обнаружен
                    cachedEntities = EntityCache.MakeCache(pattern, matchType, nameType, setType);
                    // Функтор evaluateAction заполняет entities
                    cachedEntities.Processing(evaluateAction);
                }

                return entities;
#if DEBUG && PROFILING
            }
            finally
            {
                stopwatch.Stop();
                TimeSpan time = stopwatch.Elapsed.Subtract(StartTime);
                if (time > MaxTime)
                {
                    MaxTime = time;
                    WorseTryNumber = Count;
                }
                else if (time < MinTime)
                    MinTime = time;

                long i = Math.DivRem(time.Ticks, interval, out long rem);
                if (frequencyDistribution.ContainsKey(i))
                    frequencyDistribution[i] += 1;
                else frequencyDistribution.Add(i, 1);
            }
#endif
        }

        /// <summary>
        /// Поиск ближайшего Entity, соответствующего условиям
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
        internal static Entity FindClosestEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
            bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null,
            Predicate<Entity> specialCheck = null)
        {
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            // конструируем функтор для дополнительных проверок Entity и поиска ближайшего
            float closestDistance = (range == 0) ? float.MaxValue : range;
                Entity closestEntity = null;
                Action<Entity> evaluateAction;

                // Конструируем функтор для поиска Entity в соответствии с доп. условиями
                if (customRegions != null && customRegions.Count > 0)
                {
                    if (specialCheck == null)
                        evaluateAction = (Entity e) =>
                        {
                            if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && (range == 0 || e.Location.Distance3DFromPlayer < range)
                                && (zRange == 0 || Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < range)
                                && customRegions.Find((CustomRegion cr) => e.Within(cr)) != null)
                            {
                                float eDistance = e.CombatDistance3;
                                if (eDistance < closestDistance)
                                {
                                    closestEntity = e;
                                    closestDistance = eDistance;
                                }
                            }
                        };
                    else evaluateAction = (Entity e) =>
                    {
                        if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (!healthCheck || !e.IsDead)
                            && (range == 0 || e.Location.Distance3DFromPlayer < range)
                            && (zRange == 0 || Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < range)
                            && customRegions.Find((CustomRegion cr) => e.Within(cr)) != null
                            && specialCheck(e))
                        {
                            float eDistance = e.CombatDistance3;
                            if (eDistance < closestDistance)
                            {
                                closestEntity = e;
                                closestDistance = eDistance;
                            }
                        }
                    };
                }
                else
                {
                    if (specialCheck == null)
                        evaluateAction = (Entity e) =>
                        {
                            if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && (range == 0 || e.Location.Distance3DFromPlayer < range))
                            {
                                float eDistance = e.CombatDistance3;
                                if (eDistance < closestDistance)
                                {
                                    closestEntity = e;
                                    closestDistance = eDistance;
                                }
                            }
                        };
                    else evaluateAction = (Entity e) =>
                    {
                        if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && (range == 0 || e.Location.Distance3DFromPlayer < range)
                                && specialCheck(e))
                        {
                            float eDistance = e.CombatDistance3;
                            if (eDistance < closestDistance)
                            {
                                closestEntity = e;
                                closestDistance = eDistance;
                            }
                        }

                    };
                }

                if (EntityCache.TryGetValue(out EntityCacheRecord cachedEntities, pattern, matchType, nameType, setType))
                {
                    // Обнаружена кэшированная запись
                    // Проверяем Entities применяя функтор ко всем Entity, удовлетворяющим шаблону
                    cachedEntities.Processing(evaluateAction);
                }
                else
                {
                    // Кэш не обнаружен
                    cachedEntities = EntityCache.MakeCache(pattern, matchType, nameType, setType);
                    cachedEntities.Processing(evaluateAction);
                }
                return closestEntity;
#if DEBUG && PROFILING
            }
            finally
            {
                stopwatch.Stop();
                TimeSpan time = stopwatch.Elapsed.Subtract(StartTime);
                if (time > MaxTime)
                    MaxTime = time;
                else if (time < MinTime)
                    MinTime = time;

                long i = Math.DivRem(time.Ticks, interval, out long rem);
                if (frequencyDistribution.ContainsKey(i))
                    frequencyDistribution[i] += 1;
                else frequencyDistribution.Add(i, 1);
            }
#endif
        }
    }
}