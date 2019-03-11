using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AstralVars.Classes
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
            if(Variables.TryGetValue(name, out object val))
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

    public class VariableCollection : KeyedCollection<string, Variable>
    {
        protected override string GetKeyForItem(Variable item)
        {
            return item.Key;
        }

    }
}
