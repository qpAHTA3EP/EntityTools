using System;
using System.Reflection;

namespace ACTP0Tools.Reflection
{
    public static class Activator<ReturnT>
    {
        /// <summary>
        /// Конструирование объекта типа ReturnT, имеющего закрытый конструктор
        /// </summary>
        public static ReturnT CreateInstance(BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            ConstructorInfo ctor = typeof(ReturnT).GetConstructor(flags | BindingFlags.NonPublic, null, new Type[0], null);

            if (ctor != null)
            {
                if (ctor.Invoke(new object[0]) is ReturnT result)
                    return result;
            }

            return default;
        }

        /// <summary>
        /// Конструирование объекта типа ReturnT, имеющего закрытый конструктор с аргументом типа ArgumentT
        /// </summary>
        public static ReturnT CreateInstance<ArgumentT>(ArgumentT arg1, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            if (arg1 != null)
            {
                ConstructorInfo ctor = typeof(ReturnT).GetConstructor(flags | BindingFlags.NonPublic, null, new[] { arg1.GetType() }, null);
                if (ctor != null)
                {
                    if (ctor.Invoke(new object[] { arg1 }) is ReturnT result)
                        return result;
                }
            }
            return default;
        }

        /// <summary>
        /// Конструирование объекта типа ReturnT, имеющего закрытый конструктор с аргументами типа ArgumentT1, ArgumentT2
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static ReturnT CreateInstance<ArgumentT1, ArgumentT2>(ArgumentT1 arg1, ArgumentT2 arg2, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            if (arg1 != null && arg2 != null)
            {
                ConstructorInfo ctor = typeof(ReturnT).GetConstructor(flags | BindingFlags.NonPublic, null, new[] { arg1.GetType(), arg2.GetType() }, null);
                if (ctor != null)
                {
                    if (ctor.Invoke(new object[] { arg1 }) is ReturnT result)
                        return result;
                }
            }
            return default;
        }
    }
}
