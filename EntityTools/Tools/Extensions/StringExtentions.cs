using EntityTools.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Extensions
{
    public static class StringExtentions
    {
        /// <summary>
        /// Определяем местоположение простого шаблона matchText в идентификаторе pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="matchText"></param>
        /// <returns></returns>
        public static SimplePatternPos GetSimplePatternPosition(this string pattern, out string matchText)
        {
            matchText = string.Empty;
            SimplePatternPos patternPos = SimplePatternPos.None;
            if (!string.IsNullOrEmpty(pattern))
                if (pattern[0] == '*')
                {
                    if (pattern[pattern.Length - 1] == '*')
                    {
                        patternPos = SimplePatternPos.Middle;
                        matchText = pattern.Trim('*');
                    }
                    else
                    {
                        patternPos = SimplePatternPos.End;
                        matchText = pattern.TrimStart('*');
                    }
                }
                else
                {
                    if (pattern[pattern.Length - 1] == '*')
                    {
                        patternPos = SimplePatternPos.Start;
                        matchText = pattern.TrimEnd('*');
                    }
                    else
                    {
                        patternPos = SimplePatternPos.Full;
                        matchText = pattern;
                    }
                }
            return patternPos;
        }

        /// <summary>
        /// Поиск вхождения подстроки строки с учетом заданного положения
        /// </summary>
        /// <param name="text"></param>
        /// <param name="patternPos"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool CompareToSimplePattern(this string text, SimplePatternPos patternPos, string pattern)
        {
            if (string.IsNullOrEmpty(text))
                if (string.IsNullOrEmpty(pattern))
                    return true;
                else return false;
            else if (string.IsNullOrEmpty(pattern))
                return false;

            switch (patternPos)
            {
                case SimplePatternPos.Start:
                    return text.StartsWith(pattern);
                case SimplePatternPos.Middle:
                    return text.Contains(pattern);
                case SimplePatternPos.End:
                    return text.EndsWith(pattern);
                case SimplePatternPos.Full:
                    return text == pattern;
                default:
                    return text == pattern;
            }
        }
        public static bool CompareToSimplePattern(this string text, string pattern)
        {
            SimplePatternPos patternPos = GetSimplePatternPosition(pattern, out string matchText);
            return CompareToSimplePattern(text, patternPos, matchText);
        }
    }
}
