using AstralVariables.Expressions;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace AstralVariables.Classes
{
    /// <summary>
    /// Перечисление типов переменных
    /// </summary>
    [Serializable]
    public enum VarTypes
    {
        Boolean,
        Number,
        String,
        //Numeric,
        //Counter,
        DateTime        
    }

    /// <summary>
    /// Элемент коллекции переменных
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(NumVar))]
    [XmlInclude(typeof(BoolVar))]
    [XmlInclude(typeof(StrVar))]
    [XmlInclude(typeof(DateTimeVar))]
    public abstract class Variable
    {
        [NonSerialized]
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

        /// <summary>
        /// Имя (ключ) переменной
        /// </summary>
        public string Key;
        /// <summary>
        /// Тип переменной
        /// </summary>
        public readonly VarTypes VarType;
        /// <summary>
        /// Значение переменной
        /// </summary>
        public abstract object Value { get; set; }
        /// <summary>
        /// внутренний тип данных переменной
        /// </summary>
        public abstract Type Type { get; }

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
                case VarTypes.Number:
                    return new NumVar();
                case VarTypes.String:
                    return new StrVar();
                //case VarTypes.Counter:
                //    return new CounterVar();
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
        public static Variable Make(string k, double val)
        {
            return new NumVar(k, val);
        }
        public static Variable Make(string k, DateTime val)
        {
            return new DateTimeVar(k, val);
        }
        public static Variable Make(string k, string val)
        {
            if (Parser.TryParse(val, out double numVal))
                return new NumVar(k, numVal);
            else if (Parser.TryParse(val, out bool boolVal))
                return new BoolVar(k, boolVal);
            else if (Parser.TryParse(val, out DateTime dtVal))
                return new DateTimeVar(k, dtVal);
            //else if (Parser.TryParse(val, out string itemId))
            //    return new CounterVar(k, itemId);
            else
                return new StrVar(k, val);
        }

        public override string ToString()
        {
            return $"{Key}:= ({VarType}){Value}";
        }

    }

    /// <summary>
    /// Булевая переменная 
    /// </summary>
    [Serializable]
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

        [NonSerialized]
        public static readonly bool Default = false;

        [NonSerialized]
        public static readonly Type RealType = typeof(bool);

        [NonSerialized]
        private bool _varValue;

        public override object Value
        {
            get => _varValue;
            set
            {
                if (value is bool)
                {
                    _varValue = (bool)value;

                    #if ASTRAL_LOGGER
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
                    #endif
                }
                else if (bool.TryParse(value.ToString(), out bool newValue))
                {
                    _varValue = newValue;

                    #if ASTRAL_LOGGER
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
                    #endif
                }
                else
                {
                    _varValue = false;

                    #if ASTRAL_LOGGER
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_varValue}].");
                    #endif
                }
            }
        }

        public bool ReadValue
        {
            get => _varValue;
            set => _varValue = value;
        }

        public override Type Type => _varValue.GetType();

        public override Variable Clone()
        {
            return new BoolVar(Key, _varValue);
        }
    }

    /// <summary>
    /// Переменная - действительное число
    /// </summary>
    [Serializable]
    public class NumVar : Variable
    {
        public NumVar() : base(VarTypes.Number) {}
        public NumVar(string k, double val = 0) : base(VarTypes.Number)
        {
            Key = k;
            _varValue = val;
        }
        public NumVar(string k, object val) : base(VarTypes.Number)
        {
            Key = k;
            if (val != null)
            {
                if (val is double)
                {
                    _varValue = (double)val;
                    return;
                }
                else if (double.TryParse(val.ToString(), out double newValue))
                {
                    _varValue = newValue;
                    return;
                }
            }
            _varValue = 0;
        }

        [NonSerialized]
        public static readonly double Default = 0;

        [NonSerialized]
        public static readonly Type RealType = typeof(double);

        [NonSerialized]
        private double _varValue;

        public override object Value
        {
            get => _varValue;
            set
            {
                if (value is double)
                {
                    _varValue = (double)value;
                    
#if ASTRAL_LOGGER
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
#endif
                }
                else if (double.TryParse(value.ToString(), out double newValue))
                {
                    _varValue = newValue;

#if ASTRAL_LOGGER
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
#endif
                }
                else
                {
                    _varValue = 0;

#if ASTRAL_LOGGER
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_varValue}].");
#endif
                }
            }
        }

        public double ReadValue
        {
            get => _varValue;
            set => _varValue = value;
        }

        public override Type Type => _varValue.GetType();

        public override Variable Clone()
        {
            return new NumVar(Key, _varValue);
        }
    }

    /// <summary>
    /// Переменная - строка
    /// </summary>
    [Serializable]
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


        [NonSerialized]
        public static readonly string Default = string.Empty;

        [NonSerialized]
        public static readonly Type RealType = typeof(string);

        [NonSerialized]
        private string _varValue;

        public override object Value
        {
            get => _varValue;
            set
            {
                _varValue = value.ToString();
                
#if ASTRAL_LOGGER
                Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_varValue}] as {VarType}");
