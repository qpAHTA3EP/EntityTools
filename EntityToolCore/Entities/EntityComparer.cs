using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Extensions;
using MyNW.Classes;
using System;
using System.Text.RegularExpressions;

namespace EntityCore.Entities
{
    /// <summary>
    /// Класс, инкапсулирующие все базовые проверки Entity на соответствие шаблону
    /// </summary>
    public static class EntityComparer
    {
        public static Predicate<Entity> Get(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated)
        {
            if (strMatchType == ItemFilterStringType.Simple)
            {
                if (entPattern == "*" || entPattern == "**")
                    return CompareSimpleAny;

                SimplePatternPos pos = entPattern.GetSimplePatternPosition(out string pattern);

                if (nameType == EntityNameType.InternalName)
                    switch (pos)
                    {
                        case SimplePatternPos.Full:
                            return e => CompareInternal2SimpleFull(e, pattern);
                        case SimplePatternPos.Start:
                            return e => CompareInternal2SimpleStart(e, pattern);
                        case SimplePatternPos.Middle:
                            return e => CompareInternal2SimpleMiddle(e, pattern);
                        case SimplePatternPos.End:
                            return e => CompareInternal2SimpleEnd(e, pattern);
                        default:
#if DEBUG
                            ETLogger.WriteLine(LogType.Error, $"{nameof(EntityComparer)}::{MethodBase.GetCurrentMethod().Name}: Simple pattern is invalid {{{entPattern}, {strMatchType}, {nameType}}}");
#endif
                            return null;
                    }
                if (nameType == EntityNameType.NameUntranslated)
                    switch (pos)
                    {
                        case SimplePatternPos.Full:
                            return e => CompareUntranslated2SimpleFull(e, pattern);
                        case SimplePatternPos.Start:
                            return e => CompareUntranslated2SimpleStart(e, pattern);
                        case SimplePatternPos.Middle:
                            return e => CompareUntranslated2SimpleMiddle(e, pattern);
                        case SimplePatternPos.End:
                            return e => CompareUntranslated2SimpleEnd(e, pattern);
                        default:
#if DEBUG
                            ETLogger.WriteLine(LogType.Error, $"{nameof(EntityComparer)}::{MethodBase.GetCurrentMethod().Name}: Simple pattern is invalid {{{entPattern}, {strMatchType}, {nameType}}}");
#endif
                            return null;
                    }
                return CompareEmpty;
            }

            if (nameType == EntityNameType.InternalName)
            {
                //return e => CompareInternal2Regex(e, entPattern);
                return GetComparer_Internal2CompiledRegex(entPattern);
            }

            if (nameType == EntityNameType.NameUntranslated)
            {
                //return e => CompareUntranslated2Regex(e, entPattern);
                return GetComparer_Untranslated2CompiledRegex(entPattern);
            }
            return CompareEmpty;
        }

        private static bool CompareInternal2SimpleFull(Entity e, string pattern)
        {
            return e.InternalName.Equals(pattern);
        }
        private static bool CompareUntranslated2SimpleFull(Entity e, string pattern)
        {
            return e.NameUntranslated.Equals(pattern);
        }

        private static bool CompareInternal2SimpleStart(Entity e, string pattern)
        {
            return e.InternalName.StartsWith(pattern);
        }
        private static bool CompareUntranslated2SimpleStart(Entity e, string pattern)
        {
            return e.NameUntranslated.StartsWith(pattern);
        }

        private static bool CompareInternal2SimpleMiddle(Entity e, string pattern)
        {
            return e.InternalName.Contains(pattern);
        }
        private static bool CompareUntranslated2SimpleMiddle(Entity e, string pattern)
        {
            return e.NameUntranslated.Contains(pattern);
        }

        private static bool CompareInternal2SimpleEnd(Entity e, string pattern)
        {
            return e.InternalName.EndsWith(pattern);
        }

        private static bool CompareUntranslated2SimpleEnd(Entity e, string pattern)
        {
            return e.NameUntranslated.EndsWith(pattern);
        }

        private static bool CompareSimpleAny(Entity e) => true;

        private static bool CompareInternal2Regex(Entity e, string pattern)
        {
            return Regex.IsMatch(e.InternalName, pattern);
        }
        private static bool CompareUntranslated2Regex(Entity e, string pattern)
        {
            return Regex.IsMatch(e.NameUntranslated, pattern);
        }

        private static Predicate<Entity> GetComparer_Internal2CompiledRegex(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
            return e => regex.IsMatch(e.InternalName);
        }
        private static Predicate<Entity> GetComparer_Untranslated2CompiledRegex(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
            return e => regex.IsMatch(e.NameUntranslated);
        }

        private static bool CompareEmpty(Entity e)
        {
            return string.IsNullOrEmpty(e.InternalName) && string.IsNullOrEmpty(e.NameUntranslated);
        }
    }
}
