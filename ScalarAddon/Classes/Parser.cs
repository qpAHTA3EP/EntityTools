using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AstralVars.Classes
{
    public class Parser
    {
        public static readonly string[] separators = { " ", "{", "(", "[", "]", ")", "}", "+", "-", "*", "/", "Numeric", "NumericCount", "Counter", "Count", "Items", "ItemsCount" };
        public static readonly char[] forbiddenChar = { ' ', '{', '(', '[', ']', ')', '}', '<', '>',
                                                               '+', '-', '*', '/', '^', '%',
                                                               '#', '!', '`', '~', '$', '\\', '?',
                                                               '_', '.', ',', '\'', ':', '\"', ';' };
        public static readonly string[] forbiddenLiteral = {"AND", "OR", "NOT",
                                                             "Numeric", "NumericCount", "Counter", "Count", "Items", "ItemsCount" };

        private static string forbiddenNamePartsStr = "";
        /// <summary>
        /// список символов и из сочетаний, использование которых недопустимо в имени переменной
        /// </summary>
        public static string ForbiddenNameParts
        {
            get
            {
                if (string.IsNullOrEmpty(forbiddenNamePartsStr))
                {
                    StringBuilder strBldr = new StringBuilder();

                    //strBldr.Append("{");

                    //for (int i = 0; i < forbiddenNameParts.Length - 1; i++)
                    //    strBldr.Append("'").Append(forbiddenNameParts[i]).Append("', ");


                    //strBldr.Append("'").Append(forbiddenNameParts[forbiddenNameParts.Length - 1]).Append("'").Append("}");

                    for (int i = 0; i < forbiddenChar.Length; i++)
                    {
                        strBldr.Append(forbiddenChar[i]).Append(' ');                        
                    }


                    for (int i = 0; i < forbiddenLiteral.Length; i++)
                    {
                        strBldr.Append("'").Append(forbiddenLiteral[i]).Append("'").Append(' ');
                    }

                    forbiddenNamePartsStr = strBldr.ToString();
                }
                return forbiddenNamePartsStr;
            }
        }

        /*public static readonly string counterPredicate = @"(ItemsCount|Items|NumericCount|Numeric|Counter|Count)",
                                      openBraces = @"(\[|\{|\()",
                                      closeBraces = @"(\]|\}|\))",
                                      counterPattern = $"^{counterPredicate}{openBraces}(\\w*){closeBraces}$",
                                      counterTrimPattern = $"(^{counterPredicate}{openBraces})|({closeBraces}$)";*/

        /// <summary>
        /// Символов
        /// </summary>
        public class Simbols
        {
            /// <summary>
            /// Открывающие скобки
            /// </summary>
            public static readonly char openRoundBrace = '(';
            public static readonly char openSquereBrace = '[';
            public static readonly char openCurlyBrace = '{';
            public static readonly char[] openBraces = { openRoundBrace, openSquereBrace, openCurlyBrace };
            /// <summary>
            /// Проверка символа на совпадения с открывающей скобкой
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsOpenBraces(char c)
            {
                return openBraces.Contains(c);
            }
            /// <summary>
            /// Проверка символа на совпадения с скобкой, начинающей группу
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsOpenGroupBraces(char c)
            {
                return c == openRoundBrace;
            }
            /// <summary>
            /// Закрывающие скобки
            /// </summary>
            public static readonly char closeRoundBrace = ')';
            public static readonly char closeSquereBrace = ']';
            public static readonly char closeCurlyBrace = '}';
            public static readonly char[] closeBraces = { closeRoundBrace, closeSquereBrace, closeCurlyBrace };
            /// <summary>
            /// Проверка символа на совпадения с закрывающей скобкой
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsCloseBraces(char c)
            {
                return closeBraces.Contains(c);
            }
            /// <summary>
            /// Проверка символа на совпадения со скобкой, закгрывающей группу
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsCloseGroupBraces(char c)
            {
                return c == closeRoundBrace;
            }

            /// <summary>
            /// Проверка символа на совпадения с открывающей или закрывающей скобкой
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsBraces(char c)
            {
                return openBraces.Contains(c) || closeBraces.Contains(c);
            }

            /// <summary>
            /// Символ подчеркивания
            /// </summary>
            public static readonly char Underscore = '_';
            /// <summary>
            /// Проверка символа на соответствие символу подчеркивания Underscore
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsUnderscore(char c)
            {
                return c == Underscore;
            }

            /// <summary>
            /// Системный [Десятичный разделитель]
            /// </summary>
            public static readonly char NumberDecimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            /// <summary>
            /// Проверка символа на соответствие десятичному разделителю
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsNumberDecimalSeparator(char c)
            {
                return c == '.' || c == ',';
            }
            /// <summary>
            /// замена некорректного [Десятичного разделителя] на системный
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static string CorrectNumberDecimalSeparator(ref string input)
            {
                if (NumberDecimalSeparator == '.')
                    return input.Replace(',', NumberDecimalSeparator);
                else return input.Replace('.', NumberDecimalSeparator);
            }

            /// <summary>
            /// Список смволов, обозначающих математические операторы
            /// </summary>
            public static readonly char[] MathOperator = { '-', '+', '*', '/', '%' };
            /// <summary>
            /// Проверка символа на принадлежность к математическим опреаторам
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsMathOperator(char c)
            {
                return MathOperator.Contains(c);
            }
        }

 
        public static readonly VarTypes[] varTypes = {  VarTypes.Number,
                                                        VarTypes.Boolean,            
                                                        VarTypes.String,
                                                        //VarTypes.Counter,
                                                        VarTypes.DateTime
                                                     };

        /// <summary>
        /// Символ подстановки "*", заещающий ноль или несколько алфавитно-цифровых символов в идентификаторе предмета (ItemId)
        /// </summary>
        public static readonly char WildcardAny = '*';
        /// <summary>
        /// Символ подстановки "?", заещающий один алфавитно-цифровой символ в идентификаторе предмета (ItemId)
        /// </summary>
        public static readonly char WildcardOne = '?';
        /// <summary>
        /// Проверка символа на совпадение с символом подстановки
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsWildcard(char c)
        {
            return c == WildcardAny || c == WildcardOne;
        }


        /// <summary>
        /// Предикаты функторов
        /// </summary>
        public class Predicates
        {
            /// <summary>
            /// Счетчик
            /// </summary>
            public static readonly string Counter = "Counter";
        }









        /// <summary>
        /// Получение идентификатора предмета (itemId) из выражения, заданного строкой
        /// </summary>
        /// <param name="inStr">Строка, содержащая выражение</param>
        /// <param name="itemId">Возвращаемое значение идентификатора передмета или пустая строка, если inStr не соответствует шаблону</param>
        /// <returns>true</returns>
        //public static bool GetItemID(string inStr, out string itemId)
        //{
        //    itemId = string.Empty;

        //    if (!string.IsNullOrEmpty(inStr))
        //    {
        //        if (Regex.IsMatch(inStr, counterPattern))
        //        {

        //            //numName.Replace("Numeric", String.Empty);
        //            //newVal = newVal.Substring(8, numName.Length - 9);

        //            // Удаление идентификатора функтора 
        //            itemId = Regex.Replace(inStr, counterTrimPattern, string.Empty);
        //        }
        //        //если ytn 
        //        else itemId = inStr;
        //    }
        //    return !string.IsNullOrEmpty(itemId);
        //}

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


        /// <summary>
        /// Проверка докустимости имени переменной.
        /// </summary>
        /// <param name="name">проверяемое имя</param>
        /// <returns>Результат проверки:
        /// True: корректное имя
        /// False: имя некорректно</returns>
        public static bool CheckVarName(string name)
        {
            if(string.IsNullOrEmpty(name))
                return false;

            if (name.LastIndexOfAny(forbiddenChar) >= 0)
                return false;
       
            if(forbiddenLiteral.Contains(name))
                return false;
            
            return true;
        }
        public static bool CheckVarName(object name)
        {
            if (name == null)
                return false;
            return CheckVarName(name.ToString());
        }
    }
}
