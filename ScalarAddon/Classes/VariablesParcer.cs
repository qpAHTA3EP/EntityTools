using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ValiablesAstralExtention.Classes
{
    public class VariablesParcer
    {
        protected static readonly string[] separators = { " ", "{", "(", "[", "]", ")", "}", "+", "-", "*", "/", "Numeric", "ItemsCount" };
        protected static readonly string countPattern = @"^(ItemsCount|Items|NumericCount|Numeric|Count)\(\w*\)$",
                                         countTrimPattern = @"(^(ItemsCount|Items|NumericCount|Numeric|Count)\()|(\)$)";

        /// <summary>
        /// Получение идентификатора предмета (itemId) из выражения, заданного строкой
        /// </summary>
        /// <param name="inStr">Строка, содержащая выражение</param>
        /// <param name="itemId">Возвращаемое значение идентификатора передмета или пустая строка, если inStr не соответствует шаблону</param>
        /// <returns>true</returns>
        public static bool GetItemID(string inStr, out string itemId)
        {
            itemId = string.Empty;

            string newVal = inStr as string;
            if (!string.IsNullOrEmpty(newVal))
            {
                if (Regex.IsMatch(newVal, countPattern))
                {

                    //numName.Replace("Numeric", String.Empty);
                    //newVal = newVal.Substring(8, numName.Length - 9);

                    // Удаление идентификатора функтора :
                    itemId = Regex.Replace(newVal, countTrimPattern, string.Empty);
                }
            }
            return string.IsNullOrEmpty(itemId);
        }
    }
}
