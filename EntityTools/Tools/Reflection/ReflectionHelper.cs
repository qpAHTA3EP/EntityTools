using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Основано на статье 
    /// Обращение к private методам, свойствам, полям и подписка на события с помощью Reflection в .NET [C#]
    /// Andrey Fedorov
    /// http://www.bizkit.ru/2018/05/30/14102/
    /// </summary>
    public static class ReflectionHelper
    {
        public static BindingFlags DefaultFlags = BindingFlags.Instance
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
                type = type.BaseType;
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
            flags = DefaultFlags | flags;
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

            flags = DefaultFlags | flags;
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
                flags = DefaultFlags;
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
                        return assambly.GetTypes().FirstOrDefault(t => t.FullName == typeName);
                    else return assambly.GetTypes().FirstOrDefault(t => t.Name == typeName);
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
                flags = DefaultFlags;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.Instance);//GetPrivateFieldInfo(type, fieldName, flags);
            if (pi != null)
            {
                MethodInfo[] accessors = pi.GetAccessors((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
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
                flags = DefaultFlags | BindingFlags.Instance | BindingFlags.SetProperty;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.Instance);//GetPrivateFieldInfo(type, fieldName, flags);
            if (pi == null)
                return SetBasePropertyValue(type.BaseType, obj, propName, value, flags | BindingFlags.Instance);
            else
            {
                MethodInfo[] accessors = pi.GetAccessors((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
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
        /// Задать значение <see cref="value"> статическому совойству <see cref="propName">, объявленному в типе <see cref="type">.
        /// </summary>
        /// <returns>The value of the property boxed as an object.</returns>
        public static bool SetStaticPropertyValue(Type type, string propName, object value, BindingFlags flags = BindingFlags.Default, bool searchBaseRecursive = false)
        {
            //if (flags == BindingFlags.Default)
            //    flags = DefaultFlags;
            //if (BaseType)
            //{
            //    type = type.BaseType;
            //}
            //object[] arg = new object[] { value };
            //return ExecStaticMethod(type, "set_" + propName, ref arg, out object result, flags | BindingFlags.Static | BindingFlags.InvokeMethod, false);

            if (flags == BindingFlags.Default)
                flags = DefaultFlags | BindingFlags.Static | BindingFlags.SetProperty;

            PropertyInfo pi = type.GetProperty(propName, flags | BindingFlags.Static);//GetPrivateFieldInfo(type, fieldName, flags);
            if (pi != null)
            {
                MethodInfo[] accessors = pi.GetAccessors((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
                if (accessors != null && accessors.Length == 2)
                {
                    object[] arg = new object[] { value };
                    accessors[1]?.Invoke(null, arg);
                    return true;
                }
            }
            else if (searchBaseRecursive)
                return SetBasePropertyValue(type.BaseType, null, propName, value, flags);

            return false;
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
                flags = DefaultFlags | BindingFlags.Instance;

            PropertyInfo pi = type.GetProperty(propName, flags);
            if (pi != null)
            {
                MethodInfo getter = pi.GetGetMethod((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
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
                flags = DefaultFlags;

            PropertyInfo pi = type.GetProperty(propName, flags);
            if (pi == null)
                return GetBasePropertyValue(type.BaseType, obj, propName, out result, flags);
            else
            {
                MethodInfo[] accessors = pi.GetAccessors((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
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
        /// <returns>The value of the property boxed as an object.</returns>
        public static bool GetStaticPropertyValue(Type type, string propName, out object result, BindingFlags flags = BindingFlags.Default, bool searchBaseRecursive = false)
        {
            result = null;

            if (flags == BindingFlags.Default)
                flags = DefaultFlags | BindingFlags.Static;

            PropertyInfo pi = type.GetProperty(propName, flags);
            if (pi != null)
            {
                MethodInfo getter = pi.GetGetMethod((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
                if (getter != null)
                {
                    object[] arg = new object[] { };
                    result = getter.Invoke(null, arg);
                    return true;
                }
            }
            else if (searchBaseRecursive)
                return GetBasePropertyValue(type.BaseType, null, propName, out result, flags);

            return false;
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
                flags = DefaultFlags;

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
                flags = DefaultFlags;
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
                flags = DefaultFlags;

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
                flags = DefaultFlags;
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
        public static bool ExecMethod(object obj, string methodName, object[] arguments, out object result, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            result = null;

            if (obj == null)
                return false;
            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

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
        public static bool ExecStaticMethod(Type type, string MethodName, object[] arguments, out object result, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            result = null;

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

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
        public static bool ExecStaticMethodByArgs(Type type, string MethodName, object[] arguments, out object result, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            result = null;

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            if (BaseType)
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

            MethodInfo methodInfo = type.GetMethod(MethodName, flags | BindingFlags.Static, null, types, null);
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
        /// <param name="source">Объект, генерируеющий событие</param>
        /// <param name="eventName">Название события</param>
        /// <param name="target">Объект-подписчик (слушатель)</param>
        /// <param name="methodName">Название метода объекта-подписчика, вызываемого при возникновении события</param>
        /// <param name="searchBase">Искать ли объявления в базовых классах</param>
        /// <param name="sourceFlags"></param>
        /// <param name="targetFlags"></param>
        /// <returns></returns>
        public static bool SubscribeEvent(Object source, string eventName, Object target, string methodName, bool searchBase = false, BindingFlags sourceFlags = BindingFlags.Default, BindingFlags targetFlags = BindingFlags.Default)
        {
            if (source != null && !string.IsNullOrEmpty(eventName)
                && target != null && !string.IsNullOrEmpty(methodName))
            {
                EventInfo eventInfo = (sourceFlags == BindingFlags.Default) ? source.GetType().GetEvent(eventName)
                                                    : source.GetType().GetEvent(eventName, sourceFlags);

                MethodInfo methodInfo = (targetFlags == BindingFlags.Default) ? target.GetType().GetMethod(methodName)
                                                    : target.GetType().GetMethod(methodName, targetFlags);

                if (eventInfo != null && methodInfo != null)
                {
                    Delegate @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, target, methodInfo);

                    eventInfo.AddEventHandler(source, @delegate);

                    return true;
                }
            }
            return false;
        }
        public static bool SubscribeEventStatic(Object source, string eventName, Type target, string methodName, bool searchBase = false, BindingFlags sourceFlags = BindingFlags.Default, BindingFlags targetFlags = BindingFlags.Default)
        {
            if (source != null && !string.IsNullOrEmpty(eventName)
                && target != null && !string.IsNullOrEmpty(methodName))
            {
                EventInfo eventInfo = (sourceFlags == BindingFlags.Default) ? source.GetType().GetEvent(eventName)
                                                    : source.GetType().GetEvent(eventName, sourceFlags);

                MethodInfo methodInfo = (targetFlags == BindingFlags.Default) ? target.GetMethod(methodName)
                                                    : target.GetMethod(methodName, targetFlags | BindingFlags.Static);

                if (eventInfo != null && methodInfo != null)
                {
                    Delegate @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, null, methodInfo);

                    eventInfo.AddEventHandler(source, @delegate);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Отказ от подписки на событие
        /// </summary>
        /// <param name="source">Объект, генерируеющий событие</param>
        /// <param name="eventName">Название события</param>
        /// <param name="target">Объект-подписчик (слушатель)</param>
        /// <param name="methodName">Название метода объекта-подписчика, вызываемого при возникновении события</param>
        /// <param name="searchBase">Искать ли объявления в базовых классах</param>
        /// <param name="sourceFlags"></param>
        /// <param name="targetFlags"></param>
        /// <returns></returns>
        public static bool UnsubscribeEvent(Object source, string eventName, Object target, string methodName, bool searchBase = false, BindingFlags sourceFlags = BindingFlags.Default, BindingFlags targetFlags = BindingFlags.Default)
        {
            if (source != null && !string.IsNullOrEmpty(eventName)
                && target != null && !string.IsNullOrEmpty(methodName))
            {
                EventInfo eventInfo = (sourceFlags == BindingFlags.Default) ? source.GetType().GetEvent(eventName)
                                                    : source.GetType().GetEvent(eventName, sourceFlags);

                MethodInfo methodInfo = (targetFlags == BindingFlags.Default) ? target.GetType().GetMethod(methodName)
                                                    : target.GetType().GetMethod(methodName, targetFlags);

                if (eventInfo != null && methodInfo != null)
                {
                    Delegate @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, target, methodInfo);

                    eventInfo.RemoveEventHandler(source, @delegate);

                    return true;
                }
            }
            return false;
        }
        public static bool UnsubscribeEventStatic(Object source, string eventName, Type target, string methodName, bool searchBase = false, BindingFlags sourceFlags = BindingFlags.Default, BindingFlags targetFlags = BindingFlags.Default)
        {
            if (source != null && !string.IsNullOrEmpty(eventName)
                && target != null && !string.IsNullOrEmpty(methodName))
            {
                EventInfo eventInfo = (sourceFlags == BindingFlags.Default) ? source.GetType().GetEvent(eventName)
                                                    : source.GetType().GetEvent(eventName, sourceFlags);

                MethodInfo methodInfo = (targetFlags == BindingFlags.Default) ? target.GetMethod(methodName)
                                                    : target.GetMethod(methodName, targetFlags | BindingFlags.Static);

                if (eventInfo != null && methodInfo != null)
                {
                    Delegate @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, null, methodInfo);

                    eventInfo.RemoveEventHandler(source, @delegate);

                    return true;
                }
            }
            return false;
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
