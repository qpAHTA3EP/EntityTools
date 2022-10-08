using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace ACTP0Tools.Reflection
{
    /// <summary>
    /// Фабрика делегатов, осуществляющих через механизм рефлексии 
    /// доступ к методу объекта заданного типа, и возвращающего значение типа ReturnT
    /// </summary>
    public static class InstanceFunctionFactory
    {
        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа <param name="type"></param>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<object, Func<ReturnT>> GetFunctionAccessor<ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { };

            MethodInfo method = null;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return () =>
                        {
                            object result = method.Invoke(o, new object[] { });
                            if (Equals(result, null))
                                return default;
                            return (ReturnT)result;
                        };
                    }
                    return null;
                };
            }
            return null;
        }
#else
        public static Func<object, ReturnT> GetFunctionAccessor<ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
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
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return o =>
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        object result = method.Invoke(o, new object[] { });
                        if (!Equals(result, null))
                            return (ReturnT)result;
                    }
                    return default;
                };
            }
            return null;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа <param name="type"></param>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<Object, Func<ArgumentT1, ReturnT>>
                              GetFunctionAccessor<ArgumentT1, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1) };

            MethodInfo method = null;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return a1 =>
                        {
                            object result = method.Invoke(o, new object[] { a1 });
                            if (Equals(result, null))
                                return default;
                            return (ReturnT)result;
                        };
                    }
                    return null;
                };
            }
            return null;
        }
#else
        public static Func<object, ArgumentT1, ReturnT>
            GetFunctionAccessor<ArgumentT1, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
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
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return (o, a1) =>
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        object result = method.Invoke(o, new object[] { a1 });
                        if (!Equals(result, null))
                            return (ReturnT)result;
                    }
                    return default;
                };
            }
            return null;
        }
#endif
        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа <param name="type"></param>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <typeparam name="ArgumentT2"></typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<object, Func<ArgumentT1, ArgumentT2, ReturnT>>
                              GetFunctionAccessor<ArgumentT1, ArgumentT2, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2) };

            MethodInfo method = null;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return (a1, a2) =>
                        {
                            object result = method.Invoke(o, new object[] { a1, a2 });
                            if (Equals(result, null))
                                return default;
                            return (ReturnT)result;
                        };
                    }
                    return null;
                };
            }
            return null;
        }
#else
        public static Func<object, ArgumentT1, ArgumentT2, ReturnT>
                                      GetFunctionAccessor<ArgumentT1, ArgumentT2, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
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
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return (o, a1, a2) =>
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        object result = method.Invoke(o, new object[] { a1, a2 });
                        if (!Equals(result, null))
                            return (ReturnT)result;
                    }
                    return default;
                };
            }
            return null;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа <param name="type"></param>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT2">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT3">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<Object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>>
                                      GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) };

            MethodInfo method = null;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return (a1, a2, a3) =>
                        {
                            object result = method.Invoke(o, new object[] { a1, a2, a3 });
                            if (Equals(result, null))
                                return default;
                            return (ReturnT)result;
                        };
                    }
                    return null;
                };
            }
            return null;
        } 
#else
        public static Func<object, ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>
            GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return (o, a1, a2, a3) =>
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        object result = method.Invoke(o, new object[] { a1, a2, a3 });
                        if (!Equals(result, null))
                            return (ReturnT)result;
                    }
                    return default;
                };
            }
            return null;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа <param name="type"></param>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT2">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT3">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT4">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
#if FUNCTION_WRAPPER
        public static Func<Object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>>
                                      GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4) };

            MethodInfo method = null;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return (a1, a2, a3, a4) =>
                        {
                            object result = method.Invoke(o, new object[] { a1, a2, a3, a4 });
                            if (Equals(result, null))
                                return default;
                            return (ReturnT)result;
                        };
                    }
                    return null;
                };
            }
            return null;
        } 
#else
        public static Func<object, ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>
            GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
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
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return (o, a1, a2, a3, a4) =>
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        object result = method.Invoke(o, new object[] { a1, a2, a3, a4 });
                        if (!Equals(result, null))
                            return (ReturnT)result;
                    }
                    return default;
                };
            }
            return null;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа <param name="type"/>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT2">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT3">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT4">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT5">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
