//#define PROFILING

using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EntityCore.Entities
{
    public static class SearchCached
    {
#if PROFILING
        private static readonly long interval = 10000;

        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static int _count;
        private static TimeSpan _minTime = TimeSpan.MaxValue;
        private static TimeSpan _maxTime = TimeSpan.MinValue;
        private static readonly Dictionary<long, long> FrequencyDistribution = new Dictionary<long, long>();


        public static void ResetWatch()
        {
            _count = 0;
            _minTime = TimeSpan.MaxValue;
            _maxTime = TimeSpan.MinValue;
            Stopwatch.Reset();
            FrequencyDistribution.Clear();
            ETLogger.WriteLine(LogType.Debug, $"SearchCached::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (_count > 0)
            {
                double avrgTime = Stopwatch.ElapsedMilliseconds / (double)_count;
                double avrgTicks = Stopwatch.ElapsedTicks / (double)_count;
                ETLogger.WriteLine(LogType.Debug, $"SearchCached:\tCount: {_count}, TotalTime: {Stopwatch.Elapsed}({Stopwatch.ElapsedMilliseconds:N0} ms)");
                ETLogger.WriteLine(LogType.Debug, $"SearchCached:\tMinTime: {_minTime.TotalMilliseconds:N3} ms ({_minTime.Ticks:N0} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchCached:\tMaxTime: {_maxTime.TotalMilliseconds:N3} ms ({_maxTime.Ticks:N0} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchCached:\tAverageTime: {avrgTime:N3} ms ({avrgTicks:N0} ticks)");
                if (FrequencyDistribution.Count > 0)
                {
                    ETLogger.WriteLine(LogType.Debug, $"SearchCached::FrequencyDistribution:");
                    var list = FrequencyDistribution.ToList();
                    list.Sort((l, r) => (int)l.Key - (int)r.Key);
                    foreach (var i in list)
                        ETLogger.WriteLine(LogType.Debug, $"\t\t{i.Key * (double)interval / 10000d:N3} := {i.Value:N0}");
                }
            }
            else ETLogger.WriteLine(LogType.Debug, $"SearchCached: Count: 0");
        }
#endif

        /// <summary>
        /// Кэш Entities
        /// </summary>
        internal static EntityCache EntityCache = new EntityCache();

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
#if PROFILING
            _count++;
            TimeSpan startTime = Stopwatch.Elapsed;
            Stopwatch.Start();
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
#if PROFILING
            }
            finally
            {
                Stopwatch.Stop();
                TimeSpan time = Stopwatch.Elapsed.Subtract(startTime);
                if (time > _maxTime)
                {
                    _maxTime = time;
                }
                else if (time < _minTime)
                    _minTime = time;

                long i = Math.DivRem(time.Ticks, interval, out long _);
                if (FrequencyDistribution.ContainsKey(i))
                    FrequencyDistribution[i] += 1;
                else FrequencyDistribution.Add(i, 1);
            }
#endif
        }

        /// <summary>
        /// Поиск всех <seealso cref="Entity"/>, соответствующих <paramref name="key"/> и <paramref name="specialCheck"/>
        /// </summary>
        /// <param name="key">Комплексный иднетификатор <see cref="Entity"/>, используемый в качестве ключа в кэше</param>
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static LinkedList<Entity> FindAllEntity(EntityCacheRecordKey key,
                                                       Predicate<Entity> specialCheck = null)
        {
#if PROFILING
            _count++;
            Stopwatch.Restart();
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
#if PROFILING
            }
            finally
            {
                Stopwatch.Stop();
                TimeSpan time = Stopwatch.Elapsed;
                if (time > _maxTime)
                {
                    _maxTime = time;
                }
                else if (time < _minTime)
                    _minTime = time;

                long i = Math.DivRem(time.Ticks, interval, out long _);
                if (FrequencyDistribution.ContainsKey(i))
                    FrequencyDistribution[i] += 1;
                else FrequencyDistribution.Add(i, 1);
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
#if PROFILING
            _count++;
            Stopwatch.Restart();
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
#if PROFILING
            }
            finally
            {
                Stopwatch.Stop();
                TimeSpan time = Stopwatch.Elapsed;
                if (time > _maxTime)
                    _maxTime = time;
                else if (time < _minTime)
                    _minTime = time;

                long i = Math.DivRem(time.Ticks, interval, out long _);
                if (FrequencyDistribution.ContainsKey(i))
                    FrequencyDistribution[i] += 1;
                else FrequencyDistribution.Add(i, 1);
            }
#endif
        }

        /// <summary>
        /// Поиск ближайшего Entity, соответствующего условиям
        /// </summary>
        /// <param name="key">Комплексный иднетификатор <see cref="Entity"/>, используемый в качестве ключа в кэше</param> 
        /// <param name="specialCheck">Функтор дополнительной проверки <see cref="Entity"/>, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntity(EntityCacheRecordKey key,
                                               Predicate<Entity> specialCheck = null)
        {
#if PROFILING
            _count++;
            Stopwatch.Restart();
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
#if PROFILING
            }
            finally
            {
                Stopwatch.Stop();
                TimeSpan time = Stopwatch.Elapsed;
                if (time > _maxTime)
                    _maxTime = time;
                else if (time < _minTime)
                    _minTime = time;

                long i = Math.DivRem(time.Ticks, interval, out long _);
                if (FrequencyDistribution.ContainsKey(i))
                    FrequencyDistribution[i] += 1;
                else FrequencyDistribution.Add(i, 1);
            }
#endif
        }
    }
}