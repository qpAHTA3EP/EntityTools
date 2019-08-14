using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstralVars.Expressions
{
    /// <summary>
    /// Класс, описывающий и вычисляющий результат выражение
    /// </summary>
    public abstract class AbstractExpression
    {
        /// <summary>
        /// Исходное строковое представление выражения
        /// </summary>
        protected string expression;

        /// <summary>
        /// Флаг корректности выражения и успешного формирования Абстрактного синтаксического дерева
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        /// Вычисление результата выражения
        /// </summary>
        /// <param name="result">результат вычисления</param>
        /// <returns>True: вычисление произведено успешно</returns>
        public abstract bool Calcucate(out object result);

        /// <summary>
        /// Лексический разбор выражения и построение Абстрактного синтаксического дерева
        /// </summary>
        /// <returns>Объект, описывающий результат разбора</returns>
        public abstract AST Parse();

        public override string ToString()
        {
            return expression;
        }
    }

    public abstract class Expression<T> : AbstractExpression
    {
        /// <summary>
        /// Корень Абстрактного синтаксического дерева
        /// </summary>
        protected AstNode<T> root;
    }


    /// <summary>
    /// Класс ошибки разбора выражения
    /// </summary>
    public class BaseParseError : Exception
    {
        /// <summary>
        /// Список ошибок проверки вложенных правил (элементов правила)
        /// </summary>
        public List<BaseParseError> ErrorStack;

        public BaseParseError(List<BaseParseError> subErrors = null, string mes = "", string expr = "") : base(mes)
        {
            if (subErrors != null)
                ErrorStack = subErrors;
            expression = expr;
        }
        public BaseParseError(string mes = "", string expr = "") : base(mes)
        {
            ErrorStack = new List<BaseParseError>();
            expression = expr;
        }
        public string expression;
    }

    /// <summary>
    /// Класс неустранимой ошибки разбора выражения
    /// получение которой не повзоляет продолжить разбор выражения expression
    /// </summary>
    public class FatalParseError : BaseParseError
    {
        public FatalParseError(List<BaseParseError> subErrors = null, string mes = "", string expr = "") : base(subErrors, mes, expr)
        { }

        public FatalParseError(string mes = "", string expr = "") : base(mes, expr)
        { }
    }

    /// <summary>
    /// Класс ошибки разбора выражения
    /// получение которой не препятствует разбору выражения expression
    /// и проверке других правил или элементов правила
    /// </summary>
    public class ParseError : BaseParseError
    {
        public ParseError(List<BaseParseError> subErrors = null, string mes = "", string expr = "") : base(subErrors, mes, expr)
        { }

        public ParseError(string mes = "", string expr = "") : base(mes, expr)
        { }
    }

}
