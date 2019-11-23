using Astral.Classes;
using Astral.Classes.ItemFilter;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EntityTools.Tools
{
    public static class EntityCache //: KeyedCollection<string, EntityCacheRecord>
    {
        /// <summary>
        /// Запись Кэша
        /// </summary>
        public class EntityCacheRecord
        {
            EntityCacheRecord() { }
            EntityCacheRecord(string p, ItemFilterStringType mp, EntityNameType nt, bool h, float r, bool rc, List<string> regions)
            {
                Pattern = p;
                MatchType = mp;
                NameType = nt;
                HealthCheck = h;
                RegionCheck = rc;
                Range = r;
                CustomRegionNames = regions;
            }

            public string Pattern { get; set; } = string.Empty;
            public ItemFilterStringType MatchType { get; set; } = ItemFilterStringType.Simple;
            public EntityNameType NameType { get; set; } = EntityNameType.NameUntranslated;
            public bool HealthCheck { get; set; } = false;
            public float Range { get; set; } = 0;
            public bool RegionCheck { get; set; } = false;
            public List<string> CustomRegionNames { get; set; } = null;

            public Entity Entity { get; set; }

            public bool Equals(EntityCacheRecord other)
            {
                return other != null
                        && Pattern == other.Pattern
                        && MatchType == other.MatchType
                        && NameType == NameType
                        && HealthCheck == other.HealthCheck
                        && Range == other.Range
                        && RegionCheck == other.RegionCheck
                        && CustomRegionNamesCompare(CustomRegionNames, other.CustomRegionNames);
            }

            public override int GetHashCode()
            {
                return Pattern.GetHashCode();
            }

            /// <summary>
            /// Сравнение двух списков строк
            /// </summary>
            /// <param name="l"></param>
            /// <param name="r"></param>
            /// <returns></returns>
            private static bool CustomRegionNamesCompare (List<string> l, List<string> r)
            {
                if (l == null && r == null)
                    return true;
                if (l != null && r != null)
                {
                    foreach (string s in r)
                        if (!l.Contains(s))
                            return false;
                    return true;
                }
                return false;
            }
        }

        public static void Add(EntityCacheRecord val, int mls)
        {
            if (list.ContainsKey(val))
            {
                list.Remove(val);
            }
            list.Add(val, new Timeout(mls));
        }

        public static void Add(EntityCacheRecord val)
        {
            if (list.ContainsKey(val))
            {
                list.Remove(val);
            }
            list.Add(val, new Timeout(999000));
        }

        public static bool IsBlackList(EntityCacheRecord val)
        {
            if (list.ContainsKey(val))
            {
                if (!list[val].IsTimedOut)
                {
                    return true;
                }
                list.Remove(val);
            }
            return false;
        }

        public static bool Contains(EntityCacheRecord val)
        {
            return IsBlackList(val);
        }

        public static void Remove(EntityCacheRecord val)
        {
            if (list.ContainsKey(val))
            {
                list.Remove(val);
            }
        }

        public static void Clear()
        {
            list.Clear();
        }

        private static Dictionary<EntityCacheRecord, Timeout> list = new Dictionary<EntityCacheRecord, Timeout>();
    }
}
