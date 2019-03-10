using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ValiablesAstralExtention.Classes
{
    /// <summary>
    /// Перечисление типов переменных
    /// </summary>
    public enum VariableTypes
    {
        Boolean,
        Integer,
        String,
        //Numeric,
        DateTime,
        ItemsCount
    }

    /// <summary>
    /// Элемент коллекции переменных
    /// </summary>
    public abstract class VariableItem
    {
        private VariableItem() { }
        protected VariableItem(VariableTypes tp)
        { VarType = tp; }

        public string Key;
        public readonly VariableTypes VarType;
        public abstract object Value { get; set; }

        /// <summary>
        /// Конструирование объекта нужного типа
        /// </summary>
        /// <param name="tp">Тип объекта</param>
        /// <returns>Сконструированный объект</returns>
        public VariableItem Make(VariableTypes tp)
        {
            switch (tp)
            {
                case VariableTypes.Boolean:
                    return new BooleanVariable();
                case VariableTypes.Integer:
                    return new IntVariable();
                case VariableTypes.String:
                    return new StringVariable();
                case VariableTypes.ItemsCount:
                    return new ItemsCountVariable();
                case VariableTypes.DateTime:
                    return new DateTimeVariable();
                default:
                    return null;
            }
        }

        public object GetValue()
        {
            return Value;
        }
    }

    /// <summary>
    /// Булевая переменная 
    /// </summary>
    public class BooleanVariable : VariableItem
    {
        public BooleanVariable() : base(VariableTypes.Boolean) { }

        private bool _varValue;
        public override object Value
        {
            get => _varValue;
            set
            {
                if (value is bool)
                {
                    _varValue = (bool)value;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
                }
                else if (bool.TryParse(value.ToString(), out bool newValue))
                {
                    _varValue = newValue;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
                }
                else
                {
                    _varValue = false;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_varValue}].");
                }
            }
        }

        public override string ToString()
        {
            return _varValue.ToString();
        }
    }

    /// <summary>
    /// Переменная - целое число
    /// </summary>
    public class IntVariable : VariableItem
    {
        public IntVariable() : base(VariableTypes.Integer) {}

        private int _varValue;
        public override object Value
        {
            get => _varValue;
            set
            {
                if (value is int)
                {
                    _varValue = (int)value;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
                }
                else if (int.TryParse(value.ToString(), out int newValue))
                {
                    _varValue = newValue;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
                }
                else
                {
                    _varValue = 0;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_varValue}].");
                }
            }
        }

        public override string ToString()
        {
            return _varValue.ToString();
        }
    }

    /// <summary>
    /// Переменная - строка
    /// </summary>
    public class StringVariable : VariableItem
    {
        public StringVariable() : base(VariableTypes.String) { }

        private string _varValue;
        public override object Value
        {
            get => _varValue;
            set
            {
                _varValue = value.ToString();
                Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
            }
        }

        public override string ToString()
        {
            return _varValue;
        }
    }

    /// <summary>
    /// Переменная времени
    /// </summary>
    public class DateTimeVariable : VariableItem
    {
        public DateTimeVariable() : base(VariableTypes.DateTime) { }

        private DateTime _dtValue;
        public override object Value
        {
            get => _dtValue;
            set
            {
                if (value is DateTime)
                {
                    _dtValue = (DateTime)value;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_dtValue}] as {VarType}");
                }
                else if (DateTime.TryParse(value.ToString(), out DateTime newValue))
                {
                    _dtValue = newValue;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_dtValue}] as {VarType}");
                }
                else
                {
                    _dtValue = DateTime.Now;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_dtValue}].");
                }
            }
        }

        public override string ToString()
        {
            return _dtValue.ToString();
        }
    }
    /// <summary>
    /// Переменная - счетчик предметов 
    /// </summary>
    public class ItemsCountVariable : VariableItem
    {
        public ItemsCountVariable() : base(VariableTypes.ItemsCount) { }

        private string _strValue = string.Empty;
        /// <summary>
        /// _itemId содержит идентификатор прeдметов(regex), количество которых подсчитывается
        /// </summary>
        private string _itemId = string.Empty;
        public override object Value
        {
            get
            {
                uint num = 0;
                if (!(string.IsNullOrEmpty(_itemId) && EntityManager.LocalPlayer.IsValid))
                {
                    List<InventorySlot> slotList = EntityManager.LocalPlayer.AllItems.FindAll(slot => Regex.IsMatch(slot.Item.ItemDef.InternalName, _itemId));
                    foreach (InventorySlot invSlot in slotList)
                        num += invSlot.Item.Count;
                }
                return 0u;
            }

            set
            {
                string inStr = value as String,
                       newVal = string.Empty;

                if(VariablesParcer.GetItemID(inStr, out newVal))
                { 
                    _itemId = newVal;
                    _strValue = inStr;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{inStr}] as {VarType}");
                }
                else
                {
                    _itemId = string.Empty;
                    _strValue = string.Empty;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{inStr}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_itemId}].");
                }
            }
        }

        public override string ToString()
        {
            return _strValue;
        }
    }

}
