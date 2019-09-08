using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AstralVariables.Expressions
{
    /// <summary>
    /// Класс, описывающий и вычисляющий результат выражение
    /// </summary>
    [Serializable]
    public abstract class AbstractExpression
    {
        [NonSerialized]
        protected string expression;
        /// <summary>
        /// Исходное строковое представление выражения
        /// </summary>
        [XmlText]
        public string Expression
        {
            get => expression;
            set
            {
                if (value != expression)
                {
                    ResetInternal();
                    expression = value;
                    Parse();
                }
            }
        }
        protected virtual void ResetInternal()
        {
            ParseError = null;
        }

        /// <summary>
        /// Флаг корректности выражения и успешного формирования Абстрактного синтаксического дерева
        /// </summary>
        [XmlIgnore]
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
        /// <returns>Флаг успешности разбора</returns>
        public abstract bool Parse(string newExpression = "");

        /// <summary>
        /// Ошибка разбора выражения expression
        /// возникшая при выполнении Parse()
        /// </summary>
        [XmlIgnore]
        public BaseParseError ParseError { get; protected set; }

        public override string ToString()
        {
            return expression;
        }

        /// <summary>
        /// Описание выражения
        /// </summary>
        /// <param name="indent">Отступ от начала строки</param>
        /// <returns></returns>
        public abstract string Description(int indent);
    }

    public abstract class Expression<T> : AbstractExpression
    {
        /// <summary>
        /// Корень Абстрактного синтаксического дерева
        /// </summary>
        [XmlIgnore]
        protected AstNode<T> ast;
        [XmlIgnore]
        public AstNode<T> AST { get => ast; }

        protected override void ResetInternal()
        {
            base.ResetInternal();
            ast = null;
        }

        /// <summary>
        /// Вычисление результата выражения
        /// </summary>
        /// <param name="result">результат вычисления</param>
        /// <returns>True: вычисление произведено успешно</returns>
        public abstract bool Calcucate(out T result);
        public override bool Calcucate(out object result)
        {
            result = null;
            if (Calcucate(out T res))
            {
                result = res;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Описание выражения
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public override string Description(int indent = 0)
        {
            if (IsValid)
                return AST.Description(indent);
            else
            {
                if (ParseError != null)
                    return DescribeErrorMessage(ParseError, indent);
            }
            return string.Empty;
        }

        private string DescribeErrorMessage(BaseParseError e, int indent = 0, StringBuilder sb = null)
        {
            if (sb == null)
                sb = new StringBuilder();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(e.Message).AppendLine();
            foreach (BaseParseError e2 in e.ErrorStack)
            {
                DescribeErrorMessage(e2, indent, sb);
            }

            return sb.ToString();
        }
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
        public BaseParseError(string mes = "", string expr = "") : base (mes)
        {
            ErrorStack = new List<BaseParseError>();
            expression = expr;
        }
        public string expression;
    }

    /// <summary>
    /// Класс неустранимой ошибки разбора выражения
    /// получение которой не повзоляет продолжить разбор выражения Expression
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
    /// получение которой не препятствует разбору выражения Expression
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
