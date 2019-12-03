using Astral;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace EntityTools.Tools.Entities
{
    public static class SearchCached
    {
#if PROFILING
        static readonly long interval = 10000;

        private static Stopwatch stopwatch = new Stopwatch();
        private static int Count = 0;
        private static int WorseTryNumber = 0;
        private static TimeSpan MinTime = TimeSpan.MaxValue;
        private static TimeSpan MaxTime = TimeSpan.MinValue;
        private static Dictionary<long, long> frequencyDistribution = new Dictionary<long, long>();


        public static void ResetWatch()
        {
            Count = 0;
            WorseTryNumber = 0;
            MinTime = TimeSpan.MaxValue;
            MaxTime = TimeSpan.MinValue;
            stopwatch.Reset();
            frequencyDistribution.Clear();
            Logger.WriteLine(Logger.LogType.Debug, $"SearchCached::ResetWatch()");
        }

        public static void LogWatch()
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
        public static List<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
            bool healthCheck = false, float range = 0, bool regionCheck = false, List<CustomRegion> customRegions = null, 
            Predicate<Entity> specialCheck = null)
        {
#if PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
                // конструируем функтор для дополнительных проверок Entity и поиска ближайшего
                float closestDistance = (range == 0) ? float.MaxValue : range;
                List<Entity> entities = null;
                Action<Entity> evaluateAction;

                // Конструируем функтор для поиска Entity в соответствии с доп. условиями
                if (customRegions == null)
                {
                    if (specialCheck == null)
                        evaluateAction = (Entity e) =>
                        {
                            if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead))
                                entities.Add(e);
                        };
                    else evaluateAction = (Entity e) =>
                    {
                        if((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && specialCheck(e))
                            entities.Add(e);
                    };
                }
                else
                {
                    if (specialCheck == null)
                        evaluateAction = (Entity e) =>
                        {
                            if((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(e, cr)) != null)
                                entities.Add(e);
                        };
                    else evaluateAction = (Entity e) =>
                    {
                        if((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (!healthCheck || !e.IsDead)
                            && customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(e, cr)) != null
                            && specialCheck(e))
                            entities.Add(e);
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
                    // Кэша не обнаружен
                    cachedEntities = EntityCache.MakeCache(pattern, matchType, nameType, setType);
                    // Функтор evaluateAction заполняет entities
                    cachedEntities.Processing(evaluateAction);
                }

                return entities;
#if PROFILING
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
        public static Entity FindClosestEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
            bool healthCheck = false, float range = 0, bool regionCheck = false, List<CustomRegion> customRegions = null,
            Predicate<Entity> specialCheck = null)
        {
#if PROFILING
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
                if (customRegions == null)
                {
                    if (specialCheck == null)
                        evaluateAction = (Entity e) =>
                        {
                            if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead))
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
                                && customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(e, cr)) != null)
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
                            && customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(e, cr)) != null
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
#if PROFILING
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
        //        {
        //#if PROFILING
        //            Count++;
        //            TimeSpan StartTime = stopwatch.Elapsed;
        //        stopwatch.Start();
        //            try
        //            {
        //#endif
        //                if (EntityCache.TryGetValue(out EntityCacheRecord cache, pattern, matchType, nameType, setType))
        //                {
        //                    // конструируем функтор для дополнительных проверок Entity и поиска ближайшего
        //                    float closestDistance = (range == 0) ? float.MaxValue : range;
        //        Entity closestEntity = null;
        //        Predicate<Entity> optionCheck;

        //                    if (customRegions == null)
        //                    {
        //                        if (specialCheck == null)
        //                            optionCheck = (Entity e) =>
        //                            {
        //                                return e.IsValid && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
        //                                    && (!healthCheck || !e.IsDead);
        //                            };
        //                        else optionCheck = (Entity e) =>
        //                        {
        //                            return e.IsValid && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
        //                                    && (!healthCheck || !e.IsDead)
        //                                    && specialCheck(e);
        //};
        //                    }
        //                    else
        //                    {
        //                        if (specialCheck == null)
        //                            optionCheck = (Entity e) =>
        //                            {
        //                                return e.IsValid && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
        //                                    && (!healthCheck || !e.IsDead)
        //                                    && customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(e, cr)) != null;
        //                            };
        //                        else optionCheck = (Entity e) =>
        //                        {
        //                            return e.IsValid && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
        //                                && (!healthCheck || !e.IsDead)
        //                                && customRegions.Find((CustomRegion cr) => CommonTools.IsInCustomRegion(e, cr)) != null
        //                                && specialCheck(e);
        //                        };
        //                    }

        //                    // Проверяем Entities
        //                    foreach(Entity e in cache.Entities)
        //                    {
        //                        float eDistance = e.CombatDistance3;
        //                        if (eDistance<closestDistance && optionCheck(e))
        //                        {
        //                            closestEntity = e;
        //                            closestDistance = eDistance;
        //                        }
        //                    }

        //                    if (closestEntity == null)
        //                    {
        //                        if(cache.Timer.Left > EntityCache.ChacheTime* 0.7)
        //                        {
        //                            // Не найдено ни одного Entity
        //                            // при этом до обновления Кэша осталось менее 30% времени.
        //                            // Пробуем обновить кэш и повторить поиск 
        //                            cache.Regen();
        //                            foreach (Entity e in cache.Entities)
        //                            {
        //                                float eDistance = e.CombatDistance3;
        //                                if (eDistance<closestDistance && optionCheck(e))
        //                                {
        //                                    closestEntity = e;
        //                                    closestDistance = eDistance;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    return closestEntity;
        //                }
        //                else return null;
        //#if PROFILING
        //            }
        //            finally
        //            {
        //                stopwatch.Stop();
        //                TimeSpan time = stopwatch.Elapsed.Subtract(StartTime);
        //                if (time > MaxTime)
        //                    MaxTime = time;
        //                else if (time<MinTime)
        //                    MinTime = time;

        //                long i = Math.DivRem(time.Ticks, interval, out long rem);
        //                if (frequencyDistribution.ContainsKey(i))
        //                    frequencyDistribution[i] += 1;
        //                else frequencyDistribution.Add(i, 1);
        //            }
        //#endif
        //        }

    }
}