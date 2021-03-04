using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Entities
{
    public class EntityCacheRecordKey
    {
        public EntityCacheRecordKey()
        {
            Comparer = EntityToPatternComparer.Get(Pattern, MatchType, NameType);
        }
        public EntityCacheRecordKey(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete)
        {
            Pattern = p;
            MatchType = mp;
            NameType = nt;
            EntitySetType = est;
            Comparer = EntityToPatternComparer.Get(Pattern, MatchType, NameType);
        }


        public readonly string Pattern = string.Empty;
        public readonly ItemFilterStringType MatchType = ItemFilterStringType.Simple;
        public readonly EntityNameType NameType = EntityNameType.NameUntranslated;
        public readonly EntitySetType EntitySetType = EntitySetType.Complete;
        public readonly Predicate<Entity> Comparer = null;

        public override bool Equals(object otherObj)
        {
            if (ReferenceEquals(this, otherObj))
                return true;
            else if (otherObj is EntityCacheRecordKey other)
                return Equals(other);
            return false;
        }

        public bool Equals(EntityCacheRecordKey other)
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

}
