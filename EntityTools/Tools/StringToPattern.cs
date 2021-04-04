using System;
using System.Text.RegularExpressions;
using Astral.Classes.ItemFilter;

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
        public static Predicate<string> GetComparer(this string pattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple)
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
                        predicate = str => true;
                    }
                    else if (pattern[0] == '*')
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            matchText = pattern.Trim('*');
                            predicate = str => str.Contains(matchText);
                        }
                        else
                        {
                            matchText = pattern.TrimStart('*');
                            predicate = str => str.EndsWith(matchText);
                        }
                    }
                    else
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            matchText = pattern.TrimEnd('*');
                            predicate = str => str.StartsWith(matchText);
                        }
                        else
                        {
                            matchText = pattern;
                            predicate = str => str.Equals(matchText);
                        }
                    }
                }
                else
                {
                    predicate = str => Regex.IsMatch(str, pattern);
                }
            }
            else predicate = str =>
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
        public static Predicate<T> GetComparer<T>(this string pattern, ItemFilterStringType strMatchType, Func<T, string> selector) where T : class
        {
            Predicate<string> predicate = GetComparer(pattern, strMatchType);

            return obj => predicate(selector(obj)); 
        }

    }
}
