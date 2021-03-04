#define PROFILING

using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using EntityTools;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EntityCore.Entities
{
    /// <summary>
    /// Колекция кэшированных Entity
    /// </summary>
    public class EntityCache : KeyedCollection<EntityCacheRecordKey, EntityCacheRecord>
    {
#if DEBUG && PROFILING
        private static int matchCount = 0;
        private static int mismatchCount = 0;

        public static void ResetWatch()
        {
            matchCount = 0;
            mismatchCount = 0;
            ETLogger.WriteLine(LogType.Debug, $"EntityCache::ResetWatch()");
        }

        public static void LogWatch()
        {
            ETLogger.WriteLine(LogType.Debug, $"EntityCache: matchCount: {matchCount}, mismatchCount: {mismatchCount}");
        }
#endif
        protected override EntityCacheRecordKey GetKeyForItem(EntityCacheRecord item)
        {
            return item.Key;
        }

        public bool TryGetValue(out EntityCacheRecord record, string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.InternalName, EntitySetType setType = EntitySetType.Complete)
        {
            record = null;
            EntityCacheRecordKey key = new EntityCacheRecordKey(pattern, matchType, nameType, setType);
            if (base.Contains(key))
            {
#if DEBUG && PROFILING
                matchCount++;
#endif
                record = this[key];
                return true;
            }
#if DEBUG && PROFILING
            mismatchCount++;
#endif
            return false;
        }

        public EntityCacheRecord MakeCache(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.InternalName, EntitySetType setType = EntitySetType.Complete)
        {
            EntityCacheRecord record = new EntityCacheRecord(pattern, matchType, nameType, setType);
            base.Add(record);
            return record;
        }

        public bool Contains(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.InternalName, EntitySetType setType = EntitySetType.Complete)
        {
            EntityCacheRecordKey key = new EntityCacheRecordKey(pattern, matchType, nameType, setType);
            return base.Contains(key);
        }
    }
}
