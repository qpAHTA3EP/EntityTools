using Astral.Classes.ItemFilter;
using EntityCore.Enums;
using EntityCore.Extentions;
using EntityCore.Tools;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EntityCore.Entities
{

    /// <summary>
    /// Класс, инкапсулирующие все базовые проверки Entity на соответствие шаблону
    /// 
    /// </summary>
    internal class EntityComparerToPattern
    {
        private string pattern = string.Empty;

        internal Predicate<Entity> Check { get; private set; }

        internal EntityComparerToPattern(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated)
        {
            if (strMatchType == ItemFilterStringType.Simple)
            {
                SimplePatternPos pos = entPattern.GetSimplePatternPosition(out pattern);

                if (nameType == EntityNameType.InternalName)
                    switch (pos)
                    {
                        case SimplePatternPos.Full:
                            Check = CompareInternal2SimpleFull;
                            break;
                        case SimplePatternPos.Start:
                            Check = CompareInternal2SimpleStart;
                            break;
                        case SimplePatternPos.Middle:
                            Check = CompareInternal2SimpleMiddle;
                            break;
                        case SimplePatternPos.End:
                            Check = CompareInternal2SimpleEnd;
                            break;
                        default:
                            throw new ArgumentException("Simple pattern is invalid");
                    }
                else if (nameType == EntityNameType.NameUntranslated)
                    switch (pos)
                    {
                        case SimplePatternPos.Full:
                            Check = CompareUntranslated2SimpleFull;
                            break;
                        case SimplePatternPos.Start:
                            Check = CompareUntranslated2SimpleStart;
                            break;
                        case SimplePatternPos.Middle:
                            Check = CompareUntranslated2SimpleMiddle;
                            break;
                        case SimplePatternPos.End:
                            Check = CompareUntranslated2SimpleEnd;
                            break;
                        default:
                            throw new ArgumentException("Simple pattern is invalid");
                    }
                else Check = CompareEmpty;
            }
            else
            {
                pattern = entPattern;
                if (nameType == EntityNameType.InternalName)
                    Check = CompareInternal2Regex;
                else if (nameType == EntityNameType.NameUntranslated)
                    Check = CompareUntranslated2Regex;
                else Check = CompareEmpty;
            }

        }

        private bool CompareInternal2SimpleFull(Entity e)
        {
            return e.InternalName.Equals(pattern);
        }
        private bool CompareUntranslated2SimpleFull(Entity e)
        {
            return e.NameUntranslated.Equals(pattern);
        }

        private bool CompareInternal2SimpleStart(Entity e)
        {
            return e.InternalName.StartsWith(pattern);
        }
        private bool CompareUntranslated2SimpleStart(Entity e)
        {
            return e.NameUntranslated.StartsWith(pattern);
        }

        private bool CompareInternal2SimpleMiddle(Entity e)
        {
            return e.InternalName.Contains(pattern);
        }
        private bool CompareUntranslated2SimpleMiddle(Entity e)
        {
            return e.NameUntranslated.Contains(pattern);
        }

        private bool CompareInternal2SimpleEnd(Entity e)
        {
            return e.InternalName.EndsWith(pattern);
        }
        private bool CompareUntranslated2SimpleEnd(Entity e)
        {
            return e.NameUntranslated.EndsWith(pattern);
        }

        private bool CompareInternal2Regex(Entity e)
        {
            return Regex.IsMatch(e.InternalName, pattern);
        }
        private bool CompareUntranslated2Regex(Entity e)
        {
            return Regex.IsMatch(e.NameUntranslated, pattern);
        }

        private bool CompareEmpty(Entity e)
        {
            return string.IsNullOrEmpty(e.InternalName) && string.IsNullOrEmpty(e.NameUntranslated);
        }
    }
    
}