#if FUNCTION_WRAPPER
        public static Func<Object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>>
                                      GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>(this Type type, string methodName, BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4), typeof(ArgumentT5) };

            MethodInfo method = null;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return (a1, a2, a3, a4, a5) =>
                        {
                            object result = method.Invoke(o, new object[] { a1, a2, a3, a4, a5 });
                            if (Equals(result, null))
                                return default;
                            return (ReturnT)result;
                        };
                    }
                    return null;
                };
            }
            return null;
        } 
#else
        public static Func<object, ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>
                                      GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>(this Type type, string methodName, BindingFlags flags = BindingFlags.Default)
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
                if (!FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (!FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method))
                    return null;
            }
            if (method != null)
            {
                return (o, a1, a2, a3, a4, a5) =>
                {
                    if (o != null
                        && type.IsInstanceOfType(o))
                    {
                        object result = method.Invoke(o, new object[] { a1, a2, a3, a4, a5 });
                        if (!Equals(result, null))
                            return (ReturnT)result;
                    }
                    return default;
                };
            }
            return null;
        }
#endif


        /// <summary>
        /// Поиск метода по сигнатуре и имени
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="returnType"></param>
        /// <param name="inputTypes"></param>
        /// <param name="flags"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static bool FindByNameAndSignature(Type type, string methodName, Type returnType, Type[] inputTypes, BindingFlags flags, out MethodInfo method)
        {
            if (!Equals(type, null))
            {
                method = type.GetMethod(methodName, flags, null, inputTypes, null);
                if (method != null)
                {
                    if (method.ReturnType.Equals(returnType))
                        return true;
                    method = null;
                    return false;
                }
                return FindByNameAndSignature(type.BaseType, methodName, returnType, inputTypes, flags, out method);
            }
            method = null;
            return false;
        }

        /// <summary>
        /// Поиск метода только сигнатуре (без имени)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="returnType"></param>
        /// <param name="inputTypes"></param>
        /// <param name="flags"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static bool FindBySignature(Type type, Type returnType, Type[] inputTypes, BindingFlags flags, out MethodInfo method)
        {
            if (!Equals(type, null))
            {
                foreach (MethodInfo methodInfo in type.GetMethods(flags | BindingFlags.Static))
                {
                    if (methodInfo.ReturnType.Equals(returnType) && methodInfo.GetParameters().Length == inputTypes.Length)
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
                return FindBySignature(type.BaseType, returnType, inputTypes, flags, out method);
            }
            method = null;
            return false;
        } 
    }

    public class InstanceAccessor<ContainerType> where ContainerType : class
    {
        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта типа <typeparamref name="ContainerType"/>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>.
        /// Единственным аргументом сконструированного делегата является объект типа <typeparamref name="ContainerType"/>, метод которого должен быть вызван.
        /// </summary>
        /// <typeparam name="ContainerType">Тип объекта, содержащего нужный метод</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<ContainerType, Func<ReturnT>>
                                      GetFunction<ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return o =>
                {
                    return () =>
                    {
                        object result = method.Invoke(o, new object[] { });
                        if (Equals(result, null))
                            return default;
                        return (ReturnT)result;
                    };
                };
            }

            return new Dummy<ReturnT>(methodName).DummyMethod;
        } 
#else
        public static Func<ContainerType, ReturnT>
            GetFunctionAccessor<ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return o =>
                {
                    object result = method.Invoke(o, new object[] { });
                    if (!Equals(result, null))
                        return (ReturnT) result;
                    return default;
                };
            }

            return (o) => default;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта типа <typeparamref name="ContainerType"/>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>.
        /// Первым аргументом сконструированного делегата является объект типа <typeparamref name="ContainerType"/>, метод которого должен быть вызван.
        /// Второй аргумент - аргумент вызываемого метода типа <typeparamref name="ArgumentT1"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<ContainerType, Func<ArgumentT1, ReturnT>>
                                      GetFunction<ArgumentT1, ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return o =>
                {
                    return a1 =>
                    {
                        object result = method.Invoke(o, new object[] { a1 });
                        if (Equals(result, null))
                            return default;
                        return (ReturnT)result;
                    };
                };
            }
            return new Dummy<ReturnT>(methodName).DummyMethod<ArgumentT1>;
        } 
