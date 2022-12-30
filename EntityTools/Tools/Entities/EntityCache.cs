#define PROFILING

using System.Collections.ObjectModel;
using System.Text;
using Infrastructure;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;

namespace EntityTools.Tools.Entities
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
            AstralAccessors.Quester.Core.OnProfileChanged += ResetCache;
        }

        private readonly StringBuilder logBuilder = new StringBuilder();

        public void ResetCache()
        {
            if (Count > 0)
            {
                lock (this)
                {
                    LogStatistics();
                    _matchCount = 0;
                    _mismatchCount = 0;
                    Clear();
                }
            }
        }

        public void LogStatistics()
        {
            logBuilder.Clear();
            logBuilder.Append('\'').Append(nameof(EntityCache)).AppendLine("' usage statistics:");
            logBuilder.Append("Total records: ").AppendLine(Count.ToString());
            logBuilder.Append("matchCount: ").Append(_matchCount).Append("mismatchCount: ")
                .AppendLine(_mismatchCount.ToString());
            logBuilder.Append(nameof(EntityCacheRecord.Key)).Append("; ")
                .Append(nameof(EntityCacheRecord.Rank)).Append("; ")
                .Append(nameof(EntityCacheRecord.Capacity)).Append("; ")
                .Append(nameof(EntityCacheRecord.InitializationTime)).Append("; ")
                .Append(nameof(EntityCacheRecord.ElapsedTime)).Append("; ")
                .Append(nameof(EntityCacheRecord.AccessCount)).Append("; ")
                .Append(nameof(EntityCacheRecord.LastAccessTime)).Append("; ")
                .Append(nameof(EntityCacheRecord.AccessPerSecond)).Append("; ")
                .Append(nameof(EntityCacheRecord.AccessTimeTotal)).Append("; ")
                .Append(nameof(EntityCacheRecord.AccessTimeAvg)).Append("; ")
                .Append(nameof(EntityCacheRecord.AccessTimeMin)).Append("; ")
                .Append(nameof(EntityCacheRecord.AccessTimeMax)).Append("; ")
                .Append(nameof(EntityCacheRecord.RegenCount)).Append("; ")
                .Append(nameof(EntityCacheRecord.LastRegenTime)).Append("; ")
                .Append(nameof(EntityCacheRecord.RegenTimeTotal)).Append("; ")
                .Append(nameof(EntityCacheRecord.RegenTimeAvg)).Append("; ")
                .Append(nameof(EntityCacheRecord.RegenTimeMin)).Append("; ")
                .Append(nameof(EntityCacheRecord.RegenTimeMax)).AppendLine();

            foreach (var rec in this)
            {
                logBuilder.Append(rec.Key).Append("; ")
                    .Append(rec.Rank).Append("; ")
                    .Append(rec.Capacity).Append("; ")
                    .Append(rec.InitializationTime).Append("; ")
                    .Append(rec.ElapsedTime).Append("; ")
                    .Append(rec.AccessCount).Append("; ")
                    .Append(rec.LastAccessTime).Append("; ")
                    .Append(rec.AccessPerSecond).Append("; ")
                    .Append(rec.AccessTimeAvg).Append("; ")
                    .Append(rec.AccessTimeTotal).Append("; ")
                    .Append(rec.AccessTimeMin).Append("; ")
                    .Append(rec.AccessTimeMax).Append("; ")
                    .Append(rec.RegenCount).Append("; ")
                    .Append(rec.LastRegenTime).Append("; ")
                    .Append(rec.RegenTimeAvg).Append("; ")
                    .Append(rec.RegenTimeTotal).Append("; ")
                    .Append(rec.RegenTimeMin).Append("; ")
                    .Append(rec.RegenTimeMax).AppendLine();
            }

            ETLogger.WriteLine(logBuilder.ToString());
        }

        ~EntityCache()
        {
#if false
            AstralAccessors.Quester.Core.AfterLoad -= ResetCache;
            AstralAccessors.Quester.Core.AfterNew -= ResetCache; 
#endif
            AstralAccessors.Quester.Core.OnProfileChanged -= ResetCache;
        }

#if PROFILING
        public int MatchCount => _matchCount;
        private static int _matchCount;
        public int MismatchCount => _matchCount;
        private static int _mismatchCount;

#if false
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
                _matchCount++;
#endif
                record = this[key];
                return true;
            }
#if PROFILING
            _mismatchCount++;
#endif
            return false;
        }
        public bool TryGetValue(out EntityCacheRecord record, EntityCacheRecordKey key)
        {
            record = null;
            if (Contains(key))
            {
#if PROFILING
                _matchCount++;
#endif
                record = this[key];
                return true;
            }
#if PROFILING
            _mismatchCount++;
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
            bool contains = Contains(key);
#if PROFILING
            if (contains)
            {
                _matchCount++;
            }
            else
            {
                _mismatchCount++;
            }
#endif
            return contains;
        }
        public new bool Contains(EntityCacheRecordKey key)
        {
            bool contains = base.Contains(key);
#if PROFILING
            if (contains)
            {
                _matchCount++;
            }
            else
            {
                _mismatchCount++;
            }
#endif
            return contains;
        }
    }
}
