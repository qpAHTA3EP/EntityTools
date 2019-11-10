using EntityTools.Tools;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableTools.Classes
{
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
        public VariableContainer(string n, double v = 0, VariableScopeType s = VariableScopeType.Local)
        {
            name = n;
            val = v;
            scope = s;
            switch(scope)
            {
                case VariableScopeType.Character:
                    qualifier = EntityManager.LocalPlayer.InternalName;
                    break;
                case VariableScopeType.Account:
                    qualifier = EntityManager.LocalPlayer.AccountLoginUsername;
                    break;
                case VariableScopeType.Global:
                    qualifier = string.Empty;
                    break;
                default:
                    qualifier = Astral.API.CurrentSettings.LastQuesterProfile;
                    break;
            }
        }

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
                    VariablesTools.Variables.ChangeItemKey(this, name);
                    name = value;
                }
            }
        }

        private double val = 0;
        /// <summary>
        /// Значение переменной
        /// </summary>
        public double Value
        {
            get
            {
                if (IsValid)
                    return val;
                return 0;
            }
            set => val = value;
        }

        private VariableScopeType scope = VariableScopeType.Global;
        /// <summary>
        /// Переключатель области видимости переменной
        /// </summary>
        public VariableScopeType Scope
        {
            get => scope;
            set
            {
                switch(value)
                {
                    case VariableScopeType.Character:
                        qualifier = EntityManager.LocalPlayer.InternalName;
                        break;
                    case VariableScopeType.Account:
                        qualifier = EntityManager.LocalPlayer.AccountLoginUsername;
                        break;
                    case VariableScopeType.Global:
                        qualifier = string.Empty;
                        break;
                    default:
                        qualifier = Astral.API.CurrentSettings.LastQuesterProfile;
                        break;
                }
                scope = value;
            }
        }

        protected string qualifier = string.Empty;
        /// <summary>
        /// Идентификатор области видимости переменной
        /// </summary>
        public string ScopeQualifier { get => qualifier; }

        /// <summary>
        /// Флаг сохранения в файл при закрытии Астрала
        /// </summary>
        public bool Save { get; set; }

        /// <summary>
        /// Проверка видимости переменной в данной области видимости
        /// </summary>
        public bool IsValid
        {
            get
            {
                switch (scope)
                {
                    case VariableScopeType.Character:
                        return qualifier == EntityManager.LocalPlayer.InternalName;
                    case VariableScopeType.Account:
                        return qualifier == EntityManager.LocalPlayer.AccountLoginUsername;
                    case VariableScopeType.Local:
                        return qualifier == Astral.API.CurrentSettings.LastQuesterProfile;
                    case VariableScopeType.Global:
                        return true;
                }
                return false;
            }
        }
    }
}
