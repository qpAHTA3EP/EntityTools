using System.Text;
using VariableTools.Classes;

namespace VariableTools.Expressions.Operand
{
    /// <summary>
    /// Лист AST - числова константа
    /// </summary>
    public class NumberConstant : AstNode<double>
    {
        protected double val;

        public NumberConstant(double v = 0)
        {
            val = v;
        }

        public override bool Calculate(out double result)
        {
            result = val;
            return true;
        }

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {").Append(val).Append('}');
            return sb.ToString();
        }
    }

    /// <summary>
    /// Лист AST, - Числовая переменная (ссылка на Variable)
    /// </summary>
    public class NumberVariable : AstNode<double>
    {
        VariableCollection.VariableKey varKey;

        public NumberVariable() { }
        public NumberVariable(string name, AccountScopeType accScope, ProfileScopeType profScope)
        {
            varKey = new VariableCollection.VariableKey(name, accScope, profScope);
        }
        public NumberVariable(VariableCollection.VariableKey vKey)
        {
            varKey = vKey;
        }

        public override bool Calculate(out double result)
        {
            if(VariableTools.Variables.TryGetValue(out result, varKey))
                return true;
            else
            {
                result = 0;
                return true;
            }
        }

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {").Append(varKey.ToString()).Append(", Value=").Append(Result).Append('}');
            return sb.ToString();            
        }
    }
}
