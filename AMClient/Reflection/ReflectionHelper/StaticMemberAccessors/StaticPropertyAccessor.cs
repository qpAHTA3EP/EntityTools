using System;
using System.Reflection;

namespace AcTp0Tools.Reflection
{
    public static partial class ReflectionHelper
    {
        public static StaticPropertyAccessor<PropertyType> GetStaticProperty<PropertyType>(this Type containerType, string propName, BindingFlags flags = BindingFlags.Default)
        {
            return new StaticPropertyAccessor<PropertyType>(containerType, propName, flags);
        }
    }

#if true
    /// <summary>
    /// Класс доступа к свойству тип <typeparamref name="PropertyType"/>
    /// инкапсулированного в экземпляре объекта типа <typeparamref name="ContainerType"/>
    /// </summary>
    /// <typeparam name="PropertyType"></typeparam>
    public class StaticPropertyAccessor<PropertyType> : IMemberAccessor<PropertyType>
    {
        private MethodInfo _getter;
        private MethodInfo _setter;

        public MemberInfo MemberInfo => _propertyInfo;
        public PropertyInfo PropertyInfo => _propertyInfo;
        private PropertyInfo _propertyInfo;

        public bool IsValid => _propertyInfo != null;

        public StaticPropertyAccessor(Type instanceType, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var propertyType = typeof(PropertyType);

#if true
            Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic);
#else
            if (!Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Property '{propName}' does not found in '{instanceType.FullName}'"); 
#endif
        }

        public StaticPropertyAccessor(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

#if true
            Initialize(propertyInfo);
#else
            if (!Initialize(propertyInfo))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{propertyInfo.ReflectedType.FullName}'"); 
#endif
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        private bool Initialize(Type containerType, string propName, Type propertyType, BindingFlags flags)
        {
            if (containerType is null)
                return false;

            PropertyInfo propertyInfo = containerType.GetProperty(propName, flags, null, propertyType, ReflectionHelper.EmptyTypeArray, null);
            if (Initialize(propertyInfo))
                return true;
            return Initialize(containerType.BaseType, propName, propertyType, flags);
        }

        private bool Initialize(PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
            {
                MethodInfo[] accessors = propertyInfo.GetAccessors(true);
                if (accessors?.Length > 0)
                {
                    _getter = accessors[0];
                    if (accessors.Length > 1)
                        _setter = accessors[1];
                    _propertyInfo = propertyInfo;
                } 
            }
            return _propertyInfo != null;
        }

        public PropertyType Value
        {
            get
            {
                if (_getter.Invoke(null, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                    return result;
                return default;
            }
            set
            {
                _setterParameters[0] = value;
                _setter.Invoke(null, _setterParameters);
            }
        }
        private readonly object[] _setterParameters = new object[1];

        public PropertyType GetValue()
        {
            if (_getter.Invoke(null, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                return result;
            return default;
        }
        public void SetValue(PropertyType value)
        {
            _setterParameters[0] = value;
            _setter.Invoke(null, _setterParameters);
        }

        public static implicit operator PropertyType(StaticPropertyAccessor<PropertyType> property) => property.Value;
    }
#else
    /// <summary>
    /// Класс доступа к статическому свойству
    /// </summary>
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

                    return false;
                }
                return Initialize(t.BaseType, propName, flags);
            }
            return false;
        }

        public bool IsValid => containerType != null && propertyInfo != null && getter != null;

        public PropertyType Value
        {
            get
            {
                object result = getter?.Invoke(null, new object[] { });
                if (result != null)
                    return (PropertyType)result;
                return default;
            }
            set
            {
                if (IsValid && setter != null)
                {
                    setter.Invoke(null, new object[] { value });
                }
            }
        }

        public static implicit operator PropertyType(StaticPropertyAccessor<PropertyType> accessor) => accessor.Value;
    } 
#endif
}
