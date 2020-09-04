using VariableTools.Classes;
using System;
using System.Linq;
using System.Text;

namespace VariableTools.Expressions
{
    public class Parser
    {
        public static readonly string[] separators = { " ", "{", "(", "[", "]", ")", "}", "+", "-", "*", "/", "Numeric", "NumericCount", "Counter", "Count", "Items", "ItemsCount" };

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

                    for (int i = 0; i < Symbols.forbiddenChar.Length; i++)
                    {
                        strBldr.Append(Symbols.forbiddenChar[i]).Append(' ');                        
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
        /// Проверка допустимости имени в качестве названия переменной
        /// Название недопустимо, если содержит запреденные символы или совпадает с именами функторов
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsForbidden(string name)
        {
            return string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)
                || name.IndexOfAny(Symbols.forbiddenChar) >= 0
                || name.Equals(Predicates.CountItem)
                || name.Equals(Predicates.CountNumeric)
                || name.Equals(Predicates.Random)
                || name.Equals(Predicates.Days)
                || name.Equals(Predicates.Hours)
                || name.Equals(Predicates.Minutes)
                || name.Equals(Predicates.Seconds);
        }

        /// <summary>
        /// Проверка корректности имени имени переменной и 
        /// замена запрещенных символов
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newName"></param>
        /// <returns>True, если имя было исправлено</returns>
        public static bool CorrectForbiddenName(string name, out string newName)
        {
            if ( name.Equals(Predicates.CountItem)
                || name.Equals(Predicates.CountNumeric)
                || name.Equals(Predicates.Random)
                || name.Equals(Predicates.Days)
                || name.Equals(Predicates.Hours)
                || name.Equals(Predicates.Minutes)
                || name.Equals(Predicates.Seconds))
            {
                newName = name + Symbols.Underscore;
                return true;
            }

            bool corrected = false;
            char[]  nameChars = name.ToCharArray();
            for (int i = 0; i < nameChars.Length; i++)
            {
                if (Symbols.IsForbidden(nameChars[i])
                    || Char.IsWhiteSpace(nameChars[i])
                    || Char.IsSeparator(nameChars[i]))
                {
                    nameChars[i] = Symbols.Underscore;
                    corrected = true;
                }
            }

            if (corrected)
                newName = new string(nameChars);
            else newName = string.Empty;

            return corrected;
        }

        /// <summary>
        /// Стандартный отступ текста
        /// </summary>
        public static readonly string Indent = "  ";

