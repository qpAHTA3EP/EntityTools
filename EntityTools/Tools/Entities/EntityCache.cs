using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EntityTools.Tools.Entities
{
    public class CacheRecordKey
    {
        public CacheRecordKey() { }
        public CacheRecordKey(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete)
        {
            Pattern = p;
            MatchType = mp;
            NameType = nt;
            EntitySetType = est;
        }


        public readonly string Pattern = string.Empty;
        public readonly ItemFilterStringType MatchType = ItemFilterStringType.Simple;
        public readonly EntityNameType NameType = EntityNameType.NameUntranslated;
        public readonly EntitySetType EntitySetType = EntitySetType.Complete;

        public override bool Equals(object otherObj)
        {
            if (ReferenceEquals(this, otherObj))
                return true;
            else if(otherObj is CacheRecordKey other)
                return Equals(other);
            return false;
        }

        public bool Equals(CacheRecordKey other)
        {
            return ReferenceEquals(this, other)
                || (Pattern == other.Pattern
                    && MatchType == other.MatchType
                    && NameType == other.NameType
                    && EntitySetType == other.EntitySetType);
        }

        public override int GetHashCode()
        {
            return Pattern.GetHashCode();
        }
    }

    /// <summary>
    /// Запись Кэша
    /// </summary>
    public class EntityCacheRecord
    {
#if PROFILING
        private static int RegenCount = 0;
        private static int EntitiesCount = 0;
        public static void ResetWatch()
        {
            RegenCount = 0;
            EntitiesCount = 0;
            Logger.WriteLine(Logger.LogType.Debug, $"EntityCacheRecord::ResetWatch()");
        }

        public static void LogWatch()
        {
            Logger.WriteLine(Logger.LogType.Debug, $"EntityCacheRecord: RegenCount: {RegenCount}");
        }
#endif
        public EntityCacheRecord() { }
        public EntityCacheRecord(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete)
        {
            Key = new CacheRecordKey(p, mp, nt, est);
            Timer = new Timeout(EntityCache.ChacheTime);
        }

        public CacheRecordKey Key { get; }

        public Timeout Timer { get; private set; } = new Timeout(EntityCache.ChacheTime);

        private List<Entity> entities = new List<Entity>();
        public List<Entity> Entities
        {
            get
            {

                if (Timer.IsTimedOut)
                    Regen();
                //else
                //{
                //    // Удаление невалидных сущностей
                //    entities.RemoveAll((Entity e) => !e.IsValid);
                //    if (entities.Count == 0)
                //        Regen();
                //}

                return entities;
            }
        }

        /// <summary>
        /// Обновление кэша
        /// </summary>
        public void Regen()
        {
#if PROFILING
            RegenCount++;
#endif
            List<Entity> entts;
            if (Key.EntitySetType == EntitySetType.Complete)
                entts = SearchDirect.GetEntities(Key.Pattern, Key.MatchType, Key.NameType);
            else entts = SearchDirect.GetContactEntities(Key.Pattern, Key.MatchType, Key.NameType);

            if (entts != null)
                entities = entts;
            else entities.Clear();
#if PROFILING
            EntitiesCount += entities.Count;
#endif
            Timer.ChangeTime(EntityCache.ChacheTime);
        }
        public void Regen(Action<Entity> action)
        {
#if PROFILING
            RegenCount++;
#endif
            List<Entity> entts;
            if (Key.EntitySetType == EntitySetType.Complete)
                entts = SearchDirect.GetEntities(Key.Pattern, Key.MatchType, Key.NameType, action);
            else entts = SearchDirect.GetContactEntities(Key.Pattern, Key.MatchType, Key.NameType, action);

            if (entts != null)
                entities = entts;
            else entities.Clear();
#if PROFILING
            EntitiesCount += entities.Count;
#endif
            Timer.ChangeTime(EntityCache.ChacheTime);
        }

        /// <summary>
        /// Обработка (сканирование) Entities
        /// </summary>
        /// <param name="action"></param>
        public void Processing(Action<Entity> action)
        {
            if (Timer.IsTimedOut)
                Regen(action);
            else if (entities != null && entities.Count > 0)
            {
                // Если Entity валидна - оно передается для обработки в action
                // в противном случае - удаляется из коллекции
                entities.RemoveAll((Entity e) =>
                                    {
                                        if (e.IsValid)
                                        {
                                            action(e);
                                            return false;
                                        }
                                        else return true;
                                    });
            }
        }

        public bool Equals(EntityCacheRecord other)
        {
            return ReferenceEquals(this, other) || Key.Equals(other.Key);
        }

        public override int GetHashCode()
        {
            return Key.Pattern.GetHashCode();
        }
    }

    /// <summary>
    /// Колекция кэшированных Entity
    /// </summary>
    public class EntityCache : KeyedCollection<CacheRecordKey, EntityCacheRecord>
    {
#if PROFILING
        private static int MatchCount = 0;
        private static int MismatchCount = 0;

        public static void ResetWatch()
        {
            MatchCount = 0;
            MismatchCount = 0;
            Logger.WriteLine(Logger.LogType.Debug, $"EntityCache::ResetWatch()");
        }

        public static void LogWatch()
        {
            Logger.WriteLine(Logger.LogType.Debug, $"EntityCache: MatchCount: {MatchCount}, MismatchCount: {MismatchCount}");
        }
#endif
        /// <summary>
        /// Интервал времени между обновлениями кэша
        /// </summary>
        public static int ChacheTime { get; set; } = 5000;

        protected override CacheRecordKey GetKeyForItem(EntityCacheRecord item)
        {
            return item.Key;
        }

        public bool TryGetValue(out EntityCacheRecord record, string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete)
        {
            record = null;
            CacheRecordKey key = new CacheRecordKey(pattern, matchType, nameType, setType);
            if (base.Contains(key))
            {
#if PROFILING
                MatchCount++;
#endif
                record = this[key];
                return true;
            }
            else
            {
#if PROFILING
                MismatchCount++;
#endif
                return false;
            }
        }

        public EntityCacheRecord MakeCache(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete)
        {
            EntityCacheRecord record = new EntityCacheRecord(pattern, matchType, nameType, setType);
            base.Add(record);
            return record;
        }

        public bool Contains(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete)
        {
            CacheRecordKey key = new CacheRecordKey(pattern, matchType, nameType, setType);
            return base.Contains(key);
        }
    }
}
