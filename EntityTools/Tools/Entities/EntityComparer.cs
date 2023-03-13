using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Extensions;
using Infrastructure;
using MyNW.Classes;

namespace EntityTools.Tools.Entities
{
    /// <summary>
    /// Конструктор предиката, сопоставляющего <seealso cref="Entity"/> с текстовым шаблоном
    /// </summary>
    public static class EntityComparer
    {
        public static readonly CompareInfo InvariantCultureCompareInfo = CultureInfo.InvariantCulture.CompareInfo;

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
            return e.InternalName.Equals(pattern, StringComparison.Ordinal);
        }
        private static bool CompareUntranslated2SimpleFull(Entity e, string pattern)
        {
            return e.NameUntranslated.Equals(pattern, StringComparison.Ordinal);
        }

        private static bool CompareInternal2SimpleStart(Entity e, string pattern)
        {
            return e.InternalName.StartsWith(pattern, StringComparison.Ordinal);
        }
        private static bool CompareUntranslated2SimpleStart(Entity e, string pattern)
        {
            return e.NameUntranslated.StartsWith(pattern, StringComparison.Ordinal);
        }

        private static bool CompareInternal2SimpleMiddle(Entity e, string pattern)
        {
#if false
            return e.InternalName.Contains(pattern); 
#else
            // String.Contains() реализовано через String.IndexOf() >= 0
            // Поэтому вызываем поиска подстроки без дополнительных проверок
            // https://github.com/dotnet/coreclr/blob/bc146608854d1db9cdbcc0b08029a87754e12b49/src/mscorlib/src/System/String.cs#L2338
            var intName = e.InternalName;
            return InvariantCultureCompareInfo.IndexOf(intName, pattern, 0, intName.Length, CompareOptions.Ordinal) >= 0;
#endif
        }
        private static bool CompareUntranslated2SimpleMiddle(Entity e, string pattern)
        {
#if false
            return e.NameUntranslated.Contains(pattern); 
#else
            // String.Contains() реализовано через String.IndexOf() >= 0
            // Поэтому вызываем поиска подстроки без дополнительных проверок
            // https://github.com/dotnet/coreclr/blob/bc146608854d1db9cdbcc0b08029a87754e12b49/src/mscorlib/src/System/String.cs#L2338
            var unransName = e.NameUntranslated;
            return InvariantCultureCompareInfo.IndexOf(unransName, pattern, 0, unransName.Length, CompareOptions.Ordinal) >= 0;
#endif
        }

        private static bool CompareInternal2SimpleEnd(Entity e, string pattern)
        {
            return e.InternalName.EndsWith(pattern, StringComparison.Ordinal);
        }

        private static bool CompareUntranslated2SimpleEnd(Entity e, string pattern)
        {
            return e.NameUntranslated.EndsWith(pattern, StringComparison.Ordinal);
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

        /// <summary>
        /// Кэш cкомпилированных регулярных выражений <seealso cref="Regex"/>
        /// </summary>
        private static readonly Dictionary<string, Regex> regexCache = new Dictionary<string, Regex>();

        private static Predicate<Entity> GetComparer_Internal2CompiledRegex(string pattern)
        {
            if (!regexCache.TryGetValue(pattern, out var regex))
            {
                regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
                regexCache.Add(pattern, regex);
            }
            return e => regex.IsMatch(e.InternalName);
        }
        private static Predicate<Entity> GetComparer_Untranslated2CompiledRegex(string pattern)
        {
            if (!regexCache.TryGetValue(pattern, out var regex))
            {
                regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
                regexCache.Add(pattern, regex);
            }
            return e => regex.IsMatch(e.NameUntranslated);
        }

        private static bool CompareEmpty(Entity e)
        {
            return string.IsNullOrEmpty(e.InternalName) && string.IsNullOrEmpty(e.NameUntranslated);
        }
    }
}
