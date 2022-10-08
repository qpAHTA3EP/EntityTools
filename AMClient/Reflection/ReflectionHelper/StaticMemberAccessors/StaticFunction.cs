using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace ACTP0Tools.Reflection
{
    /// <summary>
    /// Фабрика делегатов, осуществляющих через механизм рефлексии 
    /// доступ к статическим методам класса, возвращающим значение заданного типа
    /// </summary>
    public static partial class ReflectionHelper
    {
        /// <summary>
        /// Конструирование делегата <see cref="Func{ReturnT}"/>, осуществляющего через механизм рефлексии 
        /// доступ к статическому методу типа <paramref name="containerType"/>, и возвращающего значение типа <typeparamref name="ReturnT"/>
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<ReturnT> GetStaticFunction<ReturnT>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticFunction<Func<ReturnT>, ReturnT>(containerType, methodName, new Type[] { }, flags);
        }

        /// <summary>
        /// Конструирование делегата <see cref="Func{ArgumentT1, ReturnT}"/>, осуществляющего через механизм рефлексии 
        /// доступ к статическому методу типа <paramref name="containerType"/>, и возвращающего значение типа <typeparamref name="ReturnT"/>,
        /// принимающего один аргумент типа <typeparamref name="ArgumentT1"/>
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
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
        /// Конструирование делегата <see cref="Func{ArgumentT1, ArgumentT2, ReturnT}"/>, осуществляющего через механизм рефлексии 
        /// доступ к статическому методу типа <paramref name="containerType"/>, и возвращающего значение типа <typeparamref name="ReturnT"/>,
        /// принимающего два аргумента типа <typeparamref name="ArgumentT1"/> и <typeparamref name="ArgumentT2"/>
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <typeparam name="ArgumentT2"></typeparam>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
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
        /// Конструирование делегата <see cref="Func{ArgumentT1, ArgumentT2, ArgumentT3, ReturnT}"/>, осуществляющего через механизм рефлексии 
        /// доступ к статическому методу типа <paramref name="containerType"/>, и возвращающего значение типа <typeparamref name="ReturnT"/>,
        /// принимающего три аргумента типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/> и <typeparamref name="ArgumentT3"/>
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <typeparam name="ArgumentT2"></typeparam>
        /// <typeparam name="ArgumentT3"></typeparam>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
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
        /// Конструирование делегата <see cref="Func{ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT}"/>, осуществляющего через механизм рефлексии 
        /// доступ к статическому методу типа <paramref name="containerType"/>, и возвращающего значение типа <typeparamref name="ReturnT"/>,
        /// принимающего четыре аргумента типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>, <typeparamref name="ArgumentT3"/> и <typeparamref name="ArgumentT4"/>
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <typeparam name="ArgumentT1"></typeparam>
        /// <typeparam name="ArgumentT2"></typeparam>
        /// <typeparam name="ArgumentT3"></typeparam>
        /// <typeparam name="ArgumentT4"></typeparam>
        /// <typeparam name="ArgumentT5"></typeparam>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
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

        /// <summary>
        /// Конструирование делегата типа <typeparamref name="DelegateT"/>
        /// </summary>
        /// <typeparam name="DelegateT"></typeparam>
        /// <typeparam name="ReturnT"></typeparam>
        /// <param name="containerType"></param>
        /// <param name="methodName"></param>
        /// <param name="argumentTypes"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
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
                    if (method.ReturnType == returnType)
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
                            if (arguments[i].ParameterType != inputTypes[i])
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
