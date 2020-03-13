using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityCore.Reflection
{
    /// <summary>
    /// Фабрика делегатов, осуществляющих через механизм рефлексии 
    /// доступ к методу объекта заданного типа, и возвращающего значение типа ReturnT
    /// </summary>
    public static class NotStaticFunctionFactory
    {
        /// <summary>
        /// Конструирование делегата, осуществляющего через механизм рефлексии 
        /// доступ к методу объекта заданного типа, и возвращающего значение типа ReturnT
        /// </summary>
        /// <typeparam name="ReturnT">Тип возвращаемого значения</typeparam>
        /// <param name="type">Тип, декларирующий искомый методв</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Func<object, Func<ReturnT>> GetFunction<ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            Type[] argumentTypes = new Type[] { };

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
            if (method!=null)
            {
                return new Func<object, Func<ReturnT>>((object o) =>
                                {
                                    if (o != null
                                        && Equals(o.GetType(), type))
                                    {
                                        return new Func<ReturnT>(() =>
                                            {
                                                object result = method.Invoke(o, new object[] { });
                                                if (Equals(result, null))
                                                    return default(ReturnT);
                                                else return (ReturnT)result;
                                            });
                                    }
                                    return null;
                                });
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
        public static Func<Object, Func<ArgumentT1, ReturnT>>
                                      GetFunction<ArgumentT1, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            Type[] argumentTypes = new Type[] { typeof(ArgumentT1) };

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
                return new Func<object, Func<ArgumentT1, ReturnT>>((object o) =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return new Func<ArgumentT1, ReturnT>((ArgumentT1 a1) =>
                        {
                            object result = method.Invoke(o, new object[] { a1 });
                            if (Equals(result, null))
                                return default(ReturnT);
                            else return (ReturnT)result;
                        });
                    }
                    return null;
                });
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
        public static Func<Object, Func<ArgumentT1, ArgumentT2, ReturnT>>
                                      GetFunction<ArgumentT1, ArgumentT2, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            Type[] argumentTypes = new Type[] { typeof(ArgumentT1), typeof(ArgumentT2) };

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
                return new Func<object, Func<ArgumentT1, ArgumentT2, ReturnT>>((object o) =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return new Func<ArgumentT1, ArgumentT2, ReturnT>((ArgumentT1 a1, ArgumentT2 a2) =>
                        {
                            object result = method.Invoke(o, new object[] { a1, a2 });
                            if (Equals(result, null))
                                return default(ReturnT);
                            else return (ReturnT)result;
                        });
                    }
                    return null;
                });
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
        public static Func<Object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>>
                                      GetFunction<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            Type[] argumentTypes = new Type[] { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) };

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
                return new Func<object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>>((object o) =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return new Func<ArgumentT1, ArgumentT2, ArgumentT3, ReturnT>((ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3) =>
                        {
                            object result = method.Invoke(o, new object[] { a1, a2, a3 });
                            if (Equals(result, null))
                                return default(ReturnT);
                            else return (ReturnT)result;
                        });
                    }
                    return null;
                });
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
        public static Func<Object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>>
                                      GetFunction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>(this Type type, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            Type[] argumentTypes = new Type[] { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4) };

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
                return new Func<object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>>((object o) =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return new Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ReturnT>((ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3, ArgumentT4 a4) =>
                        {
                            object result = method.Invoke(o, new object[] { a1, a2, a3, a4 });
                            if (Equals(result, null))
                                return default(ReturnT);
                            else return (ReturnT)result;
                        });
                    }
                    return null;
                });
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
        public static Func<Object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>>
                                      GetFunction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>(this Type type, string methodName, BindingFlags flags = BindingFlags.Default)
        {
            if (Equals(type, null))
                return null;

            Type[] argumentTypes = new Type[] { typeof(ArgumentT1), typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4), typeof(ArgumentT5) };

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
                return new Func<object, Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>>((object o) =>
                {
                    if (o != null
                        && Equals(o.GetType(), type))
                    {
                        return new Func<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5, ReturnT>((ArgumentT1 a1, ArgumentT2 a2, ArgumentT3 a3, ArgumentT4 a4, ArgumentT5 a5) =>
                        {
                            object result = method.Invoke(o, new object[] { a1, a2, a3, a4, a5 });
                            if (Equals(result, null))
                                return default(ReturnT);
                            else return (ReturnT)result;
                        });
                    }
                    return null;
                });
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
        private static bool FindByNameAndSignature<Base>(Base o, string methodName, Type returnType, Type[] inputTypes, BindingFlags flags, out MethodInfo method)
        {
            if (!Equals(o, null))
            {
                Type type = o.GetType();
                method = type.GetMethod(methodName, flags, null, inputTypes, null);
                if (method != null)
                {
                    if (method.ReturnType.Equals(returnType))
                        return true;
                    else
                    {
                        method = null;
                        return false;
                    }
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
        private static bool FindBySignature<Base>(Base o, Type returnType, Type[] inputTypes, BindingFlags flags, out MethodInfo method)
        {
            if (Equals(o, null))
            {
                Type type = o.GetType();
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