#else
        public static Func<ContainerType, ArgumentT1, ReturnT>
            GetFunctionAccessor<ArgumentT1, ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return (o, a1) =>
                {
                    object result = method.Invoke(o, new object[] { a1 });
                    if (!Equals(result, null))
                        return (ReturnT)result;
                    return default;
                };
            }
            return (o, a1) => default;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта типа <typeparamref name="ContainerType"/>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>.
        /// Первым аргументом сконструированного делегата является объект типа <typeparamref name="ContainerType"/>, метод которого должен быть вызван.
        /// Второй и последующие аргументы - аргумент вызываемого метода типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT2">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<ContainerType, Func<ArgumentT1, ArgumentT2, ReturnT>>
                                      GetFunction<ArgumentT1, ArgumentT2, ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return o =>
                {
                    return (a1, a2) =>
                    {
                        object result = method.Invoke(o, new object[] { a1, a2 });
                        if (Equals(result, null))
                            return default;
                        return (ReturnT)result;
                    };
                };
            }
            return new Dummy<ReturnT>(methodName).DummyMethod<ArgumentT1, ArgumentT2>;
        } 
#else
        public static Func<ContainerType, ArgumentT1, ArgumentT2, ReturnT>
            GetFunctionAccessor<ArgumentT1, ArgumentT2, ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return (o, a1, a2) =>
                {
                    object result = method.Invoke(o, new object[] { a1, a2 });
                    if (!Equals(result, null))
                        return (ReturnT)result;
                    return default;
                };
            }
            return (o, a1, a2) => default;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта типа <typeparamref name="ContainerType"/>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>.
        /// Первым аргументом сконструированного делегата является объект типа <typeparamref name="ContainerType"/>, метод которого должен быть вызван.
        /// Второй и последующие аргументы - аргумент вызываемого метода типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>, <typeparamref name="ArgumentT3"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT2">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT3">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
#if FUNCTION_WRAPPER
        public static Func<ContainerType, Func<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>>
                                      GetFunction<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return o =>
                {
                    return (a1, a2, a3) =>
                    {
                        object result = method.Invoke(o, new object[] { a1, a2, a3 });
                        if (Equals(result, null))
                            return default;
                        return (ReturnT)result;
                    };
                };
            }
            return new Dummy<ReturnT>(methodName).DummyMethod<ArgumentT1, ArgumentT2, ArgumentT3>;
        }
#else
        public static Func<ContainerType, ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>
            GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return (o, a1, a2, a3)  =>
                {
                    object result = method.Invoke(o, new object[] { a1, a2, a3 });
                    if (!Equals(result, null))
                        return (ReturnT)result;
                    return default;
                };
            }

            return (o, a1, a2, a3) => default;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта типа <typeparamref name="ContainerType"/>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>.
        /// Первым аргументом сконструированного делегата является объект типа <typeparamref name="ContainerType"/>, метод которого должен быть вызван.
        /// Второй и последующие аргументы - аргумент вызываемого метода типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>, <typeparamref name="ArgumentT3"/>, <typeparamref name="ArgumentT4"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT2">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT3">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT4">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<ContainerType, Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>>
                                      GetFunction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return o =>
                {
                    return (a1, a2, a3, a4) =>
                    {
                        object result = method.Invoke(o, new object[] { a1, a2, a3, a4 });
                        if (Equals(result, null))
                            return default;
                        return (ReturnT)result;
                    };
                };
            }
            return new Dummy<ReturnT>(methodName).DummyMethod<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>;
        } 
#else
        public static Func<ContainerType, ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>
            GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>(string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return (o, a1, a2, a3, a4) =>
                {
                    object result = method.Invoke(o, new object[] { a1, a2, a3, a4 });
                    if (!Equals(result, null))
                        return (ReturnT) result;
                    return default;
                };
            }

            return (o, a1, a2, a3, a4) => default;
        }
