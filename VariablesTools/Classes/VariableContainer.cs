﻿using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using VariableTools.Expressions;
using static VariableTools.Classes.VariableCollection;

namespace VariableTools.Classes
{
    /// <summary>
    /// Область видимости переменной
    /// </summary>
    [Serializable]
    public enum AccountScopeType
    {
        /// <summary>
        /// Видима в пределах одного квестер-профиля
        /// </summary>
        //Local,
        /// <summary>
        /// Видима одному персонажу аккаунта
        /// </summary>
        Character,
        /// <summary>
        /// Видима всем персонажам аккаунта
        /// </summary>
        Account,
        /// <summary>
        /// Область видимости не ограничена
        /// </summary>
        Global
    }

    [Serializable]
    public class VariableContainer : ISerializable, IXmlSerializable
    {
        public VariableContainer() { }
        public VariableContainer(string n, double v = 0, AccountScopeType asq = AccountScopeType.Global, bool psq = false)
        {
            name = n;
            val = v;
            accountScope = asq;
            profileScope = psq;
            qualifier = VariableTools.GetScopeQualifier(accountScope, profileScope);
        }

        /// <summary>
        /// Идентификатор переменной к коллекции
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        internal VariableKey Key => new VariableKey(name, qualifier);


        [XmlIgnore]
        private string name = string.Empty;
        /// <summary>
        /// Имя переменной
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                if(name != value)
                {
                    if (VariableTools.Variables.ContainsKey(Key))
                        VariableTools.Variables.ChangeItemKey(this, new VariableKey(value, qualifier));
                    name = value;
                }
            }
        }

        [XmlIgnore]
        private double val = 0;
        /// <summary>
        /// Значение переменной
        /// </summary>
        public double Value
        {
            get
            {
                if (IsScoped)
                    return val;
                return 0;
            }
            set => val = value;
        }

        [XmlIgnore]
        private AccountScopeType accountScope = AccountScopeType.Global;
        /// <summary>
        /// Переключатель области видимости переменной для аккаунта
        /// </summary>
        public AccountScopeType AccountScope
        {
            get => accountScope;
            set
            {
                if (accountScope != value)
                {
                    string newQualifier = VariableTools.GetScopeQualifier(value, profileScope);
                    if (VariableTools.Variables.ContainsKey(Key))
                        VariableTools.Variables.ChangeItemKey(this, new VariableKey(name, newQualifier));
                    qualifier = newQualifier;
                    accountScope = value;
                }
            }
        }

        [XmlIgnore]
        private bool profileScope = false;
        /// <summary>
        /// Переключатель области видимости для квестер-профиля
        /// </summary>
        public bool ProfileScope
        {
            get => profileScope;
            set
            {
                if(profileScope != value)
                {
                    string newQualifier = VariableTools.GetScopeQualifier(accountScope, value);
                    if (VariableTools.Variables.ContainsKey(Key))
                        VariableTools.Variables.ChangeItemKey(this, new VariableKey(name, newQualifier));
                    qualifier = newQualifier;
                    profileScope = value;
                }
            }
        }

        [XmlIgnore]
        private string qualifier = string.Empty;
        /// <summary>
        /// Идентификатор области видимости переменной
        /// </summary>
        [ReadOnly(true)]
        public string ScopeQualifier { get => qualifier; }

        /// <summary>
        /// Флаг сохранения в файл при закрытии Астрала
        /// </summary>
        public bool Save { get; set; }

        /// <summary>
        /// Проверка видимости переменной в данной области видимости
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool IsScoped
        {
            get
            {
                return qualifier == VariableTools.GetScopeQualifier(accountScope, profileScope);
            }
        }

        /// <summary>
        /// Переменная корректна и содержится в коллекции
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool IsValid => !Parser.IsForbidden(name) && VariableTools.Variables.ContainsKey(Key);


        public override string ToString()
        {
            if (string.IsNullOrEmpty(name))
                return "Undefined";
            else if(ProfileScope)
                return $"{name}[{AccountScope}, Profile]";
            else return $"{name}[{AccountScope}]";
        }


        #region ISerializable
        /// <summary>
        /// Конструктор сериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected VariableContainer(SerializationInfo info, StreamingContext context)
        {
            name = info.GetString(nameof(Name));
            val = info.GetDouble(nameof(Value));

            string save_str = info.GetString(nameof(Save));
            if (bool.TryParse(save_str, out bool s))
                Save = s;
            else Save = false;

            string scope_str = info.GetString(nameof(AccountScope));
            if (!Enum.TryParse(scope_str, out accountScope))
                accountScope = AccountScopeType.Global;

            string prof_str = info.GetString(nameof(ProfileScope));
            if (bool.TryParse(prof_str, out bool p))
                ProfileScope = p;
            else ProfileScope = false;

            qualifier = info.GetString(nameof(ScopeQualifier));
        }

        /// <summary>
        /// Сериализация контейнера переменной
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Name), name);
            info.AddValue(nameof(Value), val);
            info.AddValue(nameof(Save), Save);
            info.AddValue(nameof(AccountScope), accountScope);
            info.AddValue(nameof(ProfileScope), profileScope);
            info.AddValue(nameof(ScopeQualifier), qualifier);
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
                && reader.Name == GetType().Name)
            {
                reader.ReadStartElement(nameof(VariableContainer));
                while(reader.ReadState == ReadState.Interactive)
                {
                    switch(reader.Name)
                    {
                        case "Name":
                            name = reader.ReadElementContentAsString(nameof(Name), "");
                            break;
                        case "Value":
                            val = reader.ReadElementContentAsDouble(nameof(Value), "");
                            break;
                        case "Save":
                            string save_str = reader.ReadElementString(nameof(Save));
                            if (Parser.TryParse(save_str, out bool s))
                                Save = s;
                            else Save = false;
                            break;
                        case "AccountScope":
                            string scope_str = reader.ReadElementContentAsString(nameof(AccountScope), "");
                            if (!Enum.TryParse(scope_str, out accountScope))
                                accountScope = AccountScopeType.Global;
                            break;
                        case "ProfileScope":
                            string prof_str = reader.ReadElementString(nameof(ProfileScope));
                            if (Parser.TryParse(prof_str, out bool p))
                                profileScope = p;
                            else profileScope = false;
                            break;
                        case "ScopeQualifier":
                            qualifier = reader.ReadElementContentAsString(nameof(ScopeQualifier), "");
                            break;
                        case "VariableContainer":
                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                reader.ReadEndElement();
                                return;
                            }
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(Name), name);
            writer.WriteElementString(nameof(Value), val.ToString());
            writer.WriteElementString(nameof(Save), Save.ToString());
            writer.WriteElementString(nameof(AccountScope), accountScope.ToString());
            writer.WriteElementString(nameof(ProfileScope), profileScope.ToString());
            writer.WriteElementString(nameof(ScopeQualifier), qualifier);
        }
        #endregion
    }
}