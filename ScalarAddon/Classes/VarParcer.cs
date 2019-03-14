using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AstralVars.Classes
{
    public class VarParcer
    {
        public static readonly string[] separators = { " ", "{", "(", "[", "]", ")", "}", "+", "-", "*", "/", "Numeric", "NumericCount", "Counter", "Count", "Items", "ItemsCount" };
        public static readonly string counterPredicate = @"(ItemsCount|Items|NumericCount|Numeric|Counter|Count)",
                                      openBraces = @"(\[|\{|\()",
                                      closeBraces = @"(\]|\}|\))",
                                      counterPattern = $"^{counterPredicate}{openBraces}(\\w*){closeBraces}$",
                                      counterTrimPattern = $"(^{counterPredicate}{openBraces})|({closeBraces}$)";

        public static readonly VarTypes[] varTypes = {  VarTypes.Integer,
                                                        VarTypes.Boolean,            
                                                        VarTypes.String,
                                                        VarTypes.DateTime,
                                                        VarTypes.Counter };


        /// <summary>
        /// Получение идентификатора предмета (itemId) из выражения, заданного строкой
        /// </summary>
        /// <param name="inStr">Строка, содержащая выражение</param>
        /// <param name="itemId">Возвращаемое значение идентификатора передмета или пустая строка, если inStr не соответствует шаблону</param>
        /// <returns>true</returns>
        public static bool GetItemID(string inStr, out string itemId)
        {
            itemId = string.Empty;

            if (!string.IsNullOrEmpty(inStr))
            {
                if (Regex.IsMatch(inStr, counterPattern))
                {

                    //numName.Replace("Numeric", String.Empty);
                    //newVal = newVal.Substring(8, numName.Length - 9);

                    // Удаление идентификатора функтора 
                    itemId = Regex.Replace(inStr, counterTrimPattern, string.Empty);
                }
                //если ytn 
                else itemId = inStr;
            }
            return !string.IsNullOrEmpty(itemId);
        }

        /// <summary>
        /// Перевод строки в булевый тип.
        /// Если в строке содержится "True" или целое число больше 0, тогда результат 'True' 
        /// Строка, содержащая "False" или отрицательные целые числа и 0, считается 'False'
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(string inStr, out bool result)
        {
            bool succeded = false;
            if (!bool.TryParse(inStr, out result))
            {
                if (int.TryParse(inStr, out int iRes))
                {
                    result = iRes > 0;
                    succeded = true;
                }
            }
            else succeded = true;
            return succeded;
        }

        /// <summary>
        /// Перевод строки в целое число.
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(string inStr, out int result)
        {
            if (int.TryParse(inStr, out result))
                return true;
            else
            {
                if (bool.TryParse(inStr, out bool bRes))
                    result = (bRes) ? 1 : 0;
                else result = 0;
            }
            return false;
        }

        /// <summary>
        /// Перевод строки в DateTime.
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(string inStr, out DateTime result)
        {
            if (DateTime.TryParse(inStr, out result))
                return true;
            else result = DateTime.MinValue;
            return false;
        }

        /// <summary>
        /// Перевод строки в тип 'Counter'.
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(string inStr, out string result)
        {
            return GetItemID(inStr, out result);
        }
    }
}
