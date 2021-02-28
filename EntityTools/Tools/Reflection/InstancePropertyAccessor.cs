using System;
using System.Reflection;

namespace EntityTools.Reflection
{
    public static class InstancePropertyAccessorFactory
    {
        public static InstancePropertyAccessor<ContainerType, PropertyType> GetInstanceProperty<ContainerType, PropertyType>(this ContainerType instance, string propName, BindingFlags flags = BindingFlags.Default) where ContainerType : class
        {
            return new InstancePropertyAccessor<ContainerType, PropertyType>(instance, propName, flags);
        }
    }

    /// <summary>
    /// Класс доступа к статическому свойству
    /// </summary>
    /// <typeparam name="PropertyType"></typeparam>
    public class InstancePropertyAccessor<ContainerType, PropertyType>
    {
        private Type instanceType;
        private PropertyInfo propertyInfo;
        private MethodInfo getter;
        private MethodInfo setter;

        private ContainerType instance;

        public InstancePropertyAccessor(ContainerType inst, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if(inst == null)
                throw new ArgumentException("Instance is NULL");

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentException("Property name is invalid");

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = inst.GetType();

            if (!Initialize(type, propName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                instanceType = null;
                propertyInfo = null;
                getter = null;
                setter = null;
                instance = default;
            }
            else instance = inst;
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private bool Initialize(Type type, string propName, BindingFlags flags)
        {
            bool result = false;
            if (type != null)
            {
                PropertyInfo pi = type.GetProperty(propName, flags);
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
                            instanceType = type;
                            propertyInfo = pi;
                        }
                        return result;
                    }

                    return false;
                }
                return Initialize(type.BaseType, propName, flags);
            }
            return false;
        }

        public bool IsValid => instance != null && instanceType != null && propertyInfo != null && getter != null;

        public PropertyType Value
        {
            get
            {
                object result = getter?.Invoke(instance, new object[] { });
                if (result != null)
                    return (PropertyType)result;
                return default;
            }
            set
            {
                if (IsValid&& setter != null)
                {
                    setter.Invoke(instance, new object[] { value });
                }
            }
        }

        public static implicit operator PropertyType(InstancePropertyAccessor<ContainerType, PropertyType> accessor) => accessor.Value;
    }
}
