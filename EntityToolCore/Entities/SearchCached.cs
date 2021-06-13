//#define PROFILING

using System;
using System.Collections.Generic;
using AcTp0Tools.Annotations;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using MyNW.Classes;
using EntityTools.Enums;

namespace EntityCore.Entities
{
    public static class SearchCached
    {
#if DEBUG && PROFILING
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
            ETLogger.WriteLine(LogType.Debug, $"SearchCached::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (Count > 0)
            {
                double avrgTime = (double)stopwatch.ElapsedMilliseconds / (double)Count;
                double avrgTicks = (double)stopwatch.ElapsedTicks / (double)Count;
                ETLogger.WriteLine(LogType.Debug, $"SearchCached:\tCount: {Count}, TotalTime: {stopwatch.Elapsed}({stopwatch.ElapsedMilliseconds.ToString("N0")} ms)");
                ETLogger.WriteLine(LogType.Debug, $"SearchCached:\tMinTime: {MinTime.TotalMilliseconds.ToString("N3")} ms ({MinTime.Ticks.ToString("N0")} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchCached:\tMaxTime: {MaxTime.TotalMilliseconds.ToString("N3")} ms ({MaxTime.Ticks.ToString("N0")} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchCached:\tAverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                if (frequencyDistribution.Count > 0)
                {
                    ETLogger.WriteLine(LogType.Debug, $"SearchCached::FrequencyDistribution:");
                    var list = frequencyDistribution.ToList();
                    list.Sort((KeyValuePair<long, long> l, KeyValuePair<long, long> r) => { return (int)l.Key - (int)r.Key; } );
                    foreach (var i in list)
                        ETLogger.WriteLine(LogType.Debug, $"\t\t{((double)i.Key * (double)interval / 10000d).ToString("N3")} := {i.Value.ToString("N0")}");
                }
            }
            else ETLogger.WriteLine(LogType.Debug, $"SearchCached: Count: 0");
        }
#endif

#if false
        /// <summary>
        /// Объявление типа делегата д
        /// </summary>
        /// <param name="e"></param>
        public delegate void OptionCheck(Entity e); 
#endif

        /// <summary>
        /// Кэш Entities
        /// </summary>
        private static EntityCache EntityCache = new EntityCache();

        /// <summary>
        /// Поиск всех Entity, соответствующих условиям
        /// </summary>
        /// <param name="pattern">Шаблон, которому должен соответствовать идентификатор (имя) <see cref="Entity"/>, заданные параметном </param>
        /// <param name="matchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) <see cref="Entity"/>, с которым сопостовляется entPattern</param>
        /// <param name="setType">Переключатель коллекции <seealso cref="Entity"/>, в которой должен производиться поиск</param>
        /// <param name="healthCheck">Если True, искать только <see cref="Entity"/> с количеством здоровья (НР) больше 0 и без флага <see cref="Entity.IsDead"/></param>
        /// <param name="range">Предельное расстояние поиска, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="zRange">Предельная разница высот относительно персонажа, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те <see cref="Entity"/>, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegions">Список CustomRegion'ов, в которых нужно искать <see cref="Entity"/></param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
                                                       bool healthCheck = false,
                                                       float range = 0, float zRange = 0, bool regionCheck = false,
                                                       List<CustomRegion> customRegions = null,
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
            LinkedList<Entity> entities = new LinkedList<Entity>();
            // Конструируем функтор для поиска Entity в соответствии с доп. условиями
            var predicate = SearchHelper.Construct_EntityAttributePredicate(healthCheck, 
                                                      range, zRange, 
                                                      regionCheck, 
                                                      customRegions, 
                                                      specialCheck);
            void EvaluateAndCollectEntity(Entity entity)
            {
                if (predicate(entity))
                    entities.AddLast(entity);
            }


            // Проверяем наличие кэша
            if (EntityCache.TryGetValue(out EntityCacheRecord cachedEntities, pattern, matchType, nameType, setType))
            {
                // Проверяем Entities применяя функтор ко всем Entity, удовлетворяющим шаблону
                // Функтор EvaluateAndCollectEntity заполняет entities
                cachedEntities.Processing(EvaluateAndCollectEntity);
            }
            else
            {
                // Кэш не обнаружен
                // Функтор EvaluateAndCollectEntity заполняет entities
                cachedEntities = EntityCache.MakeCache(pattern, matchType, nameType, setType);
                cachedEntities.Processing(EvaluateAndCollectEntity);
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


#if false
        /// <summary>
        /// Поиск всех <seealso cref="Entity"/>, соответствующих условиям
        /// </summary>
        /// <param name="key">Комплексный иднетификатор <see cref="Entity"/>, используемый в качестве ключа в кэше</param>
        /// <param name="healthCheck">Если True, искать только <see cref="Entity"/> с количеством здоровья (НР) больше 0 и без флага <see cref="Entity.IsDead"/></param>
        /// <param name="range">Предельное расстояние поиска, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="zRange">Предельная разница высот относительно персонажа, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те <see cref="Entity"/>, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegions">Список CustomRegion'ов, в которых нужно искать <see cref="Entity"/></param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static LinkedList<Entity> FindAllEntity(EntityCacheRecordKey key,
                                                       bool healthCheck = false,
                                                       float range = 0, float zRange = 0,
                                                       bool regionCheck = false,
                                                       Predicate<Entity> specialCheck = null)
        {
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            if (key is null && !key.IsValid)
                return null;

            // конструируем функтор для дополнительных проверок Entity и поиска ближайшего
            LinkedList<Entity> entities = new LinkedList<Entity>();
            // Конструируем функтор для поиска Entity в соответствии с доп. условиями
            Action<Entity> evaluateAction = Construct_Agregator(entities, healthCheck, range, zRange, regionCheck, specialCheck);

            // Проверяем наличие кэша
            if (EntityCache.Contains(key))
            {
                // Проверяем Entities применяя функтор ко всем Entity, удовлетворяющим шаблону
                // Функтор evaluateAction заполняет entities
                EntityCache[key].Processing(evaluateAction);
            }
            else
            {
                // Кэш не обнаружен
                var cachedEntities = EntityCache.MakeCache(key);
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
#endif

        /// <summary>
        /// Поиск всех <seealso cref="Entity"/>, соответствующих <paramref name="key"/> и <paramref name="specialCheck"/>
        /// </summary>
        /// <param name="key">Комплексный иднетификатор <see cref="Entity"/>, используемый в качестве ключа в кэше</param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static LinkedList<Entity> FindAllEntity(EntityCacheRecordKey key,
                                                       Predicate<Entity> specialCheck = null)
        {
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            if (key is null || !key.IsValid)
                return null;

            // конструируем функтор для дополнительных проверок Entity и заполнения списка
            LinkedList<Entity> entities = new LinkedList<Entity>();
            
            // Конструируем функтор для поиска Entity в соответствии с доп. условиями
            Action<Entity> evaluateAndCollectEntity;
            if (specialCheck is null)
                evaluateAndCollectEntity = (entity) => entities.AddLast(entity);
            else evaluateAndCollectEntity = (entity) =>
                    {
                        if (specialCheck(entity))
                            entities.AddLast(entity);
                    };

            // Проверяем наличие кэша
            if (EntityCache.Contains(key))
            {
                // Обнаружена кэшированная запись
                // Проверяем Entities применяя функтор ко всем Entity, удовлетворяющим шаблону
                EntityCache[key].Processing(evaluateAndCollectEntity);
            }
            else
            {
                // Кэш не обнаружен
                EntityCache.MakeCache(key).Processing(evaluateAndCollectEntity);
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
        /// <param name="pattern">Шаблон, которому должен соответствовать идентификатор (имя) <see cref="Entity"/>, заданные параметном </param>
        /// <param name="matchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) <see cref="Entity"/>, с которым сопостовляется entPattern</param>
        /// <param name="setType">Переключатель коллекции <seealso cref="Entity"/>, в которой должен производиться поиск</param>
        /// <param name="healthCheck">Если True, искать только <see cref="Entity"/> с количеством здоровья (НР) больше 0 и без флага <see cref="Entity.IsDead"/></param>
        /// <param name="range">Предельное расстояние поиска, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="zRange">Предельная разница высот относительно персонажа, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те <see cref="Entity"/>, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegions">Список CustomRegion'ов, в которых нужно искать <see cref="Entity"/></param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
                                               bool healthCheck = false,
                                               float range = 0, float zRange = 0,
                                               bool regionCheck = false,
                                               List<CustomRegion> customRegions = null,
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
            Action<Entity> processor;

            // Конструируем функтор для поиска Entity в соответствии с доп. условиями
            var predicate = SearchHelper.Construct_EntityAttributePredicate(healthCheck,
                                                      range, zRange,
                                                      regionCheck,
                                                      customRegions,
                                                      specialCheck);
            processor = (entity) =>
            {
                if (!predicate(entity)) return;

                float dist = entity.CombatDistance3;
                if (closestDistance > dist)
                {
                    closestEntity = entity;
                    closestDistance = dist;
                }
            };


            if (EntityCache.TryGetValue(out EntityCacheRecord cachedEntities, pattern, matchType, nameType, setType))
            {
                // Обнаружена кэшированная запись
                // Проверяем Entities применяя функтор ко всем Entity, удовлетворяющим шаблону
                cachedEntities.Processing(processor);
            }
            else
            {
                // Кэш не обнаружен
                cachedEntities = EntityCache.MakeCache(pattern, matchType, nameType, setType);
                cachedEntities.Processing(processor);
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

#if false
        /// <summary>
        /// Поиск ближайшего Entity, соответствующего условиям
        /// </summary>
        /// <param name="key">Комплексный иднетификатор <see cref="Entity"/>, используемый в качестве ключа в кэше</param>        
        /// <param name="healthCheck">Если True, искать только <see cref="Entity"/> с количеством здоровья (НР) больше 0 и без флага <see cref="Entity.IsDead"/></param>
        /// <param name="range">Предельное расстояние поиска, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="zRange">Предельная разница высот относительно персонажа, дальше которого <see cref="Entity"/> игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те <see cref="Entity"/>, которые находятся в одном регионе с игроком</param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntity(EntityCacheRecordKey key,
                                               bool healthCheck = false,
                                               float range = 0, float zRange = 0,
                                               bool regionCheck = false,
                                               Predicate<Entity> specialCheck = null)
        {
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            if (key is null && !key.IsValid)
                return null;

            // конструируем функтор для дополнительных проверок Entity и поиска ближайшего
            float closestDistance = (range == 0) ? float.MaxValue : range;
            Entity closestEntity = null;
            Action<Entity> evaluateAction;

            // Конструируем функтор для поиска Entity в соответствии с доп. условиями
            if (specialCheck == null)
                evaluateAction = (Entity e) =>
                {
                    if ((!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                        && (!healthCheck || !e.IsDead)
                        && (zRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange)
                        && (range <= 0 || e.Location.Distance3DFromPlayer < range))
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
                        && (zRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(e.Location) < zRange)
                        && (range <= 0 || e.Location.Distance3DFromPlayer < range)
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


            if (EntityCache.Contains(key))
            {
                // Обнаружена кэшированная запись
                // Проверяем Entities применяя функтор ко всем Entity, удовлетворяющим шаблону
                EntityCache[key].Processing(evaluateAction);
            }
            else
            {
                // Кэш не обнаружен
                var cachedEntities = EntityCache.MakeCache(key);
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
#endif

        /// <summary>
        /// Поиск ближайшего Entity, соответствующего условиям
        /// </summary>
        /// <param name="key">Комплексный иднетификатор <see cref="Entity"/>, используемый в качестве ключа в кэше</param> 
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntity(EntityCacheRecordKey key,
                                               Predicate<Entity> specialCheck = null)
        {
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            if (key is null || !key.IsValid)
                return null;

            // конструируем функтор для дополнительных проверок Entity и поиска ближайшего
            Entity closestEntity = null;
            float closestDistance = float.MaxValue;
            Action<Entity> action;

            // Конструируем функтор для поиска Entity, соответствующего specialCheck
            if (specialCheck is null)
                action = (entity) =>
                {
                    float dist = entity.CombatDistance3;
                    if (closestDistance > dist)
                    {
                        closestDistance = dist;
                        closestEntity = entity;
                    }
                };
            else action = (entity) =>
            {
                if (!specialCheck(entity)) return;

                float dist = entity.CombatDistance3;
                if (closestDistance > dist)
                {
                    closestDistance = dist;
                    closestEntity = entity;
                }
            };
            if (EntityCache.Contains(key))
            {
                // Обнаружена кэшированная запись
                // Проверяем Entities применяя функтор ко всем Entity, удовлетворяющим шаблону
                EntityCache[key].Processing(action);
            }
            else
            {
                // Кэш не обнаружен
                EntityCache.MakeCache(key).Processing(action);
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