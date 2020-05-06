﻿using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberAstNode = VariableTools.Expressions.AstNode<double>;

namespace VariableTools.Expressions.Functions
{

    /// <summary>
    /// Счетчик внутриигновый предметов (Item) в сумке персонажа, заданных шаблоном ItemId
    /// </summary>
    public class ItemCount : NumberAstNode
    {
        /// <summary>
        /// Идентификатор предметов
        /// </summary>
        public string ItemId { get; set; }

        public ItemCount(string id)
        {
            ItemId = id;
        }

        public override bool Calculate(out double result)
        {
            result = 0;
#if !TEST
            foreach (InventorySlot slot in EntityManager.LocalPlayer.AllItems)
            {
                if (slot.Item?.ItemDef?.InternalName == ItemId)
                    result += slot.Item.Count;
            }
#endif
            return true;
        }

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {Id='").Append(ItemId).Append("' Value=").Append(Result).Append("}");
            return sb.ToString();
            //return $"{GetType().Name} {{Id='{ItemId}'}}";
        }
    }

    /// <summary>
    /// Счетчик внутриигновый ценностей (Numeric) в сумке персонажа, заданных шаблоном NumericId
    /// </summary>
    public class NumericCount : NumberAstNode
    {
        /// <summary>
        /// Идентификатор предметов
        /// </summary>
        public string NumericId { get; set; }

        public NumericCount(string id)
        {
            NumericId = id;
        }

        public override bool Calculate(out double result)
        {
#if !TEST
            int? res = EntityManager.LocalPlayer?.Inventory?.GetNumericCount(NumericId);
            if (res != null)
                result = (double)res;
            else 
#endif
                result = 0;

            return true;
        }

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {Id='").Append(NumericId).Append("' Value=").Append(Result).Append('}');
            return sb.ToString();
        }
    }

    /// <summary>
    /// случайное число, в диапазоне от [0, Operand]
    /// </summary>
    public class RandomNumber : UnOperator<double>
    {
        [NonSerialized]
        private static Random rand = new Random();

        public RandomNumber(NumberAstNode max)
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

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" { Max = ");
            if(Operand != null)
            {
                sb.Append(Operand.Description(indent + 1)).AppendLine();
                sb.Insert(sb.Length, Parser.Indent, indent).Append('}').AppendLine();
            }
            else sb.Append(int.MaxValue).Append(" }");
            return sb.ToString();
        }
    }
}
