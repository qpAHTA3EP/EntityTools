using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityTools.Tools
{
    public static class ReflectionHelper
    {
        public static BindingFlags Flags = BindingFlags.Instance
                                   | BindingFlags.Static
                                   | BindingFlags.GetProperty
                                   | BindingFlags.SetProperty
                                   | BindingFlags.GetField
                                   | BindingFlags.SetField
                                   | BindingFlags.Public 
                                   | BindingFlags.NonPublic;

        /// <summary>
        /// Получение списка методов объекта <see cref="obj">
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        public static MethodInfo[] GetListOfMethods(object obj, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();

            if (BaseType)
            {
                type = type.BaseType; // BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);//  BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            }
            return GetListOfMethods(type, flags);
        }

        /// <summary>
        /// Получение списка методов типа <see cref="type">
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static MethodInfo[] GetListOfMethods(Type type, BindingFlags flags = BindingFlags.Default)
        {
            flags = Flags | flags;
            // get all public static methods of MyClass type
            MethodInfo[] methodInfos = type.GetMethods(flags);

            return methodInfos;
        }

        /// <summary>
        /// Получение списка полей объекта <see cref="obj">
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        public static FieldInfo[] GetListOfFields(object obj, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();

            FieldInfo[] fields = null;

            flags = Flags | flags;
            if (BaseType)
            {
                type = type.BaseType;
            }
            fields = type.GetFields(flags);

            return fields;
        }

        /// <summary>
        /// Получить FildInfo, описывающий приватное поле <see cref="fieldName"> типа <see cref="type">
        /// </summary>
        /// <param name="type">The Type that has the private field</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <returns>FieldInfo object. It has the field name and a useful GetValue() method.</returns>
        public static FieldInfo GetFieldInfo(Type type, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = Flags;
            FieldInfo[] fields = type.GetFields(flags);
            if (fields != null)
            {
                foreach (FieldInfo fi in fields) //fields.FirstOrDefault(feildInfo => feildInfo.Name == fieldName);
                {
                    if (fi.Name.ToLower() == fieldName.ToLower())
                        return fi;
                }
            }
            return null;
        }

        /// <summary>
        /// Поиск типа по имени <see cref="typeName">
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetTypeByName(string typeName, bool fullTypeName = false)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                foreach (var assambly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (fullTypeName)
                        return assambly.GetTypes().First(t => t.FullName == typeName);
                    else return assambly.GetTypes().First(t => t.Name == typeName);
                }
            }
            return null;
        }

        /// <summary>
        /// Задать значение <see cref="value"> совойству <see cref="propName"> объекта <see cref="obj">.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        /// <param name="flags"></param>
        /// <param name="searchBaseRecursive"></param>
        /// <returns></returns>
        public static bool SetPropertyValue(object obj, string propName, object value, BindingFlags flags = BindingFlags.Default, bool searchBaseRecursive = false)
        {
            if (obj == null)
                return false;
            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = Flags;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.GetProperty);//GetPrivateFieldInfo(type, fieldName, flags);
            if (pi != null)
            {
                MethodInfo[] accessors = pi.GetAccessors(true);
                if (accessors != null && accessors.Length == 2)
                {
                    object[] arg = new object[] { value };
                    accessors[1]?.Invoke(obj, arg);
                    return true;
                }
            }
            else if(searchBaseRecursive)
                return SetBasePropertyValue(type.BaseType, obj, propName, value, flags);

            return false;
        }
        private static bool SetBasePropertyValue(Type type, object obj, string propName, object value, BindingFlags flags = BindingFlags.Default)
        {
            if (obj == null || type == null || type == typeof(object))
                return false;

            if (flags == BindingFlags.Default)
                flags = Flags;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.GetProperty);//GetPrivateFieldInfo(type, fieldName, flags);
            if (pi == null)
                return SetBasePropertyValue(type.BaseType, obj, propName, value, flags);
            else
            {
                MethodInfo[] accessors = pi.GetAccessors(true);
                if (accessors != null && accessors.Length == 2)
                {
                    object[] arg = new object[] { value };
                    accessors[1]?.Invoke(obj, arg);
                    return true;
                }
            }
            

            return false;
        }

        /// <summary>
        /// Задать значение <see cref="value"> статическому совойству <see cref="propName"> типа <see cref="type">.
        /// </summary>
        /// <param name="type">The Type that has the private property</param>
        /// <param name="propName">The name of the private property</param>
        /// <param name="value">The instance from which to read the private value.</param>
        /// <returns>The value of the property boxed as an object.</returns>
        public static bool SetStaticPropertyValue(Type type, string propName, object value, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            if (flags == BindingFlags.Default)
                flags = Flags;
            if (BaseType)
            {
                type = type.BaseType;
            }
            object[] arg = new object[] { value };
            return ExecStaticMethod(type, "set_" + propName, ref arg, out object result, flags | BindingFlags.Static | BindingFlags.InvokeMethod, false);
        }

        /// <summary>
        /// Получить значение свойства <see cref="propName"> объекта <see cref="obj">
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <param name="flags"></param>
        /// <param name="searchBaseRecursive">Свойсто инкапсулировано в базовом классе</param>
        /// <returns></returns>
        public static bool GetPropertyValue(object obj, string propName, out object result, BindingFlags flags = BindingFlags.Default, bool searchBaseRecursive = false)
        {
            result = null;

            if (obj == null)
                return false;
            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = Flags;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.GetProperty);
            if (pi != null)
            {
                MethodInfo getter = pi.GetGetMethod();
                if(getter != null)
                {
                    object[] arg = new object[] { };
                    result = getter.Invoke(obj, arg);
                    return true;
                }                
            }
            else if(searchBaseRecursive)
                return GetBasePropertyValue(type.BaseType, obj, propName, out result, flags);

            return false;
        }
        private static bool GetBasePropertyValue(Type type, object obj, string propName, out object result, BindingFlags flags = BindingFlags.Default)
        {
            result = null;

            if (obj == null || type == null || type == typeof(object))
                return false;

            if (flags == BindingFlags.Default)
                flags = Flags;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.GetProperty);
            if (pi == null)
                return GetBasePropertyValue(type.BaseType, obj, propName, out result, flags);
            else
            {
                MethodInfo[] accessors = pi.GetAccessors(true);
                if (accessors != null && accessors.Length > 0)
                {
                    object[] arg = new object[] { };
                    result = accessors[0]?.Invoke(obj, arg);
                    return true;
                }
                //MethodInfo getter = pi.GetGetMethod();
                //if (getter != null)
                //{
                //    object[] arg = new object[] { };
                //    result = getter.Invoke(obj, arg);
                //    return true;
                //}
            }            

            return false;
        }

        /// <summary>
        /// Получить значение статического свойства <see cref="propName"> типа <see cref="type">
        /// </summary>
        /// <param name="type">The Type that has the private property</param>
        /// <param name="propName">The name of the private property</param>
        /// <returns>The value of the property boxed as an object.</returns>
        public static bool GetStaticPropertyValue(Type type, string propName, out object result, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            result = null;
            if (flags == BindingFlags.Default)
                flags = Flags;
            if (BaseType)
            {
                type = type.BaseType;
            }
            object[] arg = new object[] { };

            return ExecStaticMethod(type, "get_" + propName, ref arg, out result, flags | BindingFlags.Static | BindingFlags.InvokeMethod, false);
        }


        /// <summary>
        /// Задать значение <see cref="value"> поля <see cref="fieldName"> объекта <see cref="obj">.
        /// </summary>
        /// <param name="obj">The instance contains private filed</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <param name="value">The value which assigns to the private field</param>
        /// <param name="flags"></param>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        public static bool SetFieldValue(object obj, string fieldName, object value, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            if (flags == BindingFlags.Default)
                flags = Flags;

            if (obj == null)
                return false;
            Type type = obj.GetType();

            if (BaseType)
            {
                type = type.BaseType;
            }
            FieldInfo info = type.GetField(fieldName, flags);//GetFieldInfo(type, fieldName, flags);
            if (info != null)
            {
                info.SetValue(obj, value);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Задать значение <see cref="value"> статического поля <see cref="fieldName"> типа <see cref="type">.
        /// </summary>
        /// <param name="type">The type that contains static filed</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <param name="value">The value which assigns to the private field</param>
        /// <param name="flags"></param>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        public static bool SetStaticFieldValue(Type type, string fieldName, object value, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            if (flags == BindingFlags.Default)
                flags = Flags;
            if (BaseType)
            {
                type = type.BaseType;
            }

            FieldInfo info = type.GetField(fieldName, flags | BindingFlags.Static);
            if (info != null)
            {
                info.SetValue(type, value);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Получить значение приватного поля <see cref="fieldName"> объекта <see cref="type">
        /// </summary>
        /// <param name="obj">The instance from which to read the private value.</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <returns>The value of the property boxed as an object.</returns>
        public static bool GetFieldValue(object obj, string fieldName, out object result, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            result = null;

            if (obj == null)
                return false;
            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = Flags;

            if (BaseType)
            {
                type = type.BaseType;
            }

            FieldInfo fi = type.GetField(fieldName, flags);//GetPrivateFieldInfo(type, fieldName, flags);
            if (fi != null)
            {
                result = fi.GetValue(obj);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Получить значение статического поля <see cref="fieldName"> типа <see cref="type">
        /// </summary>
        /// <param name="type">The Type that has the private field</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <returns>The value of the property boxed as an object.</returns>
        public static bool GetStaticFieldValue(Type type, string fieldName, out object result, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            result = null;
            if (flags == BindingFlags.Default)
                flags = Flags;
            if (BaseType)
            {
                type = type.BaseType;
            }

            FieldInfo fi = type.GetField(fieldName, flags | BindingFlags.Static);
            if (fi != null)
            {
                result = fi.GetValue(type);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Вызов метода <see cref="methodName"> объекта <see cref="obj">
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments">Массив с аргументами вызываемого метода. Если аргумент ссылочный вида ref или out, то после вызова invoke массив <see cref="arguments"> будет изменен</param>
        /// <param name="flags"></param>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        public static bool ExecMethod(object obj, string methodName, ref Object[] arguments, out object result, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            result = null;

            if (obj == null)
                return false;
            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = Flags;

            flags = Flags | flags;
            if (BaseType)
            {
                type = type.BaseType;
            }

            MethodInfo m = type.GetMethod(methodName, flags);
            if (m != null)
            {
                result = m.Invoke(obj, arguments);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Вызов статического метода <see cref="methodName"> типа <see cref="type">
        /// </summary>
        /// <param name="type"></param>
        /// <param name="MethodName"></param>
        /// <param name="arguments">Массив с аргументами вызываемого метода. Если аргумент ссылочный вида ref или out, то после вызова invoke массив <see cref="arguments"> будет изменен</param>
        /// <param name="flags"></param>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        public static bool ExecStaticMethod(Type type, string MethodName, ref Object[] arguments, out object result, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            result = null;

            if (flags == BindingFlags.Default)
                flags = Flags;

            if (BaseType)
            {
                type = type.BaseType;
            }

            MethodInfo methodInfo = type.GetMethod(MethodName, flags | BindingFlags.Static);
            if (methodInfo != null)
            {
                result = methodInfo.Invoke(null, arguments);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Подписка на событие
        /// </summary>
        /// <param name="obj">Объект класса в котором создано событие</param>
        /// <param name="control">Control в котром формируется событие. Скажем, кнопка</param>
        /// <param name="typeHandler">делегат: typeof(...)</param>
        /// <param name="EventName"></param>
        /// <param name="method">метод, который будет вызываться при возникновении события</param>
        /// <param name="IsConsole"></param>
        public static void SubscribeEvent(object obj, Type control, Type typeHandler, string EventName, MethodInfo method, bool IsConsole = false)
        {
            EventInfo eventInfo = control.GetEvent(EventName); //"Load"
                                                               // Create the delegate on the test class because that's where the
                                                               // method is. This corresponds with `new EventHandler(test.WriteTrace)`.
                                                               //Type type = typeof(EventHandler);
            Delegate handler;
            if (IsConsole)
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

        /// <summary>
        /// Подписка на событие
        /// </summary>
        /// <param name="obj">Объект класса в котором создано событие</param>
        /// <param name="control">Control в котром формируется событие. Скажем, кнопка</param>
        /// <param name="typeHandler">делегат: typeof(...)</param>
        /// <param name="EventName"></param>
        /// <param name="method">метод, который будет вызываться при возникновении события</param>
        public static void SubscribeEvent(object obj, Control control, Type typeHandler, string EventName, MethodInfo method)
        {
            if (typeof(Control).IsAssignableFrom(control.GetType()))
            {
                SubscribeEvent(obj, control.GetType(), typeHandler, EventName, method);
            }
        }

        /// <summary>
        /// Отписка от события
        /// </summary>
        /// <param name="obj">Объект, на событие которого подписываемся</param>
        /// <param name="control"></param>
        /// <param name="typeHandler"></param>
        /// <param name="EventName"></param>
        /// <param name="method"></param>
        /// <param name="IsConsole"></param>
        public static void UnSubscribeEvent(object obj, Type control, Type typeHandler, string EventName, MethodInfo method, bool IsConsole = false)
        {
            if (obj != null)
            {
                EventInfo eventInfo = control.GetEvent(EventName);
                //Type type = typeof(EventHandler);
                if (IsConsole)
                {
                    Delegate handler = Delegate.CreateDelegate(typeHandler, null, method);
                    // detach the event handler
                    if (handler != null)
                        eventInfo.RemoveEventHandler(obj, handler);
                }
                else
                {
                    Delegate handler = Delegate.CreateDelegate(typeHandler, obj, method);
                    // detach the event handler
                    if (handler != null)
                        eventInfo.RemoveEventHandler(control, handler);
                }
            }
        }
        public static void UnSubscribeEvent(object obj, Control control, Type typeHandler, string EventName, MethodInfo method)
        {
            if (typeof(Control).IsAssignableFrom(control.GetType()))
            {
                UnSubscribeEvent(obj, control.GetType(), typeHandler, EventName, method);
            }
        }

        //public static void DumpAssembly(string path, string methodName)
        //{
        //    System.IO.File.AppendAllText(methodName+".dump", "Dump started... " + Environment.NewLine);
        //    var assembly = AssemblyDefinition.ReadAssembly(path);
        //    foreach (var typeDef in assembly.MainModule.Types)
        //    {
        //        foreach (var method in typeDef.Methods)
        //        {
        //            if (String.IsNullOrEmpty(methodName) || method.Name == methodName)
        //            {
        //                System.IO.File.AppendAllText(methodName + ".dump", "Method: " + method.ToString());
        //                System.IO.File.AppendAllText(methodName + ".dump", Environment.NewLine);
        //                foreach (var instruction in method.Body.Instructions)
        //                {
        //                    System.IO.File.AppendAllText(methodName + ".dump", instruction.ToString() + Environment.NewLine);
        //                }
        //            }
        //        }
        //    }
        //}
        //public static bool DumpClassMethod(Type type, string methodName)
        //{
        //    if (type == null || string.IsNullOrEmpty(methodName))
        //        return false;

        //    TypeDefinition typeDef = new TypeDefinition(type.Namespace, type.Name, Mono.Cecil.TypeAttributes.Class | Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.NotPublic);
        //    foreach (var method in typeDef.Methods)
        //    {
        //        if (string.IsNullOrEmpty(methodName) || method.Name == methodName)
        //        {
        //            System.IO.File.AppendAllText(methodName + ".dump", "Method: " + method.FullName);
        //            System.IO.File.AppendAllText(methodName + ".dump", Environment.NewLine);

        //            foreach (var instruction in method.Body.Instructions)
        //                System.IO.File.AppendAllText(methodName + ".dump", instruction.ToString() + Environment.NewLine);

        //            break;
        //        }
        //    }
        //    return true;
        //}
    }
}
