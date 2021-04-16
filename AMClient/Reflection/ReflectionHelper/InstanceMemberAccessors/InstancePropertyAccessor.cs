using System;
using System.Collections.Generic;
using System.Reflection;

namespace AcTp0Tools.Reflection
{
#if false
    public static class InstancePropertyAccessorFactory
    {
        public static InstancePropertyAccessor<ContainerType, PropertyType> GetProperty<ContainerType, PropertyType>(this ContainerType instance, string propName, BindingFlags flags = BindingFlags.Default) where ContainerType : class
        {
            return new InstancePropertyAccessor<ContainerType, PropertyType>(instance, propName, flags);
        }
        public static InstancePropertyAccessor<PropertyType> GetProperty<PropertyType>(this object obj, string propName, BindingFlags flags = BindingFlags.Default) //where ContainerType : class
        {
            return new InstancePropertyAccessor<PropertyType>(obj, propName, flags);
        }
    }

    /// <summary>
    /// Класс доступа к свойству объекта
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
            if (inst == null)
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
                if (IsValid && setter != null)
                {
                    setter.Invoke(instance, new object[] { value });
                }
            }
        }

        public static implicit operator PropertyType(InstancePropertyAccessor<ContainerType, PropertyType> accessor) => accessor.Value;
    }

    public class InstancePropertyAccessor<PropertyType>
    {
        private Type instanceType;
        private PropertyInfo propertyInfo;
        private MethodInfo getter;
        private MethodInfo setter;

        private object instance;

        public InstancePropertyAccessor(object obj, string propName, BindingFlags flags = BindingFlags.Default)
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
                if (IsValid && setter != null)
                {
                    setter.Invoke(instance, new object[] { value });
                }
            }
        }

        public static implicit operator PropertyType(InstancePropertyAccessor<PropertyType> accessor) => accessor.Value;
    } 
