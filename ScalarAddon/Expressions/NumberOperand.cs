using AstralVars.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstralVars.Expressions.Numbers
{
    /// <summary>
    /// Числовая константа
    /// </summary>
    public class ConstantNode : Term<double>
    {
        protected double val;

        public ConstantNode(double v = 0)
        {
            val = v;
        }

        public override bool Calculate(out double result)
        {
            result = val;
            return true;
        }

        public override uint Priority { get => 1; }
    }

    /// <summary>
    /// Числовая переменная - ссылка на Variable
    /// </summary>
    public class VariableNode : Term<double>
    {
        protected NumVar val;

        public VariableNode()
        {
            val = new NumVar();
        }
        public VariableNode(string var_name)
        {
            val = new NumVar(var_name);
        }

        public override bool Calculate(out double result)
        {
            result = val.ReadValue;
            return true;
        }

        public override uint Priority { get => 1; }
    }
}
