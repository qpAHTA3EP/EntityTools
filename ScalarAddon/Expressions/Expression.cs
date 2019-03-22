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
        protected Term<T> root;
    }

}
