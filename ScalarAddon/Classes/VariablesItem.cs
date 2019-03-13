﻿using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AstralVars.Classes
{
    /// <summary>
    /// Перечисление типов переменных
    /// </summary>
    public enum VarTypes
    {
        Boolean,
        Integer,
        String,
        //Numeric,
        DateTime,
        Counter
    }

    /// <summary>
    /// Элемент коллекции переменных
    /// </summary>
    public abstract class Variable
    {
        private static Random rand = new Random();

        private Variable()
        {
            throw new NotImplementedException();
        }
        protected Variable(VarTypes tp)
        {
            VarType = tp;            
            Key = $"Var{rand.Next()}";
        }
        protected Variable(VarTypes tp, String k)
        {
            VarType = tp;
            Key = k;
        }

        public string Key;
        public readonly VarTypes VarType;
        public abstract object Value { get; set; }

        /// <summary>
        /// Абстрактынй конструктор копий
        /// </summary>
        /// <returns>Копия объекта</returns>
        public abstract Variable Clone();

        /// <summary>
        /// Конструирование объекта нужного типа
        /// </summary>
        /// <param name="tp">Тип объекта</param>
        /// <returns>Сконструированный объект</returns>
        public static Variable Make(VarTypes tp)
        {
            switch (tp)
            {
                case VarTypes.Boolean:
                    return new BoolVar();
                case VarTypes.Integer:
                    return new IntVar();
                case VarTypes.String:
                    return new StrVar();
                case VarTypes.Counter:
                    return new CounterVar();
                case VarTypes.DateTime:
                    return new DateTimeVar();
                default:
                    return null;
            }
        }

        public static Variable Make(Variable source)
        {
            return source.Clone();
        }

        public static Variable Make(string k, bool val)
        {
            return new BoolVar(k, val);
        }
        public static Variable Make(string k, int val)
        {
            return new IntVar(k, val);
        }
        public static Variable Make(string k, string val)
        {
            if (VariablesParcer.TryParse(val, out int intVal))
                return new IntVar(k, intVal);
            else if (VariablesParcer.TryParse(val, out bool boolVal))
                return new BoolVar(k, boolVal);
            else if (VariablesParcer.TryParse(val, out DateTime dtVal))
                return new DateTimeVar(k, dtVal);
            else if (VariablesParcer.TryParse(val, out string itemId))
                return new CounterVar(k, itemId);
            else
                return new StrVar(k, val);
        }
        public static Variable Make(string k, DateTime val)
        {
            return new DateTimeVar(k, val);
        }
    }

    /// <summary>
    /// Булевая переменная 
    /// </summary>
    public class BoolVar : Variable
    {
        public BoolVar() : base(VarTypes.Boolean) { }
        public BoolVar(string k, bool val = false) : base(VarTypes.Boolean)
        {
            Key = k;
            _varValue = val;
        }
        public BoolVar(string k, object val) : base(VarTypes.Boolean)
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

                    #if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
                    #endif
                }
                else if (bool.TryParse(value.ToString(), out bool newValue))
                {
                    _varValue = newValue;

                    #if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
                    #endif
                }
                else
                {
                    _varValue = false;

                    #if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_varValue}].");
                    #endif
                }
            }
        }

        public override Variable Clone()
        {
            return new BoolVar(Key, _varValue);
        }

        public override string ToString()
        {
            return _varValue.ToString();
        }
    }

    /// <summary>
    /// Переменная - целое число
    /// </summary>
    public class IntVar : Variable
    {
        public IntVar() : base(VarTypes.Integer) {}
        public IntVar(string k, int val = 0) : base(VarTypes.Integer)
        {
            Key = k;
            _varValue = val;
        }
        public IntVar(string k, object val) : base(VarTypes.Integer)
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
                    
#if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
#endif
                }
                else if (int.TryParse(value.ToString(), out int newValue))
                {
                    _varValue = newValue;

#if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
#endif
                }
                else
                {
                    _varValue = 0;

#if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_varValue}].");
#endif
                }
            }
        }

        public override string ToString()
        {
            return _varValue.ToString();
        }

        public override Variable Clone()
        {
            return new IntVar(Key, _varValue);
        }
    }

    /// <summary>
    /// Переменная - строка
    /// </summary>
    public class StrVar : Variable
    {
        public StrVar() : base(VarTypes.String) { }
        public StrVar(string k, string val = "") : base(VarTypes.String)
        {
            Key = k;
            _varValue = val;
        }
        public StrVar(string k, object val) : base(VarTypes.String)
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
                
#if AstralLoaded
                Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
#endif
            }
        }

        public override string ToString()
        {
            return _varValue;
        }

        public override Variable Clone()
        {
            return new StrVar(Key, _varValue);
        }
    }

    /// <summary>
    /// Переменная времени
    /// </summary>
    public class DateTimeVar : Variable
    {
        public DateTimeVar() : base(VarTypes.DateTime) { }
        public DateTimeVar(string k) : base(VarTypes.DateTime)
        {
            Key = k;
            _dtValue = DateTime.Now;
        }
        public DateTimeVar(string k, DateTime val) : base(VarTypes.DateTime)
        {
            Key = k;
            _dtValue = val;
        }
        public DateTimeVar(string k, object val = null) : base(VarTypes.DateTime)
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
                    
#if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_dtValue}] as {VarType}");
#endif
                }
                else if (DateTime.TryParse(value.ToString(), out DateTime newValue))
                {
                    _dtValue = newValue;
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_dtValue}] as {VarType}");
                }
                else
                {
                    _dtValue = DateTime.Now;
                    
#if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_dtValue}].");
#endif
                }
            }
        }

        public override string ToString()
        {
            return _dtValue.ToString();
        }

        public override Variable Clone()
        {
            return new DateTimeVar(Key, _dtValue);
        }
    }

    /// <summary>
    /// Переменная - счетчик предметов 
    /// </summary>
    public class CounterVar : Variable
    {
        public CounterVar() : base(VarTypes.Counter) { }
        public CounterVar(string k) : base(VarTypes.Counter)
        {
            Key = k;
            _itemId = string.Empty;
            _strValue = string.Empty;
        }
        public CounterVar(string k, string val) : base(VarTypes.Counter)
        {
            Key = k;
            if (VariablesParcer.GetItemID(val, out string newVal))
            {
                _itemId = newVal;
                _strValue = val;
                return;
            }
            _itemId = _strValue = val;
        }
        public CounterVar(string k, object val) : base(VarTypes.Counter)
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

        /// <summary>
        /// Исходная неформатированная строка, содержащая вычисляемое выражение
        /// </summary>
        private string _strValue = string.Empty;
        /// <summary>
        /// _itemId содержит идентификатор прeдметов(regex), количество которых подсчитывается
        /// </summary>
        private string _itemId = string.Empty;
        public override object Value
        {
            get
            {
#if AstralLoaded
                uint num = 0;
                if (!(string.IsNullOrEmpty(_itemId) && EntityManager.LocalPlayer.IsValid))
                {
                    List<InventorySlot> slotList = EntityManager.LocalPlayer.AllItems.FindAll(slot => Regex.IsMatch(slot.Item.ItemDef.InternalName, _itemId));
                    foreach (InventorySlot invSlot in slotList)
                        num += invSlot.Item.Count;
                }
#endif
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
                    
#if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{inStr}] as {VarType}");
#endif
                }
                else
                {
                    _itemId = string.Empty;
                    _strValue = string.Empty;

#if AstralLoaded
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{inStr}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_itemId}].");
#endif
                }
            }
        }

        public override string ToString()
        {
            return _strValue;
        }

        public override Variable Clone()
        {
            CounterVar newItmCnt = new CounterVar(Key)
            {
                _itemId = _itemId,
                _strValue = _strValue
            };
            return newItmCnt;
        }
    }

}
