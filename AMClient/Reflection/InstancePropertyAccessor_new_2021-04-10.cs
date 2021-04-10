using System;
using System.Collections.Generic;
using System.Reflection;

namespace EntityTools.Reflection
{
    public static class InstancePropertyAccessorFactory
    {
#if false
        /// <summary>
        /// Конструирование функтора
        /// </summary>
        public static Func<object, Property<PropertyType>> GetInstancePropertyAccessor<PropertyType>(this Type containerType, string propertyName, BindingFlags flags = BindingFlags.Default)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (containerType is null)
                throw new ArgumentNullException(nameof(containerType));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            PropertyInfo propInfo = containerType.GetProperty(propertyName, flags | BindingFlags.Instance | BindingFlags.NonPublic);
            if (propInfo != null)
            {
                if (propInfo.PropertyType.Equals(typeof(PropertyType)))
                {
                    return (object instance) =>
                    {
                        if (instance.GetType().Equals(containerType))
                            return new PropertyAccessor<PropertyType>(instance, propInfo);
                        return new PropertyAccessor<PropertyType>();
                    };
                }
            }
            return null;
        }
#endif

        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта <paramref name="instance"/>
        /// </summary>
        public static Property<ContainerType, PropertyType> GetProperty<ContainerType, PropertyType>(this ContainerType instance, string propertyName, BindingFlags flags = BindingFlags.Default) where ContainerType : class
        {
            return new Property<ContainerType, PropertyType>(instance, propertyName, flags);
        }
        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта <paramref name="instance"/>
        /// </summary>
        public static Property<PropertyType> GetProperty<PropertyType>(this object instance, string propertyName, BindingFlags flags = BindingFlags.Default) //where ContainerType : class
        {
            return new Property<PropertyType>(instance, propertyName, flags);
        }
        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта типа <paramref name="instanceType"/>
        /// </summary>
        public static Property<PropertyType> GetProperty<PropertyType>(this Type instanceType, string propertyName, BindingFlags flags = BindingFlags.Default) //where ContainerType : class
        {
            return new Property<PropertyType>(instanceType, propertyName, flags);
        }
        /// <summary>
        /// Получение доступа к коллекции свойств экземпляра объекта <paramref name="instance"/> имеющим тип <typeparamref name="PropertyType"/>
        /// </summary>
        public static IEnumerable<Property<PropertyType>> GetProperties<PropertyType>(this object instance, BindingFlags flags = BindingFlags.Default)
        {
            //return new InstancePropertyAccessor<PropertyType>(obj, propName, flags);
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var type = instance.GetType();
            var propType = typeof(PropertyType);
            foreach(var propInfo in type.GetProperties(flags))
            {
                if (propInfo.PropertyType == propType)
                    yield return new Property<PropertyType>(instance, propInfo);
            }
        }
        /// <summary>
        /// Получение доступа к коллекции свойств объекта типа <paramref name="instanceType"/>, имеющим тип <typeparamref name="PropertyType"/>
        /// </summary>
        public static IEnumerable<Property<PropertyType>> GetProperties<PropertyType>(this Type instanceType, BindingFlags flags = BindingFlags.Default)
        {
            //return new InstancePropertyAccessor<PropertyType>(obj, propName, flags);
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var propType = typeof(PropertyType);
            foreach (var propInfo in instanceType.GetProperties(flags))
            {
                if (propInfo.PropertyType == propType)
                    yield return new Property<PropertyType>(instanceType, propInfo);
            }
        }
    }

    /// <summary>
    /// Класс доступа к свойству тип <typeparamref name="PropertyType"/>
    /// инкапсулированного в экземпляре объекта типа <typeparamref name="ContainerType"/>
    /// </summary>
    /// <typeparam name="PropertyType"></typeparam>
    public class Property<ContainerType, PropertyType>
    {
        private MethodInfo _getter;
        private MethodInfo _setter;

        public ContainerType Instance => _instance;
        private ContainerType _instance;

        public PropertyInfo PropertyInfo => _propertyInfo;
        private PropertyInfo _propertyInfo;

        public bool IsValid => _propertyInfo != null;

        public Property(Type instanceType, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var propertyType = typeof(PropertyType);

            if (!Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Property '{propName}' does not found in '{_instance.GetType().FullName}'");
        }

        public Property(ContainerType instance, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (instance != null)
                throw new ArgumentNullException(nameof(instance));
            _instance = instance;

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var instanceType = instance.GetType();
            var propertyType = typeof(PropertyType);

            if (!Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic))

                throw new TargetException($"Property '{propName}' does not found in '{instance.GetType().FullName}'");
        }

        public Property(Type instanceType, PropertyInfo propertyInfo)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var propertyType = typeof(PropertyType);

            if (!Initialize(propertyInfo, propertyType))

                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{_instance.GetType().FullName}'");
        }

        public Property(ContainerType instance, PropertyInfo propertyInfo)
        {
            if (instance != null)
                throw new ArgumentNullException(nameof(instance));
            _instance = instance;

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var propertyType = typeof(PropertyType);

            if (!Initialize(propertyInfo, propertyType))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{instance.GetType().FullName}'");
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        private bool Initialize(Type containerType, string propName, Type propertyType, BindingFlags flags)
        {
            if (containerType is null)
                return false;

#if false
            PropertyInfo pi = containerType.GetProperty(propName, flags, null, propertyType, ReflectionHelper.EmptyTypeArray, null); 
#elif false
            PropertyInfo pi = containerType.GetProperty(propName, flags, null, propertyType, new Type[0], null);
#else
            PropertyInfo pi = containerType.GetProperty(propName, flags);
#endif
            if (Initialize(pi, propertyType))
                return true;
            return Initialize(containerType.BaseType, propName, propertyType, flags);
        }

        private bool Initialize(PropertyInfo propertyInfo, Type propertyType)
        {
            if (propertyInfo?.PropertyType == propertyType)
            {
                MethodInfo[] accessors = propertyInfo.GetAccessors(true);
                if (accessors?.Length > 0)
                {
                    _getter = accessors[0];
                    if (accessors.Length > 1)
                        _setter = accessors[1];
                    _propertyInfo = propertyInfo;
                }
                 return _propertyInfo != null;
           }
            return false;
        }

        /// <summary>
        /// Доступ к свойству объекта <paramref name="instance"/>
        /// </summary>
        public PropertyType this[ContainerType instance]
        {
            get
            {
                {
                    object result = _getter.Invoke(instance, ReflectionHelper.EmptyObjectArray);
                    if (result != null)
                        return (PropertyType)result;
                }
                return default;
            }
            set
            {
                if (_setter != null)
                {
                    _setterParameters[0] = value;
                    _setter.Invoke(instance, _setterParameters);
                }
            }
        }
        private object[] _setterParameters = new object[1];

        public PropertyType Value
        {
            get
            {
                object result = _getter.Invoke(_instance, new object[] { });
                if (result != null)
                    return (PropertyType)result;
                return default;
            }
            set
            {
                if (_setter != null)
                {
                    _setterParameters[0] = value;
                    _setter.Invoke(_instance, _setterParameters);
                }
            }
        }

        public static implicit operator PropertyType(Property<ContainerType, PropertyType> property) => property.Value;
    }

    /// <summary>
    /// Класс, инкапсулирующий доступ к свойству типа <typeparamref name="PropertyType"/> экземпляра объекта, 
    /// тип которого задается в момент инициализации,
    /// а экземпляр передается в качестве аргумента индексатора this[object instance]
    /// </summary>
    public class Property<PropertyType>
    {
        private MethodInfo _getter;
        private MethodInfo _setter;

        public object Instance => _instance;
        private object _instance;

        public PropertyInfo PropertyInfo => _propertyInfo;
        private PropertyInfo _propertyInfo;

        public bool IsValid => _propertyInfo != null;

        public Property(Type instanceType, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var propertyType = typeof(PropertyType);

            if (!Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Property '{propName}' does not found in '{_instance.GetType().FullName}'");
        }

        public Property(object instance, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            _instance = instance;

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var instanceType = instance.GetType();
            var propertyType = typeof(PropertyType);

            if (!Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Property '{propName}' does not found in '{instance.GetType().FullName}'");
        }

        public Property(Type instanceType, PropertyInfo propertyInfo)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var propertyType = typeof(PropertyType);

            if (!Initialize(propertyInfo, propertyType))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{_instance.GetType().FullName}'");
        }

        public Property(object instance, PropertyInfo propertyInfo)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            _instance = instance;

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var propertyType = typeof(PropertyType);

            if (!Initialize(propertyInfo, propertyType))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{instance.GetType().FullName}'");
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        private bool Initialize(Type containerType, string propName, Type propertyType, BindingFlags flags)
        {
            if (containerType is null)
                return false;

#if false
            PropertyInfo pi = containerType.GetProperty(propName, flags, null, propertyType, ReflectionHelper.EmptyTypeArray, null); 
#elif false
            PropertyInfo pi = containerType.GetProperty(propName, flags, null, propertyType, new Type[0], null);
#else
            PropertyInfo pi = containerType.GetProperty(propName, flags);
#endif
            if (Initialize(pi, propertyType))
                return true;
            return Initialize(containerType.BaseType, propName, propertyType, flags);
        }

        private bool Initialize(PropertyInfo propertyInfo, Type propertyType)
        {
            if (propertyInfo?.PropertyType == propertyType)
            {
                MethodInfo[] accessors = propertyInfo.GetAccessors(true);
                if (accessors?.Length > 0)
                {
                    _getter = accessors[0];
                    if (accessors.Length > 1)
                        _setter = accessors[1];
                    _propertyInfo = propertyInfo;
                }
                return _propertyInfo != null;
            }
            return false;
        }

        /// <summary>
        /// Доступ к свойству объекта <paramref name="instance"/>
        /// </summary>
        public PropertyType this[object instance]
        {
            get
            {
                object result = _getter.Invoke(instance, ReflectionHelper.EmptyObjectArray);
                if (result != null)
                    return (PropertyType)result;
                return default;
            }
            set
            {
                if(_setter != null)
                {
                    _setterParameters[0] = value;
                    _setter.Invoke(instance, _setterParameters);
                }
            }
        }
        private object[] _setterParameters = new object[1];

        public PropertyType Value
        {
            get
            {
                object result = _getter.Invoke(_instance, new object[] { });
                if (result != null)
                    return (PropertyType)result;
                return default;
            }
            set
            {
                if (_setter != null)
                {
                    _setterParameters[0] = value;
                    _setter.Invoke(_instance, _setterParameters);
                }
            }
        }

        public static implicit operator PropertyType(Property<PropertyType> property) => property.Value;
    }

#if false
    public class PropertyAccessor<PropertyType>
    {
        private Type instanceType;
        private PropertyInfo propertyInfo;
        private MethodInfo getter;
        private MethodInfo setter;

        private object instance;

        public PropertyAccessor() { }

        public PropertyAccessor(object obj, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (obj == null)
                throw new ArgumentException("Instance is NULL");

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentException("Property name is invalid");

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = obj.GetType();

            if (!Initialize(type, propName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                instanceType = null;
                propertyInfo = null;
                getter = null;
                setter = null;
                instance = default;
            }
            else instance = obj;
        }

        public PropertyAccessor(object obj, PropertyInfo propInfo)
        {
            if (obj is null)
                throw new ArgumentException("Instance is NULL");
            if (propInfo is null)
                throw new ArgumentException("PropertyInfo is NULL");
            var t = propInfo.DeclaringType;

            MethodInfo[] accessors = propInfo.GetAccessors(true);
            if (accessors?.Length > 0)
            {
                getter = accessors[0];
                if (accessors.Length > 1)
                    setter = accessors[1];
            }
            else throw new ArgumentException("PropertyInfo does not have accessors");

            instanceType = obj.GetType();
            propertyInfo = propInfo;
            instance = obj;
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
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
                if (IsValid && setter != null)
                {
                    setter.Invoke(instance, new object[] { value });
                }
            }
        }
        //
        public PropertyType GetValueFrom(object obj)
        {
            object result = getter?.Invoke(obj, new object[] { });
            if (result != null)
                return (PropertyType)result;
            return default;
        }

        public static implicit operator PropertyType(PropertyAccessor<PropertyType> accessor) => accessor.Value;
    }
#endif
}
