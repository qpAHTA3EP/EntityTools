using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberAstNode = AstralVars.Expressions.AstNode<double>;

namespace AstralVars.Expressions
{

    /// <summary>
    /// Счетчик внутриигновый предметов (Item) в сумке персонажа, заданных шаблоном ItemId
    /// </summary>
    public class ItemCountNode : NumberAstNode
    {
        /// <summary>
        /// Идентификатор предметов
        /// </summary>
        public string ItemId { get; set; }

        public ItemCountNode(string id)
        {
            ItemId = id;
        }

        public override bool Calculate(out double result)
        {
            //throw new NotImplementedException("Операция подсчета числа предметов не реализована");
            result = 0;
            return false;
        }

        public override string Description(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine().Append(prefix).Append(GetType().Name).Append(" {Id='").Append(ItemId).Append("'}");
            return sb.ToString();
            //return $"{GetType().Name} {{Id='{ItemId}'}}";
        }
    }

    /// <summary>
    /// Счетчик внутриигновый ценностей (Numeric) в сумке персонажа, заданных шаблоном NumericId
    /// </summary>
    public class NumericCountNode : NumberAstNode
    {
        /// <summary>
        /// Идентификатор предметов
        /// </summary>
        public string NumericId { get; set; }

        public NumericCountNode(string id)
        {
            NumericId = id;
        }

        public override bool Calculate(out double result)
        {
            //throw new NotImplementedException("Операция подсчета числа предметов не реализована");
            result = 0;

            return false;
        }

        public override string Description(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(prefix).Append(GetType().Name).Append(" {Id='").Append(NumericId).Append("'}");
            return sb.ToString();
            //return $"{GetType().Name} {{Id='{NumericId}'}}";
        }
    }

    /// <summary>
    /// случайное число, в диапазоне от [0, Operand]
    /// </summary>
    public class RandomNode : UnOperator<double>
    {
        [NonSerialized]
        private static Random rand = new Random();

        public RandomNode(NumberAstNode max)
        {
            Operand = max;
        }

        public override bool Calculate(out double result)
        {

            if (Operand == null || !Operand.Calculate(out double max))
                result = rand.Next();
            else result = rand.Next() % max;

            return true;
        }

        public override string Description(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(prefix).Append(GetType().Name).Append(" { Max=");
            sb.AppendLine().Append(prefix).Append('{');
            sb.AppendLine().Append(Operand.Description(string.IsNullOrEmpty(prefix) ? Parser.Symbols.Tab.ToString() : prefix + prefix));
            sb.AppendLine().Append(prefix).Append("}}");
            return sb.ToString();
            //return $"{GetType().Name} {{Max='{Operand.ToString()}'}}";
        }
    }

}