#endif
            }
        }

        public string ReadValue
        {
            get => _varValue;
            set => _varValue = value;
        }

        public override Type Type => _varValue.GetType();

        public override Variable Clone()
        {
            return new StrVar(Key, _varValue);
        }
    }

    /// <summary>
    /// Переменная времени
    /// </summary>
    [Serializable]
    public class DateTimeVar : Variable
    {
        public DateTimeVar() : base(VarTypes.DateTime) { }
        public DateTimeVar(string k) : base(VarTypes.DateTime)
        {
            Key = k;
            _dtValue = Default;
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
            _dtValue = Default;
        }

        [NonSerialized]
        public static readonly DateTime Default = DateTime.Now;

        [NonSerialized]
        public static readonly Type RealType = typeof(DateTime);

        [NonSerialized]
        private DateTime _dtValue;

        public override object Value
        {
            get => _dtValue;
            set
            {
                if (value is DateTime)
                {
                    _dtValue = (DateTime)value;
                    
#if ASTRAL_LOGGER
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{_dtValue}] as {VarType}");
#endif
                }
                else if (DateTime.TryParse(value.ToString(), out DateTime newValue))
                {
                    _dtValue = newValue;
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesAddon)} Variable '{Key}' set to [{_dtValue}] as {VarType}");
                }
                else
                {
                    _dtValue = Default;
                    
#if ASTRAL_LOGGER
                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{value}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_dtValue}].");
#endif
                }
            }
        }

        public DateTime ReadValue
        {
            get => _dtValue;
            set => _dtValue = value;
        }

        public override Type Type => _dtValue.GetType();

        public override Variable Clone()
        {
            return new DateTimeVar(Key, _dtValue);
        }
    }

    /// <summary>
    /// Переменная - счетчик предметов 
    /// </summary>
//    public class CounterVar : Variable
//    {
//        public CounterVar() : base(VarTypes.Counter) { }
//        public CounterVar(string k) : base(VarTypes.Counter)
//        {
//            Key = k;
//            _itemId = string.Empty;
//            _strValue = string.Empty;
//        }
//        public CounterVar(string k, string val) : base(VarTypes.Counter)
//        {
//            Key = k;
//            if (Parser.GetItemID(val, out string newVal))
//            {
//                _itemId = newVal;
//                _strValue = val;
//                return;
//            }
//            _itemId = _strValue = val;
//        }
//        public CounterVar(string k, object val) : base(VarTypes.Counter)
//        {
//            Key = k;
//            if (val != null)
//            {
//                string inStr = val as string;
//                if (!string.IsNullOrEmpty(inStr))
//                {
//                    if (Parser.GetItemID(inStr, out string newVal))
//                    {
//                        _itemId = newVal;
//                        _strValue = inStr;
//                        return;
//                    }
//                }
//            }
//            _itemId = string.Empty;
//            _strValue = string.Empty;
//        }

//        /// <summary>
//        /// Исходная неформатированная строка, содержащая вычисляемое выражение
//        /// </summary>
//        private string _strValue = string.Empty;
//        /// <summary>
//        /// _itemId содержит идентификатор прeдметов(regex), количество которых подсчитывается
//        /// </summary>
//        private string _itemId = string.Empty;
//        public override object Value
//        {
//            get
//            {
//#if ASTRAL_LOADED
//                uint num = 0;
//                if (!(string.IsNullOrEmpty(_itemId) && EntityManager.LocalPlayer.IsValid))
//                {
//                    List<InventorySlot> slotList = EntityManager.LocalPlayer.AllItems.FindAll(slot => Regex.IsMatch(slot.Item.ItemDef.InternalName, _itemId));
//                    foreach (InventorySlot invSlot in slotList)
//                        num += invSlot.Item.Count;
//                }
//#endif
//                return 0u;
//            }

//            set
//            {
//                string inStr = value as string,
//                       newVal = string.Empty;

//                if(Parser.GetItemID(inStr, out newVal))
//                { 
//                    _itemId = newVal;
//                    _strValue = inStr;
                    
//#if ASTRAL_LOGGER
//                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Variable '{Key}' set to [{inStr}] as {VarType}");
//#endif
//                }
//                else
//                {
//                    _itemId = string.Empty;
//                    _strValue = string.Empty;

//#if ASTRAL_LOGGER
//                    Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} Invalid value [{inStr}] to Variable '{Key}' type of {VarType}. Variable '{Key}' reseted to [{_itemId}].");
//#endif
//                }
//            }
//        }

//        public override string ToString()
//        {
//            return _strValue;
//        }

//        public override Variable Clone()
//        {
//            CounterVar newItmCnt = new CounterVar(Key)
//            {
//                _itemId = _itemId,
//                _strValue = _strValue
//            };
//            return newItmCnt;
//        }
//    }

}
