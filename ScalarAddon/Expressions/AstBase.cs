using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstralVars.Expressions
{
    /// <summary>
    /// Абстрактный узел Абстрактного синтаксического дерева (Abstract syntas Tree)    /// 
    /// </summary>
    public abstract class AST
    {
        /// <summary>
        /// Вычислить значение терма
        /// </summary>
        public abstract object Result { get; }

        public virtual string Description(string prefix = "")
        {
            return prefix + string.Empty;
        }
    }

    /// <summary>
    /// Типизированный узел AST
    /// </summary>
    public abstract class AstNode<T> : AST
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

    }

    /// <summary>
    /// Узел AST, представляющий бинарный оператор 
    /// (имеющий две ветви)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BinOperator<T> : AstNode<T>
    {
        //protected AstNode<T> root;
        protected AstNode<T> leftOperand;
        protected AstNode<T> rightOperand;

        public override string Description(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(prefix).Append(GetType().Name).Append(':');
            sb.AppendLine().Append(prefix).Append('{');
            sb.AppendLine().Append(leftOperand.Description(string.IsNullOrEmpty(prefix) ? Parser.Symbols.Tab.ToString() : prefix + prefix));
            sb.AppendLine().Append(rightOperand.Description(string.IsNullOrEmpty(prefix) ? Parser.Symbols.Tab.ToString() : prefix + prefix));
            sb.AppendLine().Append(prefix).Append('}');
            return sb.ToString();

            //return prefix + GetType().Name + ":\n" +
            //    leftOperand.Description(string.IsNullOrEmpty(prefix) ? Parser.Symbols.Tab.ToString() : prefix + prefix) +
            //    rightOperand.Description(string.IsNullOrEmpty(prefix) ? Parser.Symbols.Tab.ToString() : prefix + prefix);
        }
    }

    /// <summary>
    /// Узел AST, представляющий унарный оператор 
    /// (имеющий одну ветвь)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UnOperator<T> : AstNode<T>
    {
        //protected AstNode<T> root;
        protected AstNode<T> Operand;

        public override string Description(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(prefix).Append(GetType().Name).Append(':');
            sb.AppendLine().Append(prefix).Append('{');
            sb.AppendLine().Append(Operand.Description(string.IsNullOrEmpty(prefix) ? Parser.Symbols.Tab.ToString() : prefix + prefix));
            sb.AppendLine().Append(prefix).Append('}');
            return sb.ToString();

            //return prefix + GetType().Name + ":\n" +
            //       Operand.Description(string.IsNullOrEmpty(prefix) ? Parser.Symbols.Tab.ToString() : prefix + prefix);
        }
    }
}
