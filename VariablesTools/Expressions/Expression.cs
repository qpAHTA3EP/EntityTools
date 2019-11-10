using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NumberAST = VariableTools.Expressions.AstNode<double>;

namespace VariableTools.Expressions
{
    /// <summary>
    /// Класс, описывающий и вычисляющий результат выражение
    /// </summary>
    [Serializable]
    public abstract class AbstractExpression
    {
        [NonSerialized]
        // Текст выражения в исходом виде, заданном пользователем
        protected string text;
        [NonSerialized]
        // Текст выражения без пробельных символов
        protected string shortText;
        /// <summary>
        /// Исходное текстовое представление выражения
        /// </summary>
        [XmlText]
        public virtual string Text
        {
            get => text;
            set
            {
                if (value != text)
                {
                    ResetInternal();
                    text = value;
                    Parser.DeleteWhiteSpaces(text, out shortText);
                    //Parse();
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
        protected abstract bool Parse();

        protected BaseParseError parseError = null;
        /// <summary>
        /// Ошибка разбора выражения expression
        /// возникшая при выполнении Parse()
        /// </summary>
        [XmlIgnore]
        public BaseParseError ParseError { get => parseError; protected set => parseError = value; }

        public override string ToString()
        {
            return text;
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

        /// <summary>
        /// Кэш абстрактных синтаксических деревьев
        /// Предназначенная для сокращения времени на разбор одинаковых АСТ
        /// </summary>
        protected static Dictionary<string, AstNode<T>> astCollection = new Dictionary<string, AstNode<T>>();

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
