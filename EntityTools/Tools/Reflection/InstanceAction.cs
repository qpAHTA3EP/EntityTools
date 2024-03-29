﻿using System;
using System.Reflection;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Фабрика делегатов, осуществляющих через механизм рефлексии 
    /// доступ к методу объекта заданного типа, и не возвращающего значение
    /// </summary>
    public static class InstanceActionFactory
    {
        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа, и возвращающего значение типа ReturnT
        /// </summary>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<object, Action> GetAction(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
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
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return () =>
                        {
                            method.Invoke(o, new object[] { });
                        };
                    }
                    return null;
                };
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа, принимающего аргумент тип ArgumentT1
        /// и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<object, Action<ArgumentT1>>
                                      GetAction<ArgumentT1>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
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
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return a1 =>
                        {
                            method.Invoke(o, new object[] { a1 });
                        };
                    }
                    return null;
                };
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<object, Action<ArgumentT1, ArgumentT2>>
                                      GetAction<ArgumentT1, ArgumentT2>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
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
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return (a1, a2) =>
                        {
                            method.Invoke(o, new object[] { a1, a2 });
                        };
                    }
                    return null;
                };
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<object, Action<ArgumentT1, ArgumentT2, ArgumentT3>>
                                      GetAction<ArgumentT1, ArgumentT2, ArgumentT3>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            Type[] argumentTypes = { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) };

            MethodInfo method = null;

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
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return (a1, a2, a3) =>
                        {
                            method.Invoke(o, new object[] { a1, a2, a3 });
                        };
                    }
                    return null;
                };
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<object, Action<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>>
                                      GetAction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
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
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return (a1, a2, a3, a4) =>
                        {
                            method.Invoke(o, new object[] { a1, a2, a3, a4 });
                        };
                    }
                    return null;
                };
            }
            return null;
        }

        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<object, Action<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>>
                                      GetAction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>(this Type type, string methodName, BindingFlags flags = BindingFlags.Default)
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
                return o =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return (a1, a2, a3, a4, a5) =>
                        {
                            method.Invoke(o, new object[] { a1, a2, a3, a4, a5 });
                        };
                    }
                    return null;
                };
            }
            return null;
        }

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
    }
}
