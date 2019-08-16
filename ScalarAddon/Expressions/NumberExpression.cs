using AstralVariables.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static AstralVariables.Expressions.Parser;
using NumberAstNode = AstralVariables.Expressions.AstNode<double>;
using ErrorList = System.Collections.Generic.List<AstralVariables.Expressions.BaseParseError>;
using AstralVariables.Expressions.Operand;
using AstralVariables.Expressions.Functions;
using AstralVariables.Expressions.Operators;
using System.Diagnostics;

namespace AstralVariables.Expressions
{
    public class NumberExpression : Expression<double>
    {
        public override bool IsValid => !string.IsNullOrEmpty(Expression) && !string.IsNullOrWhiteSpace(Expression) && (ast != null);

        public override bool Calcucate(out double result)
        {
            result = 0;

            if (string.IsNullOrEmpty(Expression) || string.IsNullOrWhiteSpace(Expression))
                return false;

            if (ast == null)
                Parse();
#if DEBUG
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            if (ast != null && ast.Calculate(out double res))
            {
                result = res;
#if DEBUG
                stopwatch.Stop();
                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesAddon)}: Calculate '{_expression}' time: {stopwatch.ElapsedMilliseconds} ms");
#endif
                return true;
            }
#if DEBUG
            stopwatch.Stop();
            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesAddon)}: Calculate '{_expression}' time: {stopwatch.ElapsedMilliseconds} ms");
#endif
            return false;
        }

        /// <summary>
        /// Синтаксический разбор выражения expression 
        /// и построение Абстрактного синтаксического дерева (AST)
        /// </summary>
        /// <param name="newExpression">Новое значение Expression. Если не задано - производится разбор прежнего значения Expression</param>
        /// <returns></returns>
        public override bool Parse(string newExpression = "")
        {
#if DEBUG
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            if (!string.IsNullOrEmpty(newExpression) && !string.IsNullOrWhiteSpace(newExpression))
                Expression = newExpression;
            else if (string.IsNullOrEmpty(Expression) || string.IsNullOrWhiteSpace(Expression))
            {
                ast = null;
                parseError = new FatalParseError("Parse: Expression string is empty");
                return false;
            }

            string expr = Expression;
            ast = null;
            try
            {
                ast = ParseAddition(ref expr);
            }
            catch(BaseParseError e)
            {
                parseError = e;
                return false;
            }
#if DEBUG
            stopwatch.Stop();
            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesAddon)}: Parse '{_expression}' time: {stopwatch.ElapsedMilliseconds} ms");
