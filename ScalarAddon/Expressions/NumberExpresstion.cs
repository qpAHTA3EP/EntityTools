using AstralVars.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstralVars.Expressions.Numbers
{
    public class NumberExpresstion : Expression<double>
    {
        public override bool IsValid => !string.IsNullOrEmpty(expression) && root != null;

        public override bool Calcucate(out object result)
        {
            result = null;
            double res = 0;
            if (string.IsNullOrEmpty(expression))
                return false;

            if (root != null && root.Calculate(out res))
            {
                result = res;
                return true;
            }

            return false;
        }

        public override ParseStatus Parse()
        {
            ParseStatus status = new ParseStatus();
            if(string.IsNullOrEmpty(expression))
            {
                status.Sucseeded = false;
                status.Message = "Expression string is empty";
                return status;
            }


            return status;
        }
    }
}