        public static string MakeIndent(int indent)
        {
            if (indent > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < indent; i++)
                    sb.Append(Indent);
                return sb.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Конец строки
        /// </summary>
        public static readonly string LineEnd = "\r\n";

        /// <summary>
        /// Перечисление типов операторов
        /// </summary>
        public enum MathOperatorType
        {
            NOP, // нет операции (пустое значение)
            Addition, // Сложение
            Substruction, // Вычетание
            Multiplication, // Умножение
            Division, // Деление
            Remainder // Взятие остатка от деления
        }

        /// <summary>
        /// Символов
        /// </summary>
        public class Symbols
        {
            public static readonly char[] forbiddenChar = { ' ', '{', '(', '[', ']', ')', '}', '<', '>',
                                                               '+', '-', '*', '/', '^', '%',
                                                               '№', '#', '@', '!', '~', '$', '&', '|', '\\', '?',
                                                               '.', ',', ':', '\'', '`', '\"', ';', '=' };

            public static bool IsForbidden(char c)
            {
                foreach (char fc in forbiddenChar)
                    if (fc == c)
                        return true;

                return false;
            }

            /// <summary>
            /// Символ табуляции - горизонтальный отступ
            /// </summary>
            public static readonly char Tab = '\t';

            /// <summary>
            /// Проверка символа на принадлежность к буквам английского или русского алфавита
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsLetter(char c)
            {
                return ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')) 
                    || ((c >= 'а') && (c <= 'я')) || (c == 'ё') || ((c >= 'А') && (c <= 'Я')) || (c == 'Ё');
            }

            /// <summary>
            /// Проверка символа на принадлежность к буквам английского или русского алфавита
            /// или к цифрам
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsLetterOrDigit(char c)
            {
                return ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z'))
                    || ((c >= 'а') && (c <= 'я')) || (c == 'ё') || ((c >= 'А') && (c <= 'Я')) || (c == 'Ё')
                    || ((c >= '0') && (c <= '9'));
            }
            /// <summary>
            /// Открывающие скобки
            /// </summary>
            public static readonly char openRoundBrace = '(';
            public static readonly char openGroupBrace = openRoundBrace;
            public static readonly char openSquareBrace = '[';
            public static readonly char openCurlyBrace = '{';
            public static readonly char[] openBraces = { openRoundBrace, openSquareBrace, openCurlyBrace };
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
            /// Проверка символа на совпадения с открывающей квадратной скобкой '['
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsOpenSquareBraces(char c)
            {
                return openBraces.Contains(c);
            }            /// <summary>
            /// Удаляет пустые символы и первую встреченную открывающую скобку в начале строки
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public static bool TrimOpenGroupBracesAndWhiteSpace(ref string expression)
            {
                if (string.IsNullOrEmpty(expression) || string.IsNullOrWhiteSpace(expression))
                    return false;
                bool trimBrace = false;
                int i;
                for(i = 0; i < expression.Length; i++)
                {
                    if (char.IsWhiteSpace(expression[i]))
                        continue;
                    else if (IsOpenGroupBraces(expression[i]) && !trimBrace)
                    {
                        trimBrace = true;// Запоминаем, что скобка уже пропущена
                        continue;
                    }
                    else break;
                }

                if(i > 0 && i < expression.Length + 1 && trimBrace)
                {
                    expression = expression.Remove(0, i);
                    return true;
                }

                return false;
            }
            /// <summary>
            /// Удаляет пустые символы и первую встреченную открывающую квадратну скобку в начале строки
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public static bool TrimOpenSquareBracesAndWhiteSpace(ref string expression)
            {
                if (string.IsNullOrEmpty(expression) || string.IsNullOrWhiteSpace(expression))
                    return false;
                bool trimBrace = false;
                int i;
                for (i = 0; i < expression.Length; i++)
                {
                    if (char.IsWhiteSpace(expression[i]))
                        continue;
                    else if (expression[i] == openSquareBrace && !trimBrace)
                    {
                        trimBrace = true;// Запоминаем, что скобка уже пропущена
                        continue;
                    }
                    else break;
                }

                if (i > 0 && i < expression.Length + 1 && trimBrace)
                {
                    expression = expression.Remove(0, i);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Закрывающие скобки
            /// </summary>
            public static readonly char closeRoundBrace = ')';
            public static readonly char closeGroupBrace = closeRoundBrace;
            public static readonly char closeSquareBrace = ']';
            public static readonly char closeCurlyBrace = '}';
            public static readonly char[] closeBraces = { closeRoundBrace, closeSquareBrace, closeCurlyBrace };
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
            /// Проверка символа на совпадения со скобкой, закрывающей группу
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsCloseGroupBraces(char c)
            {
                return c == closeRoundBrace;
            }
            /// <summary>
            /// Проверка символа на совпадения со закрывающй квадратной скобкой ']'
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsCloseSquareBraces(char c)
            {
                return c == closeSquareBrace;
            }
            /// <summary>
            /// Удаляет пустые символы и первую встреченную закрывающую скобку в начале строки
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public static bool TrimCloseGroupBracesAndWhiteSpace(ref string expression)
            {
                if (string.IsNullOrEmpty(expression) || string.IsNullOrWhiteSpace(expression))
                    return false;
                bool trimBrace = false;
                int i;
                for (i = 0; i < expression.Length; i++)
                {
                    if (char.IsWhiteSpace(expression[i]))
                        continue;
                    else if (IsCloseGroupBraces(expression[i]) && !trimBrace)
                    {
                        trimBrace = true; // запоминаем, что скобка уже встретилась
                        continue;
                    }
                    else break;
                }

                if (i > 0 && i < expression.Length + 1 && trimBrace)
                {
                    expression = expression.Remove(0, i);
                    return true;
                }

                return false;
            }
            /// <summary>
            /// Удаляет пустые символы и первую встреченную закрывающую квадратную скобку в начале строки
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public static bool TrimCloseSquareBracesAndWhiteSpace(ref string expression)
            {
                if (string.IsNullOrEmpty(expression) || string.IsNullOrWhiteSpace(expression))
                    return false;
                bool trimBrace = false;
                int i;
                for (i = 0; i < expression.Length; i++)
                {
                    if (char.IsWhiteSpace(expression[i]))
                        continue;
                    else if (expression[i] == closeSquareBrace && !trimBrace)
                    {
                        trimBrace = true; // запоминаем, что скобка уже встретилась
                        continue;
                    }
                    else break;
                }

                if (i > 0 && i < expression.Length + 1 && trimBrace)
                {
                    expression = expression.Remove(0, i);
                    return true;
                }

                return false;
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
            /// Удаляет пустые символы и первую встреченную запятую ',' в начале строки
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public static bool TrimCommaAndWhiteSpace(ref string expression)
            {
                if (string.IsNullOrEmpty(expression) || string.IsNullOrWhiteSpace(expression))
                    return false;
                bool trimComma = false;
                int i;
                for (i = 0; i < expression.Length; i++)
                {
                    if (char.IsWhiteSpace(expression[i]))
                        continue;
                    else if (expression[i] == ',' && !trimComma)
                    {
                        trimComma = true; // запоминаем, что запятая ',' уже встретилась
                        continue;
                    }
                    else break;
                }

                if (i > 0 && i < expression.Length + 1 && trimComma)
                {
                    expression = expression.Remove(0, i);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Определение типа математического оператора, соответствующего символу
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static MathOperatorType GetMathOperatorType(char c)
            {
                switch(c)
                {
                    case '+':
                        return MathOperatorType.Addition;
                    case '-':
                        return MathOperatorType.Substruction;
                    case '*':
                        return MathOperatorType.Multiplication;
                    case '/':
                        return MathOperatorType.Division;
                    case '%':
                        return MathOperatorType.Remainder;
                    default:
                        return MathOperatorType.NOP;
                }
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
            /// <summary>
            /// Проверка символа на принадлежность аддитивным операторам: '+', '-'
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsAdditionOperator(char c)
            {
                return c == '+' || c == '-';
            }
            /// <summary>
            /// Проверка символа на принадлежность аддитивным операторам: '+', '-'
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static bool IsMultiplicationOperator(char c)
            {
                return c == '*' || c == '/' || c == '%';
            }
            public static bool IsMultiplicationOperator(MathOperatorType c)
            {
                return c == MathOperatorType.Multiplication 
					|| c == MathOperatorType.Division 
					|| c == MathOperatorType.Remainder;
            }            
			/// <summary>
            /// Удаляет пустые символы и  первый встреченный MultiplicationOperator в начале строки
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public static MathOperatorType TrimMultiplicationOperatorAndWhiteSpace(ref string expression)
            {
                if (string.IsNullOrEmpty(expression))
                    return MathOperatorType.NOP;

                char oper = '\u0000'; 
                int i;
                for (i = 0; i < expression.Length; i++)
                {
                    if (char.IsWhiteSpace(expression[i]))
                        continue;
                    else if (IsMultiplicationOperator(expression[i]))
                    {
                        if (oper == '\u0000')
                            oper = expression[i];// сохраняем первый оператор
                        else break; // символ оператора встречается второй раз
                        continue;
                    }
                    else break;
                }

                if (i > 0 && i < expression.Length + 1)
                {
                    expression = expression.Remove(0, i);
                    return GetMathOperatorType(oper);
                }

                return MathOperatorType.NOP;
            }
            /// <summary>
            /// Удаляет пустые символы и первый встреченный AdditionOperator в начале строки
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public static MathOperatorType TrimAdditionOperatorAndWhiteSpace(ref string expression)
            {
                if (string.IsNullOrEmpty(expression))
                    return MathOperatorType.NOP;

                char oper = '\u0000'; 
                int i;
                for (i = 0; i < expression.Length; i++)
                {
                    if (char.IsWhiteSpace(expression[i]))
                        continue;
                    else if (IsAdditionOperator(expression[i]))
                    {
                        if (oper == '\u0000')
                            oper = expression[i];// сохраняем первый оператор
                        else break; // символ оператора встречается второй раз

                        continue;
                    }
                    else break;
                }

                if (i > 0 && i < expression.Length + 1)
                {
                    expression = expression.Remove(0, i);
                    return GetMathOperatorType(oper);
                }

                return MathOperatorType.NOP;
            }

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
        }//class Symbol


        /// <summary>
        /// Предикаты функторов
        /// </summary>
        public class Predicates
        {
            private static readonly string[] predicates;

            static Predicates()
            {
                var fields = typeof(Predicates).GetFields();
                predicates = new string[fields.Length];
                for(int i = 0; i < fields.Length; i++)
                {
                    //if(fields[i].FieldType.Equals(typeof(string)))
                    {
                        predicates[i] = fields[i].Name;
                    }
                }
            }



            /// <summary>
            /// Счетчик Items
            /// </summary>
            public static readonly string CountItem = "ItemCount";

            /// <summary>
            /// Счетчик Numerics
            /// </summary>
            public static readonly string CountNumeric = "NumericCount";

            /// <summary>
            /// Генератор случайных чисел
            /// </summary>
            public static readonly string Random = "Random";

            /// <summary>
            /// Текущее время и дата
            /// </summary>
            public static readonly string Now = "Now";

            /// <summary>
            /// Количество дней
            /// </summary>
            public static readonly string Days = "Days";
#if false
            /// <summary>
            /// Количество месяцев
            /// </summary>
            public static readonly string Months = "Months";
            /// <summary>
            /// Количество лет
            /// </summary>
            public static readonly string Years = "Years"; 
#endif

            /// <summary>
            /// Количество часов
            /// </summary>
            public static readonly string Hours = "Hours";
            /// <summary>
            /// Количество минут
            /// </summary>
            public static readonly string Minutes = "Minutes";
            /// <summary>
            /// Количество секунд
            /// </summary>
            public static readonly string Seconds = "Seconds";
        }//class Predicates


        #region Parsers Нужны ли они ?
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
            {
                result = false;
                return false;
            }
            if (inObj is bool b)
            {
                result = b;
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
            {
                result = false;
                return false;
            }
            if (inObj is bool b)
            {
                result = b;
                return true;
            }
            else return bool.TryParse(inObj.ToString(), out result);
        }

        /// <summary>
        /// Перевод строки в число.
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
        /// Перевод объекта в число.
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(object inObj, out double result)
        {
            if (inObj == null)
            {
                result = 0;
                return false;
            }
            if (inObj is double d)
            {
                result = d;
                return true;
            }
            else
            {
                string str = inObj.ToString();
                return TryParse(Parser.Symbols.CorrectNumberDecimalSeparator(ref str), out result);
            }
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
            {
                result = 0;
                return false;
            }
            if (inObj is double d)
            {
                result = d;
                return true;
            }
            else
            {
                string str = inObj.ToString();
                return double.TryParse(Parser.Symbols.CorrectNumberDecimalSeparator(ref str), out result);
            }
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
            {
                result = DateTime.MinValue;
                return false;
            }
            if (inObj is DateTime dt)
            {
                result = dt;
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
        /// преобразование объекта в AccountScopeType
        /// </summary>
        /// <param name="inObj"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static bool TryParse(object inObj, out AccountScopeType scope)
        {
            scope = AccountScopeType.Global;

            if (inObj == null)
                return false;

            if (inObj is AccountScopeType sc)
            {
                scope = sc;
                return true;
            }

            return Enum.TryParse(inObj.ToString(), out scope);
        }

        /// <summary>
        /// Преобразование объекта в AccountScopeType
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="result">результат преобразования</param>
        /// <returns>Флаг успеха преобразования</returns>
        public static bool TryParse(object inObj, out ProfileScopeType scope)
        {
            scope = ProfileScopeType.Common;

            if (inObj == null)
                return false;

            if (inObj is ProfileScopeType sc)
            {
                scope = sc;
                return true;
            }

            return Enum.TryParse(inObj.ToString(), out scope);
        } 
        #endregion

        /// <summary>
        /// Удаляет все бесполезные пробелы из <see cref="input"> и помещает результат в <see cref="out">
        /// Корректность входной строки не проверяется
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <returns>индек некорректного символа входной строки начиная с 0
        /// возвращает -1, если результат корректен</returns>
        public static int DeleteWhiteSpaces(string input, out string result)
        {
            if(string.IsNullOrEmpty(input))
            {
                result = string.Empty;
                return 0;
            }

            /// Недопустимой является комбирация (\w|.|,|_)\s(\w|.|,|_)
            /// где \w - любой алфавитно-цифровой символ
            ///     \s - любой символ пробела
            /// Другая недопустимая комбинация -
            /// десятичный разделить между нецифровыми символами \D(.|,)\D
            /// где \D - не цифровой символ

            // Буфер, в который копируются значащие символы
            char[] buffer = new char[input.Length];
            // количество скопированных символов (индекс последнего скопированного символа)
            int copiedChars = 0;

            for(int i = 0; i < input.Length; i++)
            {
                // пропускаем пробелы и символы-разделители
                while (i < input.Length
                    && (char.IsWhiteSpace(input[i])
                        || char.IsSeparator(input[i])))
                        i++;
                
                // Копируем числа, переменные или функторы
                while ( i < input.Length
                        && ( Symbols.IsLetterOrDigit(input[i])
                             || Symbols.IsUnderscore(input[i])
                             || Symbols.IsNumberDecimalSeparator(input[i])))
                {
                    buffer[copiedChars] = input[i];
                    i++;
                    copiedChars++;
                }

                // пропускаем пробелы и символы-разделители
                while (i < input.Length
                    && (char.IsWhiteSpace(input[i])
                        || char.IsSeparator(input[i])))
                    i++;

                // Анализируем встреченный символ
                // если встретился алфавитно-цифровой символ, десятичный разделитель или подсеркивание ('_')
                // значит мы встретили комбинацию "(\w|.|,|_)\s(\w|.|,|_)" 
                if ( i < input.Length
                     && (Symbols.IsLetterOrDigit(input[i])
                            || Symbols.IsUnderscore(input[i])
                            || Symbols.IsNumberDecimalSeparator(input[i])))
                {
                    // добавляем в выходную строку пробел
                    buffer[copiedChars] = ' ';
                    copiedChars++;
                    // добавляем в выходную строку найденный символ
                    buffer[copiedChars] = input[i];
                    copiedChars++;
                    continue;
                }

                // Копируем скобки и математические операторы операторы
                if ( i < input.Length
                     && (Symbols.IsMathOperator(input[i])
                         || Symbols.IsBraces(input[i])))
                {
                    buffer[copiedChars] = input[i];
                    copiedChars++;
                    continue;
                }

                if (i < input.Length
                    && (char.IsWhiteSpace(input[i])
                        || char.IsSeparator(input[i])))
                    continue;

                if (i < input.Length)
                {
                    // Если мы добрались сюда, то во входной строке содержатся некорректные символы
                    result = string.Empty;
                    return i;
                }
            }

            if(copiedChars > 0)
            {
                result = new string(buffer, 0, copiedChars);
                return -1;
            }

            result = string.Empty;
            return 0;
        }
    }

    public static class StandartClassExtention
    {
        public static string AddCopies(this string str, string indent, int count = 0)
        {
            if (count > 0)
            {
                StringBuilder sb = new StringBuilder(str);
                for (int i = 0; i < count; i++)
                    sb.Append(indent);
                return sb.ToString();
            }
            return str;
        }

        public static StringBuilder AppendCopies(this StringBuilder sb, string indent, int count = 0)
        {
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                    sb.Append(indent);
                return sb;
            }
            return sb;
        }
    }
}
