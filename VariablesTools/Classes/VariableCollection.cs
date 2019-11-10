using EntityTools.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VariableTools.Classes
{
    [Serializable]
    public class VariableCollection : KeyedCollection< string, VariableContainer>
    {
        protected override string GetKeyForItem(VariableContainer item)
        {
            return item.Name;
        }

        internal new void ChangeItemKey(VariableContainer item, string newKey)
        {
            base.ChangeItemKey(item, newKey);
        }

        public bool TryGetValue(string name, out double value)
        {
            if ( base.Dictionary.TryGetValue(name, out VariableContainer var)
                && var.IsValid)
            {
                value = var.Value;
                return true;
            }
            value = 0;
            return true;
        }

        public bool TryGetValue(string name, out VariableContainer value)
        {
            if (base.Dictionary.TryGetValue(name, out VariableContainer var)
                && var.IsValid)
            {
                value = var;
                return true;
            }
            value = null;
            return false;
        }

        public bool TryAdd(string name, double value, VariableScopeType scope = VariableScopeType.Local)
        {
            if (TryGetValue(name, out VariableContainer val))
            {
                val.Value = value;
                val.Scope = scope;
            }
            else
            {
                VariableContainer newVal = new VariableContainer(name, value, scope);
                Dictionary.Add(name, newVal);
            }
            return false;
        }

        public VariableContainer Add(string name, double value, VariableScopeType scope = VariableScopeType.Local)
        {
            if (TryGetValue(name, out VariableContainer val))
            {
                val.Value = value;
                val.Scope = scope;
                return val;
            }
            else
            {
                VariableContainer newVal = new VariableContainer(name, value, scope);
                Dictionary.Add(name, newVal);
                return newVal;
            }
        }

        public new VariableContainer this[string name]
        {
            get
            {
                if (base.Dictionary.TryGetValue(name, out VariableContainer var)
                && var.IsValid)
                {
                    return var;
                }
                return null;
            }
        }

        public bool ContainsKey(string name)
        {
            return base.Dictionary.TryGetValue(name, out VariableContainer var)
                        && var.IsValid;
        }
    }
}
