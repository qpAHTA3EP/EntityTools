using System;
using System.Reflection;

namespace Infrastructure.Reflection
{
    public static partial class ReflectionHelper
    {
        /// <summary>
        /// Функтор, конструирующий объект доступа к статическому свойству <paramref name="propName"/>,
        /// класса типа <paramref name="containerType"/>
        /// </summary>
        /// <typeparam name="PropertyType">Тип свойства</typeparam>
        /// <param name="containerType"></param>
        /// <param name="propName"></param>
        /// <param name="flags"></param>
        /// <param name="action">Действие, выполняемое при попытке чтения/записи свойства, в случае, если доступ к нему не был получен</param>
        /// <returns></returns>
        public static StaticPropertyAccessor<PropertyType> GetStaticProperty<PropertyType>(this Type containerType, string propName, BindingFlags flags = BindingFlags.Default)
        {
            return new StaticPropertyAccessor<PropertyType>(containerType, propName, flags);
        }

        public static StaticPropertyAccessor GetStaticProperty(this Type containerType, string propName, Type propertyType, BindingFlags flags = BindingFlags.Default)
        {
            if (propertyType is null)
                throw new ArgumentNullException(nameof(propertyType));

            return new StaticPropertyAccessor(containerType, propName, propertyType, flags);
        }
    }

    /// <summary>
    /// Типизированный класс доступа к статическому свойству типа <typeparamref name="PropertyType"/>,
    /// объявленному в типе, указанному в аргументе конструктора
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

            Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public StaticPropertyAccessor(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            Initialize(propertyInfo);
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
                if (accessors.Length > 0)
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

    /// <summary>
    /// Не типизорованный класс доступа к статическому свойству 
    /// </summary>
    public class StaticPropertyAccessor
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
            
            Initialize(instanceType, propName, flags | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public StaticPropertyAccessor(Type instanceType, string propName, Type propertyType, BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public StaticPropertyAccessor(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            Initialize(propertyInfo);
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        private bool Initialize(Type containerType, string propName, BindingFlags flags)
        {
            if (containerType is null)
                return false;

            PropertyInfo propertyInfo = containerType.GetProperty(propName, flags);

            if (Initialize(propertyInfo))
                return true;
            return Initialize(containerType.BaseType, propName, flags);
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
                if (accessors.Length > 0)
                {
                    _getter = accessors[0];
                    if (accessors.Length > 1)
                        _setter = accessors[1];
                    _propertyInfo = propertyInfo;
                }
            }
            return _propertyInfo != null;
        }

        public object Value
        {
            get
            {
                return _getter.Invoke(null, ReflectionHelper.EmptyObjectArray);
            }
            set
            {
                _setterParameters[0] = value;
                _setter.Invoke(null, _setterParameters);
            }
        }
        private readonly object[] _setterParameters = new object[1];

        public object GetValue()
        {
            return _getter.Invoke(null, ReflectionHelper.EmptyObjectArray);
        }
        public void SetValue(object value)
        {
            _setterParameters[0] = value;
            _setter.Invoke(null, _setterParameters);
        }
    }
}
