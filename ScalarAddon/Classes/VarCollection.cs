﻿using System;
using System.Collections.ObjectModel;

namespace AstralVars.Classes
{
    public class VarCollection : KeyedCollection<string, Variable>
    {
        protected override string GetKeyForItem(Variable item)
        {
            return item.Key;
        }
        /// <summary>
        /// Поиск переменной в коллекции по ключу 'key'
        /// </summary>
        /// <param name="key">Имя искомой переменной</param>
        /// <param name="val">Возвращаемое значение переменной, хранящейся в коллекции.
        /// null,  если значение ключа 'key' было пустым или null.</param>
        /// <returns>true - переменная с ключем 'key' найдена в коллекции
        /// false - переменная с ключем 'key' в коллекции отсутствует</returns>
        public bool Get(string key, out Variable val)
        {
            if (string.IsNullOrEmpty(key))
            {
                val = null;
                return false;
            }

            if (Dictionary.TryGetValue(key, out val))
                return false;
            else return true;
        }


        /// <summary>
        /// Установить переменной с именем 'key' значение 'val'
        /// Если такой переменной нет в коллекции - она создается
        /// Если тип переменной не совпадает - тип переменной меняется
        /// </summary>
        /// <param name="key">Имя (ключ) переменной. Пустое значение не докупскается.</param>
        /// <param name="val">Новое значение переменной</param>
        /// <returns>Переменная, добавленная в коллекцию.
        /// null, если значение ключа 'key' было пустым или null</returns>
        public Variable Set(string key, int val)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (Dictionary.TryGetValue(key, out Variable var))
            {
                if (var.VarType == VarTypes.Integer)
                {
                    var.Value = val;
                    return var;
                }
                else Dictionary.Remove(key);
            }
            var = Variable.Make(key, val);
            Add(var);
            return var;
        }

        /// <summary>
        /// Установить переменной с именем 'key' значение 'val'
        /// Если такой переменной нет в коллекции - она создается
        /// Если тип переменной не совпадает - тип переменной меняется
        /// </summary>
        /// <param name="key">Имя (ключ) переменной. Пустое значение не докупскается.</param>
        /// <param name="val">Новое значение переменной</param>
        /// <returns>Переменная, добавленная в коллекцию.
        /// null, если значение ключа 'key' было пустым или null</returns>
        public Variable Set(string key, bool val)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (Dictionary.TryGetValue(key, out Variable var))
            {
                if (var.VarType == VarTypes.Boolean)
                {
                    var.Value = val;
                    return var;
                }
                else Dictionary.Remove(key);
            }
            var = Variable.Make(key, val);
            Add(var);
            return var;
        }

        /// <summary>
        /// Установить переменной с именем 'key' значение 'val'
        /// Если такой переменной нет в коллекции - она создается
        /// Если тип переменной не совпадает - тип переменной меняется
        /// </summary>
        /// <param name="key">Имя (ключ) переменной. Пустое значение не докупскается.</param>
        /// <param name="val">Новое значение переменной</param>
        /// <returns>Переменная, добавленная в коллекцию.
        /// null, если значение ключа 'key' было пустым или null</returns>
        public Variable Set(string key, string val)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (Dictionary.TryGetValue(key, out Variable var))
            {
                if (var.VarType == VarTypes.String)
                {
                    var.Value = val;
                    return var;
                }
                else Dictionary.Remove(key);
            }
            var = Variable.Make(key, val);
            Add(var);
            return var;
        }

        /// <summary>
        /// Установить переменной с именем 'key' значение 'val'
        /// Если такой переменной нет в коллекции - она создается
        /// Если тип переменной не совпадает - тип переменной меняется
        /// </summary>
        /// <param name="key">Имя (ключ) переменной. Пустое значение не докупскается.</param>
        /// <param name="val">Новое значение переменной</param>
        /// <returns>Переменная, добавленная в коллекцию.
        /// null, если значение ключа 'key' было пустым или null</returns>
        public Variable Set(string key, object val)
        {
            if (val is int)
                return Set(key, (int)val);
            if (val is bool)
                return Set(key, (bool)val);
            if (val is DateTime)
                return Set(key, (DateTime)val);
            return Set(key, val.ToString());
        }

        /// <summary>
        /// Установить переменной с именем 'key' значение 'val' с контролем типа на соответствие 'type'
        /// Если значение 'val' и не может быть конвертировано к не соответствует типу 'type', генерируется исключение
        /// Если такой переменной нет в коллекции - она создается
        /// </summary>
        /// <param name="key">Имя (ключ) переменной. Пустое значение не докупскается.</param>
        /// <param name="type">Тип переменной, которому должна соответствовать перемееная</param>
        /// <param name="val">Новое значение переменной</param>
        /// <returns>Переменная, добавленная в коллекцию.
        /// null, если значение ключа 'key' было пустым или null</returns>
        public Variable Set(string key, VarTypes type, object val)
        {
            if (string.IsNullOrEmpty(key) || val == null)
                return null;
            if (type == VarTypes.String)
                return Set(key, val.ToString());
            else if (val is int && type != VarTypes.Integer)
                return Set(key, (int)val);
            else if (val is bool && type != VarTypes.Boolean)
                return Set(key, (bool)val);
            else if (val is DateTime && type != VarTypes.DateTime)
                return Set(key, (DateTime)val);

            throw new NotValidVarValueException($"Value '{val}' can not be converted to type '{type}' for variable '{key}'");
        }
        /// <summary>
        /// Установить переменной с именем 'key' значение 'val' с контролем типа на соответствие 'type'
        /// Если значение 'val' и не может быть конвертировано к не соответствует типу 'type', генерируется исключение
        /// Если такой переменной нет в коллекции - она создается
        /// </summary>
        /// <param name="key">Имя (ключ) переменной. Пустое значение не докупскается.</param>
        /// <param name="type">Тип переменной, которому должна соответствовать перемееная</param>
        /// <param name="val">Новое значение переменной</param>
        /// <returns>Переменная, добавленная в коллекцию.
        /// null, если значение ключа 'key' было пустым или null</returns>
        public Variable Set(string key, object objType, object val)
        {
            if (key == null || objType == null || val == null)
                return null;
            if (VarParcer.GetType(objType, out VarTypes type))
            {
                return Set(key, type, val);
            }
            throw new NotValidVarValueException($"Object '{objType}' can not be converted to type 'VarTypes' to set value for the variable '{key}'");
        }
        /// <summary>
        /// Добавление в коллекцию копию аргумента 'val'
        /// Если в коллекции имеется переменная с ключем 'vel.Key', её значение обновляется в соответствии с val
        /// </summary>
        /// <param name="val">Новое значение переменной</param>
        /// <returns>Переменная, добавленная в коллекцию</returns>
        public Variable Set(Variable val)
        {
            if (val!= null && Dictionary.TryGetValue(val.Key, out Variable var))
            {
                if (var.VarType == val.VarType)
                {
                    var.Value = val;
                    return var;
                }
                else Dictionary.Remove(val.Key);
            }
            var = Variable.Make(val);
            Add(var);
            return var;
        }
    }

    /// <summary>
    /// Исключение, сообщающее о недопустимости значения переменной
    /// </summary>
    public class NotValidVarValueException : Exception
    {
        public NotValidVarValueException(string mess) : base(mess)
        { }
    }
}
