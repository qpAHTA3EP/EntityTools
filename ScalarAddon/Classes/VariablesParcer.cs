using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AstralVars.Classes
{
    public class VariablesParcer
    {
        public static readonly string[] separators = { " ", "{", "(", "[", "]", ")", "}", "+", "-", "*", "/", "Numeric", "NumericCount", "Counter", "Count", "Items", "ItemsCount" };
        public static readonly string counterPredicate = @"(ItemsCount|Items|NumericCount|Numeric|Counter|Count)",
                                      openBraces = @"(\[|\{|\()",
                                      closeBraces = @"(\]|\}|\))",
                                      counterPattern = $"^{counterPredicate}{openBraces}(\\w*){closeBraces}$",
                                      counterTrimPattern = $"(^{counterPredicate}{openBraces})|({closeBraces}$)";

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
                if (Regex.IsMatch(newVal, counterPattern))
                {

                    //numName.Replace("Numeric", String.Empty);
                    //newVal = newVal.Substring(8, numName.Length - 9);

                    // Удаление идентификатора функтора :
                    itemId = Regex.Replace(newVal, counterTrimPattern, string.Empty);
                }
            }
            return string.IsNullOrEmpty(itemId);
        }
    }
}
