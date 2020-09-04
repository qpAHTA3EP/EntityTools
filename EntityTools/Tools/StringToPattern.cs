using Astral.Classes.ItemFilter;
using EntityTools;
using EntityTools.Enums;
using EntityTools.Extensions;
using MyNW.Classes;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace EntityTools.Tools
{
    /// <summary>
    /// Класс, конструирующий предикат сопоставления строки с шаблоном
    /// </summary>
    public static class StringToPatternComparer
    {
        /// <summary>
        /// Конструирование предиката для сопоставления строки с шаблоном <paramref name="pattern"/> типа <paramref name="strMatchType"/>
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="strMatchType"></param>
        /// <returns></returns>
        public static Predicate<string> Get(string pattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple)
        {
            Predicate<string> predicate = null;

            string matchText = string.Empty;

            if (!string.IsNullOrEmpty(pattern))
            {
                if (strMatchType == ItemFilterStringType.Simple)
                {
                    if (pattern == "*" || pattern == "**")
                    {
                        // pattern == '*' || '**'
                        predicate = (string str) => true;
                    }
                    else if (pattern[0] == '*')
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            matchText = pattern.Trim('*');
                            predicate = (string str) => str.Contains(matchText);
                        }
                        else
                        {
                            matchText = pattern.TrimStart('*');
                            predicate = (string str) => str.EndsWith(matchText);
                        }
                    }
                    else
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            matchText = pattern.TrimEnd('*');
                            predicate = (string str) => str.StartsWith(matchText);
                        }
                        else
                        {
                            matchText = pattern;
                            predicate = (string str) => str.Equals(matchText);
                        }
                    }
                }
                else
                {
                    predicate = (string str) => Regex.IsMatch(str, pattern);
                }
            }
            else predicate = (str) =>
            {
#if DEBUG
                ETLogger.WriteLine($"Invalid prediate to compare string to '{pattern}'[{strMatchType}]");
#endif
                return false;
            };
            return predicate;
        }

        /// <summary>
        /// Конструирование предиката для сопоставления строки, извлекаемой <paramref name="selector"/> с шаблоном <paramref name="pattern"/> типа <paramref name="strMatchType"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pattern"></param>
        /// <param name="strMatchType"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Predicate<T> Get<T>(string pattern, ItemFilterStringType strMatchType, Func<T, string> selector) where T : class
        {
            Predicate<string> predicate = Get(pattern, strMatchType);

            return (T obj) => predicate(selector(obj)); 
        }

    }
}
