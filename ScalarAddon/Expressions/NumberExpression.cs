using AstralVars.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AstralVars.Expressions.Numbers
{
    public class NumberExpresstion : Expression<double>
    {
        public override bool IsValid => !string.IsNullOrEmpty(expression) && root != null;

        public override bool Calcucate(out object result)
        {
            result = null;
            if (string.IsNullOrEmpty(expression))
                return false;

            if (root != null && root.Calculate(out double res))
            {
                result = res;
                return true;
            }

            return false;
        }

        public override ParseStatus Parse()
        {
            ParseStatus status = new ParseStatus();
            if(string.IsNullOrEmpty(expression))
            {
                throw new ParseError("Expression string is empty");
            }



            return status;
        }

        /// <summary>
        /// Извлечение числа из входной строки expression
        /// В случае успеха соответствующая числу подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева ConstantNode</returns>
        public static ConstantNode ParseNumber(ref string expression)
        {
            expression = expression.TrimStart();

            if (string.IsNullOrEmpty(expression))
            {
                throw new ParseError("Expression string is empty");
            }

            // Число десятичных разделителей
            int numDecimalSeparator = 0;
            int i = 0;

            // Поиск подстроки, обозначающей число
            //
            // Поиск с использование регулярных выражений
            //Match match = Regex.Match(expression, @"^\d+((,|.)?\d)?");
            //
            // Поиск путем анализа символов строки
            for (; i < expression.Length;i++)
            {
                if (char.IsDigit(expression[i]))
                {
                    continue;
                }

                if (char.IsWhiteSpace(expression[i]))
                    // Найден провел
                    // Следовательно число закончилось на предыдущем символе
                    break;

                if (Parser.Simbols.IsMathOperator(expression[i]))
                    // Найден математический оператор
                    // Следовательно число закончилось на предыдущем символе
                    break;

                if (Parser.Simbols.IsCloseBraces(expression[i]))
                    // Найдена закрывающая скобка
                    // Следовательно число закончилось на предыдущем символе
                    break;

                if (Parser.Simbols.IsOpenBraces(expression[i]))
                {
                    // Встретилась открывающая скобка
                    // Следовательно входная строка не содержит числа или некорректна
                    throw new ParseError($"Had [OpenBraces] when there should be [Digit] or the end of [Number]", i);
                }

                if (Parser.Simbols.IsNumberDecimalSeparator(expression[i]))
                {
                    if (numDecimalSeparator > 1)
                    {
                        // Десятичный разделитель встретился второй раз, 
                        // Это является недопустимым
                        // Следовательно входная строка не содержит числа или некорректна
                        throw new ParseError($"Had [DecimalSeparator] when there should be [Digit] or the end of [Number]", i);
                    }
                    numDecimalSeparator++;
                    continue;
                }

                if (char.IsLetter(expression[i]))
                {
                    // Встретилась буква
                    // Следовательно входная строка не содержит числа или некорректна
                    throw new ParseError($"Had [Letter] when there should be [Digit] or the end of [Number]", i);
                }
            }

            if(i == 0)
                throw new ParseError($"[Number] does not found at the beginning of the  expression '{expression}'");

            // попытка преобразовать часть строки в число
            double value;
            bool parseSucceeded = false;
            if (numDecimalSeparator > 0)
                 parseSucceeded = double.TryParse(Parser.Simbols.CorrectNumberDecimalSeparator(ref expression).Substring(0, i), out value);
            else parseSucceeded = double.TryParse(expression.Substring(0, i), out value);

            if (parseSucceeded)
            {
                // удаление распознанной подстроки из входного выражения 
                expression = expression.Remove(0, i);
                // конструирование листового узла синтаксического дерева
                return new ConstantNode(value);
            }
            else throw new ParseError($"Parse [Number] unsucceeded from the expression '{expression}'");
        }

        /// <summary>
        /// Извлечение имени переменной из входной строки expression
        /// В случае успеха соответствующая переменной подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева VariableNode</returns>
        public static VariableNode ParseVariable(ref string expression)
        {
            expression = expression.TrimStart();

            if (string.IsNullOrEmpty(expression))
            {
                throw new ParseError("Expression string is empty");
            }

            // Индекс символа во входной строке
            int i = 0;

            // Поиск путем анализа символов строки
            for (; i < expression.Length; i++)
            {
                if (char.IsLetterOrDigit(expression[i]))
                    continue;

                if (Parser.Simbols.IsUnderscore(expression[i]))
                    continue;

                if (Parser.Simbols.IsMathOperator(expression[i]))
                    // Обнаружен математический оператор 
                    // Следовательно Имя переменной закончилось на предыдущем символе
                    break;

                if (char.IsWhiteSpace(expression[i]))
                    // Обнаружен пробел
                    // Следовательно Имя переменной закончилось на предыдущем символе
                    break;

                if (Parser.Simbols.IsCloseBraces(expression[i]))
                    // Найдена закрывающая скобка
                    // Следовательно Имя переменной закончилось на предыдущем символе
                    break;

                if (Parser.Simbols.IsOpenBraces(expression[i]))
                {
                    // Встретилась открывающая скобка
                    // Следовательно входная строка некорректна
                    throw new ParseError($"Had [OpenBraces] when there should be end of [Variable_Name]", i);
                }
            }

            if (i == 0)
                throw new ParseError($"[Variable_Name] does not found at the beginning of the expression '{expression}'");

            // Извлечение названия переменной из подстроки
            string var_name = expression.Substring(0, i);
            if (!string.IsNullOrEmpty(var_name))
            {
                // удаление распознанной подстроки из входного выражения 
                expression = expression.Remove(0, i);
                // конструирование листового узла синтаксического дерева
                return new VariableNode(var_name);
            }
            else throw new ParseError($"Parse [Variable_Name] unsucceeded from the expression '{expression}'");
        }

        /// <summary>
        /// Извлечение идентификатора предмета (ItemId) из входной строки expression
        /// В случае успеха соответствующая идентификатору предмета подстрока (ItemId) удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Идентификатор предмета ItemId</returns>
        private static string ParseItemId(ref string expression)
        {
            expression = expression.TrimStart();

            if (string.IsNullOrEmpty(expression))
            {
                throw new ParseError("Expression string is empty");
            }

            // Индекс символа во входной строке
            int i = 0;

            // Поиск путем анализа символов строки
            for (; i < expression.Length; i++)
            {
                if (char.IsLetterOrDigit(expression[i]))
                    continue;

                if (Parser.Simbols.IsUnderscore(expression[i]))
                    continue;

                //if (Parser.IsWildcard(expression[i]))
                //    continue;

                if (char.IsWhiteSpace(expression[i]))
                    // Найден пробел
                    // Следовательно  ItemId закончился на предыдущем символе
                    break;

                if (Parser.Simbols.IsCloseBraces(expression[i]))
                    // Найдена закрывающая скобка
                    // Следовательно ItemId закончился на предыдущем символе
                    break;

                if (Parser.Simbols.IsOpenBraces(expression[i]))
                {
                    // Найдена открывающая скобка
                    // Следовательно входная строка некорректна
                    throw new ParseError($"Had [OpenBraces] when there should be the end of [ItemId]", i);
                }

                if (Parser.Simbols.IsMathOperator(expression[i]))
                {
                    // Найден символ математического оператора
                    // Математический операции над идентификаторами не допускаются
                    // Следовательно входная строка некорректна
                    throw new ParseError($"Had [MathOperator] when there should be the end of [ItemId]", i);
                }
            }

            if (i == 0)
                throw new ParseError($"[ItemId] does not found at the beginning of the expression '{expression}'");

            // Извлечение названия переменной из подстроки
            string itemId = expression.Substring(0, i);
            if (!string.IsNullOrEmpty(itemId))
            {
                // удаление распознанной подстроки из входного выражения 
                expression = expression.Remove(0, i);
                // конструирование листового узла синтаксического дерева
                return itemId;
            }
            else throw new ParseError($"Parse [ItemId] unsucceeded from the expression '{expression}'");
        }


        /// <summary>
        /// Извлечение функции 'Counter' из входной строки expression
        /// В случае успеха соответствующая функции подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева CounterNode</returns>
        public static CounterNode ParseCounter(ref string expression)
        {
            expression = expression.TrimStart();

            if (string.IsNullOrEmpty(expression))
            {
                throw new ParseError("Expression string is empty");
            }

            if (expression.StartsWith(Parser.Predicates.Counter, StringComparison.OrdinalIgnoreCase))
            {
                // Обнаружен предикативны литерал Counter
                int i = Parser.Predicates.Counter.Length;
                int openBraceInd = 0;
                int openRoundBrace = 0;
                int closeRoundBrace = 0;
                //int RoundBracePair = 0;

                //int openSquereBrace = 0;
                //int closeSquereBrace = 0;
                //int SquereBracePair = 0;

                //int openCurlyBraceNum = 0;
                //int closeCurlyBraceNum = 0;
                //int CurlyBraceNumPair = 0;

                // число значащих символов аргумента
                int idCharNumber = 0;

                // При установленном флаге единственным допустимыми символами будут являться пробел или финальная закрывающая скобка
                bool possibleParseErrorFlag = false;

                // Производим поиск первой открывающей и финальной закрывающей скобок
                for (; i < expression.Length; i++)
                {
                    if (Parser.Simbols.IsOpenGroupBraces(expression[i]) // '('
                        && !possibleParseErrorFlag) 
                    {
                        if (openRoundBrace == 0)
                            // Запоминаем индекс первой открывающей скобки
                            openBraceInd = i;
                        openRoundBrace++;
                    }
                    else if (Parser.Simbols.IsCloseGroupBraces(expression[i])) // ')'
                        closeRoundBrace++;
                    // Проверяем является ли текущий символ алфавитно-цифровым, подчеркиванием или знаком подстановки
                    else if ((char.IsLetterOrDigit(expression[i]) || Parser.Simbols.IsUnderscore(expression[i]) || Parser.IsWildcard(expression[i])) 
                            && !possibleParseErrorFlag)
                    {
                        idCharNumber++;
                    }
                    else if (char.IsWhiteSpace(expression[i]))
                    {
                        // Если число значимых симвлов аргумента больше 0,
                        // следовательно обнаруженный пробел находится в середине строки-аргумента
                        // или стоит перед финальной закрывающей скобкой
                        if (idCharNumber > 0)
                            possibleParseErrorFlag = true;
                    }
                    else
                    {
                        if (possibleParseErrorFlag)
                            throw new ParseError($"Had '{expression[i]}' when there should be '{Parser.Simbols.closeRoundBrace}'", i);
                        idCharNumber++;
                    }

                    // Сравниваем количество открывающих и закрывающих скобок
                    if (openRoundBrace > 0 && openRoundBrace == closeRoundBrace)
                    {
                        // Найдена финальная закрывающая скобка для списка аргументов Counter

                        // Извлекаем строку аргументов
                        string arg = expression.Substring(openBraceInd + 1, i - openBraceInd - 1);

                        // Удалени распознанной подстроки из expression
                        expression = expression.Remove(0, i);

                        // формирование узла синтаксического дерева
                        return new CounterNode(arg);
                    }

                    if (openRoundBrace < closeRoundBrace)
                    {
                        // Число открывающих скобок меньше числа закрывающих
                        // Следовательно входнаяая строка некорректна
                        throw new ParseError("Had [CloseRoundBrace] when there should be [ItemId]", i);
                    }
                }

                // Неожиданное окончание входной строки
                throw new ParseError($"Parse [{Parser.Predicates.Counter}] unsucceeded. Open braces is {openRoundBrace}. Close Braces is {closeRoundBrace}. Unexpected end of exprssion '{expression}'");

            }
            else throw new ParseError($"Predicate [{Parser.Predicates.Counter}] does not found at the beginning of the  expression '{expression}'");
        }

        private static ParseStatus ParseFunction(ref string expression)
        {
            ParseStatus status = new ParseStatus();
            if (string.IsNullOrEmpty(expression))
            {
                throw new ParseError("Expression string is empty");
            }

            throw new ParseError("Unexpected end of method 'ParseFunction'");
        }

        private static ParseStatus ParseMultiDiv(ref string expression)
        {
            ParseStatus status = new ParseStatus();
            if (string.IsNullOrEmpty(expression))
            {
                throw new ParseError("Expression string is empty");
            }

            throw new ParseError("Unexpected end of method 'ParseMultiDiv'");
        }

        private static ParseStatus ParseAddDiv(ref string expression)
        {
            ParseStatus status = new ParseStatus();
            if (string.IsNullOrEmpty(expression))
            {
                throw new ParseError("Expression string is empty");
            }

            throw new ParseError("Unexpected end of method 'ParseAddDiv'");
        }

    }
}
