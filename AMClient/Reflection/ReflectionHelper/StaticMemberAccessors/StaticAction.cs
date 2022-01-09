using System;
using System.Reflection;

namespace AcTp0Tools.Reflection
{
    /// <summary>
    /// Фабрика делегатов, осуществляющих через механизм рефлексии 
    /// доступ к статическим методам класса, не возвращающим значение
    /// </summary>
    public static partial class ReflectionHelper
    {
        /// <summary>
        /// Конструирование делегата <seealso cref="Action"/>, осуществляющего через механизм рефлексии,
        /// доступ к методу <paramref name="methodName"/> объекта типа <paramref name="containerType"/>.
        /// </summary>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Action GetStaticAction(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticAction<Action>(containerType, methodName, new Type[] { }, flags);
        }

        /// <summary>
        /// Конструирование делегата <seealso cref="Action{ArgumentT1}"/>, осуществляющего через механизм рефлексии,
        /// доступ к методу <paramref name="methodName"/> объекта типа <paramref name="containerType"/>,
        /// который принимает один аргумент типа <typeparamref name="ArgumentT1"/>
        /// </summary>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Action<ArgumentT1>
                             GetStaticAction<ArgumentT1>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticAction<Action<ArgumentT1>>(containerType, methodName,
                            new[] { typeof(ArgumentT1) }, flags);
        }

        /// <summary>
        /// Конструирование делегата <seealso cref="Action{ArgumentT1, ArgumentT2}"/>, осуществляющего через механизм рефлексии,
        /// доступ к методу <paramref name="methodName"/> объекта типа <paramref name="containerType"/>,
        /// который принимает два аргумента типа <typeparamref name="ArgumentT1"/> и <typeparamref name="ArgumentT2"/>
        /// </summary>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Action<ArgumentT1, ArgumentT2>
                             GetStaticAction<ArgumentT1, ArgumentT2>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticAction<Action<ArgumentT1, ArgumentT2>>(containerType, methodName,
                            new[] { typeof(ArgumentT1), typeof(ArgumentT2) }, flags);
        }

        /// <summary>
        /// Конструирование делегата <seealso cref="Action{ArgumentT1, ArgumentT2, ArgumentT3}"/>, осуществляющего через механизм рефлексии,
        /// доступ к методу <paramref name="methodName"/> объекта типа <paramref name="containerType"/>,
        /// который принимает три аргумента типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/> и <typeparamref name="ArgumentT3"/>
        /// </summary>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Action<ArgumentT1, ArgumentT2, ArgumentT3>
                             GetStaticAction<ArgumentT1, ArgumentT2, ArgumentT3>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticAction<Action<ArgumentT1, ArgumentT2, ArgumentT3>>(containerType, methodName,
                            new[] { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3) },
                            flags);
        }

        /// <summary>
        /// Конструирование делегата <seealso cref="Action{ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4}"/>, осуществляющего через механизм рефлексии,
        /// доступ к методу <paramref name="methodName"/> объекта типа <paramref name="containerType"/>,
        /// который принимает четыре аргумента типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>, <typeparamref name="ArgumentT3"/> и <typeparamref name="ArgumentT4"/>
        /// </summary>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Action<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>
                             GetStaticAction<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>(this Type containerType, string methodName = "", BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticAction<Action<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4>>(containerType, methodName,
                            new[] { typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4) },
                            flags);
        }

        /// <summary>
        /// Конструирование делегата <seealso cref="Action{ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5}"/>, осуществляющего через механизм рефлексии,
        /// доступ к методу <paramref name="methodName"/> объекта типа <paramref name="containerType"/>,
        /// который принимает пять аргументов типа <typeparamref name="ArgumentT1"/>, <typeparamref name="ArgumentT2"/>, <typeparamref name="ArgumentT3"/>, <typeparamref name="ArgumentT4"/> и <typeparamref name="ArgumentT5"/>
        /// </summary>
        /// <param name="containerType">Тип, декларирующий искомый метод</param>
        /// <param name="methodName">Имя метода</param>
        /// <param name="flags"></param>
        /// <returns>Сконструированный делегат</returns>
        public static Action<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>
                             GetStaticVoidDelegate<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>(this Type containerType, string methodName, BindingFlags flags = BindingFlags.Default)
        {
            return ConstructStaticAction<Action<ArgumentT1, ArgumentT2, ArgumentT3, ArgumentT4, ArgumentT5>>(containerType, methodName,
                            new[] { typeof(ArgumentT1), typeof(ArgumentT1), typeof(ArgumentT2), typeof(ArgumentT3), typeof(ArgumentT4), typeof(ArgumentT5) },
                            flags);
        }

        /// <summary>
        /// Метод, конструирующий делегат типа <typeparamref name="DelegateT"/>
        /// </summary>
        /// <typeparam name="DelegateT"></typeparam>
        /// <param name="containerType"></param>
        /// <param name="methodName"></param>
        /// <param name="argumentTypes"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private static DelegateT ConstructStaticAction<DelegateT>(Type containerType, string methodName, Type[] argumentTypes, BindingFlags flags = BindingFlags.Default) where DelegateT : class
        {
            if (containerType is null)
                return null;

            if (argumentTypes == null)
                argumentTypes = new Type[] { };

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            if (string.IsNullOrEmpty(methodName))
            {
                // Поиск метода по сигнатуре (без имени)
                if (FindBySignature(containerType, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out MethodInfo method))
                    return Delegate.CreateDelegate(typeof(DelegateT), method) as DelegateT;
            }
            else
            {
                // Поиск метода по имени и сигнатуре
                if (FindByNameAndSignature(containerType, methodName, argumentTypes, flags | BindingFlags.Static | BindingFlags.NonPublic, out MethodInfo method))
                    return Delegate.CreateDelegate(typeof(DelegateT), method) as DelegateT;
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
