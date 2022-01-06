using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Astral.Classes.ItemFilter;

namespace EntityTools.Tools
{
    /// <summary>
    /// Класс, конструирующий предикат сопоставления строки с шаблоном
    /// </summary>
    public static class StringToPatternComparer
    {
        public static readonly CompareInfo InvariantCultureCompareInfo = CultureInfo.InvariantCulture.CompareInfo;
        /// <summary>
        /// Конструирование предиката для сопоставления строки с шаблоном <paramref name="pattern"/> типа <paramref name="strMatchType"/>
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="strMatchType"></param>
        /// <returns></returns>
        public static Predicate<string> GetComparer(this string pattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple)
        {
            Predicate<string> predicate;

            string matchText;

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
                            predicate = str =>
                            {
#if false
                                return str.Contains(matchText); 
#else
                                // String.Contains() реализовано через String.IndexOf() >= 0
                                // Поэтому вызываем поиска подстроки без дополнительных проверок
                                // https://github.com/dotnet/coreclr/blob/bc146608854d1db9cdbcc0b08029a87754e12b49/src/mscorlib/src/System/String.cs#L2338
                                return InvariantCultureCompareInfo.IndexOf(str, matchText, 0, str.Length, CompareOptions.Ordinal) >= 0;
#endif
                            };
                        }
                        else
                        {
                            matchText = pattern.TrimStart('*');
                            predicate = str => str.EndsWith(matchText, StringComparison.Ordinal);
                        }
                    }
                    else
                    {
                        if (pattern[pattern.Length - 1] == '*')
                        {
                            matchText = pattern.TrimEnd('*');
                            predicate = str => str.StartsWith(matchText, StringComparison.Ordinal);
                        }
                        else
                        {
                            matchText = pattern;
                            predicate = str => str.Equals(matchText, StringComparison.Ordinal);
                        }
                    }
                }
                else
                {
                    if (!regexCache.TryGetValue(pattern, out Regex regex))
                    {
                        regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
                        regexCache.Add(pattern, regex);
                    }
                    predicate = str => regex.IsMatch(str);
                }
            }
            else predicate = str =>
            {
#if DEBUG
                ETLogger.WriteLine($"Invalid predicate to compare string to '{pattern}'[{strMatchType}]");
#endif
                return false;
            };
            return predicate;
        }

        private static readonly Dictionary<string, Regex> regexCache = new Dictionary<string, Regex>();

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
            var predicate = GetComparer(pattern, strMatchType);

            return obj => predicate(selector(obj)); 
        }
        public static Func<T, bool> GetCompareFunc<T>(this string pattern, ItemFilterStringType strMatchType, Func<T, string> selector) where T : class
        {
            var predicate = GetComparer(pattern, strMatchType);

            return obj => predicate(selector(obj));
        }


    }
}
