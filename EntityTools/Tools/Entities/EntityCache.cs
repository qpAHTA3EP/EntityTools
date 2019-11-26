using Astral.Classes;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        public EntityCacheRecord() { }
        public EntityCacheRecord(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete, int t = 1000)
        {
            Key = new CacheRecordKey(p, mp, nt, est);
            timer = new Timeout(t);
        }

        public CacheRecordKey Key { get; }

        private Timeout timer = new Timeout(1000);

        private List<Entity> entities = new List<Entity>();
        public List<Entity> Entities
        {
            get
            {
                if (timer.IsTimedOut
                    || entities.Count == 0)
                {
                    Regen();
                }
                return entities;
            }
        }

        /// <summary>
        /// Обновление кэша
        /// </summary>
        public void Regen()
        {
            List<Entity> entts;
            if (Key.EntitySetType == EntitySetType.Complete)
                entts = UncachedSearch.GetEntities(Key.Pattern, Key.MatchType, Key.NameType);
            else entts = UncachedSearch.GetContactEntities(Key.Pattern, Key.MatchType, Key.NameType);

            if (entts != null)
                entities = entts;
            else entities.Clear();
            timer.Reset();
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
        public int ChachTime { get; set; } = 1000;

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
                record = this[key];
                return true;
            }
            else
            {
                record = new EntityCacheRecord(pattern, matchType, nameType, setType, ChachTime);
                Add(record);
                return true;
            }
        }


    }
}
