using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ACTP0Tools.Reflection
{
    /// <summary>
    /// Основано на статье 
    /// Обращение к private методам, свойствам, полям и подписка на события с помощью Reflection в .NET [C#]
    /// Andrey Fedorov
    /// http://www.bizkit.ru/2018/05/30/14102/
    /// </summary>
    public static partial class ReflectionHelper
    {
        public static readonly BindingFlags DefaultFlags = BindingFlags.Instance
                                                         | BindingFlags.Static
                                                         | BindingFlags.Public
                                                         | BindingFlags.NonPublic
                                                         | BindingFlags.GetField
                                                         | BindingFlags.SetField
                                                         | BindingFlags.GetProperty
                                                         | BindingFlags.SetProperty;

        public static readonly object[] EmptyObjectArray = new object[0];
        public static readonly Type[]   EmptyTypeArray =   new Type[0];

        /// <summary>
        /// Получение списка методов объекта <param name="obj"/>
        /// </summary>
        public static MethodInfo[] GetListOfMethods(object obj, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            if (BaseType)
            {
                type = type.BaseType;
            }
            return GetListOfMethods(type, flags);
        }

        /// <summary>
        /// Получение списка методов типа <param name="type"/>
        /// </summary>
        public static MethodInfo[] GetListOfMethods(Type type, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            // get all public static methods of MyClass type
            MethodInfo[] methodInfos = type.GetMethods(flags);

            return methodInfos;
        }

        /// <summary>
        /// Получение списка полей объекта <param name="obj"/>
        /// </summary>
        public static IEnumerable<FieldInfo> EnumerateFields(this Type type, BindingFlags flags = BindingFlags.Default, bool baseType = true)
        {
            if (type is null)
                yield break;

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            foreach (var field in type.GetFields(flags))
                yield return field;
            if (!baseType)
                yield break;
            
            type = type.BaseType;
            while (type != null)
            {
                foreach (var field in type.GetFields(flags))
                    yield return field;
                type = type.BaseType;
            }
        }

        /// <summary>
        /// Получить <seealso cref="FieldInfo"/>, описывающий приватное поле <param name="fieldName"/> типа <param name="type"/>
        /// </summary>
        /// <param name="type">The Type that has the private field</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <returns>FieldInfo object. It has the field name and a useful GetValue() method.</returns>
        public static FieldInfo GetFieldInfo(Type type, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            FieldInfo[] fields = type.GetFields(flags);
            if (fields.Length > 0)
            {
                foreach (FieldInfo fi in fields) //fields.FirstOrDefault(feildInfo => feildInfo.Name == fieldName);
                {
                    if (string.Equals(fi.Name, fieldName, StringComparison.CurrentCultureIgnoreCase))
                        return fi;
                }
            }
            return null;
        }

        /// <summary>
        /// Поиск типа по имени <param name="typeName"/>
        /// </summary>
        public static Type GetTypeByName(string typeName, bool fullTypeName = false)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            if (fullTypeName)
                return AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType(typeName))
                    .FirstOrDefault(type => type != null);
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetTypes().FirstOrDefault(t => t.Name == typeName))
                .FirstOrDefault(type => type != null);
        }

        /// <summary>
        /// Задать значение <param name="value"/> совойству <param name="propName"/> объекта <param name="obj"/>.
        /// </summary>
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
                    object[] arg = { value };
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
            MethodInfo[] accessors = pi.GetAccessors((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
            if (accessors != null && accessors.Length == 2)
            {
                object[] arg = { value };
                accessors[1]?.Invoke(obj, arg);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Задать значение <param name="value"/> статическому совойству <param name="propName"/>, объявленному в типе <param name="type"/>.
        /// </summary>
        /// <returns>The value of the property boxed as an object.</returns>
        public static bool SetStaticPropertyValue(Type type, string propName, object value, BindingFlags flags = BindingFlags.Default, bool searchBaseRecursive = false)
        {
            //if (flags == BindingFlags.Default)
            //    flags = DefaultFlags;
            //if (baseType)
            //{
            //    type = type.baseType;
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
                    object[] arg = { value };
                    accessors[1]?.Invoke(null, arg);
                    return true;
                }
            }
            else if (searchBaseRecursive)
                return SetBasePropertyValue(type.BaseType, null, propName, value, flags);

            return false;
        }

        /// <summary>
        /// Получить значение свойства <param name="propName"/> объекта <param name="obj"/>
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
                    object[] arg = { };
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
            MethodInfo[] accessors = pi.GetAccessors((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
            if (accessors != null && accessors.Length > 0)
            {
                object[] arg = { };
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

            return false;
        }

        /// <summary>
        /// Получить значение статического свойства <param name="propName"/> типа <param name="type"/>
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
                    object[] arg = { };
                    result = getter.Invoke(null, arg);
                    return true;
                }
            }
            else if (searchBaseRecursive)
                return GetBasePropertyValue(type.BaseType, null, propName, out result, flags);

            return false;
        }


        /// <summary>
        /// Задать значение <param name="value"/> поля <param name="fieldName"/> объекта <param name="obj"/>.
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

            return false;
        }

        /// <summary>
        /// Задать значение <param name="value"/> статического поля <param name="fieldName"/> типа <param name="type"/>.
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

            return false;
        }

        /// <summary>
        /// Получить значение приватного поля <param name="fieldName"/> объекта <param name="type"/>
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
        /// Получить значение статического поля <param name="fieldName"/> типа <param name="type"/>
        /// </summary>
        /// <param name="type">The Type that has the private field</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <returns>The value of the property boxed as an object.</returns>
        public static bool GetStaticFieldValue(Type type, string fieldName, out object result, BindingFlags flags = BindingFlags.Default, bool baseType = false)
        {
            result = null;
            if (type is null)
                return false;

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;


            FieldInfo fi = type.GetField(fieldName, flags | BindingFlags.Static);
            if (fi != null)
            {
                result = fi.GetValue(type);
                return true;
            }

            if (baseType && (type = type.BaseType) != null)
                return GetStaticFieldValue(type, fieldName, out result, flags, baseType);
            return false;
        }


        /// <summary>
        /// Вызов метода <param name="methodName"/> объекта <param name="obj"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments">Массив с аргументами вызываемого метода. Если аргумент ссылочный вида ref или out, то после вызова invoke массив <see cref="arguments"> будет изменен</param>
        /// <param name="flags"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool ExecMethod(object obj, string methodName, object[] arguments, out object result, BindingFlags flags = BindingFlags.Default, bool baseType = false)
        {
            result = null;

            if (obj == null)
                return false;

            Type type = obj.GetType();

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            if (baseType)
            {
                type = type.BaseType;
            }

            MethodInfo m = type?.GetMethod(methodName, flags);
            if (m != null)
            {
                result = m.Invoke(obj, arguments);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Вызов статического метода <param name="methodName"/> типа <param name="type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments">Массив с аргументами вызываемого метода. Если аргумент ссылочный вида ref или out, то после вызова invoke массив <param name="arguments"/> будет изменен</param>
        /// <param name="flags"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool ExecStaticMethod(Type type, string methodName, object[] arguments, out object result, BindingFlags flags = BindingFlags.Default, bool baseType = false)
        {
            result = null;

            if (type is null)
                return false;

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            MethodInfo methodInfo = type.GetMethod(methodName, flags | BindingFlags.Static);
            if (methodInfo != null)
            {
                result = methodInfo.Invoke(null, arguments);
                return true;
            }

            if (baseType && (type = type.BaseType) != null)
                return ExecStaticMethod(type, methodName, arguments, out result, flags);

            return false;
        }
        public static bool ExecStaticMethodByArgs(Type type, string methodName, object[] arguments, out object result, BindingFlags flags = BindingFlags.Default, bool baseType = false)
        {
            result = null;

            if (type is null)
                return false;

            if (flags == BindingFlags.Default)
                flags = DefaultFlags;

            Type[] types;
            if (arguments != null && arguments.Length > 0)
            {
                types = new Type[arguments.Length];
                for (int i = 0; i < arguments.Length; i++)
                    types[i] = arguments[i].GetType();
            }
            else types = new Type[] { };

            MethodInfo methodInfo = type.GetMethod(methodName, flags | BindingFlags.Static, null, types, null);
            if (methodInfo != null)
            {
                result = methodInfo.Invoke(null, arguments);
                return true;
            }

            if (baseType && (type = type.BaseType) != null)
                return ExecStaticMethodByArgs(type, methodName, arguments, out result, flags);

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
        public static bool SubscribeEvent(object source, string eventName, object target, string methodName, bool searchBase = false, BindingFlags sourceFlags = BindingFlags.Default, BindingFlags targetFlags = BindingFlags.Default)
        {
            if (source != null && !string.IsNullOrEmpty(eventName)
                && target != null && !string.IsNullOrEmpty(methodName))
            {
                if (sourceFlags == BindingFlags.Default)
                    sourceFlags = DefaultFlags;

                if (targetFlags == BindingFlags.Default)
                    targetFlags = DefaultFlags;

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
        public static bool SubscribeEventStatic(object source, string eventName, Type target, string methodName, bool searchBase = false, BindingFlags sourceFlags = BindingFlags.Default, BindingFlags targetFlags = BindingFlags.Default)
        {
            if (source != null && !string.IsNullOrEmpty(eventName)
                && target != null && !string.IsNullOrEmpty(methodName))
            {
                if (sourceFlags == BindingFlags.Default)
                    sourceFlags = DefaultFlags;

                if (targetFlags == BindingFlags.Default)
                    targetFlags = DefaultFlags;

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
        public static bool UnsubscribeEvent(object source, string eventName, object target, string methodName, bool searchBase = false, BindingFlags sourceFlags = BindingFlags.Default, BindingFlags targetFlags = BindingFlags.Default)
        {
            if (source != null && !string.IsNullOrEmpty(eventName)
                && target != null && !string.IsNullOrEmpty(methodName))
            {
                if (sourceFlags == BindingFlags.Default)
                    sourceFlags = DefaultFlags;

                if (targetFlags == BindingFlags.Default)
                    targetFlags = DefaultFlags;

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
        public static bool UnsubscribeEventStatic(object source, string eventName, Type target, string methodName, bool searchBase = false, BindingFlags sourceFlags = BindingFlags.Default, BindingFlags targetFlags = BindingFlags.Default)
        {
            if (source != null && !string.IsNullOrEmpty(eventName)
                && target != null && !string.IsNullOrEmpty(methodName))
            {
                if (sourceFlags == BindingFlags.Default)
                    sourceFlags = DefaultFlags;

                if (targetFlags == BindingFlags.Default)
                    targetFlags = DefaultFlags;

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
    }
}
