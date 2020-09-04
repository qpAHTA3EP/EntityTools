using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableTools.Expressions
{
    /// <summary>
    /// Абстрактный узел Абстрактного синтаксического дерева (Abstract syntas Tree)    /// 
    /// </summary>
    public abstract class AST
    {
        protected StringBuilder sb = new StringBuilder();

        /// <summary>
        /// Вычислить значение терма
        /// </summary>
        public abstract object Result { get; }

        public virtual string Description(int indent = 0)
        {
            return string.Empty;
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
        protected AstNode<T> leftOperand;
        protected AstNode<T> rightOperand;

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.AppendCopies(Parser.Indent, indent).Append(GetType().Name).Append(':').AppendLine();
            sb.AppendCopies(Parser.Indent, indent).Append('{').AppendLine();
            sb.Append(leftOperand.Description(indent+1)).AppendLine();
            sb.Append(rightOperand.Description(indent + 1)).AppendLine();
            sb.AppendCopies(Parser.Indent, indent).Append('}')/*.AppendLine()*/;
            return sb.ToString();
        }
    }

    /// <summary>
    /// Узел AST, представляющий унарный оператор 
    /// (имеющий одну ветвь)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UnOperator<T> : AstNode<T>
    {
        protected AstNode<T> Operand;

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.AppendCopies(Parser.Indent, indent).Append(GetType().Name).Append(':').AppendLine();
            sb.AppendCopies(Parser.Indent, indent).Append('{').AppendLine();
            sb.Append(Operand.Description(indent + 1)).AppendLine();
            sb.AppendCopies(Parser.Indent, indent).Append('}')/*.AppendLine()*/;
            return sb.ToString();
        }
    }
}
