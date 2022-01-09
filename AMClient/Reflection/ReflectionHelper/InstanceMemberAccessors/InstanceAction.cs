using System;
using System.Reflection;

namespace AcTp0Tools.Reflection
{
    /// <summary>
    /// Фабрика делегатов, осуществляющих через механизм рефлексии 
    /// доступ к методу объекта заданного типа, и не возвращающего значение
    /// </summary>
    public static partial class ReflectionHelper
    {
        /// <summary>
        /// Конструирование делегата <seealso cref="Action{object}"/>, осуществляющего через механизм рефлексии, доступ к методу <paramref name="methodName"/> объекта заданного типа <paramref name="type"/>.
        /// Первым аргументом сконструированного делегата является объект <see langword="object"/>, метод которого необходимо вызвать.
        /// </summary>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static Action<object> GetAction(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type,argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method!=null)
            {
                void InstanceMethod(object o)
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        method.Invoke(o, new object[] { });
                    }
                }

                return InstanceMethod;
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата <see cref="Action{object, ArgumentT1}"/>, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта заданного типа <paramref name="type"/>, принимающего один аргумент типа <typeparamref name="ArgumentT1"/>
        /// Первым аргументом сконструированного делегата является объект <see langword="object"/>, метод которого должен быть вызван.
        /// </summary>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static Action<object, ArgumentT1>
                                      GetAction<ArgumentT1>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                void InstanceMethod(object o, ArgumentT1 a1) 
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        method.Invoke(o, new object[] { a1 });
                    }
                }

                return InstanceMethod;
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата <see cref="Action{object, ArgumentT1, ArgumentT2}"/>, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта заданного типа <paramref name="type"/>, принимающего два аргумента типа <typeparamref name="ArgumentT1"/> и <typeparamref name="ArgumentT2"/>
        /// Первым аргументом сконструированного делегата является объект <see langword="object"/>, метод которого должен быть вызван.
        /// </summary>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <typeparam name="ArgumentT2"></typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static Action <object, ArgumentT1, ArgumentT2>
                                      GetAction<ArgumentT1, ArgumentT2>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                void InstanceMethod(object o, ArgumentT1 a1, ArgumentT2 a2)
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        method.Invoke(o, new object[] { a1, a2 });
                    }
                }

                return InstanceMethod;
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата <see cref="Action{object, ArgumentT1, ArgumentT2, ArgumentT3}"/>, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта заданного типа <paramref name="type"/>, принимающего три аргумента типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/> и <typeparamref name="ArgumentT3"/>.
        /// Первым аргументом сконструированного делегата является объект <see langword="object"/>, метод которого должен быть вызван.
        /// </summary>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <typeparam name="ArgumentT2"></typeparam>
        /// <typeparam name="ArgumentT3"></typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static Action<object, ArgumentT1, ArgumentT2, ArgumentT3>
                                      GetAction<ArgumentT1, ArgumentT2, ArgumentT3>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                void InstanceMethod(object o, ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3)
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        method.Invoke(o, new object[] { a1, a2, a3 });
                    }
                }

                return InstanceMethod;
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата <see cref="Action{object, ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4}"/>, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа <paramref name="type"/>, принимающего четыре аргумента типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>, <typeparamref name="ArgumentT3"/> и <typeparamref name="ArgumentT4"/>
        /// Первым аргументом сконструированного делегата является объект <see langword="object"/>, метод которого должен быть вызван.
        /// </summary>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <typeparam name="ArgumentT2"></typeparam>
        /// <typeparam name="ArgumentT3"></typeparam>
        /// <typeparam name="ArgumentT4"></typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static Action<object, ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>
                                      GetAction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                void InstanceMethod(object o, ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3, ArgumentT4 a4)
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        method.Invoke(o, new object[] { a1, a2, a3, a4 });
                    }
                }

                return InstanceMethod;
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата <see cref="Action{object, ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5}"/>, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа <paramref name="type"/>, принимающего пять аргументов типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>, <typeparamref name="ArgumentT3"/>, <typeparamref name="ArgumentT4"/> и <typeparamref name="ArgumentT5"/>
        /// Первым аргументом сконструированного делегата является объект <see langword="object"/>, метод которого должен быть вызван.
        /// </summary>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <typeparam name="ArgumentT2"></typeparam>
        /// <typeparam name="ArgumentT3"></typeparam>
        /// <typeparam name="ArgumentT4"></typeparam>
        /// <typeparam name="ArgumentT5"></typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static Action<object, ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>
                                      GetAction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>(this Type type, string methodName, BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4), typeof(ArgumentT5) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                void InstanceMethod(object o, ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3, ArgumentT4 a4, ArgumentT5 a5)
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        method.Invoke(o, new object[] { a1, a2, a3, a4, a5 });
                    }
                }

                return InstanceMethod;
            }
            return null;
        }

#if false
        /// <summary>
        /// Поиск метода по сигнатуре и имени
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="inputTypes"></param>
        /// <param name="flags"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static bool FindByNameAndSignature(Type type, string methodName, Type[] inputTypes, BindingFlags flags, out MethodInfo method)
        {
            if (type != null)
            {
                method = type.GetMethod(methodName, flags, null, inputTypes, null);
                if (method != null)
                {
                    return true;
                }
                return FindByNameAndSignature(type.BaseType, methodName, inputTypes, flags, out method);
            }
            method = null;
            return false;
        }

        /// <summary>
        /// Поиск метода только сигнатуре (без имени)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inputTypes"></param>
        /// <param name="flags"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static bool FindBySignature(Type type, Type[] inputTypes, BindingFlags flags, out MethodInfo method)
        {
            if (type != null)
            {
                foreach (MethodInfo methodInfo in type.GetMethods(flags | BindingFlags.Static))
                {
                    if (methodInfo.GetParameters().Length == inputTypes.Length)
                    {
                        var arguments = methodInfo.GetParameters();
                        bool flag = true;
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            if (!arguments[i].ParameterType.Equals(inputTypes[i]))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            method = methodInfo;
                            return true;
                        }
                    }
                }
                return FindBySignature(type.BaseType, inputTypes, flags, out method);
            }
            method = null;
            return false;
        } 
#endif
    }
}
