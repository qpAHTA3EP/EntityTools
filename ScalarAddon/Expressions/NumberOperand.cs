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
    public class NumConstant : Term<double>
    {
        protected double val;

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
    public class NumVariable : Term<double>
    {
        protected NumVar val;

        public override bool Calculate(out double result)
        {
            result = val.ReadValue;
            return true;
        }

        public override uint Priority { get => 1; }
    }
}
