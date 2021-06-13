using Astral.Classes.ItemFilter;
using EntityTools;
using EntityTools.Enums;
using EntityTools.Extensions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityCore.Entities
{
    public class EntityCacheRecordKey
    {
        public EntityCacheRecordKey()
        {
            comparer = CompareFalse;
        }
        public EntityCacheRecordKey(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.InternalName, EntitySetType est = EntitySetType.Complete)
        {
            Pattern = p;
            MatchType = mp;
            NameType = nt;
            EntitySetType = est;
            comparer = initialize_Comparer(Pattern, MatchType, NameType);
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
            else if (otherObj is EntityCacheRecordKey other)
                return Equals(other);
            return false;
        }

        public bool Equals(EntityCacheRecordKey other)
        {
            return Pattern == other.Pattern
                    && MatchType == other.MatchType
                    && NameType == other.NameType
                    && EntitySetType == other.EntitySetType;
        }

        public override int GetHashCode()
        {
            return Pattern.GetHashCode();
        }

        #region IsMatch
        /// <summary>
        /// Проверка 
        /// </summary>
        public bool IsMatch(Entity e)
        {
            if (e is null) return false;
            return comparer(e);
        }

        internal bool Validate(Entity e)
        {
            return e != null && e.IsValid
                    && (e.Character.IsValid || e.Critter.IsValid || e.Player.IsValid)
                    && comparer(e);
        }

        private readonly Predicate<Entity> comparer = null;
        string treatedPattern;
        /// <summary>
        /// Метод, инициализирующий функтор <see cref="comparer"/>,
        /// использующийся для проверки сущности <seealso cref="Entity"/> на соответствия сочетанию признаков <param name="entityId"/>, <param name="matchType"/>, <param name="entityNameType"/>
        /// </summary>
        private Predicate<Entity> initialize_Comparer(string entityId, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType entityNameType = EntityNameType.NameUntranslated)
        {
            //bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.DebugEntityTools;
            //string currentMethodName = extendedDebugInfo ? string.Concat(GetType().Name, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

            Predicate<Entity> predicate;
            treatedPattern = string.Empty;

            if (matchType == ItemFilterStringType.Simple)
            {
                if (entityId == "*" || entityId == "**")
                    predicate = CompareSimpleAny;
                else
                {
                    SimplePatternPos pos = entityId.GetSimplePatternPosition(out treatedPattern);

                    if (entityNameType == EntityNameType.InternalName)
                        switch (pos)
                        {
                            case SimplePatternPos.Full:
                                predicate = CompareInternal2SimpleFull;
                                break;
                            case SimplePatternPos.Start:
                                predicate = CompareInternal2SimpleStart;
                                break;
                            case SimplePatternPos.Middle:
                                predicate = CompareInternal2SimpleMiddle;
                                break;
                            case SimplePatternPos.End:
                                predicate = CompareInternal2SimpleEnd;
                                break;
                            default:
                                predicate = CompareFalse;
                                break;
                        }
                    else if (entityNameType == EntityNameType.NameUntranslated)
                        switch (pos)
                        {
                            case SimplePatternPos.Full:
                                predicate = CompareUntranslated2SimpleFull;
                                break;
                            case SimplePatternPos.Start:
                                predicate = CompareUntranslated2SimpleStart;
                                break;
                            case SimplePatternPos.Middle:
                                predicate = CompareUntranslated2SimpleMiddle;
                                break;
                            case SimplePatternPos.End:
                                predicate = CompareUntranslated2SimpleEnd;
                                break;
                            default:
                                predicate = CompareFalse;
                                break;
                        }
                    else predicate = CompareEmpty;
                }
            }
            else
            {
                if (entityNameType == EntityNameType.InternalName)
                    predicate = CompareInternal2Regex;
                else if (entityNameType == EntityNameType.NameUntranslated)
                    predicate = CompareUntranslated2Regex;
                else predicate = CompareEmpty;
            }

            return predicate;
        }

        private bool CompareInternal2SimpleFull(Entity e) => e.InternalName.Equals(treatedPattern);
        private bool CompareUntranslated2SimpleFull(Entity e) => e.NameUntranslated.Equals(treatedPattern);

        private bool CompareInternal2SimpleStart(Entity e) => e.InternalName.StartsWith(treatedPattern);
        private bool CompareUntranslated2SimpleStart(Entity e) => e.NameUntranslated.StartsWith(treatedPattern);

        private bool CompareInternal2SimpleMiddle(Entity e) => e.InternalName.Contains(treatedPattern);
        private bool CompareUntranslated2SimpleMiddle(Entity e) => e.NameUntranslated.Contains(treatedPattern);

        private bool CompareInternal2SimpleEnd(Entity e) => e.InternalName.EndsWith(treatedPattern);
        private bool CompareUntranslated2SimpleEnd(Entity e) => e.NameUntranslated.EndsWith(treatedPattern);

        private bool CompareSimpleAny(Entity e) => true;

        private bool CompareInternal2Regex(Entity e) => Regex.IsMatch(e.InternalName, Pattern);
        private bool CompareUntranslated2Regex(Entity e) => Regex.IsMatch(e.NameUntranslated, Pattern);

        private bool CompareEmpty(Entity e) => string.IsNullOrEmpty(e.InternalName) && string.IsNullOrEmpty(e.NameUntranslated);

        private bool CompareFalse(Entity e) => false;
        #endregion
    }
}
