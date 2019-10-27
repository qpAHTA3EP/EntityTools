using System;
using static AstralVariables.Expressions.Parser;
using NumberAstNode = AstralVariables.Expressions.AstNode<double>;
using ErrorList = System.Collections.Generic.List<AstralVariables.Expressions.BaseParseError>;
using AstralVariables.Expressions.Operand;
using AstralVariables.Expressions.Functions;
using AstralVariables.Expressions.Operators;
using System.Diagnostics;
using System.Xml.Serialization;

namespace AstralVariables.Expressions
{
    [Serializable]
    public class NumberExpression : Expression<double>
    {
        [XmlIgnore]
        public override bool IsValid => !string.IsNullOrEmpty(expression) && !string.IsNullOrWhiteSpace(expression) && (ast != null);

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
                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesTools)}: Calculate '{expression}' time: {stopwatch.ElapsedMilliseconds} ms");
#endif
                return true;
            }
#if DEBUG
            stopwatch.Stop();
            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesTools)}: Calculate '{expression}' time: {stopwatch.ElapsedMilliseconds} ms");
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
                expression = newExpression;
            else if (string.IsNullOrEmpty(expression) || string.IsNullOrWhiteSpace(expression))
            {
                ast = null;
                ParseError = new FatalParseError("Parse: Expression string is empty");
                return false;
            }

            string expr = expression;
            ast = null;
            try
            {
                ast = ParseAddition(ref expr);
            }
            catch(BaseParseError e)
            {
                ParseError = e;
                return false;
            }
#if DEBUG
            stopwatch.Stop();
            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesTools)}: Parse '{expression}' time: {stopwatch.ElapsedMilliseconds} ms");