#endif

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу <paramref name="methodName"/> объекта типа <typeparamref name="ContainerType"/>,
        /// и возвращающего значение типа <typeparamref name="ReturnT"/>.
        /// Первым аргументом сконструированного делегата является объект типа <typeparamref name="ContainerType"/>, метод которого должен быть вызван.
        /// Второй и последующие аргументы - аргумент вызываемого метода типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>, <typeparamref name="ArgumentT3"/>, <typeparamref name="ArgumentT4"/>, <typeparamref name="ArgumentT5"/>
        /// </summary>
        /// <typeparam name="ArgumentT1">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT2">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT3">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT4">Тип аргумента метода</typeparam>
        /// <typeparam name="ArgumentT5">Тип аргумента метода</typeparam>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        /// <remarks>Следует иметь в виду, что вызов виртуальных методов дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для вызова виртуальных методов объекта, функтор должен быть построен для объекта конкретного типа</remarks>
#if FUNCTION_WRAPPER
        public static Func<ContainerType, Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>>
                                      GetFunction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>(string methodName, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4), typeof(ArgumentT5) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return o =>
                {
                    return (a1, a2, a3, a4, a5) =>
                    {
                        object result = method.Invoke(o, new object[] { a1, a2, a3, a4, a5 });
                        if (Equals(result, null))
                            return default;
                        return (ReturnT)result;
                    };
                };
            }
            return new Dummy<ReturnT>(methodName).DummyMethod<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>;
        } 
#else
        public static Func<ContainerType, ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>
            GetFunctionAccessor<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>(string methodName, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4), typeof(ArgumentT5) };

            MethodInfo method;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                InstanceFunctionFactory.FindBySignature(type, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                InstanceFunctionFactory.FindByNameAndSignature(type, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out method);
            }
            if (method != null)
            {
                return (o, a1, a2, a3, a4, a5) =>
                {
                    object result = method.Invoke(o, new object[] {  });
                    if (!Equals(result, null))
                        return (ReturnT)result;
                    return default;
                };
            }

            return (o, a1, a2, a3, a4, a5) => default;
        }
#endif

#if FUNCTION_WRAPPER
        /// <summary>
        /// Методы заглушки, вызываемый в случае неудачи при получении доступа к члену или методу
        /// </summary>
        /// <typeparam name="ReturnT"></typeparam>
        /// <returns></returns>
        private class Dummy<ReturnT>
        {
            string message;

            public Dummy(string methodName)
            {
                message = $"Fail to access to method '{typeof(ContainerType).Name}.{methodName}'";
            }

            public Func<ReturnT> DummyMethod(ContainerType c)
            {
                return InternalDummyMethod;
            }
            public Func<ArgumentT1, ReturnT> DummyMethod<ArgumentT1>(ContainerType c)
            {
                return InternalDummyMethod;
            }
            public Func<ArgumentT1, ArgumentT2, ReturnT> DummyMethod<ArgumentT1, ArgumentT2>(ContainerType c)
            {
                return InternalDummyMethod;
            }
            public Func<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT> DummyMethod<ArgumentT1, ArgumentT2, ArgumentT3>(ContainerType c)
            {
                return InternalDummyMethod;
            }
            public Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT> DummyMethod<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>(ContainerType c)
            {
                return InternalDummyMethod;
            }
            public Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT> DummyMethod<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>(ContainerType c)
            {
                return InternalDummyMethod;
            }

            private static ReturnT InternalDummyMethod()
            {
                return default;
            }
            private static ReturnT InternalDummyMethod<ArgumentT1>(ArgumentT1 a1)
            {
                return InternalDummyMethod();
            }
            private static ReturnT InternalDummyMethod<ArgumentT1, ArgumentT2>(ArgumentT1 a1, ArgumentT2 a2)
            {
                return InternalDummyMethod();
            }
            private static ReturnT InternalDummyMethod<ArgumentT1, ArgumentT2, ArgumentT3>(ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3)
            {
                return InternalDummyMethod();
            }
            private static ReturnT InternalDummyMethod<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>(ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3, ArgumentT4 a4)
            {
                return InternalDummyMethod();
            }
            private static ReturnT InternalDummyMethod<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>(ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3, ArgumentT4 a4, ArgumentT5 a5)
            {
                return InternalDummyMethod();
            }
        } 
#endif
    }
}
