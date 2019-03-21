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

        public static readonly VarTypes[] varTypes = {  VarTypes.Number,
                                                        VarTypes.Boolean,            
                                                        VarTypes.String,
                                                        //VarTypes.Counter,
                                                        VarTypes.DateTime
                                                     };


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
        /// Получение типа переменной VarTypes из строки
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool GetType(string inStr, out VarTypes type)
        {
            return Enum.TryParse(inStr, true, out type);
        }
        /// <summary>
        /// Получение типа переменной VarTypes из объекта
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool GetType(object inObj, out VarTypes type)
        {
            if (inObj == null)
                throw new ArgumentNullException();
            if (inObj is VarTypes)
            {
                type = (VarTypes)inObj;
                return true;
            }
            else return Enum.TryParse(inObj.ToString(), out type);
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
                if (double.TryParse(inStr, out double nRes))
                {
                    result = nRes > 0;
                    succeded = true;
                }
            }
            else succeded = true;
            return succeded;
        }
        /// <summary>
        /// Перевод объекта в булевый тип.
        /// Если объект - строка, содержащая "True" или целое число больше 0, тогда результат 'True' 
        /// Строка, содержащая "False" или отрицательные целые числа и 0, считается 'False'
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(object inObj, out bool result)
        {
            if (inObj == null)
                throw new ArgumentNullException();
            if (inObj is bool)
            {
                result = (bool)inObj;
                return true;
            }
            return TryParse(inObj.ToString(), out result);
        }
        /// <summary>
        /// Строгое преобразование объекта в булевый тип.
        /// Если объект - строка, содержащая "True" или "False", она будет преобразована в булевый тип
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParseStrict(object inObj, out bool result)
        {
            if (inObj == null)
                throw new ArgumentNullException();
            if (inObj is bool)
            {
                result = (bool)inObj;
                return true;
            }
            else return bool.TryParse(inObj.ToString(), out result);
        }

        /// <summary>
        /// Перевод строки в целое число.
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(string inStr, out double result)
        {
            if (double.TryParse(inStr, out result))
                return true;
            else if (bool.TryParse(inStr, out bool bRes))
            {
                result = (bRes) ? 1 : 0;
                return true;
            }
            else result = 0;
            return false;
        }
        /// <summary>
        /// Перевод объекта в целое число.
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(object inObj, out double result)
        {
            if (inObj == null)
                throw new ArgumentNullException();
            if (inObj is double)
            {
                result = (double)inObj;
                return true;
            }
            else return TryParse(inObj.ToString(), out result);
        }
        /// <summary>
        /// Строгое преобразование объекта в булевый тип.
        /// Если объект - строка, целое число, она будет преобразована
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParseStrict(object inObj, out double result)
        {
            if (inObj == null)
                throw new ArgumentNullException();
            if (inObj is double)
            {
                result = (double)inObj;
                return true;
            }
            else return double.TryParse(inObj.ToString(), out result);
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
        /// Перевод объекта в DateTime.
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(object inObj, out DateTime result)
        {
            if (inObj == null)
                throw new ArgumentNullException();
            if (inObj is DateTime)
            {
                result = (DateTime)inObj;
                return true;
            }
            else return TryParse(inObj.ToString(), out result);
        }
        /// <summary>
        /// Строгое преобразование объекта в DateTime.
        /// Если объект - строка, содержащая DateTime, она будет преобразована
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParseStrict(object inObj, out DateTime result)
        {
            return TryParse(inObj, out result);
        }

        /// <summary>
        /// Перевод строки в тип 'Counter'.
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        //public static bool TryParse(string inStr, out string result)
        //{
        //    return GetItemID(inStr, out result);
        //}

    }
}
