using System;
using System.Reflection;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Фабрика делегатов, осуществляющих через механизм рефлексии 
    /// доступ к статическим методам класса, возвращающим значение заданного типа
    /// </summary>
    public static class StaticFunctionFactory
    {
        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к статическкому методу класса, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<ReturnT> GetStaticFunction<ReturnT>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticFunction<Func<ReturnT>, ReturnT>(containerType, methodName, new Type[] { }, flags);
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к статическкому методу класса, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<ArgumentT1, ReturnT>
                                      GetStaticFunction<ArgumentT1, ReturnT>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticFunction<Func<ArgumentT1, ReturnT>, ReturnT>(containerType, methodName,
                                new[] { typeof(ArgumentT1) }, flags);
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к статическкому методу класса, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<ArgumentT1, ArgumentT2, ReturnT>
                                      GetStaticFunction<ArgumentT1, ArgumentT2, ReturnT>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticFunction<Func<ArgumentT1, ArgumentT2, ReturnT>, ReturnT>(containerType, methodName,
                            new[] { typeof(ArgumentT1), typeof(ArgumentT2) }, flags);
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к статическкому методу класса, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>
                                      GetStaticFunction<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticFunction<Func<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>, ReturnT>(containerType, methodName,
                            new[] { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) },
                            flags);
        }

        public static Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>
                                      GetStaticFunction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticFunction<Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>, ReturnT>(containerType, methodName,
                            new[] { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4) },
                            flags);
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к статическкому методу класса, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>
                                      GetStaticFunction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>(this Type containerType, string methodName, BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticFunction<Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>, ReturnT>(containerType, methodName,
                         new[] { typeof(ArgumentT1), typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4), typeof(ArgumentT5) },
                        flags);
        }

        private static DelegateT ConstructStaticFunction<DelegateT, ReturnT>(Type containerType, string methodName, Type[] argumentTypes, BindingFlags flags = BindingFlags.Default) where DelegateT : class
        {
            if (containerType is null)
                return null;

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            if (argumentTypes == null)
                argumentTypes = new Type[] { };

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (FindBySignature(containerType, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out MethodInfo method))
                    return Delegate.CreateDelegate(typeof(DelegateT), method) as DelegateT;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (FindByNameAndSignature(containerType, methodName, typeof(ReturnT), argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out MethodInfo method))
                    return Delegate.CreateDelegate(typeof(DelegateT), method) as DelegateT;
            }
            return null;
        }

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
        private static bool FindByNameAndSignature(Type type, string methodName, Type returnType, Type[] inputTypes, BindingFlags flags, out MethodInfo method)
        {
            if (type != null)
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
        private static bool FindBySignature(Type type, Type returnType, Type[] inputTypes, BindingFlags flags, out MethodInfo method)
        {
            if (type != null)
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
}
