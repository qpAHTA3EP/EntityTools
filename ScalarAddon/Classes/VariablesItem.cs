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
        private static Random rand = new Random();

        private VariableItem()
        {
            throw new NotImplementedException();
        }
        protected VariableItem(VariableTypes tp)
        {
            VarType = tp;            
            Key = $"Var{rand.Next()}";
        }
        protected VariableItem(VariableTypes tp, String k)
        {
            VarType = tp;
            Key = k;
        }

        public string Key;
        public readonly VariableTypes VarType;
        public abstract object Value { get; set; }

        /// <summary>
        /// Абстрактынй конструктор копий
        /// </summary>
        /// <returns>Копия объекта</returns>
        public abstract VariableItem Clone();

        /// <summary>
        /// Конструирование объекта нужного типа
        /// </summary>
        /// <param name="tp">Тип объекта</param>
        /// <returns>Сконструированный объект</returns>
        public static VariableItem Make(VariableTypes tp)
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

        public static VariableItem Make(VariableItem source)
        {
            return source.Clone();
        }

        public static VariableItem Make(string k, bool val)
        {
            return new BooleanVariable(k, val);
        }
        public static VariableItem Make(string k, int val)
        {
            return new IntVariable(k, val);
        }
        public static VariableItem Make(string k, string val)
        {
            if (int.TryParse(val, out int intVal))
                return new IntVariable(k, intVal);
            else if (bool.TryParse(val, out bool boolVal))
                return new BooleanVariable(k, boolVal);
            else if (DateTime.TryParse(val, out DateTime dtVal))
                return new DateTimeVariable(k, dtVal);
            else if (VariablesParcer.GetItemID(val, out string itemId))
                return new ItemsCountVariable(k, itemId);
            else
                return new StringVariable(k, val);
        }
        public static VariableItem Make(string k, DateTime val)
        {
            return new DateTimeVariable(k, val);
        }
    }

    /// <summary>
    /// Булевая переменная 
    /// </summary>
    public class BooleanVariable : VariableItem
    {
        public BooleanVariable() : base(VariableTypes.Boolean) { }
        public BooleanVariable(string k, bool val = false) : base(VariableTypes.Boolean)
        {
            Key = k;
            _varValue = val;
        }
        public BooleanVariable(string k, object val) : base(VariableTypes.Boolean)
        {
            Key = k;
            if (val != null)
            {
                if (val is bool)
                {
                    _varValue = (bool)val;
                    return;
                }
                else if (bool.TryParse(val.ToString(), out bool newValue))
                {
                    _varValue = newValue;
                    return;
                }
            }
            _varValue = false;
        }

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

        public override VariableItem Clone()
        {
            return new BooleanVariable(Key, _varValue);
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
        public IntVariable(string k, int val = 0) : base(VariableTypes.Integer)
        {
            Key = k;
            _varValue = val;
        }
        public IntVariable(string k, object val) : base(VariableTypes.Integer)
        {
            Key = k;
            if (val != null)
            {
                if (val is int)
                {
                    _varValue = (int)val;
                    return;
                }
                else if (int.TryParse(val.ToString(), out int newValue))
                {
                    _varValue = newValue;
                    return;
                }
            }
            _varValue = 0;
        }

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

        public override VariableItem Clone()
        {
            return new IntVariable(Key, _varValue);
        }
    }

    /// <summary>
    /// Переменная - строка
    /// </summary>
    public class StringVariable : VariableItem
    {
        public StringVariable() : base(VariableTypes.String) { }
        public StringVariable(string k, string val = "") : base(VariableTypes.String)
        {
            Key = k;
            _varValue = val;
        }
        public StringVariable(string k, object val) : base(VariableTypes.String)
        {
            Key = k;
            if (val != null)
            {
                _varValue = val.ToString();
            }
            _varValue = string.Empty;
        }


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

        public override VariableItem Clone()
        {
            return new StringVariable(Key, _varValue);
        }
    }

    /// <summary>
    /// Переменная времени
    /// </summary>
    public class DateTimeVariable : VariableItem
    {
        public DateTimeVariable() : base(VariableTypes.DateTime) { }
        public DateTimeVariable(string k) : base(VariableTypes.DateTime)
        {
            Key = k;
            _dtValue = DateTime.Now;
        }
        public DateTimeVariable(string k, DateTime val) : base(VariableTypes.DateTime)
        {
            Key = k;
            _dtValue = val;
        }
        public DateTimeVariable(string k, object val = null) : base(VariableTypes.DateTime)
        {
            Key = k;
            if (val != null)
            {
                if (val is DateTime)
                {
                    _dtValue = (DateTime)val;
                    return;
                }
                else if (DateTime.TryParse(val.ToString(), out DateTime newValue))
                {
                    _dtValue = newValue;
                    return;
                }
            }
            _dtValue = DateTime.Now;
        }

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

        public override VariableItem Clone()
        {
            return new DateTimeVariable(Key, _dtValue);
        }
    }

    /// <summary>
    /// Переменная - счетчик предметов 
    /// </summary>
    public class ItemsCountVariable : VariableItem
    {
        public ItemsCountVariable() : base(VariableTypes.ItemsCount) { }
        public ItemsCountVariable(string k) : base(VariableTypes.ItemsCount)
        {
            Key = k;
            _itemId = string.Empty;
            _strValue = string.Empty;
        }
        public ItemsCountVariable(string k, string itemId) : base(VariableTypes.ItemsCount)
        {
            Key = k;
            _itemId = itemId;
            _strValue = itemId;
        }
        public ItemsCountVariable(string k, object val) : base(VariableTypes.ItemsCount)
        {
            Key = k;
            if (val != null)
            {
                string inStr = val as string;
                if (!string.IsNullOrEmpty(inStr))
                {
                    if (VariablesParcer.GetItemID(inStr, out string newVal))
                    {
                        _itemId = newVal;
                        _strValue = inStr;
                        return;
                    }
                }
            }
            _itemId = string.Empty;
            _strValue = string.Empty;
        }


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
                string inStr = value as string,
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

        public override VariableItem Clone()
        {
            ItemsCountVariable newItmCnt = new ItemsCountVariable(Key)
            {
                _itemId = _itemId,
                _strValue = _strValue
            };
            return newItmCnt;
        }
    }

}
