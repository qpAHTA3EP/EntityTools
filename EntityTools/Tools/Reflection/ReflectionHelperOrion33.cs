using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace QuesterAssistant.Classes.Common
{
    public static class ReflectionHelper
    {
        public const BindingFlags DefaultFlags = BindingFlags.Instance
                                   | BindingFlags.Static
                                   | BindingFlags.GetProperty
                                   | BindingFlags.SetProperty
                                   | BindingFlags.GetField
                                   | BindingFlags.SetField
                                   | BindingFlags.Public
                                   | BindingFlags.NonPublic;

        public static MethodInfo[] GetListOfMethods(this object obj, BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();
            if (baseType)
            {
                type = type.BaseType;
            }
            return type.GetListOfMethods(flags);
        }

        public static MethodInfo[] GetListOfMethods(this Type type, BindingFlags flags = DefaultFlags)
        {
            MethodInfo[] methodInfos = type.GetMethods(flags);
            return methodInfos;
        }

        public static FieldInfo[] GetListOfFields(this object obj, BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();
            FieldInfo[] fields = null;
            flags = DefaultFlags | flags;
            if (baseType)
            {
                type = type.BaseType;
            }
            fields = type.GetFields(flags);
            return fields;
        }

        public static FieldInfo GetFieldInfo(this Type type, string fieldName, BindingFlags flags = DefaultFlags)
        {
            if (flags == BindingFlags.Default)
                flags = DefaultFlags;
            FieldInfo[] fields = type.GetFields(flags);
            foreach (FieldInfo fi in fields)
            {
                if (fi.Name.ToLower() == fieldName.ToLower())
                    return fi;
            }
            return null;
        }

        public static Type GetTypeByName(string assemblyName, string typeName, bool fullTypeName = false)
        {
            if (!string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(assemblyName))
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.FullName.Contains(assemblyName))
                    .GetTypes().ToList().FirstOrDefault(t => t.FullName != null && t.FullName.Contains(typeName));
            }
            return null;
        }

        public static bool SetPropertyValue(this object obj, string propName, object value,
            BindingFlags flags = DefaultFlags, bool searchBaseRecursive = false)
        {
            if (obj == null)
                return false;
            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.Instance);
            if (pi != null)
            {
                MethodInfo[] accessors = pi.GetAccessors(true);
                if (accessors.Length == 2)
                {
                    object[] arg = { value };
                    accessors[1]?.Invoke(obj, arg);
                    return true;
                }
            }
            else if (searchBaseRecursive)
                return SetBasePropertyValue(type.BaseType, obj, propName, value, flags);
            return false;
        }

        private static bool SetBasePropertyValue(Type type, object obj, string propName, object value, BindingFlags flags = DefaultFlags)
        {
            if (obj == null || type == null || type == typeof(object))
                return false;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.Instance);
            if (pi == null)
                return SetBasePropertyValue(type.BaseType, obj, propName, value, flags | BindingFlags.Instance);
            MethodInfo[] accessors = pi.GetAccessors(true);
            if (accessors.Length == 2)
            {
                object[] arg = { value };
                accessors[1]?.Invoke(obj, arg);
                return true;
            }
            return false;
        }

        public static bool SetStaticPropertyValue(this Type type, string propName, object value,
            BindingFlags flags = DefaultFlags, bool searchBaseRecursive = false)
        {
            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.Static);
            if (pi != null)
            {
                MethodInfo[] accessors = pi.GetAccessors(true);
                if (accessors.Length == 2)
                {
                    object[] arg = { value };
                    accessors[1]?.Invoke(null, arg);
                    return true;
                }
            }
            else if (searchBaseRecursive)
                return SetBasePropertyValue(type.BaseType, null, propName, value, flags);
            return false;
        }

        public static object GetPropertyValue(this object obj, string propName, 
            BindingFlags flags = DefaultFlags, bool searchBaseRecursive = false)
        {
            if (obj == null)
                return false;
            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = DefaultFlags | BindingFlags.Instance;

            PropertyInfo pi = type.GetProperty(propName, flags);
            if (pi != null)
            {
                MethodInfo getter = pi.GetMethod;
                if (getter != null)
                {
                    object[] arg = { };
                    return getter.Invoke(obj, arg);
                }
            }
            else if (searchBaseRecursive)
                return GetBasePropertyValue(type.BaseType, obj, propName, flags);
            return null;
        }

        private static object GetBasePropertyValue(Type type, object obj, string propName,
            BindingFlags flags = DefaultFlags)
        {
            if (obj == null || type == null || type == typeof(object))
                return false;

            PropertyInfo pi = type.GetProperty(propName, flags);
            if (pi == null)
                return GetBasePropertyValue(type.BaseType, obj, propName, flags);
            MethodInfo[] accessors = pi.GetAccessors(true);
            if (accessors.Length > 0)
            {
                object[] arg = { };
                return accessors[0]?.Invoke(obj, arg);
            }
            return null;
        }

        public static object GetStaticPropertyValue(this Type type, string propName,
            BindingFlags flags = DefaultFlags, bool searchBaseRecursive = false)
        {
            PropertyInfo pi = type.GetProperty(propName, flags);
            if (pi != null)
            {
                MethodInfo getter = pi.GetMethod;
                if (getter != null)
                {
                    object[] arg = { };
                    return getter.Invoke(null, arg);
                }
            }
            else if (searchBaseRecursive)
                return GetBasePropertyValue(type.BaseType, null, propName, flags);
            return null;
        }

        public static bool SetFieldValue(this object obj, string fieldName, object value,
            BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (obj == null)
                return false;
            Type type = obj.GetType();
            if (baseType)
            {
                type = type.BaseType;
            }
            FieldInfo info = type.GetField(fieldName, flags);
            if (info == null) return false;
            info.SetValue(obj, value);
            return true;
        }

        public static bool SetStaticFieldValue(this Type type, string fieldName, object value,
            BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (baseType)
            {
                type = type.BaseType;
            }
            FieldInfo info = type.GetField(fieldName, flags | BindingFlags.Static);
            if (info == null) return false;
            info.SetValue(type, value);
            return true;
        }

        public static object GetFieldValue(this object obj, string fieldName, 
            BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (obj == null)
                return false;
            Type type = obj.GetType();
            if (flags == BindingFlags.Default)
                flags = DefaultFlags;
            if (baseType)
            {
                type = type.BaseType;
            }
            FieldInfo fi = type.GetField(fieldName, flags);
            return fi == null ? null : fi.GetValue(obj);
        }

        public static object GetStaticFieldValue(this Type type, string fieldName, 
            BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (baseType)
            {
                type = type.BaseType;
            }
            FieldInfo fi = type.GetField(fieldName, flags | BindingFlags.Static);
            return fi == null ? null : fi.GetValue(type);
        }

        public static T GetStaticFieldValue<T>(this Type type, int index = 0, BindingFlags flags = DefaultFlags) where T : class
        {
            var fields = type.GetFields(DefaultFlags).Where(f => f.FieldType == typeof(T)).ToArray();
            var fi = fields[index];
            return fi == null ? null : fi.GetValue(type) as T;
        }

        public static object ExecMethod(this object obj, string methodName, object[] arguments,
            BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (obj == null)
                return false;
            Type type = obj.GetType();
            if (flags == BindingFlags.Default)
                flags = DefaultFlags;
            flags = DefaultFlags | flags;
            if (baseType)
            {
                type = type.BaseType;
            }
            MethodInfo m = type.GetMethod(methodName, flags);
            return m == null ? false : m.Invoke(obj, arguments);
        }

        public static object ExecStaticMethod(this Type type, string methodName,
            BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            return type.ExecStaticMethod(methodName, new object[0], flags);
        }

        public static object ExecStaticMethod(this Type type, string methodName, object[] arguments,
            BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (baseType)
            {
                type = type.BaseType;
            }
            MethodInfo methodInfo = type.GetMethod(methodName, flags | BindingFlags.Static);
            return methodInfo == null ? null : methodInfo.Invoke(null, arguments);
        }

        public static object ExecStaticMethodByArgs(this Type type, string methodName, object[] arguments,
            BindingFlags flags = DefaultFlags, bool baseType = false)
        {
            if (baseType)
            {
                type = type.BaseType;
            }
            Type[] types;
            if (arguments != null && arguments.Length > 0)
            {
                types = new Type[arguments.Length];
                for (int i = 0; i < arguments.Length; i++)
                    types[i] = arguments[i].GetType();
            }
            else types = new Type[] { };
            MethodInfo methodInfo = type.GetMethod(methodName, flags | BindingFlags.Static, null, types, null);
            return methodInfo == null ? null : methodInfo.Invoke(null, arguments);
        }

        public static TDelegate GetStaticMethodByName<TDelegate>(this Type type, string methodName,
            BindingFlags flags = DefaultFlags, bool baseType = false) where TDelegate : class 
        {
            if (baseType)
                type = type.BaseType;
            MethodInfo methodInfo = type.GetMethod(methodName, flags | BindingFlags.Static);
            return methodInfo is null ? null : Delegate.CreateDelegate(typeof(TDelegate), methodInfo) as TDelegate;
        }

        public static TDelegate GetStaticMethodBySignature<TDelegate>(this Type type, Type returnType, Type[] inputTypes = null,
            BindingFlags flags = DefaultFlags, bool baseType = false) where TDelegate : class
        {
            if (baseType)
                type = type.BaseType;
            if (inputTypes == null || inputTypes.Length == 0)
                inputTypes = new Type[] { };
            foreach (MethodInfo methodInfo in type.GetMethods(flags | BindingFlags.Static))
            {
                var arguments = methodInfo.GetParameters();
                if (methodInfo.ReturnType == returnType && arguments.Length == inputTypes.Length)
                {
                    bool flag = true;
                    foreach (var argument in arguments)
                    {
                        if (inputTypes.Count(a => a == argument.ParameterType) != arguments.Count(a => a.ParameterType == argument.ParameterType))
                            flag = false;
                    }
                    if (flag)
                        return Delegate.CreateDelegate(typeof(TDelegate), methodInfo) as TDelegate;
                }
            }
            return null;
        }

        private static void SubscribeEvent(this object obj, Type control, Type typeHandler, string eventName, MethodInfo method, bool isConsole = false)
        {
            EventInfo eventInfo = control.GetEvent(eventName);
            Delegate handler;
            if (isConsole)
            {
                handler = Delegate.CreateDelegate(typeHandler, null, method);
                eventInfo.AddEventHandler(obj, handler);
            }
            else
            {
                handler = Delegate.CreateDelegate(typeHandler, obj, method);
                eventInfo.AddEventHandler(control, handler);
            }
        }

        public static void SubscribeEvent(this object obj, Control control, Type typeHandler, string eventName, MethodInfo method)
        {
            if (control is Control)
            {
                obj.SubscribeEvent(control.GetType(), typeHandler, eventName, method);
            }
        }

        private static void UnSubscribeEvent(this object obj, Type control, Type typeHandler, string eventName, MethodInfo method, bool isConsole = false)
        {
            if (obj != null)
            {
                EventInfo eventInfo = control.GetEvent(eventName);
                if (isConsole)
                {
                    Delegate handler = Delegate.CreateDelegate(typeHandler, null, method);
                    eventInfo.RemoveEventHandler(obj, handler);
                }
                else
                {
                    Delegate handler = Delegate.CreateDelegate(typeHandler, obj, method);
                    eventInfo.RemoveEventHandler(control, handler);
                }
            }
        }
        public static void UnSubscribeEvent(this object obj, Control control, Type typeHandler, string eventName, MethodInfo method)
        {
            if (control is Control)
            {
                obj.UnSubscribeEvent(control.GetType(), typeHandler, eventName, method);
            }
        }

        public static object CreateInstance(this Type type, BindingFlags flags = DefaultFlags, object[] arguments = null)
        {
            Type[] types = arguments == null ? new Type[0] : arguments.Select(a => a.GetType()).ToArray();
            return type.GetConstructor(flags, null, types, null)?.Invoke(arguments);
        }
    }
}
