using EntityTools.Tools;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableTools.Classes
{
    public enum VaribaleStoreType
    {
        Temp = 0,
        Day,
        Month
    }

    /// <summary>
    /// Область видимости переменной
    /// </summary>
    public enum VariableScopeType
    {
        /// <summary>
        /// Видима в пределах одного квестер-профиля
        /// </summary>
        Local,
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
    public class VariableContainer
    {
        protected Pair<string, string> key = new Pair<string, string>();
        /// <summary>
        /// Имя переменной
        /// </summary>
        public string Name { get => key.First; protected set => key.First = value; }

        /// <summary>
        /// Значение переменной
        /// </summary>
        public double Value { get; set; }

        private VariableScopeType scope = VariableScopeType.Global;
        /// <summary>
        /// Переключатель области видимости переменной
        /// </summary>
        public VariableScopeType VariableScope
        {
            get => scope;
            protected set
            {
                switch(value)
                {
                    case VariableScopeType.Character:
                        ScopeQualifier = EntityManager.LocalPlayer.InternalName;
                        break;
                    case VariableScopeType.Account:
                        ScopeQualifier = EntityManager.LocalPlayer.AccountLoginUsername;
                        break;
                    case VariableScopeType.Global:
                        ScopeQualifier = string.Empty;
                        break;
                    default:
                        ScopeQualifier = 
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ScopeQualifier { get => key.Second; protected set => key.First = value; }

        /// <summary>
        /// Флаг сохранения в файл при закрытии Астрала
        /// </summary>
        public bool Save { get; set; }
    }
}
