﻿using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using VariableTools.Expressions;

namespace VariableTools.Classes
{
    [Serializable]
    public class VariableCollection : IEnumerable<VariableContainer>, ISerializable, IXmlSerializable
    {
#if false
        private static readonly ConstructorInfo VariableConstructor = null;
        private static readonly ConstructorInfo VariableParamConstructor = null;
        static VariableCollection()
        {
            Type type = typeof(VariableContainer);
            ConstructorInfo[] ctors = type.GetConstructors(BindingFlags.NonPublic);
            if (ctors.Length > 0)
            {

            }
            else throw new Exception("Не найдены конструкторы VariableContainer");
        }

#endif
        /// <summary>
        /// Ключ идентифицирующий переменную в коллекцию
        /// </summary>
        [Serializable]
        public class VariableKey
        {
            internal VariableKey() { }
            //internal VariableKey(string n, string q)
            //{
            //    Name = n;
            //    Qualifier = q;
            //}
            public VariableKey(string n, AccountScopeType asc, ProfileScopeType psc)
            {
                name = n;
                accountScope = asc;
                profileScope = psc;
                Qualifier = VariableTools.GetScopeQualifier(asc, psc);
                valid = !Parser.IsForbidden(name);
            }

            private AccountScopeType accountScope = AccountScopeType.Global;
            private ProfileScopeType profileScope = ProfileScopeType.Common;
            private string name = string.Empty;
            private bool valid = false;