#endif
            // После разбора выражения строка expr должна остаться пустой
            // в противном случае выражение содержит ошибку
            if (!string.IsNullOrWhiteSpace(expr) && !string.IsNullOrEmpty(expr))
            {
                ast = null;
                expr = expr.TrimStart();
                ParseError = new FatalParseError($"Parse: Undefined symbol at position {expression.Length - expr?.Length}" , expr);
                return false;
            }

            return ast != null;
        }

        /// <summary>
        /// Извлечение 'числовой константы' из входной строки expression
        /// В случае успеха соответствующая числу подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>Узел синтаксического дерева NumberNode</returns>
        public static NumberConstant ParseNumber(ref string expr)
        {
            string exprBackup = expr;
            expr = expr.TrimStart();

            if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
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
                for (; i < expr.Length;i++)
                {
                    if (char.IsDigit(expr[i]))
                    {
                        continue;
                    }

                    if (char.IsWhiteSpace(expr[i]))
                        // Найден провел
                        // Следовательно число закончилось на предыдущем символе
                        break;

                    if (Parser.Symbols.IsMathOperator(expr[i]))
                        // Найден математический оператор
                        // Следовательно число закончилось на предыдущем символе
                        break;


                    if (Parser.Symbols.IsCloseBraces(expr[i]))
                        // Найдена закрывающая скобка
                        // Следовательно число закончилось на предыдущем символе
                        break;


                    if (Parser.Symbols.IsNumberDecimalSeparator(expr[i]))
                    {
                        if (numDecimalSeparator > 1)
                        {
                            // Десятичный разделитель встретился второй раз, 
                            // Это является недопустимым
                            // Следовательно входная строка не содержит числа или некорректна
                            throw new FatalParseError($"ParseNumber: Have '{expr[i]}' when there should be 'Digit' or the end of {{Number}}", expr.Substring(i));
                        }
                        numDecimalSeparator++;
                        continue;
                    }

                    if (Parser.Symbols.IsUnderscore(expr[i])
                        || Parser.Symbols.IsLetter(expr[i]))
                    {
                        // Встретился символ подчеркивания '_'
                        // или буква или любой символ, не входящий в числовой литерал
                        // Следовательно входная строка не содержит числа
                        throw new ParseError($"ParseNumber: Have '{expr[i]}' when there should be 'Digit' or the end of {{Number}}", expr.Substring(i));
                    }

                    // Встретился недопустимый символ 
                    throw new FatalParseError($"ParseNumber: Have forbidden symbol '{expr[i]}' when there should be 'Digit' or the end of {{Number}}", expr.Substring(i));
                }

                if (i == 0)
                    throw new ParseError($"ParseNumber: {{Number}} does not found at the beginning of the expression", expr);

                // попытка преобразовать часть строки в число
                double value;
                bool parseSucceeded = false;
                if (numDecimalSeparator > 0)
                     parseSucceeded = double.TryParse(Parser.Symbols.CorrectNumberDecimalSeparator(ref expr).Substring(0, i), out value);
                else parseSucceeded = double.TryParse(expr.Substring(0, i), out value);

                if (parseSucceeded)
                {
                    // удаление распознанной подстроки из входного выражения 
                    expr = expr.Remove(0, i);
                    // конструирование листового узла синтаксического дерева
                    return new NumberConstant(value);
                }
                else throw new ParseError("ParseNumber: Unsucceeded", expr);
            }
            catch (Exception e)
            {
                expr = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Извлечение имени переменной из входной строки expression
        /// В случае успеха соответствующая переменной подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>Узел синтаксического дерева NumberVariable</returns>
        public static NumberVariable ParseVariable(ref string expr)
        {
            string exprBackup = expr;

            if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                throw new FatalParseError("ParseVariable: Expression string is empty");

            expr = expr.TrimStart();

            try
            {
                // Индекс символа во входной строке
                int i;
                // Поиск путем анализа символов строки
                for (i = 0; i < expr.Length; i++)
                {
                    if (Parser.Symbols.IsLetterOrDigit(expr[i]))
                        continue;

                    if (Parser.Symbols.IsUnderscore(expr[i]))
                        continue;

                    if (Parser.Symbols.IsMathOperator(expr[i]))
                        // Обнаружен математический оператор 
                        // Следовательно Имя переменной закончилось на предыдущем символе
                        break;                    

                    if (char.IsWhiteSpace(expr[i]))
                        // Обнаружен пробел
                        // Следовательно Имя переменной закончилось на предыдущем символе
                        break;

                    if (Parser.Symbols.IsCloseGroupBraces(expr[i]))
                        // Найдена закрывающая скобка
                        // Следовательно Имя переменной закончилось на предыдущем символе
                        break;

                    if (Parser.Symbols.IsOpenGroupBraces(expr[i]))
                    {
                        // Встретилась открывающая скобка
                        // Следовательно входная строка некорректна
                        throw new FatalParseError($"ParseVariable: Unexpeted symbol '{expr[i]}'", expr.Substring(i));
                    }

                    // Встретился недопустимый символ
                    // Следовательно входная строка некорректна
                    throw new FatalParseError($"ParseVariable: Symbol '{expr[i]}' is forbiden in the name of {{Variable}}", expr.Substring(i));
                }

                if (i == 0)
                    throw new ParseError("ParseVariable: {Variable} does not found at the beginning of the expression", expr.Substring(i));

                // Извлечение названия переменной из подстроки
                string var_name = expr.Substring(0, i);
                if (Parser.IsForbidden(var_name))
                    throw new FatalParseError($"ParseVariable: Name {{{var_name}}} if forbidden", expr);

                if (!string.IsNullOrWhiteSpace(var_name) && !string.IsNullOrEmpty(var_name))
                {
                    // удаление распознанной подстроки из входного выражения 
                    expr = expr.Remove(0, i);
                    // конструирование листового узла синтаксического дерева
                    return new NumberVariable(var_name);
                }
                else throw new ParseError("ParseVariable: Unsucceeded", expr.Substring(i));
            }
            catch(Exception e)
            {
                expr = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Обработка правила
        /// Value = Number
        ///       | Variable
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static NumberAstNode ParseValue(ref string expr)
        {
            string exprBackup = expr;
            ErrorList subError = new ErrorList();

            if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                throw new FatalParseError("ParseValue: Expression string is empty");

            try
            {
                try
                {
                    // Пытаетмся извлечь числовую константу Value = Number
                    return ParseNumber(ref expr);
                }
                catch (ParseError numErr)
                {
                    subError.Add(numErr);

                    expr = exprBackup;
                    // Пытаемся извлечь переменную Value = Variable
                    try
                    {
                        return ParseVariable(ref expr);
                    }
                    catch (ParseError varErr)
                    {
                        subError.Add(varErr);

                        expr = exprBackup;
                    }
                }
            }
            catch(FatalParseError fatErr)
            {
                expr = exprBackup;
                subError.Add(fatErr);
                throw new FatalParseError(subError, "ParseValue: Fatal error", fatErr.expression);
            }

            expr = exprBackup;
            //errorMess.Insert(0, "ParseValue: No {Number} or {Variable} was found\n");
            //throw new ParseError(subError, errorMess.ToString(), expression);
            throw new ParseError(subError, "ParseValue: No {Number} or {Variable} was found", expr);
        }


        /// <summary>
        /// Извлечение идентификатора предмета (ItemId) из входной строки expression
        /// В случае успеха соответствующая идентификатору предмета подстрока (ItemId) удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>Идентификатор предмета ItemId</returns>
        public static string ParseId(ref string expr)
        {

            if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                throw new FatalParseError("ParseItemId: Expression string is empty");

            expr = expr.TrimStart();

            // Индекс символа во входной строке
            int i = 0;

            // Поиск путем анализа символов строки
            for (; i < expr.Length; i++)
            {
                if (Parser.Symbols.IsLetterOrDigit(expr[i]))
                    continue;

                if (Parser.Symbols.IsUnderscore(expr[i]))
                    continue;

                //if (Parser.IsWildcard(expression[i]))
                //    continue;

                if (char.IsWhiteSpace(expr[i]))
                    // Найден пробел
                    // Следовательно  ItemId закончился на предыдущем символе
                    break;

                if (Parser.Symbols.IsCloseBraces(expr[i]))
                    // Найдена закрывающая скобка
                    // Следовательно ItemId закончился на предыдущем символе
                    break;

                if (Parser.Symbols.IsOpenBraces(expr[i]))
                {
                    // Найдена открывающая скобка
                    // Следовательно входная строка некорректна
                    throw new FatalParseError($"ParseItemId: Have '{expr[i]}' when there should be the end of {{ItemId}}", expr.Substring(i));
                }

                if (Parser.Symbols.IsMathOperator(expr[i]))
                {
                    // Найден символ математического оператора
                    // Математический операции над идентификаторами не допускаются
                    // Следовательно входная строка некорректна
                    throw new FatalParseError("ParseItemId: Have {MathOperator} when there should be the end of {ItemId}", expr.Substring(i));
                }

                throw new FatalParseError($"ParseItemId: Have '{expr[i]}' when there should be {{ItemId}}", expr.Substring(i));
            }

            if (i == 0)
                throw new ParseError("ParseItemId: {ItemId} does not found at the beginning of the expression", expr);

            // Извлечение названия переменной из подстроки
            string itemId = expr.Substring(0, i);
            if (!string.IsNullOrWhiteSpace(itemId) && !string.IsNullOrEmpty(itemId))
            {
                // удаление распознанной подстроки из входного выражения 
                expr = expr.Remove(0, i);
                // конструирование листового узла синтаксического дерева
                return itemId;
            }
            else throw new ParseError("ParseItemId: Unsucceeded", expr);
        }

        /// <summary>
        /// Обработка правила Addition
        /// Addition = Multiplication
        ///     | Multiplication ('+' | '-') Addition
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static NumberAstNode ParseAddition(ref string expr)
        {
            string exprBackup = expr;
            ErrorList subError = new ErrorList();
            //StringBuilder errorMess = new StringBuilder();

            if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                throw new FatalParseError("ParseAddition: Expression string is empty");

            try
            {
                NumberAstNode leftOperand = ParseMultiplication(ref expr);
                
				MathOperatorType operatorType = Parser.Symbols.TrimAdditionOperatorAndWhiteSpace(ref expr);
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
					NumberAstNode rightOperand = ParseAddition(ref expr);
					
					///
					/// Конструируем узел дерева
					///
					return CreateOperatorNode(operatorType, leftOperand, rightOperand);
				}				

            }
            catch (FatalParseError fatErr)
            {
                expr = exprBackup;
                subError.Add(fatErr);
                throw new FatalParseError(subError, "ParseAddition: Fatal error", fatErr.expression);
            }
            catch (Exception e)
            {
                expr = exprBackup;
                throw e;
            }

            throw new FatalParseError("ParseAddition: Unexpected end of method");
        }

        /// <summary>
        /// Разбор строки соответствующей '(' Addition ')' 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
		public static NumberAstNode ParseBracketedAddition(ref string expr)
		{
            string exprBackup = expr;

			if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                throw new FatalParseError("ParseBracketedAddition: Expression string is empty");

            try
            {
                // Ищем открывающу скобку '('
                if (Parser.Symbols.TrimOpenGroupBracesAndWhiteSpace(ref expr))
                {
                    NumberAstNode addNode = null;
                    // Нашли и удалили открывающую скобку '('
                    try
                    {
                        // Извлекаем Addition
                        addNode = ParseAddition(ref expr);
                    }
                    catch (ParseError e)
                    {
                        // После открывающей скобки не найдено выражение, соответствующее правилу Addition 
                        // Следовательно входная строка не соответствует выражению '(' Addition ')' 
                        // Восстанавливаем входную строку 
                        expr = exprBackup;
                        throw e;
                    }

                    // Ищем и удаляем закрывающую скобку ')' после Addition
                    if (Parser.Symbols.TrimCloseGroupBracesAndWhiteSpace(ref expr))
                        return addNode;
                    else
                    {
                        // Закрывающая скобка не найдена
                        // следовательно входная строка не соответствует '(' Addition ')'
                        throw new FatalParseError($"ParseBracketedAddition: not found '{Parser.Symbols.closeGroupBrace}'", expr);
                    }
                }
                else
                {
                    // Открывающая '(' скобка не найдена
                    // следовательно входная строка не соответствует '(' Addition ')'
                    throw new ParseError($"ParseBracketedAddition: not found '{Parser.Symbols.openGroupBrace}'", expr);
                }
            }
            catch(ParseError e)
            {
                // Восстанавливаем входную строку 
                expr = exprBackup;
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
        /// <param name="expr"></param>
        /// <returns></returns>
        public static NumberAstNode ParseMultiplication(ref string expr)
        {
            return ParseMultiplication(ref expr, null);
        }
        public static NumberAstNode ParseMultiplication(ref string expr, NumberAstNode left)
        {
            string exprBackup = expr;
			NumberAstNode leftOperand = left;
            ErrorList subError = new ErrorList();
            //StringBuilder errorMess = new StringBuilder();

            expr = expr.TrimStart();

            try
            {
                if(leftOperand == null)
				{
					// Левый операнд не был задан при вызове метода
					if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                        throw new FatalParseError("ParseMultiplication: Expression string is empty");

                    // Пробуем извлечь левый операнд вида '(' Addition ')' 
                    try
                    {
                        try
                        {
                            // Извлекаем '(' Addition ')' 
                            leftOperand = ParseBracketedAddition(ref expr);
                        }
                        catch (ParseError brAdditionErr)
                        {
                            //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/brAdditionErr.Message)/*.Append(" in expression: ").Append(expression).AppendLine()*/;
                            subError.Add(brAdditionErr);

                            // Входная строка не содержит выражения, соответствующего '(' Addition ')' 
                            // Восстанавливаем входную строку 
                            expr = exprBackup;

                            // Пробуем извлечь левый операнд типа Function
                            try
                            {
                                //  Пробуем извлечь левый операнд вида Function
                                leftOperand = ParseFunction(ref expr);
                            }
                            catch (ParseError funErr)
                            {
                                //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/funErr.Message)/*.Append(" in expression: ").Append(expression).AppendLine()*/;
                                subError.Add(funErr);

                                // Входная строка не содержит выражения, соответствующего Function 
                                // Восстанавливаем входную строку 
                                expr = exprBackup;

                                // Извлекаем Value
                                try
                                {
                                    leftOperand = ParseValue(ref expr);
                                }
                                catch (ParseError valErr)
                                {
                                    //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/valErr.Message)/*.Append(" in expression: ").Append(expression).AppendLine()*/;
                                    subError.Add(valErr);

                                    // Входная строка не содержит выражения, соответствующего Value 
                                    // Восстанавливаем входную строку 
                                    //errorMess.Insert(0, "ParseMultiplication: No {BracketedAddition}, {Function} or {Value} was found where should be {LeftOperand}:\n");
                                    //throw new ParseError(subErrors, errorMess.ToString(), expression);
                                    throw new ParseError(subError, "ParseMultiplication: No {BracketedAddition}, {Function} or {Value} was found where should be {LeftOperand}", expr);
                                }
                            }
                        }
                    }
                    catch (FatalParseError fatErr)
                    {
                        expr = exprBackup;
                        subError.Add(fatErr);
                        throw new FatalParseError(subError, "ParseMultiplication: Fatal error", fatErr.expression);
                    }


                    if (leftOperand == null)
                    {
                        expr = exprBackup;
                        //errorMess.Insert(0, "ParseMultiplication: {LeftOperand} not found:\n");
                        //throw new ParseError(errorMess.ToString(), expression);
                        throw new ParseError(subError, "ParseMultiplication: {LeftOperand} not found", expr);
                    }
					
                    ///
                    /// Проверяем правую часть правила Multiplication
                    ///		
					if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                        return leftOperand;
					else return ParseMultiplication(ref expr, leftOperand);					
				}
				else
				{
					///
					/// Проверяем правую часть правила Multiplication 
					///
					/// Извлекаем математический оператор
					///
					MathOperatorType operatorType = Parser.Symbols.TrimMultiplicationOperatorAndWhiteSpace(ref expr);
					if(operatorType != MathOperatorType.NOP)
					{
						// Найден математический оператор
						// Сохраняем резервную копию входной строки без математического оператора
						string exprBackup2 = expr;
						
						NumberAstNode rightOperand = null;

                        try
                        {
                            try
                            {
                                // Пробуем извлечь второй опренад вида '(' Addition ')' 
                                rightOperand = ParseBracketedAddition(ref expr);
                            }
                            catch (ParseError brAddErr)
                            {
                                // Входная строка не содержит выражения, соответствующего '(' Addition ')'
                                //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/brAddErr.Message).AppendLine();
                                subError.Add(brAddErr);

                                // Пробуем извлеч второй операнд типа Function
                                try
                                {
                                    rightOperand = ParseFunction(ref expr);
                                }
                                catch (ParseError funErr)
                                {
                                    // Входная строка не содержит выражения, соответствующего Value
                                    //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/funErr.Message).AppendLine();
                                    subError.Add(funErr);

                                    // Восстанавливаем входную строку
                                    expr = exprBackup2;

                                    // Пробуем извлеч второй операнд типа Value
                                    try
                                    {
                                        rightOperand = ParseValue(ref expr);
                                    }
                                    catch (ParseError valErr)
                                    {
                                        subError.Add(valErr);
                                        //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/valErr.Message).AppendLine();
                                        //errorMess.Insert(0, "ParseMultiplication: No {BracketedAddition}, {Function} or {Value} was found where should be {RightOperand}:\n");
                                        //throw new ParseError(errorMess.ToString(), expression);
                                        throw new ParseError(subError, "ParseMultiplication: No {BracketedAddition}, {Function} or {Value} was found where should be {RightOperand}", expr);
                                    }
                                }
                            }
                        }
                        catch (FatalParseError fatErr)
                        {
                            expr = exprBackup;
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
                            throw new FatalParseError(subError, "ParseMultiplication: {RightOperand} not found", expr);
                        }
					
						leftOperand = CreateOperatorNode(operatorType, leftOperand, rightOperand);
						return ParseMultiplication(ref expr, leftOperand);
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
				expr = exprBackup;
				throw e;
			}
        }

        /// <summary>
        /// Обработка правила Function
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>Узел синтаксического дерева NumberAstNode</returns>
        public static NumberAstNode ParseFunction(ref string expr)
        {
            string exprBackup = expr;
            ErrorList subErrors = new ErrorList();
            
            try
            { 
                try
                {
                    return ParseItemCount(ref expr);
                }
                catch (ParseError itemCntErr)
                {
                    expr = exprBackup;
                    subErrors.Add(itemCntErr);
                    //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/itemCntErr.Message).AppendLine();

                    try
                    {
                        return ParseNumericCount(ref expr);
                    }
                    catch (ParseError numCntErr)
                    {
                        expr = exprBackup;
                        subErrors.Add(numCntErr);
                        //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/numCntErr.Message).AppendLine();
                        try
                        {
                            return ParseRandom(ref expr);
                        }
                        catch (ParseError rndErr)
                        {
                            expr = exprBackup;
                            subErrors.Add(rndErr);
                            //errorMess.Append(/*).Append(Parser.Symbols.Tab).Append(*/rndErr.Message).AppendLine();
                        }
                    }
                }
            }
            catch (FatalParseError fatErr)
            {
                expr = exprBackup;
                subErrors.Add(fatErr);
                throw new FatalParseError(subErrors, "ParseMultiplication: Fatal error", fatErr.expression);
            }

            expr = exprBackup;
            
            //errorMess.Insert(0, "ParseFunction: No {ItemCount}, {NumericCount} or {Random} functor was found:\n");
            //throw new ParseError(errorMess.ToString(), expression);
            
            throw new ParseError(subErrors, "ParseFunction: No {ItemCount}, {NumericCount} or {Random} functor was found", expr);
        }

        /// <summary>
        /// Извлечение функции 'CountItem' из входной строки expression
        /// В случае успеха соответствующая функции подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>Узел синтаксического дерева ItemCount</returns>
        public static ItemCount ParseItemCount(ref string expr)
        {
            string exprBackup = expr;

            try
            {
                if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                {
                    throw new FatalParseError("ParseItemCount: Expression string is empty");
                }

                expr = expr.TrimStart();

                if (expr.StartsWith(Parser.Predicates.CountItem, StringComparison.OrdinalIgnoreCase))
                {
                    expr = expr.Substring(Parser.Predicates.CountItem.Length);

                    if(Parser.Symbols.TrimOpenGroupBracesAndWhiteSpace(ref expr))
                    {
                        try
                        {
                            string id = ParseId(ref expr);

                            if (Parser.Symbols.TrimCloseGroupBracesAndWhiteSpace(ref expr))
                                return new ItemCount(id);
                            else throw new FatalParseError($"ParseItemCount: '{Parser.Symbols.closeGroupBrace}' does not found after {{ItemId}}", expr);
                        }
                        catch(BaseParseError idErr)
                        {
                            throw new FatalParseError(new ErrorList {idErr}, $"ParseItemCount: {{ItemId}} does not found after '{Parser.Symbols.openGroupBrace}'", expr);
                        }
                    }
                    else throw new FatalParseError($"ParseItemCount: '{Parser.Symbols.openGroupBrace}' does not found after {{{Parser.Predicates.CountItem}}} predicate", expr);
                }
                else throw new ParseError($"ParseItemCount: Predicate {{{Parser.Predicates.CountItem}}} does not found at the beginning of the expression", expr);
            }
            catch (Exception e)
            {
                expr = exprBackup;
                throw e;
            }
        }
        public static ItemCount ParseRegexItemCount(ref string expr)
        {
            string exprBackup = expr;

            try
            {
                if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                {
                    throw new FatalParseError("Expression string is empty");
                }

                expr = expr.TrimStart();

                if (expr.StartsWith(Parser.Predicates.CountItem, StringComparison.OrdinalIgnoreCase))
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
                    for (; i < expr.Length; i++)
                    {
                        if (Parser.Symbols.IsOpenGroupBraces(expr[i]) // '('
                            && !possibleParseErrorFlag)
                        {
                            if (openRoundBrace == 0)
                                // Запоминаем индекс первой открывающей скобки
                                openBraceInd = i;
                            openRoundBrace++;
                        }
                        else if (Parser.Symbols.IsCloseGroupBraces(expr[i])) // ')'
                            closeRoundBrace++;
                        // Проверяем является ли текущий символ алфавитно-цифровым, подчеркиванием или знаком подстановки
                        else if ((Parser.Symbols.IsLetterOrDigit(expr[i]) || Parser.Symbols.IsUnderscore(expr[i]) || Parser.Symbols.IsWildcard(expr[i]))
                                && !possibleParseErrorFlag)
                        {
                            idCharNumber++;
                        }
                        else if (char.IsWhiteSpace(expr[i]))
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
                                throw new FatalParseError($"ParseItemCount: Have '{expr[i]}' when there should be '{Parser.Symbols.closeRoundBrace}'", expr);
                            idCharNumber++;
                        }

                        // Сравниваем количество открывающих и закрывающих скобок
                        if (openRoundBrace > 0 && openRoundBrace == closeRoundBrace)
                        {
                            // Найдена финальная закрывающая скобка для списка аргументов Counter

                            if ((i - openBraceInd - 1) < 1)
                                throw new FatalParseError("ParseItemCount: {ItemId} is Empty string", expr.Substring(i));

                            // Извлекаем строку аргументов
                            string arg = expr.Substring(openBraceInd + 1, i - openBraceInd - 1);

                            // Удалени распознанной подстроки из expression
                            expr = expr.Remove(0, i + 1);

                            // формирование узла синтаксического дерева
                            return new ItemCount(arg);
                        }

                        if (openRoundBrace < closeRoundBrace)
                        {
                            // Число открывающих скобок меньше числа закрывающих
                            // Следовательно входнаяая строка некорректна
                            throw new FatalParseError($"ParseItemCount: Have '{Parser.Symbols.closeGroupBrace}' when there should be {{ItemId}}", expr);
                        }
                    }

                    // Неожиданное окончание входной строки
                    throw new FatalParseError($"ParseItemCount: Open braces is {openRoundBrace}. Close Braces is {closeRoundBrace}. Unexpected end of expression", expr);

                }
                else throw new ParseError($"ParseItemCount: Predicate {{{Parser.Predicates.CountItem}}} does not found at the beginning of the expression", expr);
            }
            catch (Exception e)
            {
                expr = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Извлечение функции 'NumericCount' из входной строки expression
        /// В случае успеха соответствующая функции подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>Узел синтаксического дерева NumericCount</returns>
        public static NumericCount ParseNumericCount(ref string expr)
        {
            string exprBackup = expr;

            try
            {
                if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                {
                    throw new FatalParseError("ParseNumericCount: Expression string is empty");
                }

                expr = expr.TrimStart();

                if (expr.StartsWith(Parser.Predicates.CountNumeric, StringComparison.OrdinalIgnoreCase))
                {
                    // Обнаружен предикативны литерал CountNumeric
                    expr = expr.Substring(Parser.Predicates.CountNumeric.Length);

                    if (Parser.Symbols.TrimOpenGroupBracesAndWhiteSpace(ref expr))
                    {
                        try
                        {
                            string id = ParseId(ref expr);

                            if (Parser.Symbols.TrimCloseGroupBracesAndWhiteSpace(ref expr))
                                return new NumericCount(id);
                            else throw new FatalParseError($"ParseNumericCount: '{Parser.Symbols.closeGroupBrace}' does not found after {{NumericId}}", expr);
                        }
                        catch (BaseParseError idErr)
                        {
                            throw new FatalParseError(new ErrorList { idErr }, $"ParseItemCount: {{NumericId}} does not found after '{Parser.Symbols.openGroupBrace}'", expr);
                        }
                    }
                    else throw new FatalParseError($"ParseNumericCount: '{Parser.Symbols.openGroupBrace}' does not found after {{{Parser.Predicates.CountNumeric}}} predicate", expr);
                }
                else throw new ParseError($"ParseNumericCount: Predicate {{{Parser.Predicates.CountNumeric}}} does not found at the beginning of the expression", expr);
            }
            catch (Exception e)
            {
                expr = exprBackup;
                throw e;
            }
        }
        public static NumericCount ParseRegexNumericCount(ref string expr)
        {
            string exprBackup = expr;

            try
            {
                if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                {
                    throw new FatalParseError("ParseNumericCount: Expression string is empty");
                }

                expr = expr.TrimStart();

                if (expr.StartsWith(Parser.Predicates.CountNumeric, StringComparison.OrdinalIgnoreCase))
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
                    for (; i < expr.Length; i++)
                    {
                        if (Parser.Symbols.IsOpenGroupBraces(expr[i]) // '('
                            && !possibleParseErrorFlag)
                        {
                            if (openRoundBrace == 0)
                                // Запоминаем индекс первой открывающей скобки
                                openBraceInd = i;
                            openRoundBrace++;
                        }
                        else if (Parser.Symbols.IsCloseGroupBraces(expr[i])) // ')'
                            closeRoundBrace++;
                        // Проверяем является ли текущий символ алфавитно-цифровым, подчеркиванием или знаком подстановки
                        else if ((Parser.Symbols.IsLetterOrDigit(expr[i]) || Parser.Symbols.IsUnderscore(expr[i]) || Parser.Symbols.IsWildcard(expr[i]))
                                && !possibleParseErrorFlag)
                        {
                            idCharNumber++;
                        }
                        else if (char.IsWhiteSpace(expr[i]))
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
                                throw new FatalParseError($"ParseNumericCount: Have '{expr[i]}' when there should be '{Parser.Symbols.closeRoundBrace}'", expr);
                            idCharNumber++;
                        }

                        // Сравниваем количество открывающих и закрывающих скобок
                        if (openRoundBrace > 0 && openRoundBrace == closeRoundBrace)
                        {
                            // Найдена финальная закрывающая скобка для списка аргументов Counter
                            if ((i - openBraceInd - 1) < 1)
                                throw new FatalParseError("ParseNumericCount: {NumericId} is Empty string", expr.Substring(i));

                            // Извлекаем строку аргументов
                            string arg = expr.Substring(openBraceInd + 1, i - openBraceInd - 1);

                            // Удалени распознанной подстроки из expression
                            expr = expr.Remove(0, i + 1);

                            // формирование узла синтаксического дерева
                            return new NumericCount(arg);
                        }

                        if (openRoundBrace < closeRoundBrace)
                        {
                            // Число открывающих скобок меньше числа закрывающих
                            // Следовательно входнаяая строка некорректна
                            throw new FatalParseError($"ParseNumericCount: Have '{Parser.Symbols.closeGroupBrace}' when there should be {{NumericId}}", expr);
                        }
                    }

                    // Неожиданное окончание входной строки
                    throw new FatalParseError($"ParseNumericCount: Open braces is {openRoundBrace}. Close Braces is {closeRoundBrace}. Unexpected end of expression", expr);
                }
                else throw new ParseError($"ParseNumericCount: Predicate {{{Parser.Predicates.CountNumeric}}} does not found at the beginning of the expression", expr);
            }
            catch (Exception e)
            {
                expr = exprBackup;
                throw e;
            }
        }

        /// <summary>
        /// Извлечение функции 'Random' из входной строки expression
        /// В случае успеха соответствующая функции подстрока удаляется из expression
        /// В противном случае генерируется исключение ParseError
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>Узел синтаксического дерева RandomNumber</returns>
        public static RandomNumber ParseRandom(ref string expr)
        {
            string exprBackup = expr;
            ErrorList subErrors = new ErrorList();

            try
            {
                if (string.IsNullOrWhiteSpace(expr) || string.IsNullOrEmpty(expr))
                {
                    throw new FatalParseError("ParseRandom: Expression string is empty");
                }

                expr = expr.TrimStart();

                if (expr.StartsWith(Parser.Predicates.Random, StringComparison.OrdinalIgnoreCase))
                {
                    // Обнаружен предикативный литерал 'Random'
                    expr = expr.Substring(Parser.Predicates.Random.Length);

                    // Ищем открывающую скобку '('
                    if (Parser.Symbols.TrimOpenGroupBracesAndWhiteSpace(ref expr))
					{
						// Найдена открывающая скобка '('
						
						// Ищем выражение вида Addition
						NumberAstNode operand = null;
						try
						{
							operand = ParseAddition(ref expr);
						}
						catch (ParseError opErr)
						{
                            // Операнд не найден, значит максимум случайного числа не определен
                            subErrors.Add(opErr);

                        }
						
						// Ищем закрывающую скобку ')'
						if(Parser.Symbols.TrimCloseGroupBracesAndWhiteSpace(ref expr))
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
							throw new FatalParseError(subErrors, $"ParseRandom: Symbol '{Parser.Symbols.closeGroupBrace}' does not found", expr);

                        }
					}
					else
					{
						// Открывающая скобка '(' не найдена
						throw new FatalParseError($"ParseRandom: Symbol '{Parser.Symbols.openGroupBrace}' does not found after {{{Parser.Predicates.Random}}} predicate", expr);
					}
                }
                else throw new ParseError($"ParseRandom: Predicate {{{Parser.Predicates.Random}}} does not found at the beginning of the expression", expr);
            }
            catch (Exception e)
            {
                expr = exprBackup;
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
