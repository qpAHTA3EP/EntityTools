using AstralVars.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstralVars.Expressions
{
    /// <summary>
    /// Лист AST - числова константа
    /// </summary>
    public class NumberConstantNode : AstNode<double>
    {
        protected double val;

        public NumberConstantNode(double v = 0)
        {
            val = v;
        }

        public override bool Calculate(out double result)
        {
            result = val;
            return true;
        }

        public override string Description(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(prefix).Append(GetType().Name).Append(" {").Append(val).Append('}');
            return sb.ToString();
            //return $"{GetType().Name} {{{val}}}";
        }
    }

    /// <summary>
    /// Лист AST, - Числовая переменная (ссылка на Variable)
    /// </summary>
    public class NumberVariableNode : AstNode<double>
    {
        protected NumVar val;

        public NumberVariableNode()
        {
            val = new NumVar();
        }
        public NumberVariableNode(string var_name)
        {
            val = new NumVar(var_name);
        }

        public override bool Calculate(out double result)
        {
            result = val.ReadValue;
            return true;
        }

        public override string Description(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(prefix).Append(GetType().Name).Append(" {Key='").Append(val.Key).Append("', Value=").Append(val.Value).Append('}');
            return sb.ToString();
            //return $"{GetType().Name} {{Key='{val.Key}', Value={val.Value}}}";
        }
    }
}
