using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Extensions;
using MyNW.Classes;
using System;
using System.Text.RegularExpressions;

namespace EntityCore.Entities
{
    public class EntityCacheRecordKey
    {
        public EntityCacheRecordKey(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.InternalName, EntitySetType est = EntitySetType.Complete)
        {
            Pattern = p;
            MatchType = mp;
            NameType = nt;
            EntitySetType = est;
            comparer = Comparer_Initializer;
        }


        public readonly string Pattern = string.Empty;
        public readonly ItemFilterStringType MatchType = ItemFilterStringType.Simple;
        public readonly EntityNameType NameType = EntityNameType.NameUntranslated;
        public readonly EntitySetType EntitySetType = EntitySetType.Complete;

        public bool IsValid => NameType == EntityNameType.Empty || !string.IsNullOrEmpty(Pattern);

        public override bool Equals(object otherObj)
        {
            if (ReferenceEquals(this, otherObj))
                return true;
            if (otherObj is EntityCacheRecordKey other)
                return Equals(other);
            return false;
        }

        public bool Equals(EntityCacheRecordKey other)
        {
            return MatchType == other.MatchType
                   && NameType == other.NameType
                   && EntitySetType == other.EntitySetType
                   && Pattern == other.Pattern;
        }

        public override int GetHashCode()
        {
            return Pattern.GetHashCode();
        }

        #region IsMatch
        /// <summary>
        /// Проверка 
        /// </summary>
        public bool IsMatch(Entity entity)
        {
            if (entity is null) return false;
            return comparer(entity);
        }

        internal bool Validate(Entity entity)
        {
            return entity != null && entity.IsValid
                    && (entity.Character.IsValid || entity.Critter.IsValid || entity.Player.IsValid)
                    && comparer(entity);
        }

        private Predicate<Entity> comparer;

        /// <summary>
        /// Метод ленивой инициализации компаратора <see cref="comparer"/>, который выполняет сопоставление <param name="entity"/> с заданным набором атрибутов <see cref="Pattern"/>, <see cref="MatchType"/> и <see cref="NameType"/>
        /// </summary>
        private bool Comparer_Initializer(Entity entity)
        {
            var predicate = EntityComparer.Get(Pattern, MatchType, NameType);
            
            comparer = predicate ?? (ett => false);

            return comparer(entity);
        }
        #endregion
    }
}
