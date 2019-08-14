using AstralVars.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberAstNode = AstralVars.Expressions.AstNode<double>;

namespace AstralVars.Expressions
{
    /// <summary>
    /// Операция сложения чисел
    /// </summary>
    public class Add : BinOperator<double>
    {
		public Add(NumberAstNode left, NumberAstNode right)
		{
			leftOperand = left; rightOperand = right;
		}
        public override bool Calculate(out double result)
        {
            result = 0;

            if (leftOperand == null && rightOperand == null)
                return false;
            if (leftOperand != null && leftOperand.Calculate(out double lRes))
                result += 0;
            if (rightOperand != null && rightOperand.Calculate(out double rRes))
                result += rRes;

            return true;
        }
    }

    /// <summary>
    /// Операция разности чисел
    /// </summary>
    public class Substract : BinOperator<double>
    {
		public Substract(NumberAstNode left, NumberAstNode right)
		{
			leftOperand = left; rightOperand = right;
		}
        public override bool Calculate(out double result)
        {
            result = 0;

            if (leftOperand == null && rightOperand == null)
                return false;
            if (leftOperand != null && leftOperand.Calculate(out double lRes))
                result += lRes;
            if (rightOperand != null && rightOperand.Calculate(out double rRes))
                result -= rRes;

            return true;
        }
    }

    /// <summary>
    /// Операция изменения знака
    /// </summary>
    public class Negate : UnOperator<double>
    {
        public override bool Calculate(out double result)
        {
            result = 0;

            if (Operand == null)
                return false;
            if (Operand.Calculate(out double temRes))
                result = -temRes;

            return false;
        }
    }

    /// <summary>
    /// Операция умножения чисел
    /// </summary>
    public class Multiply : BinOperator<double>
    {
		public Multiply(NumberAstNode left, NumberAstNode right)
		{
			leftOperand = left; rightOperand = right;
		}
        public override bool Calculate(out double result)
        {
            result = 1;

            if (leftOperand == null && rightOperand == null)
                return false;
            if (leftOperand != null && leftOperand.Calculate(out double lRes))
                result = lRes;
            if (rightOperand != null && rightOperand.Calculate(out double rRes))
                result *= rRes;

            return true;
        }
    }

    /// <summary>
    /// Операция деления чисел
    /// </summary>
    public class Devide : BinOperator<double>
    {
		public Devide(NumberAstNode left, NumberAstNode right)
		{
			leftOperand = left; rightOperand = right;
		}
        public override bool Calculate(out double result)
        {
            result = 0;

            if (leftOperand == null || rightOperand == null)
                return false;
            if (leftOperand.Calculate(out double lRes) && rightOperand.Calculate(out double rRes))
                if (rRes != 0)
                    result = lRes / rRes;

            return false;
        }
    }

    /// <summary>
    /// Операция деления чисел
    /// </summary>
    public class Remainde : BinOperator<double>
    {
		public Remainde(NumberAstNode left, NumberAstNode right)
		{
			leftOperand = left; rightOperand = right;
		}
        public override bool Calculate(out double result)
        {
            result = 0;

            if (leftOperand == null || rightOperand == null)
                return false;
            if (leftOperand.Calculate(out double lRes) && rightOperand.Calculate(out double rRes))
                if (rRes != 0)
                    result = lRes % rRes;

            return false;
        }
    }
}