            [Description("Имя переменной.\n" +
                         "The Name of the Variable")]
            public string Name
            {
                get => name;
                set 
                {
                    if (name != value)
                    {
                        if (Parser.CorrectForbiddenName(value, out string corrected))
                        {
                            // Имя переменной некорректно
                            // Запрашиваем замену
                            if (XtraMessageBox.Show(/*Form.ActiveForm, */
                                                    $"Задано недопустимое имя переменно '{value}'!\n" +
                                                    $"Хотите его исправить на '{corrected}'?\n" +
                                                    $"The name '{value}' is incorrect! \n" +
                                                    $"Whould you like to change it to '{corrected}'?",
                                                    "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                            {
                                // Пользователь не согласился заменить некорректное имя переменной
                                name = value;
                                valid = false;
                            }
                            else
                            {
                                // Пользователь согласился заменить имя переменной на корректное
                                name = corrected;
                                valid = true;
                            }
                        }
                        else
                        {
                            name = value;
                            valid = true;
                        }
                    }
                }
            }

            [Description("Идентификатор видимости переменной в профиле Квестера.\n" +
                "The Scope of the Variable for the quester-profiles")]
            public ProfileScopeType ProfileScope
            {
                get => profileScope;
                set
                {
                    if (profileScope != value)
                    {
                        Qualifier = VariableTools.GetScopeQualifier(accountScope, value);
                        profileScope = value;
                    }
                }
            }

            [Description("Идентификатор видимости переменной по отношению к персонажам аккаунта.\n" +
                "The Scope of the Variable for the characters of the accounts")]
            public AccountScopeType AccountScope
            {
                get => accountScope;
                set
                {
                    if (accountScope != value)
                    {
                        Qualifier = VariableTools.GetScopeQualifier(value, profileScope);
                        accountScope = value;
                    }
                }
            }

            [Description("Идентификатор области видимости переменной\n" +
                "Scope qualifier of the Variable")]
            [XmlIgnore]
            public string Qualifier { get; protected set; } = string.Empty;

            [XmlIgnore]
            [Browsable(false)]
            public bool IsValid
            {
                get => valid;
                protected set => valid = value; }

            public override bool Equals(object obj)
            {
                if (obj is VariableKey vKey)
                {
                    return Name == vKey.Name
                        && Qualifier == vKey.Qualifier;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ Qualifier.GetHashCode();
            }

            public override string ToString()
            {
                return $"{Name}[{AccountScope}, {ProfileScope}]";
            }
        }


        /// <summary>
        /// Класс-коллекция переменных
        /// </summary>
        internal class InternalVariableCollection : KeyedCollection<VariableKey, VariableContainer>
        {
            internal class VariableKeyComparer : IEqualityComparer<VariableKey>
            {
                public bool Equals(VariableKey x, VariableKey y)
                {
                    if (x == null || y == null)
                        return false;
                    else return x.Name == y.Name
                            && x.ProfileScope == y.ProfileScope
                            && x.AccountScope == y.AccountScope;
                }

                public int GetHashCode(VariableKey obj)
                {
                    if (obj is VariableKey key)
                    {
                        return key.Name.GetHashCode() ^ key.Qualifier.GetHashCode();
                    }
                    else return obj.GetHashCode();
                }
            }

            internal InternalVariableCollection() : base(new VariableKeyComparer(), 1) { }

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

        /// <summary>
        /// Объявление делегата, уведомляющего контейнеры переменных об изменении идентификаторов
        /// </summary>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        delegate void NotifyVariableChangeScopeDelegat(VariableContainer oldKey, VariableKey newKey);

        NotifyVariableChangeScopeDelegat NotifyVariableChangeScope;

        public void ChangeItemKey(VariableContainer item, VariableKey newKey)
        {
            if (item != null && newKey != null)
            {
                if (collection.ContainsKey(item.Key))
                {
                    collection.ChangeItemKey(item, newKey);
                    NotifyVariableChangeScope?.Invoke(item, newKey);
                }
            }
        }

        public bool TryGetValue(out double value, VariableKey key)
        {
            return collection.TryGetValue(key, out value);
        }

        public bool TryGetValue(out VariableContainer value, VariableKey key)
        {
            return collection.TryGetValue(key, out value);
        }

        public bool TryGetValue(out VariableContainer value, string name, AccountScopeType accScope = AccountScopeType.Global, ProfileScopeType profScope = ProfileScopeType.Common)
        {
            if (collection.TryGetValue(new VariableKey(name, accScope, profScope), out value))
                return true;
            else return false;
        }

#if disabled_at_20200505_1413
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
            if (!collection.TryGetValue(new VariableKey(name, AccountScopeType.Character, ProfileScopeType.Profile), out value))
                if (!collection.TryGetValue(new VariableKey(name, AccountScopeType.Character, ProfileScopeType.Common), out value))
                    if (!collection.TryGetValue(new VariableKey(name, AccountScopeType.Account, ProfileScopeType.Profile), out value))
                        if (!collection.TryGetValue(new VariableKey(name, AccountScopeType.Account, ProfileScopeType.Common), out value))
                            if (!collection.TryGetValue(new VariableKey(name, AccountScopeType.Global, ProfileScopeType.Profile), out value))
                                collection.TryGetValue(new VariableKey(name, AccountScopeType.Global, ProfileScopeType.Common), out value);
            return value != null;
        } 
#endif

#if disabled_at_20200505_1428
        public VariableContainer TryAdd(double value, string name, AccountScopeType accScope = AccountScopeType.Global, ProfileScopeType profScope = ProfileScopeType.Common)
        {
            VariableKey key = new VariableKey(name, accScope, profScope);
            if (collection.Contains(key))
            {
                collection[key].Value = value;
                return collection[key];
            }
            else
            {
                VariableContainer var = MakeVariableContainer(value, name, accScope, profScope);
                collection.Add(var);
                NotifyVariableChangeScope += var.ChangeScopeImplementation;
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
#if disabled_at_20200505_1410
            {
                collection.Add(variable);
                NotifyVariableChangeScope += variable.ChangeScopeImplementation;
                return collection.Contains(variable);
            } 
#else
                return false;
#endif
        }

#endif
        public VariableContainer Add(double value, string name, AccountScopeType accScope = AccountScopeType.Character, ProfileScopeType profScope = ProfileScopeType.Common, bool save = false)
        {
#if disabled_at_20200505_1421
            if (collection.TryGetValue(new VariableKey(name, accScope, profScope), out VariableContainer varContainer))
            {
                varContainer.Value = value;

                return varContainer;
            }
            else
#else
            if(!collection.ContainsKey(new VariableKey(name, accScope, profScope)))
#endif
            {
                VariableContainer newVar = MakeVariableContainer(value, name, accScope, profScope, save);
                collection.Add(newVar);
                NotifyVariableChangeScope += newVar.ChangeScopeImplementation;
                return newVar;
            }
            return null;
        }

#if disabled_at_20200505_1413
        public VariableContainer this[string name]
        {
            get
            {
                if (TryGetValue(out VariableContainer var, name))
                    return var;
                else return null;
            }
        } 
#endif

        public VariableContainer this[string name , AccountScopeType accScope, ProfileScopeType profScope]
        {
            get
            {
                if (collection.TryGetValue(new VariableKey(name, accScope, profScope), out VariableContainer var))
                    return var;
                return null;
            }
        }

        public bool ContainsKey(VariableKey key)
        {
            return key != null && collection.ContainsKey(key);
        }
        public bool ContainsKey(string name, AccountScopeType accScope, ProfileScopeType profScope)
        {
            return ContainsKey(new VariableKey(name, accScope, profScope));
        }

        public void Clear()
        {
            collection.Clear();
            NotifyVariableChangeScope = null;
        }

        public bool Remove(VariableKey key)
        {
            return collection.Remove(key);
        }

        public bool Remove(string name, AccountScopeType accScope, ProfileScopeType profScope)
        {
            return collection.Remove(new VariableKey(name, accScope, profScope));
        }

        public int Count => collection.Count;

        public VariableCollection() { }

        #region Фабрика переменных
        protected VariableContainer MakeVariableContainer()
        {
            return Activator.CreateInstance(typeof(VariableContainer), true) as VariableContainer;
        }
        protected VariableContainer MakeVariableContainer(double value, string name, AccountScopeType accScope = AccountScopeType.Character, ProfileScopeType profScope = ProfileScopeType.Common, bool save = false)
        {
            return Activator.CreateInstance(typeof(VariableContainer), 
                BindingFlags.NonPublic| BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, 
                null, 
                new object[] { value, name, accScope, profScope, save }, 
                null) as VariableContainer;
        }
        #endregion

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
                    if (enumerator.Current.Value is VariableContainer var && var.Save)
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

#if disabled_at_20200505_1433
        public bool Add(object obj)
        {
            if (obj is VariableContainer variable)
                return TryAdd(variable);
            return false;
        } 
#endif
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
                        VariableContainer v = MakeVariableContainer();
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
