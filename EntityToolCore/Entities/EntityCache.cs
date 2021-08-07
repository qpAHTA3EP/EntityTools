﻿//#define PROFILING

using AcTp0Tools;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using System.Collections.ObjectModel;
using EntityTools;

namespace EntityCore.Entities
{
    /// <summary>
    /// Колекция кэшированных Entity
    /// </summary>
    public class EntityCache : KeyedCollection<EntityCacheRecordKey, EntityCacheRecord>
    {
        public EntityCache()
        {
#if false
            AstralAccessors.Quester.Core.AfterLoad += ResetCache;
            AstralAccessors.Quester.Core.AfterNew += ResetCache; 
#endif
            //AstralAccessors.Quester.Core.OnProfileChanged += ResetCache;
        }

        private void ResetCache()
        {
            lock (this)
            {
                Clear(); 
            }
        }

        ~EntityCache()
        {
#if false
            AstralAccessors.Quester.Core.AfterLoad -= ResetCache;
            AstralAccessors.Quester.Core.AfterNew -= ResetCache; 

#endif
            AstralAccessors.Quester.Core.OnProfileChanged += ResetCache;
        }

#if PROFILING
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
            if (Contains(key))
            {
#if PROFILING
                matchCount++;
#endif
                record = this[key];
                return true;
            }
#if PROFILING
            mismatchCount++;
#endif
            return false;
        }

        public EntityCacheRecord MakeCache(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.InternalName, EntitySetType setType = EntitySetType.Complete)
        {
            EntityCacheRecord record = new EntityCacheRecord(pattern, matchType, nameType, setType);
            Add(record);
            return record;
        }
        public EntityCacheRecord MakeCache(EntityCacheRecordKey key)
        {
            EntityCacheRecord record = new EntityCacheRecord(key);
            Add(record);
            return record;
        }
        public bool Contains(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.InternalName, EntitySetType setType = EntitySetType.Complete)
        {
            EntityCacheRecordKey key = new EntityCacheRecordKey(pattern, matchType, nameType, setType);
            return Contains(key);
        }
    }
}