#endif
            if (!string.IsNullOrWhiteSpace(expr) && !string.IsNullOrEmpty(Expression))
            {
                expr = expr.TrimStart();
                parseError = new FatalParseError($"Parse: Undefined symbol at position {Expression.Length - expr?.Length}" , expr);
                return false;
            }

            return ast != null;
        }

        /// <summary>
        /// Извлечение 'числовой константы' из входной строки expression
        /// В случае успеха соответствующая числу подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева NumberNode</returns>
        public static NumberConstant ParseNumber(ref string expression)
        {
            string exprBackup = expression;
            expression = expression.TrimStart();

            if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                throw new FatalParseError("ParseNumber: Expression string is empty");

            try
            {
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

                    if (Parser.Symbols.IsMathOperator(expression[i]))
                        // Найден математический оператор
                        // Следовательно число закончилось на предыдущем символе
                        break;


                    if (Parser.Symbols.IsCloseBraces(expression[i]))
                        // Найдена закрывающая скобка
                        // Следовательно число закончилось на предыдущем символе
                        break;


                    if (Parser.Symbols.IsNumberDecimalSeparator(expression[i]))
                    {
                        if (numDecimalSeparator > 1)
                        {
                            // Десятичный разделитель встретился второй раз, 
                            // Это является недопустимым
                            // Следовательно входная строка не содержит числа или некорректна
                            throw new FatalParseError($"ParseNumber: Have '{expression[i]}' when there should be 'Digit' or the end of {{Number}}", expression.Substring(i));
                        }
                        numDecimalSeparator++;
                        continue;
                    }

                    if (Parser.Symbols.IsUnderscore(expression[i])
                        || Parser.Symbols.IsLetter(expression[i]))
                    {
                        // Встретился символ подчеркивания '_'
                        // или буква или любой символ, не входящий в числовой литерал
                        // Следовательно входная строка не содержит числа
                        throw new ParseError($"ParseNumber: Have '{expression[i]}' when there should be 'Digit' or the end of {{Number}}", expression.Substring(i));
                    }

                    // Встретился недопустимый символ 
                    throw new FatalParseError($"ParseNumber: Have forbidden symbol '{expression[i]}' when there should be 'Digit' or the end of {{Number}}", expression.Substring(i));
                }

                if (i == 0)
                    throw new ParseError($"ParseNumber: {{Number}} does not found at the beginning of the expression", expression);

                // попытка преобразовать часть строки в число
                double value;
                bool parseSucceeded = false;
                if (numDecimalSeparator > 0)
                     parseSucceeded = double.TryParse(Parser.Symbols.CorrectNumberDecimalSeparator(ref expression).Substring(0, i), out value);
                else parseSucceeded = double.TryParse(expression.Substring(0, i), out value);

                if (parseSucceeded)
                {
                    // удаление распознанной подстроки из входного выражения 
                    expression = expression.Remove(0, i);
                    // конструирование листового узла синтаксического дерева
                    return new NumberConstant(value);
                }
                else throw new ParseError("ParseNumber: Unsucceeded", expression);
            }
            catch (Exception e)
            {
                expression = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Извлечение имени переменной из входной строки expression
        /// В случае успеха соответствующая переменной подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева NumberVariable</returns>
        public static NumberVariable ParseVariable(ref string expression)
        {
            string exprBackup = expression;

            if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                throw new FatalParseError("ParseVariable: Expression string is empty");

            expression = expression.TrimStart();

            try
            {
                // Индекс символа во входной строке
                int i;
                // Поиск путем анализа символов строки
                for (i = 0; i < expression.Length; i++)
                {
                    if (Parser.Symbols.IsLetterOrDigit(expression[i]))
                        continue;

                    if (Parser.Symbols.IsUnderscore(expression[i]))
                        continue;

                    if (Parser.Symbols.IsMathOperator(expression[i]))
                        // Обнаружен математический оператор 
                        // Следовательно Имя переменной закончилось на предыдущем символе
                        break;                    

                    if (char.IsWhiteSpace(expression[i]))
                        // Обнаружен пробел
                        // Следовательно Имя переменной закончилось на предыдущем символе
                        break;

                    if (Parser.Symbols.IsCloseGroupBraces(expression[i]))
                        // Найдена закрывающая скобка
                        // Следовательно Имя переменной закончилось на предыдущем символе
                        break;

                    if (Parser.Symbols.IsOpenGroupBraces(expression[i]))
                    {
                        // Встретилась открывающая скобка
                        // Следовательно входная строка некорректна
                        throw new FatalParseError($"ParseVariable: Unexpeted symbol '{expression[i]}'", expression.Substring(i));
                    }

                    // Встретился недопустимый символ
                    // Следовательно входная строка некорректна
                    throw new FatalParseError($"ParseVariable: Symbol '{expression[i]}' is forbiden in the name of {{Variable}}", expression.Substring(i));
                }

                if (i == 0)
                    throw new ParseError("ParseVariable: {Variable} does not found at the beginning of the expression", expression.Substring(i));

                // Извлечение названия переменной из подстроки
                string var_name = expression.Substring(0, i);
                if (Parser.IsForbidden(var_name))
                    throw new FatalParseError($"ParseVariable: Name {{{var_name}}} if forbidden", expression);

                if (!string.IsNullOrWhiteSpace(var_name) && !string.IsNullOrEmpty(var_name))
                {
                    // удаление распознанной подстроки из входного выражения 
                    expression = expression.Remove(0, i);
                    // конструирование листового узла синтаксического дерева
                    return new NumberVariable(var_name);
                }
                else throw new ParseError("ParseVariable: Unsucceeded", expression.Substring(i));
            }
            catch(Exception e)
            {
                expression = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Обработка правила
        /// Value = Number
        ///       | Variable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static NumberAstNode ParseValue(ref string expression)
        {
            string exprBackup = expression;
            //StringBuilder errorMess = new StringBuilder();
            ErrorList subError = new ErrorList();

            if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                throw new FatalParseError("ParseValue: Expression string is empty");

            try
            {
                try
                {
                    // Пытаетмся извлечь числовую константу Value = Number
                    return ParseNumber(ref expression);
                }
                catch (ParseError numErr)
                {
                    //errorMess.AppendLine(numErr.Message)/*.Append("in expression: ").Append(expression*/;
                    subError.Add(numErr);

                    expression = exprBackup;
                    // Пытаемся извлечь переменную Value = Variable
                    try
                    {
                        return ParseVariable(ref expression);
                    }
                    catch (ParseError varErr)
                    {
                        //errorMess.AppendLine(varErr.Message)/*.Append("in expression: ").Append(expression)*/;
                        subError.Add(varErr);

                        expression = exprBackup;
                    }
                }
            }
            catch(FatalParseError fatErr)
            {
                expression = exprBackup;
                subError.Add(fatErr);
                throw new FatalParseError(subError, "ParseValue: Fatal error", fatErr.expression);
            }

            expression = exprBackup;
            //errorMess.Insert(0, "ParseValue: No {Number} or {Variable} was found\n");
            //throw new ParseError(subError, errorMess.ToString(), expression);
            throw new ParseError(subError, "ParseValue: No {Number} or {Variable} was found", expression);
        }


        /// <summary>
        /// Извлечение идентификатора предмета (ItemId) из входной строки expression
        /// В случае успеха соответствующая идентификатору предмета подстрока (ItemId) удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Идентификатор предмета ItemId</returns>
        public static string ParseId(ref string expression)
        {

            if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                throw new FatalParseError("ParseItemId: Expression string is empty");

            expression = expression.TrimStart();

            // Индекс символа во входной строке
            int i = 0;

            // Поиск путем анализа символов строки
            for (; i < expression.Length; i++)
            {
                if (Parser.Symbols.IsLetterOrDigit(expression[i]))
                    continue;

                if (Parser.Symbols.IsUnderscore(expression[i]))
                    continue;

                //if (Parser.IsWildcard(expression[i]))
                //    continue;

                if (char.IsWhiteSpace(expression[i]))
                    // Найден пробел
                    // Следовательно  ItemId закончился на предыдущем символе
                    break;

                if (Parser.Symbols.IsCloseBraces(expression[i]))
                    // Найдена закрывающая скобка
                    // Следовательно ItemId закончился на предыдущем символе
                    break;

                if (Parser.Symbols.IsOpenBraces(expression[i]))
                {
                    // Найдена открывающая скобка
                    // Следовательно входная строка некорректна
                    throw new FatalParseError($"ParseItemId: Have '{expression[i]}' when there should be the end of {{ItemId}}", expression.Substring(i));
                }

                if (Parser.Symbols.IsMathOperator(expression[i]))
                {
                    // Найден символ математического оператора
                    // Математический операции над идентификаторами не допускаются
                    // Следовательно входная строка некорректна
                    throw new FatalParseError("ParseItemId: Have {MathOperator} when there should be the end of {ItemId}", expression.Substring(i));
                }

                throw new FatalParseError($"ParseItemId: Have '{expression[i]}' when there should be {{ItemId}}", expression.Substring(i));
            }

            if (i == 0)
                throw new ParseError("ParseItemId: {ItemId} does not found at the beginning of the expression", expression);

            // Извлечение названия переменной из подстроки
            string itemId = expression.Substring(0, i);
            if (!string.IsNullOrWhiteSpace(itemId) && !string.IsNullOrEmpty(itemId))
            {
                // удаление распознанной подстроки из входного выражения 
                expression = expression.Remove(0, i);
                // конструирование листового узла синтаксического дерева
                return itemId;
            }
            else throw new ParseError("ParseItemId: Unsucceeded", expression);
        }

        /// <summary>
        /// Обработка правила Addition
        /// Addition = Multiplication
        ///     | Multiplication ('+' | '-') Addition
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static NumberAstNode ParseAddition(ref string expression)
        {
            string exprBackup = expression;
            ErrorList subError = new ErrorList();
            //StringBuilder errorMess = new StringBuilder();

            if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                throw new FatalParseError("ParseAddition: Expression string is empty");

            try
            {
                NumberAstNode leftOperand = ParseMultiplication(ref expression);
                
				MathOperatorType operatorType = Parser.Symbols.TrimAdditionOperatorAndWhiteSpace(ref expression);
				if(operatorType == MathOperatorType.NOP)
				{
                    // Математический оператор не найден
                    // Следовательно строка 
                    // содержит единственный операнд - leftOperand
                    /*// или является некорректной (если после leftOperand имеются другие значащие символы)
                    if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))*/
                        return leftOperand;
                    /*else throw new ParseError("ParseAddition: Unexpected literal after {LeftOperand}", expression);*/
				}
				else
				{
					// Найден Математический оператор
					// Пробуем извлечь правый операнд
					NumberAstNode rightOperand = ParseAddition(ref expression);
					
					///
					/// Конструируем узел дерева
					///
					return CreateOperatorNode(operatorType, leftOperand, rightOperand);
				}				

            }
            catch (FatalParseError fatErr)
            {
                expression = exprBackup;
                subError.Add(fatErr);
                throw new FatalParseError(subError, "ParseAddition: Fatal error", fatErr.expression);
            }
            catch (Exception e)
            {
                expression = exprBackup;
                throw e;
            }

            throw new FatalParseError("ParseAddition: Unexpected end of method");
        }

        /// <summary>
        /// Разбор строки соответствующей '(' Addition ')' 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
		public static NumberAstNode ParseBracketedAddition(ref string expression)
		{
            string exprBackup = expression;

			if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                throw new FatalParseError("ParseBracketedAddition: Expression string is empty");

            try
            {
                // Ищем открывающу скобку '('
                if (Parser.Symbols.TrimOpenGroupBracesAndWhiteSpace(ref expression))
                {
                    NumberAstNode addNode = null;
                    // Нашли и удалили открывающую скобку '('
                    try
                    {
                        // Извлекаем Addition
                        addNode = ParseAddition(ref expression);
                    }
                    catch (ParseError e)
                    {
                        // После открывающей скобки не найдено выражение, соответствующее правилу Addition 
                        // Следовательно входная строка не соответствует выражению '(' Addition ')' 
                        // Восстанавливаем входную строку 
                        expression = exprBackup;
                        throw e;
                    }

                    // Ищем и удаляем закрывающую скобку ')' после Addition
                    if (Parser.Symbols.TrimCloseGroupBracesAndWhiteSpace(ref expression))
                        return addNode;
                    else
                    {
                        // Закрывающая скобка не найдена
                        // следовательно входная строка не соответствует '(' Addition ')'
                        throw new FatalParseError($"ParseBracketedAddition: not found '{Parser.Symbols.closeGroupBrace}'", expression);
                    }
                }
                else
                {
                    // Открывающая '(' скобка не найдена
                    // следовательно входная строка не соответствует '(' Addition ')'
                    throw new ParseError($"ParseBracketedAddition: not found '{Parser.Symbols.openGroupBrace}'", expression);
                }
            }
            catch(ParseError e)
            {
                // Восстанавливаем входную строку 
                expression = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Разбор строки по правилу
        /// Multiplication = Multiplication ('*' | '/' | '%') '(' Addition ')'
        ///                | Multiplication ('*' | '/' | '%') Value
        ///                | Multiplication ('*' | '/' | '%') Function  
        ///				   | '(' Addition ')' 
        ///                | Function
        ///                | Value
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static NumberAstNode ParseMultiplication(ref string expression)
        {
            return ParseMultiplication(ref expression, null);
        }
        public static NumberAstNode ParseMultiplication(ref string expression, NumberAstNode left)
        {
            string exprBackup = expression;
			NumberAstNode leftOperand = left;
            ErrorList subError = new ErrorList();
            //StringBuilder errorMess = new StringBuilder();

            expression = expression.TrimStart();

            try
            {
                if(leftOperand == null)
				{
					// Левый операнд не был задан при вызове метода
					if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                        throw new FatalParseError("ParseMultiplication: Expression string is empty");

                    // Пробуем извлечь левый операнд вида '(' Addition ')' 
                    try
                    {
                        try
                        {
                            // Извлекаем '(' Addition ')' 
                            leftOperand = ParseBracketedAddition(ref expression);
                        }
                        catch (ParseError brAdditionErr)
                        {
                            //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/brAdditionErr.Message)/*.Append(" in expression: ").Append(expression)*/;
                            subError.Add(brAdditionErr);

                            // Входная строка не содержит выражения, соответствующего '(' Addition ')' 
                            // Восстанавливаем входную строку 
                            expression = exprBackup;

                            // Пробуем извлечь левый операнд типа Function
                            try
                            {
                                //  Пробуем извлечь левый операнд вида Function
                                leftOperand = ParseFunction(ref expression);
                            }
                            catch (ParseError funErr)
                            {
                                //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/funErr.Message)/*.Append(" in expression: ").Append(expression)*/;
                                subError.Add(funErr);

                                // Входная строка не содержит выражения, соответствующего Function 
                                // Восстанавливаем входную строку 
                                expression = exprBackup;

                                // Извлекаем Value
                                try
                                {
                                    leftOperand = ParseValue(ref expression);
                                }
                                catch (ParseError valErr)
                                {
                                    //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/valErr.Message)/*.Append(" in expression: ").Append(expression)*/;
                                    subError.Add(valErr);

                                    // Входная строка не содержит выражения, соответствующего Value 
                                    // Восстанавливаем входную строку 
                                    //errorMess.Insert(0, "ParseMultiplication: No {BracketedAddition}, {Function} or {Value} was found where should be {LeftOperand}:\n");
                                    //throw new ParseError(subErrors, errorMess.ToString(), expression);
                                    throw new ParseError(subError, "ParseMultiplication: No {BracketedAddition}, {Function} or {Value} was found where should be {LeftOperand}", expression);
                                }
                            }
                        }
                    }
                    catch (FatalParseError fatErr)
                    {
                        expression = exprBackup;
                        subError.Add(fatErr);
                        throw new FatalParseError(subError, "ParseMultiplication: Fatal error", fatErr.expression);
                    }


                    if (leftOperand == null)
                    {
                        expression = exprBackup;
                        //errorMess.Insert(0, "ParseMultiplication: {LeftOperand} not found:\n");
                        //throw new ParseError(errorMess.ToString(), expression);
                        throw new ParseError(subError, "ParseMultiplication: {LeftOperand} not found", expression);
                    }
					
                    ///
                    /// Проверяем правую часть правила Multiplication
                    ///		
					if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                        return leftOperand;
					else return ParseMultiplication(ref expression, leftOperand);					
				}
				else
				{
					///
					/// Проверяем правую часть правила Multiplication 
					///
					/// Извлекаем математический оператор
					///
					MathOperatorType operatorType = Parser.Symbols.TrimMultiplicationOperatorAndWhiteSpace(ref expression);
					if(operatorType != MathOperatorType.NOP)
					{
						// Найден математический оператор
						// Сохраняем резервную копию входной строки без математического оператора
						string exprBackup2 = expression;
						
						NumberAstNode rightOperand = null;

                        try
                        {
                            try
                            {
                                // Пробуем извлечь второй опренад вида '(' Addition ')' 
                                rightOperand = ParseBracketedAddition(ref expression);
                            }
                            catch (ParseError brAddErr)
                            {
                                // Входная строка не содержит выражения, соответствующего '(' Addition ')'
                                //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/brAddErr.Message);
                                subError.Add(brAddErr);

                                // Пробуем извлеч второй операнд типа Function
                                try
                                {
                                    rightOperand = ParseFunction(ref expression);
                                }
                                catch (ParseError funErr)
                                {
                                    // Входная строка не содержит выражения, соответствующего Value
                                    //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/funErr.Message);
                                    subError.Add(funErr);

                                    // Восстанавливаем входную строку
                                    expression = exprBackup2;

                                    // Пробуем извлеч второй операнд типа Value
                                    try
                                    {
                                        rightOperand = ParseValue(ref expression);
                                    }
                                    catch (ParseError valErr)
                                    {
                                        subError.Add(valErr);
                                        //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/valErr.Message);
                                        //errorMess.Insert(0, "ParseMultiplication: No {BracketedAddition}, {Function} or {Value} was found where should be {RightOperand}:\n");
                                        //throw new ParseError(errorMess.ToString(), expression);
                                        throw new ParseError(subError, "ParseMultiplication: No {BracketedAddition}, {Function} or {Value} was found where should be {RightOperand}", expression);
                                    }
                                }
                            }
                        }
                        catch (FatalParseError fatErr)
                        {
                            expression = exprBackup;
                            subError.Add(fatErr);
                            throw new FatalParseError(subError, "ParseMultiplication: Fatal error", fatErr.expression);
                        }

                        ///
                        /// Конструируем узел дерева и 
                        /// Помещаем его в качестве левого операнда
                        ///
                        if (rightOperand == null)
                        {
                            //errorMess.Insert(0, "ParseMultiplication: {RightOperand} not found:\n");
                            //throw new ParseError(errorMess.ToString(), expression);
                            throw new FatalParseError(subError, "ParseMultiplication: {RightOperand} not found", expression);
                        }
					
						leftOperand = CreateOperatorNode(operatorType, leftOperand, rightOperand);
						return ParseMultiplication(ref expression, leftOperand);
					}
                    else
                    {
                        /*// При наличии левого оператора входная строка должна содержать 
                        // математический оператор или должна быть пустой
                        // Никакие иные литералы в этом случае не допускаются
                        if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))*/
                            return leftOperand;
                        /*else
                        {
                            errorMess.Insert(0, "ParseMultiplication: Unexpected literal after {LeftOperand}:\n");
                            throw new ParseError(errorMess.ToString(), expression);
                        }*/
                    }
                }	
			}
            catch (Exception e)
			{
				// Восстанавливаем входную строку
				expression = exprBackup;
				throw e;
			}
        }

        /// <summary>
        /// Обработка правила Function
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева NumberAstNode</returns>
        public static NumberAstNode ParseFunction(ref string expression)
        {
            string exprBackup = expression;
            ErrorList subErrors = new ErrorList();
            
            try
            { 
                try
                {
                    return ParseItemCount(ref expression);
                }
                catch (ParseError itemCntErr)
                {
                    expression = exprBackup;
                    subErrors.Add(itemCntErr);
                    //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/itemCntErr.Message);

                    try
                    {
                        return ParseNumericCount(ref expression);
                    }
                    catch (ParseError numCntErr)
                    {
                        expression = exprBackup;
                        subErrors.Add(numCntErr);
                        //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/numCntErr.Message);
                        try
                        {
                            return ParseRandom(ref expression);
                        }
                        catch (ParseError rndErr)
                        {
                            expression = exprBackup;
                            subErrors.Add(rndErr);
                            //errorMess.AppendLine(/*).Append(Parser.Symbols.Tab).Append(*/rndErr.Message);
                        }
                    }
                }
            }
            catch (FatalParseError fatErr)
            {
                expression = exprBackup;
                subErrors.Add(fatErr);
                throw new FatalParseError(subErrors, "ParseMultiplication: Fatal error", fatErr.expression);
            }

            expression = exprBackup;
            
            //errorMess.Insert(0, "ParseFunction: No {ItemCount}, {NumericCount} or {Random} functor was found:\n");
            //throw new ParseError(errorMess.ToString(), expression);
            
            throw new ParseError(subErrors, "ParseFunction: No {ItemCount}, {NumericCount} or {Random} functor was found", expression);
        }

        /// <summary>
        /// Извлечение функции 'CountItem' из входной строки expression
        /// В случае успеха соответствующая функции подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева ItemCount</returns>
        public static ItemCount ParseItemCount(ref string expression)
        {
            string exprBackup = expression;

            try
            {
                if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                {
                    throw new FatalParseError("ParseItemCount: Expression string is empty");
                }

                expression = expression.TrimStart();

                if (expression.StartsWith(Parser.Predicates.CountItem, StringComparison.OrdinalIgnoreCase))
                {
                    expression = expression.Substring(Parser.Predicates.CountItem.Length);

                    if(Parser.Symbols.TrimOpenGroupBracesAndWhiteSpace(ref expression))
                    {
                        try
                        {
                            string id = ParseId(ref expression);

                            if (Parser.Symbols.TrimCloseGroupBracesAndWhiteSpace(ref expression))
                                return new ItemCount(id);
                            else throw new FatalParseError($"ParseItemCount: '{Parser.Symbols.closeGroupBrace}' does not found after {{ItemId}}", expression);
                        }
                        catch(BaseParseError idErr)
                        {
                            throw new FatalParseError(new ErrorList {idErr}, $"ParseItemCount: {{ItemId}} does not found after '{Parser.Symbols.openGroupBrace}'", expression);
                        }
                    }
                    else throw new FatalParseError($"ParseItemCount: '{Parser.Symbols.openGroupBrace}' does not found after {{{Parser.Predicates.CountItem}}} predicate", expression);
                }
                else throw new ParseError($"ParseItemCount: Predicate {{{Parser.Predicates.CountItem}}} does not found at the beginning of the expression", expression);
            }
            catch (Exception e)
            {
                expression = exprBackup;
                throw e;
            }
        }
        public static ItemCount ParseRegexItemCount(ref string expression)
        {
            string exprBackup = expression;

            try
            {
                if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                {
                    throw new FatalParseError("Expression string is empty");
                }

                expression = expression.TrimStart();

                if (expression.StartsWith(Parser.Predicates.CountItem, StringComparison.OrdinalIgnoreCase))
                {
                    // Обнаружен предикативны литерал CountItem
                    int i = Parser.Predicates.CountItem.Length;
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
                        if (Parser.Symbols.IsOpenGroupBraces(expression[i]) // '('
                            && !possibleParseErrorFlag)
                        {
                            if (openRoundBrace == 0)
                                // Запоминаем индекс первой открывающей скобки
                                openBraceInd = i;
                            openRoundBrace++;
                        }
                        else if (Parser.Symbols.IsCloseGroupBraces(expression[i])) // ')'
                            closeRoundBrace++;
                        // Проверяем является ли текущий символ алфавитно-цифровым, подчеркиванием или знаком подстановки
                        else if ((Parser.Symbols.IsLetterOrDigit(expression[i]) || Parser.Symbols.IsUnderscore(expression[i]) || Parser.Symbols.IsWildcard(expression[i]))
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
                                throw new FatalParseError($"ParseItemCount: Have '{expression[i]}' when there should be '{Parser.Symbols.closeRoundBrace}'", expression);
                            idCharNumber++;
                        }

                        // Сравниваем количество открывающих и закрывающих скобок
                        if (openRoundBrace > 0 && openRoundBrace == closeRoundBrace)
                        {
                            // Найдена финальная закрывающая скобка для списка аргументов Counter

                            if ((i - openBraceInd - 1) < 1)
                                throw new FatalParseError("ParseItemCount: {ItemId} is Empty string", expression.Substring(i));

                            // Извлекаем строку аргументов
                            string arg = expression.Substring(openBraceInd + 1, i - openBraceInd - 1);

                            // Удалени распознанной подстроки из expression
                            expression = expression.Remove(0, i + 1);

                            // формирование узла синтаксического дерева
                            return new ItemCount(arg);
                        }

                        if (openRoundBrace < closeRoundBrace)
                        {
                            // Число открывающих скобок меньше числа закрывающих
                            // Следовательно входнаяая строка некорректна
                            throw new FatalParseError($"ParseItemCount: Have '{Parser.Symbols.closeGroupBrace}' when there should be {{ItemId}}", expression);
                        }
                    }

                    // Неожиданное окончание входной строки
                    throw new FatalParseError($"ParseItemCount: Open braces is {openRoundBrace}. Close Braces is {closeRoundBrace}. Unexpected end of expression", expression);

                }
                else throw new ParseError($"ParseItemCount: Predicate {{{Parser.Predicates.CountItem}}} does not found at the beginning of the expression", expression);
            }
            catch (Exception e)
            {
                expression = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Извлечение функции 'NumericCount' из входной строки expression
        /// В случае успеха соответствующая функции подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева NumericCount</returns>
        public static NumericCount ParseNumericCount(ref string expression)
        {
            string exprBackup = expression;

            try
            {
                if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                {
                    throw new FatalParseError("ParseNumericCount: Expression string is empty");
                }

                expression = expression.TrimStart();

                if (expression.StartsWith(Parser.Predicates.CountNumeric, StringComparison.OrdinalIgnoreCase))
                {
                    // Обнаружен предикативны литерал CountNumeric
                    expression = expression.Substring(Parser.Predicates.CountNumeric.Length);

                    if (Parser.Symbols.TrimOpenGroupBracesAndWhiteSpace(ref expression))
                    {
                        try
                        {
                            string id = ParseId(ref expression);

                            if (Parser.Symbols.TrimCloseGroupBracesAndWhiteSpace(ref expression))
                                return new NumericCount(id);
                            else throw new FatalParseError($"ParseNumericCount: '{Parser.Symbols.closeGroupBrace}' does not found after {{NumericId}}", expression);
                        }
                        catch (BaseParseError idErr)
                        {
                            throw new FatalParseError(new ErrorList { idErr }, $"ParseItemCount: {{NumericId}} does not found after '{Parser.Symbols.openGroupBrace}'", expression);
                        }
                    }
                    else throw new FatalParseError($"ParseNumericCount: '{Parser.Symbols.openGroupBrace}' does not found after {{{Parser.Predicates.CountNumeric}}} predicate", expression);
                }
                else throw new ParseError($"ParseNumericCount: Predicate {{{Parser.Predicates.CountNumeric}}} does not found at the beginning of the expression", expression);
            }
            catch (Exception e)
            {
                expression = exprBackup;
                throw e;
            }
        }
        public static NumericCount ParseRegexNumericCount(ref string expression)
        {
            string exprBackup = expression;

            try
            {
                if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                {
                    throw new FatalParseError("ParseNumericCount: Expression string is empty");
                }

                expression = expression.TrimStart();

                if (expression.StartsWith(Parser.Predicates.CountNumeric, StringComparison.OrdinalIgnoreCase))
                {
                    // Обнаружен предикативны литерал CountNumeric
                    int i = Parser.Predicates.CountNumeric.Length;
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
                        if (Parser.Symbols.IsOpenGroupBraces(expression[i]) // '('
                            && !possibleParseErrorFlag)
                        {
                            if (openRoundBrace == 0)
                                // Запоминаем индекс первой открывающей скобки
                                openBraceInd = i;
                            openRoundBrace++;
                        }
                        else if (Parser.Symbols.IsCloseGroupBraces(expression[i])) // ')'
                            closeRoundBrace++;
                        // Проверяем является ли текущий символ алфавитно-цифровым, подчеркиванием или знаком подстановки
                        else if ((Parser.Symbols.IsLetterOrDigit(expression[i]) || Parser.Symbols.IsUnderscore(expression[i]) || Parser.Symbols.IsWildcard(expression[i]))
                                && !possibleParseErrorFlag)
                        {
                            idCharNumber++;
                        }
                        else if (char.IsWhiteSpace(expression[i]))
                        {
                            // Если число значимых символов аргумента больше 0,
                            // следовательно обнаруженный пробел находится в середине строки-аргумента
                            // или стоит перед финальной закрывающей скобкой
                            if (idCharNumber > 0)
                                possibleParseErrorFlag = true;
                        }
                        else
                        {
                            if (possibleParseErrorFlag)
                                throw new FatalParseError($"ParseNumericCount: Have '{expression[i]}' when there should be '{Parser.Symbols.closeRoundBrace}'", expression);
                            idCharNumber++;
                        }

                        // Сравниваем количество открывающих и закрывающих скобок
                        if (openRoundBrace > 0 && openRoundBrace == closeRoundBrace)
                        {
                            // Найдена финальная закрывающая скобка для списка аргументов Counter
                            if ((i - openBraceInd - 1) < 1)
                                throw new FatalParseError("ParseNumericCount: {NumericId} is Empty string", expression.Substring(i));

                            // Извлекаем строку аргументов
                            string arg = expression.Substring(openBraceInd + 1, i - openBraceInd - 1);

                            // Удалени распознанной подстроки из expression
                            expression = expression.Remove(0, i + 1);

                            // формирование узла синтаксического дерева
                            return new NumericCount(arg);
                        }

                        if (openRoundBrace < closeRoundBrace)
                        {
                            // Число открывающих скобок меньше числа закрывающих
                            // Следовательно входнаяая строка некорректна
                            throw new FatalParseError($"ParseNumericCount: Have '{Parser.Symbols.closeGroupBrace}' when there should be {{NumericId}}", expression);
                        }
                    }

                    // Неожиданное окончание входной строки
                    throw new FatalParseError($"ParseNumericCount: Open braces is {openRoundBrace}. Close Braces is {closeRoundBrace}. Unexpected end of expression", expression);
                }
                else throw new ParseError($"ParseNumericCount: Predicate {{{Parser.Predicates.CountNumeric}}} does not found at the beginning of the expression", expression);
            }
            catch (Exception e)
            {
                expression = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Извлечение функции 'Random' из входной строки expression
        /// В случае успеха соответствующая функции подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Узел синтаксического дерева RandomNumber</returns>
        public static RandomNumber ParseRandom(ref string expression)
        {
            string exprBackup = expression;
            ErrorList subErrors = new ErrorList();

            try
            {
                if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrEmpty(expression))
                {
                    throw new FatalParseError("ParseRandom: Expression string is empty");
                }

                expression = expression.TrimStart();

                if (expression.StartsWith(Parser.Predicates.Random, StringComparison.OrdinalIgnoreCase))
                {
                    // Обнаружен предикативный литерал 'Random'
                    expression = expression.Substring(Parser.Predicates.Random.Length);

                    // Ищем открывающую скобку '('
                    if (Parser.Symbols.TrimOpenGroupBracesAndWhiteSpace(ref expression))
					{
						// Найдена открывающая скобка '('
						
						// Ищем выражение вида Addition
						NumberAstNode operand = null;
						try
						{
							operand = ParseAddition(ref expression);
						}
						catch (ParseError opErr)
						{
                            // Операнд не найден, значит максимум случайного числа не определен
                            subErrors.Add(opErr);

                        }
						
						// Ищем закрывающую скобку ')'
						if(Parser.Symbols.TrimCloseGroupBracesAndWhiteSpace(ref expression))
						{
							// Найдена закрывающая скобка ')'
							
							// Конструируем функтор Random
							return new RandomNumber(operand);							
						}
						else
						{
							// Закрывающая скобка ')' не найдена
							// следовательно входная строка не соответствует правилу Random
							// Восстанавливаем входную строку
							throw new FatalParseError(subErrors, $"ParseRandom: Symbol '{Parser.Symbols.closeGroupBrace}' does not found", expression);

                        }
					}
					else
					{
						// Открывающая скобка '(' не найдена
						throw new FatalParseError($"ParseRandom: Symbol '{Parser.Symbols.openGroupBrace}' does not found after {{{Parser.Predicates.Random}}} predicate", expression);
					}
                }
                else throw new ParseError($"ParseRandom: Predicate {{{Parser.Predicates.Random}}} does not found at the beginning of the expression", expression);
            }
            catch (Exception e)
            {
                expression = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Создание узла АСТ, представляющего бинарный математический оператор
        /// </summary>
        /// <param name="opType"></param>
        /// <param name="leftNode"></param>
        /// <param name="rightNode"></param>
        /// <returns></returns>
        public static NumberAstNode CreateOperatorNode(MathOperatorType opType, NumberAstNode leftNode, NumberAstNode rightNode)
        {
            if (leftNode == null)
                throw new FatalParseError("CreateOperatorNode: Have null when there should be {LeftOperand}");

            if (rightNode == null)
                throw new FatalParseError("CreateOperatorNode: Have null when there should be {RightOperand}");

            switch (opType)
            {
                case MathOperatorType.Multiplication:
                    return new Multiply(leftNode, rightNode);
                case MathOperatorType.Division:
                    return new Devide(leftNode, rightNode);
                case MathOperatorType.Remainder:
                    return new Remainde(leftNode, rightNode);
                case MathOperatorType.Addition:
                    return new Add(leftNode, rightNode);
                case MathOperatorType.Substruction:
                    return new Substract(leftNode, rightNode);
                case MathOperatorType.NOP:
                    throw new FatalParseError("CreateOperatorNode: Have NOP when there should be 'MathOperatorType'");
            }
            throw new FatalParseError("CreateOperatorNode: have incorrect 'MathOperatorType'");
        }
    }
}