#else
    public static partial class ReflectionHelper
    {
        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта <paramref name="instance"/>
        /// </summary>
        public static PropertyAccessor<ContainerType, PropertyType> GetProperty<ContainerType, PropertyType>(this ContainerType instance, string propertyName, BindingFlags flags = BindingFlags.Default) where ContainerType : class
        {
            return new PropertyAccessor<ContainerType, PropertyType>(instance, propertyName, flags);
        }
        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта <paramref name="instance"/>
        /// </summary>
        public static PropertyAccessor<PropertyType> GetProperty<PropertyType>(this object instance, string propertyName, BindingFlags flags = BindingFlags.Default) //where ContainerType : class
        {
            return new PropertyAccessor<PropertyType>(instance, propertyName, flags);
        }
        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта типа <paramref name="instanceType"/>
        /// </summary>
        public static PropertyAccessor<PropertyType> GetProperty<PropertyType>(this Type instanceType, string propertyName, BindingFlags flags = BindingFlags.Default) //where ContainerType : class
        {
            return new PropertyAccessor<PropertyType>(instanceType, propertyName, flags);
        }
        /// <summary>
        /// Получение доступа к коллекции свойств экземпляра объекта <paramref name="instance"/> имеющим тип <typeparamref name="PropertyType"/>
        /// </summary>
        public static IEnumerable<PropertyAccessor<PropertyType>> GetProperties<PropertyType>(this object instance, BindingFlags flags = BindingFlags.Default)
        {
            //return new InstancePropertyAccessor<PropertyType>(obj, propName, flags);
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var type = instance.GetType();
            var propType = typeof(PropertyType);
            foreach (var propInfo in type.GetProperties(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (propInfo.PropertyType == propType)
                    yield return new PropertyAccessor<PropertyType>(instance, propInfo);
            }
        }
        /// <summary>
        /// Получение доступа к коллекции свойств объекта типа <paramref name="instanceType"/>, имеющим тип <typeparamref name="PropertyType"/>
        /// </summary>
        public static IEnumerable<PropertyAccessor<PropertyType>> GetProperties<PropertyType>(this Type instanceType, BindingFlags flags = BindingFlags.Default)
        {
            //return new InstancePropertyAccessor<PropertyType>(obj, propName, flags);
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var propType = typeof(PropertyType);
            foreach (var propInfo in instanceType.GetProperties(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (propInfo.PropertyType == propType)
                    yield return new PropertyAccessor<PropertyType>(instanceType, propInfo);
            }
        }
    }

    /// <summary>
    /// Класс доступа к свойству тип <typeparamref name="PropertyType"/>
    /// инкапсулированного в экземпляре объекта типа <typeparamref name="ContainerType"/>
    /// </summary>
    /// <typeparam name="PropertyType"></typeparam>
    public class PropertyAccessor<ContainerType, PropertyType> : IInstanceMemberAccessor<ContainerType, PropertyType>
    {
        private MethodInfo _getter;
        private MethodInfo _setter;

        public ContainerType Instance
        {
            get => _instance;
            set => _instance = value;
        }

        private ContainerType _instance;

        public MemberInfo MemberInfo => _propertyInfo;
        public PropertyInfo PropertyInfo => _propertyInfo;
        private PropertyInfo _propertyInfo;

        public bool IsValid => _propertyInfo != null;

        public PropertyAccessor(Type instanceType, string propName, BindingFlags flags = BindingFlags.Default)
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

        public PropertyAccessor(ContainerType instance, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            _instance = instance;

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var instanceType = instance.GetType();
            var propertyType = typeof(PropertyType);

#if true
            Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic);
#else
            if (!Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Property '{propName}' does not found in '{instance.GetType().FullName}'"); 
#endif
        }

#if false
        public Property(Type instanceType, PropertyInfo propertyInfo)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            //var propertyType = typeof(PropertyType);

            if (!Initialize(propertyInfo))

                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{instanceType.FullName}'");
        } 
#else
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var containerType = typeof(ContainerType);
            if (propertyInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{propertyInfo.Name}' is '{propertyInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(propertyInfo));

#if true
            Initialize(propertyInfo);
#else
            if (!Initialize(propertyInfo))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{containerType.FullName}'"); 
#endif
        }
#endif

        public PropertyAccessor(ContainerType instance, PropertyInfo propertyInfo)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var containerType = instance.GetType();
            if (propertyInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{propertyInfo.Name}' is '{propertyInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(propertyInfo));

#if false
            if (!Initialize(propertyInfo))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{containerType.FullName}'"); 
#else
            if (Initialize(propertyInfo))
#endif
                _instance = instance;
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

        /// <summary>
        /// Доступ к свойству объекта <paramref name="instance"/>
        /// </summary>
        public PropertyType this[ContainerType instance]
        {
            get
            {
                if (_getter.Invoke(instance, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                    return result;
                return default;
            }
            set
            {
                //if (_setter != null)
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
                if(_getter.Invoke(_instance, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                    return result;
                return default;
            }
            set
            {
                //if (_setter != null)
                {
                    _setterParameters[0] = value;
                    _setter.Invoke(_instance, _setterParameters);
                }
            }
        }

        public PropertyType GetValue()
        {
            if (_getter.Invoke(_instance, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                return result;
            return default;
        }
        public void SetValue(PropertyType value)
        {
            _setterParameters[0] = value;
            _setter.Invoke(_instance, _setterParameters);
        }

        public PropertyType GetValueFrom(ContainerType instance)
        {
            if (_getter.Invoke(instance, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                return result;
            return default;
        }
        public void SetValueTo(ContainerType instance, PropertyType value)
        {
            _setterParameters[0] = value;
            _setter.Invoke(instance, _setterParameters);
        }

        public static implicit operator PropertyType(PropertyAccessor<ContainerType, PropertyType> property) => property.Value;
    }

#if true
    public class PropertyAccessor<PropertyType> : PropertyAccessor<object, PropertyType>
    {
        public PropertyAccessor(Type instanceType, string propName, BindingFlags flags = BindingFlags.Default) : base(instanceType, propName, flags) { }
        public PropertyAccessor(object instance, string propName, BindingFlags flags = BindingFlags.Default) : base(instance, propName, flags) { }
        public PropertyAccessor(PropertyInfo propertyInfo) : base(propertyInfo) { }
        public PropertyAccessor(object instance, PropertyInfo propertyInfo) : base(instance, propertyInfo) { }
    }
#else
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
                throw new TargetException($"Property '{propName}' does not found in '{instanceType.FullName}'");
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

#if false
        public Property(Type instanceType, PropertyInfo propertyInfo)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            if (!Initialize(propertyInfo))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{instanceType.FullName}'");
        }
#else
        public Property(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));
            _propertyInfo = propertyInfo;

            if (!Initialize(propertyInfo))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{propertyInfo.ReflectedType.FullName}'");
        }

#endif
        public Property(object instance, PropertyInfo propertyInfo)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            _instance = instance;

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var containerType = instance.GetType();
            if (propertyInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{propertyInfo.Name}' is '{propertyInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(propertyInfo));

            if (!Initialize(propertyInfo))
                throw new TargetException($"Property '{propertyInfo.Name}' does not present in '{containerType.FullName}'");
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

        /// <summary>
        /// Доступ к свойству объекта <paramref name="instance"/>
        /// </summary>
        public PropertyType this[object instance]
        {
            get
            {
                if (_getter.Invoke(instance, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                    return result;
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
                if (_getter.Invoke(_instance, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                    return result;
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

        public PropertyType GetValue()
        {
            if (_getter.Invoke(_instance, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                return result;
            return default;
        }
        public void SetValue(PropertyType value)
        {
            _setterParameters[0] = value;
            _setter.Invoke(_instance, _setterParameters);
        }

        public PropertyType GetValueFrom(object instance)
        {
            if (_getter.Invoke(instance, ReflectionHelper.EmptyObjectArray) is PropertyType result)
                return result;
            return default;
        }
        public void SetValueTo(object instance, PropertyType value)
        {
            _setterParameters[0] = value;
            _setter.Invoke(instance, _setterParameters);
        }

        public static implicit operator PropertyType(Property<PropertyType> property) => property.Value;
    } 
#endif
#endif
}
