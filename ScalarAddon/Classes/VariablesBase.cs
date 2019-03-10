using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ValiablesAstralExtention.Classes
{
    public class VariablesBase
    {
        private static Dictionary<string , object> Variables = new Dictionary<string, object>();

        public static bool GetValue(String name, out object val)
        {
            return Variables.TryGetValue(name, out val);
        }

        public static object GetValue(String name)
        {
            object val = null;
            if(Variables.TryGetValue(name, out val))
            {
                Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} no variables named '{name}'");
            }
            return val;
        }

        public static bool SetValue(String name, object val)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Variables[name] = val;
                Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} variable {name} set to {val}");
                return true;
            }
            Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} empty variable name is invalid");
            return false;
        }

        public static bool SetValue(string name, string val)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Variables[name] = val;
                Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} variable {name} set to {val}");
                return true;
            }
            Astral.Logger.WriteLine($"{VariablesAddon.LoggerPredicate} empty variable name is invalid");
            return false;
        }
    }

    public class VariableCollection : KeyedCollection<string, VariableItem>
    {
        protected override string GetKeyForItem(VariableItem item)
        {
            return item.Key;
        }

    }
}
