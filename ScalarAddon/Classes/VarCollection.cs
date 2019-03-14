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
        /// <param name="key">Имя (ключе) переменной. Пустое значение не докупскается.</param>
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
        /// <param name="key">Имя (ключе) переменной. Пустое значение не докупскается.</param>
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
        /// <param name="key">Имя (ключе) переменной. Пустое значение не докупскается.</param>
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
}
