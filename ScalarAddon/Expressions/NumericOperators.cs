using AstralVars.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstralVars.Expressions.Numbers
{
    /// <summary>
    /// Операция сложения чисел
    /// </summary>
    public class Add : BinOperator<double>
    {
        public override bool Calculate(out double result)
        {
            double  lRes = 0,
                    rRes = 0;
            result = 0;

            if (leftOperand == null && rightOperand == null)
                return false;
            if (leftOperand != null && leftOperand.Calculate(out lRes))
                result += lRes;
            if (rightOperand != null && rightOperand.Calculate(out rRes))
                result += rRes;

            return true;
        }

        public override uint Priority { get => 3; }
    }

    /// <summary>
    /// Операция разности чисел
    /// </summary>
    public class Subtract : BinOperator<double>
    {
        public override bool Calculate(out double result)
        {
            double lRes = 0,
                    rRes = 0;
            result = 0;

            if (leftOperand == null && rightOperand == null)
                return false;
            if (leftOperand != null && leftOperand.Calculate(out lRes))
                result += lRes;
            if (rightOperand != null && rightOperand.Calculate(out rRes))
                result -= rRes;

            return true;
        }

        public override uint Priority { get => 3; }
    }

    /// <summary>
    /// Операция изменения знака
    /// </summary>
    public class Negate : UnOperator<double>
    {
        public override bool Calculate(out double result)
        {
            double temRes = 0;
            result = 0;

            if (Operand == null)
                return false;
            if (Operand.Calculate(out temRes))
                result = -temRes;

            return false;
        }

        public override uint Priority { get => 3; }
    }

    /// <summary>
    /// Счетчик внутриигновый предметов в сумке персонажа, заданных шаблоном ItemId
    /// </summary>
    public class Counter : UnOperator<double>
    {
        public override bool Calculate(out double result)
        {
            throw new NotImplementedException("Операция подсчета числа предметов не реализована");

            result = NumVar.Default;
            return false;
        }

        public override uint Priority { get => 1; }
    }

    /// <summary>
    /// случайное число, в диапазоне от [0, Operand]
    /// </summary>
    public class Rand : UnOperator<double>
    {
        [NonSerialized]
        private static Random rand = new Random();

        public override bool Calculate(out double result)
        {
            double max = 0;

            if (Operand == null || !Operand.Calculate(out max))
                result = rand.Next();
            else result = rand.Next() % max;

            return true;
        }

        public override uint Priority { get => 1; }
    }

    /// <summary>
    /// Операция умножения чисел
    /// </summary>
    public class Multiply : BinOperator<double>
    {
        public override bool Calculate(out double result)
        {
            double lRes = 0,
                    rRes = 0;
            result = 1;

            if (leftOperand == null && rightOperand == null)
                return false;
            if (leftOperand != null && leftOperand.Calculate(out lRes))
                result = lRes;
            if (rightOperand != null && rightOperand.Calculate(out rRes))
                result *= rRes;

            return true;
        }

        public override uint Priority { get => 2; }
    }

    /// <summary>
    /// Операция деления чисел
    /// </summary>
    public class Devide : BinOperator<double>
    {
        public override bool Calculate(out double result)
        {
            double lRes = 0,
                    rRes = 0;
            result = 0;

            if (leftOperand == null || rightOperand == null)
                return false;
            if (leftOperand.Calculate(out lRes) && rightOperand.Calculate(out rRes))
                if (rRes != 0)
                    result = lRes / rRes;

            return false;
        }

        public override uint Priority { get => 2; }
    }
}
