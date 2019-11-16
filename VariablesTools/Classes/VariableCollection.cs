using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace VariableTools.Classes
{
    [Serializable]
    public class VariableCollection : IEnumerable<VariableContainer>, ISerializable, IXmlSerializable
    {
        /// <summary>
        /// Ключ идентифицирующий переменную в коллекцию
        /// </summary>
        internal class VariableKey
        {
            internal VariableKey() { }
            internal VariableKey(string n, string q)
            {
                Name = n;
                Qualifier = q;
            }

            internal string Name = string.Empty;
            internal string Qualifier = string.Empty;

            public override bool Equals(object obj)
            {
                if(obj is VariableKey vKey)
                {
                    return Name == vKey.Name
                        && Qualifier == vKey.Qualifier;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() & Qualifier.GetHashCode();
            }
        }

        /// <summary>
        /// Обертка идентифицирующая переменную
        /// </summary>
        internal class VariableScopeIdentifier
        {
            public VariableScopeIdentifier() { }
            public VariableScopeIdentifier(string name, AccountScopeType asc, bool psc)
            {
                Name = name;
                AccountScope = asc;
                ProfileScope = psc;
            }

            public string Name { get; set; }
            public bool ProfileScope { get; set; }
            public AccountScopeType AccountScope { get; set; }

            public bool Equals(VariableScopeIdentifier id)
            {
                return Name == id.Name
                    && AccountScope == id.AccountScope
                    && ProfileScope == id.ProfileScope;
            }
        }

        /// <summary>
        /// Класс-коллекция переменных
        /// </summary>
        internal class InternalVariableCollection : KeyedCollection<VariableKey, VariableContainer>
        {
            internal InternalVariableCollection() : base() { }

            protected override VariableKey GetKeyForItem(VariableContainer item)
            {
                return item.Key;
            }

            internal new void ChangeItemKey(VariableContainer item, VariableKey newKey)
            {
                base.ChangeItemKey(item, newKey);
            }

            internal bool TryGetValue(VariableKey key, out double value)
            {
                if (key != null && base.Contains(key))
                {
                    value = base[key].Value;
                    return true;
                }
                value = 0;
                return false;
            }

            internal bool TryGetValue(VariableKey key, out VariableContainer value)
            {
                if (key != null && base.Contains(key))
                {
                    value = base[key];
                    return true;

                }
                value = null;
                return false;
            }

            internal bool ContainsKey(VariableKey key)
            {
                return key != null && base.Contains(key);
            }
        }

        private InternalVariableCollection collection = new InternalVariableCollection();

        internal void ChangeItemKey(VariableContainer item, VariableKey newKey)
        {
            collection.ChangeItemKey(item, newKey);
        }

        internal bool TryGetValue(out double value, VariableKey key)
        {
            return collection.TryGetValue(key, out value);
        }

        internal bool TryGetValue(out VariableContainer value, VariableKey key)
        {
            return collection.TryGetValue(key, out value);
        }

        public bool TryGetValue(out double value, string name)
        {
            value = 0;
            if (TryGetValue(out VariableContainer var, name))
            {
                value = var.Value;
                return true;
            }
            return false;
        }

        public bool TryGetValue(out VariableContainer value, string name)
        {
            value = null;
            if (!collection.TryGetValue(new VariableKey(name, VariableTools.GetScopeQualifier(AccountScopeType.Character, true)), out value))
                if (!collection.TryGetValue(new VariableKey(name, VariableTools.GetScopeQualifier(AccountScopeType.Character, false)), out value))
                    if (!collection.TryGetValue(new VariableKey(name, VariableTools.GetScopeQualifier(AccountScopeType.Account, true)), out value))
                        if (!collection.TryGetValue(new VariableKey(name, VariableTools.GetScopeQualifier(AccountScopeType.Account, false)), out value))
                            if(!collection.TryGetValue(new VariableKey(name, VariableTools.GetScopeQualifier(AccountScopeType.Global, true)), out value))
                                collection.TryGetValue(new VariableKey(name, VariableTools.GetScopeQualifier(AccountScopeType.Global, false)), out value);
            return value != null;
        }

        public VariableContainer TryAdd(double value, string name, AccountScopeType accScope = AccountScopeType.Global, bool profScope = false)
        {
            VariableKey key = new VariableKey(name, VariableTools.GetScopeQualifier(accScope, profScope));
            if(collection.Contains(key))
            {
                collection[key].Value = value;
                return collection[key];
            }
            else
            {
                VariableContainer var = new VariableContainer(name, value, accScope);
                collection.Add(var);
                return var;
            }         
        }

        public bool TryAdd(VariableContainer variable)
        {
            if (collection.Contains(variable.Key))
            {
                collection[variable.Key].Value = variable.Value;
                collection[variable.Key].Save = variable.Save;
                return true;
            }
            else
            {
                collection.Add(variable);
                return collection.Contains(variable);
            }
        }

        public VariableContainer Add(string name, double value, AccountScopeType accScope = AccountScopeType.Character, bool profScope = false)
        {
            VariableContainer var = TryAdd(value, name, accScope, profScope);
            if (var != null)
            {
                var.Value = value;
                return var;
            }
            else
            {
                VariableContainer newVal = new VariableContainer(name, value, accScope, profScope);
                collection.Add(newVal);
                return newVal;
            }

        }

        public VariableContainer this[string name]
        {
            get
            {
                if (TryGetValue(out VariableContainer var, name))
                    return var;
                else return null;

            }
        }

        public VariableContainer this[string name , AccountScopeType accScope, bool profScope]
        {
            get
            {
                if (collection.TryGetValue(new VariableKey(name, VariableTools.GetScopeQualifier(accScope, profScope)), out VariableContainer var))
                    return var;
                return null;
            }
        }


        internal bool ContainsKey(VariableKey key)
        {
            return key != null && collection.ContainsKey(key);
        }

        //public bool ContainsKey(string name)
        //{
        //    VariableKey key = new VariableKey(name, VariableTools.GetScopeQualifier(AccountScopeType.Local));
        //    if (!collection.ContainsKey(key))
        //    {
        //        key.Qualifier = VariableTools.GetScopeQualifier(AccountScopeType.Character);
        //        if(!collection.ContainsKey(key))
        //        {
        //            key.Qualifier = VariableTools.GetScopeQualifier(AccountScopeType.Account);
        //            if(!collection.ContainsKey(key))
        //            {
        //                key.Qualifier = VariableTools.GetScopeQualifier(AccountScopeType.Global);
        //                if (!collection.ContainsKey(key))
        //                    return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        public void Clear()
        {
            collection.Clear();
        }

        public int Count => collection.Count;

        public VariableCollection() { }

        #region IEnumerable
        public IEnumerator<VariableContainer> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
        #endregion

        #region ISerializable
        /// <summary>
        /// Конструктор сериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected VariableCollection(SerializationInfo info, StreamingContext context)
        {
            if (collection == null)
                collection = new InternalVariableCollection();

            info.SetType(typeof(VariableContainer));
            var enumerator = info.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current.ObjectType == typeof(VariableContainer))
                {
                    VariableContainer var = enumerator.Current.Value as VariableContainer;
                    if(var != null && var.Save)
                        collection.Add(var);
                }
            }

        }

        /// <summary>
        /// Сериализация коллекции переменных
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using (var varEnumerator = collection.GetEnumerator())
            {
                while(varEnumerator.MoveNext())
                    info.AddValue(nameof(VariableContainer), varEnumerator.Current);
            }
        }

        public bool Add(object obj)
        {
            if(obj is VariableContainer variable)
                if (collection.ContainsKey(variable.Key))
                {
                    collection[variable.Key].Value = variable.Value;
                    collection[variable.Key].Save = variable.Save;
                    return true;
                }
                else
                {
                    collection.Add(variable);
                    return collection.Contains(variable);
                }
            return false;
        }
        #endregion

        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if ( reader.IsStartElement()
                && reader.Name == nameof(VariableCollection))
            {
                reader.ReadStartElement(nameof(VariableCollection));
                while(reader.ReadState == ReadState.Interactive)
                {
                    if (reader.Name == nameof(VariableContainer))
                    {
                        VariableContainer v = new VariableContainer();
                        v.ReadXml(reader);
                        if (!string.IsNullOrEmpty(v.Name))
                            collection.Add(v);
                    }
                    else reader.Skip();
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            using (var varEnumerator = collection.GetEnumerator())
            {
                while (varEnumerator.MoveNext())
                {
                    if (varEnumerator.Current.Save)
                    {
                        writer.WriteStartElement(nameof(VariableContainer), "");
                        varEnumerator.Current.WriteXml(writer);
                        writer.WriteEndElement();
                    }
                }
            }
        }
        #endregion
    }
}
