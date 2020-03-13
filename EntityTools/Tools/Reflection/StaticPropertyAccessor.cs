using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Reflection
{
    public static class StaticPropertyAccessorFactory
    {
        public static StaticPropertyAccessor<PropertyType> GetStaticProperty<PropertyType>(this Type containerType, string propName, BindingFlags flags = BindingFlags.Default)
        {
            return new StaticPropertyAccessor<PropertyType>(containerType, propName, flags);
        }
    }

    /// <summary>
    /// Класс доступа к статическому свойству
    /// </summary>
    /// <typeparam name="PropertyType"></typeparam>
    public class StaticPropertyAccessor<PropertyType>
    {
        private Type containerType;
        private PropertyInfo propertyInfo;
        private MethodInfo getter;
        private MethodInfo setter;

        public StaticPropertyAccessor(Type t, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (string.IsNullOrEmpty(propName))
                throw new ArgumentException("Property name is invalid");

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            if (!Initialize(t, propName, flags | BindingFlags.Static | BindingFlags.NonPublic))
            {
                containerType = null;
                propertyInfo = null;
                getter = null;
                setter = null;
            }
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        /// <param name="t"></param>
        /// <param name="propName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private bool Initialize(Type t, string propName, BindingFlags flags)
        {
            bool result = false;
            if (t != null)
            {
                PropertyInfo pi = t.GetProperty(propName, flags);
                if (pi != null)
                {
                    if (pi.PropertyType.Equals(typeof(PropertyType)))
                    {
                        MethodInfo[] accessors = pi.GetAccessors((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
                        if (accessors != null)
                        {
                            if (accessors.Length > 0)
                            {
                                getter = accessors[0];
                                result = true;
                            }
                            if (accessors.Length > 1)
                            {
                                setter = accessors[1];
                                result = true;
                            }
                        }
                        if (result)
                        {
                            containerType = t;
                            propertyInfo = pi;
                        }
                        return result;
                    }
                    else return false;
                }
                return Initialize(t.BaseType, propName, flags);
            }
            return false;
        }

        public bool IsValid()
        {
            return containerType != null && propertyInfo != null && getter != null;
        }

        public PropertyType Value
        {
            get
            {
                object result = getter?.Invoke(null, new object[] { });
                if (result != null)
                    return (PropertyType)result;
                else return default(PropertyType);                
            }
            set
            {
                if (IsValid() && setter != null)
                {
                    setter.Invoke(null, new object[] { value });
                }
            }
        }

        public static implicit operator PropertyType(StaticPropertyAccessor<PropertyType> accessor) => accessor.Value;
    }
}
