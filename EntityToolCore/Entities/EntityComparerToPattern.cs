using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Logger;
using MyNW.Classes;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace EntityCore.Entities
{
    /// <summary>
    /// Класс, инкапсулирующие все базовые проверки Entity на соответствие шаблону
    /// </summary>
    public static class EntityToPatternComparer
    {
        public static Predicate<Entity> Get(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated)
        {
            if (strMatchType == ItemFilterStringType.Simple)
            {
                SimplePatternPos pos = entPattern.GetSimplePatternPosition(out string pattern);

                if (nameType == EntityNameType.InternalName)
                    switch (pos)
                    {
                        case SimplePatternPos.Full:
                            return (Entity e) => CompareInternal2SimpleFull(e, pattern);
                        case SimplePatternPos.Start:
                            return (Entity e) => CompareInternal2SimpleStart(e, pattern);
                        case SimplePatternPos.Middle:
                            return (Entity e) => CompareInternal2SimpleMiddle(e, pattern);
                        case SimplePatternPos.End:
                            return (Entity e) => CompareInternal2SimpleEnd(e, pattern);
                        default:
#if DEBUG
                            EntityToolsLogger.WriteLine(LogType.Error, $"{nameof(EntityToPatternComparer)}::{MethodBase.GetCurrentMethod().Name}: Simple pattern is invalid {{{entPattern}, {strMatchType}, {nameType}}}");
#endif
                            return null;
                    }
                else if (nameType == EntityNameType.NameUntranslated)
                    switch (pos)
                    {
                        case SimplePatternPos.Full:
                            return (Entity e) => CompareUntranslated2SimpleFull(e, pattern);
                        case SimplePatternPos.Start:
                            return (Entity e) => CompareUntranslated2SimpleStart(e, pattern);
                        case SimplePatternPos.Middle:
                            return (Entity e) => CompareUntranslated2SimpleMiddle(e, pattern);
                        case SimplePatternPos.End:
                            return (Entity e) => CompareUntranslated2SimpleEnd(e, pattern);
                        default:
#if DEBUG
                            EntityToolsLogger.WriteLine(LogType.Error, $"{nameof(EntityToPatternComparer)}::{MethodBase.GetCurrentMethod().Name}: Simple pattern is invalid {{{entPattern}, {strMatchType}, {nameType}}}");
#endif
                            return null;
                    }
                else return CompareEmpty;
            }
            else
            {
                if (nameType == EntityNameType.InternalName)
                    return (Entity e) => CompareInternal2Regex(e, entPattern);
                else if (nameType == EntityNameType.NameUntranslated)
                    return (Entity e) => CompareUntranslated2Regex(e, entPattern);
                else return CompareEmpty;
            }
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

        private static bool CompareInternal2Regex(Entity e, string pattern)
        {
            return Regex.IsMatch(e.InternalName, pattern);
        }
        private static bool CompareUntranslated2Regex(Entity e, string pattern)
        {
            return Regex.IsMatch(e.NameUntranslated, pattern);
        }

        private static bool CompareEmpty(Entity e)
        {
            return string.IsNullOrEmpty(e.InternalName) && string.IsNullOrEmpty(e.NameUntranslated);
        }
    }
}
