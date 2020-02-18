using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EntityTools.Tools.Entities
{
    public class CacheRecordKey
    {
        public CacheRecordKey()
        {
            Comparer = new EntityComparerToPattern(Pattern, MatchType, NameType);
        }
        public CacheRecordKey(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete)
        {
            Pattern = p;
            MatchType = mp;
            NameType = nt;
            EntitySetType = est;
            Comparer = new EntityComparerToPattern(Pattern, MatchType, NameType);
        }


        public readonly string Pattern = string.Empty;
        public readonly ItemFilterStringType MatchType = ItemFilterStringType.Simple;
        public readonly EntityNameType NameType = EntityNameType.NameUntranslated;
        public readonly EntitySetType EntitySetType = EntitySetType.Complete;
        public readonly EntityComparerToPattern Comparer = null;

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
#if DEBUG && PROFILING
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
            //if (EntityManager.LocalPlayer.InCombat
            //    && !Astral.Quester.API.IgnoreCombat)
            //        Timer = new Timeout(EntityCache.CombatChacheTime);
            //else Timer = new Timeout(EntityCache.ChacheTime);
        }

        public CacheRecordKey Key { get; }

        public Timeout Timer { get; private set; } = new Timeout(0);

        private LinkedList<Entity> entities = new LinkedList<Entity>();
        public LinkedList<Entity> Entities
        {
            get
            {
                if (Timer.IsTimedOut)
                    Regen();
                return entities;
            }
        }

        /// <summary>
        /// Обновление кэша
        /// </summary>
        public void Regen()
        {
#if DEBUG && PROFILING
            RegenCount++;
#endif
            LinkedList<Entity> entts;
            if (Key.EntitySetType == EntitySetType.Complete)
                entts = SearchDirect.GetEntities(Key);
            else entts = SearchDirect.GetContactEntities(Key);

            if (entts != null)
                entities = entts;
            else entities.Clear();
#if DEBUG && PROFILING
            EntitiesCount += entities.Count;
#endif
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                    Timer = new Timeout(EntityCache.ChacheTime);
            else Timer = new Timeout(EntityCache.CombatChacheTime);
        }
        public void Regen(Action<Entity> action)
        {
#if DEBUG && PROFILING
            RegenCount++;
#endif
            LinkedList<Entity> entts;
            if (Key.EntitySetType == EntitySetType.Complete)
                entts = SearchDirect.GetEntities(Key, action);
            else entts = SearchDirect.GetContactEntities(Key, action);

            if (entts != null)
                entities = entts;
            else entities.Clear();
#if DEBUG && PROFILING
            EntitiesCount += entities.Count;
#endif
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                    Timer = new Timeout(EntityCache.ChacheTime);
            else Timer = new Timeout(EntityCache.CombatChacheTime);
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
                LinkedList<Entity> newEntities = new LinkedList<Entity>();
                
                while(entities.Count > 0)
                {
                    LinkedListNode<Entity> eNode = entities.First;
                    entities.RemoveFirst();
                    if (Key.Comparer.Check(eNode.Value))
                    {
                        action(eNode.Value);
                        newEntities.AddLast(eNode);
                    }
                }
                //entities.RemoveAll((Entity e) =>
                //                    {
                //                        if (Key.Comparer.Check(e))
                //                        {
                //                            action(e);
                //                            return false;
                //                        }
                //                        else return true;
                //                    });
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
#if DEBUG && PROFILING
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
        public static int ChacheTime { get; set; } = 500;

        /// <summary>
        /// Интервал времени между обновлениями кэша во время боя
        /// </summary>
        public static int CombatChacheTime { get; set; } = 200;

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
#if DEBUG && PROFILING
                MatchCount++;
#endif
                record = this[key];
                return true;
            }
            else
            {
#if DEBUG && PROFILING
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
