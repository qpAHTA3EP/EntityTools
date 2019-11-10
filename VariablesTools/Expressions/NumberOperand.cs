using System.Text;

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
            StringBuilder sb = new StringBuilder();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {").Append(val).Append('}');
            return sb.ToString();
            //return $"{GetType().Name} {{{val}}}";
        }
    }

    /// <summary>
    /// Лист AST, - Числовая переменная (ссылка на Variable)
    /// </summary>
    public class NumberVariable : AstNode<double>
    {
        //protected NumVar val;
        protected string variableName;

        public NumberVariable()
        {
            //val = new NumVar();
        }
        public NumberVariable(string var_name)
        {
            //val = new NumVar(var_name);
            variableName = var_name;
        }

        public override bool Calculate(out double result)
        {
            //result = val.ReadValue;

            if(VariablesTools.Variables.TryGetValue(variableName, out result))
                return true;
            else
            {
                result = 0;
                return true;
            }
        }

        public override string Description(int indent = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {Key='").Append(variableName).Append("', Value=").Append(Result).Append('}');
            return sb.ToString();            
        }
    }
}
