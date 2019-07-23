using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstralVars.Expressions
{
    /// <summary>
    /// Абстрактный терм - единичный неделимый элемент выражения Expression
    /// </summary>
    public abstract class AbstractTerm
    {
        /// <summary>
        /// Вычислить значение терма
        /// </summary>
        public abstract object Result { get; }
    }

    /// <summary>
    /// Типизированный класс, описывающий Терм - единичный неделимый элемент выражения Expression
    /// </summary>
    public abstract class Term<T> : AbstractTerm
    {
        public override object Result
        {
            get
            {
                if (Calculate(out T result))
                    return result;
                return null;
            }
        }

        /// <summary>
        /// Вычислить значение терма
        /// </summary>
        /// <param name="result">результат вычисления</param>
        /// <returns></returns>
        public abstract bool Calculate(out T result);

        /// <summary>
        /// Приоритет
        /// </summary>
        public abstract uint Priority { get; }
    }

    public abstract class BinOperator<T> : Term<T>
    {
        protected Term<T> root;
        protected Term<T> leftOperand;
        protected Term<T> rightOperand;
    }

    public abstract class UnOperator<T> : Term<T>
    {
        protected Term<T> root;
        protected Term<T> Operand;
    }
}
