using System;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Infrastructure.Reflection
{
    public static partial class ReflectionHelper
    {
        //TODO Добавить нетипизированную перегрузку PropertyAccessor<object> GetProperty(this object instance, string propertyName, Type propertyType, BindingFlags flags = BindingFlags.Default)
        /// <summary>
        /// Конструирование нетипизированного функтора для доступа к свойству <paramref name="propertyName"/> объекту типа <paramref name="propertyType"/>
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="onFail">Действие, выполняемое если производится попытка чтения/записи свойства, доступ к которому не был получен</param>
        /// <param name="instanceType"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <remarks>Следует иметь в виду, что доступ к виртуальным свойствам дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для доступа к виртуальным свойствам объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static PropertyAccessor GetProperty(this Type instanceType, string propertyName, Type propertyType = null, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null)
        {
            var accessor = new PropertyAccessor(instanceType, propertyName, propertyType, flags);
            if (onFail != null
                && !accessor.IsValid)
                accessor = new PropertyAccessorStub(onFail);
            return accessor;
        }

        /// <summary>
        /// Конструирование нетипизированного функтора для доступа к свойству <paramref name="propertyName"/> объекта <paramref name="instance"/>
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="onFail">Действие, выполняемое если производится попытка чтения/записи свойства, доступ к которому не был получен</param>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <remarks>Следует иметь в виду, что доступ к виртуальным свойствам дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для доступа к виртуальным свойствам объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static PropertyAccessor GetProperty(this object instance, string propertyName, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null)
        {
            var accessor = new PropertyAccessor(instance, propertyName, flags);
            if (onFail != null
                && !accessor.IsValid)
                accessor = new PropertyAccessorStub(onFail);
            return accessor;
        }

        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта <paramref name="instance"/> типа <typeparamref name="TInstance"/>
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="onFail">Действие, выполняемое если производится попытка чтения/записи свойства, доступ к которому не был получен</param>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <remarks>Следует иметь в виду, что доступ к виртуальным свойствам дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для доступа к виртуальным свойствам объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static PropertyAccessor<TInstance, TProperty> GetProperty<TInstance, TProperty>(this TInstance instance, string propertyName, BindingFlags flags = BindingFlags.Default, Action<TInstance, MethodBase> onFail = null) where TInstance : class
        {
            var accessor = new PropertyAccessor<TInstance, TProperty>(instance, propertyName, flags);
            if (onFail != null
                && !accessor.IsValid)
                accessor = new PropertyAccessorStub<TInstance, TProperty>(onFail);
            return accessor;
        }

        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="flags"></param>
        /// <param name="onFail">Действие, выполняемое если производится попытка чтения/записи свойства, доступ к которому не был получен</param>
        /// <remarks>Следует иметь в виду, что доступ к виртуальным свойствам дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для доступа к виртуальным свойствам объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static PropertyAccessor<TProperty> GetProperty<TProperty>(this object instance, string propertyName, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null) //where ContainerType : class
        {
            var accessor = new PropertyAccessor<TProperty>(instance, propertyName, flags);
            if (onFail != null
                && !accessor.IsValid)
                accessor = new PropertyAccessorStub<TProperty>(onFail);
            return accessor;
        }

        /// <summary>
        /// Доступ к свойству <paramref name="propertyName"/> экземпляра объекта типа <paramref name="instanceType"/>
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="propertyName"></param>
        /// <param name="flags"></param>
        /// <param name="onFail">Действие, выполняемое если производится попытка чтения/записи свойства, доступ к которому не был получен</param>
        /// <remarks>Следует иметь в виду, что доступ к виртуальным свойствам дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для доступа к виртуальным свойствам объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static PropertyAccessor<TProperty> GetProperty<TProperty>(this Type instanceType, string propertyName, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null) //where ContainerType : class
        {
            var accessor = new PropertyAccessor<TProperty>(instanceType, propertyName, flags);
            if (onFail != null
                && !accessor.IsValid)
                accessor = new PropertyAccessorStub<TProperty>(onFail);
            return accessor;
        }

        /// <summary>
        /// Получение доступа к коллекции свойств экземпляра объекта <paramref name="instance"/> имеющим тип <typeparamref name="TProperty"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="flags"></param>
        /// <param name="onFail">Действие, выполняемое если производится попытка чтения/записи свойства, доступ к которому не был получен</param>
        /// <remarks>Следует иметь в виду, что доступ к виртуальным свойствам дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для доступа к виртуальным свойствам объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static IEnumerable<PropertyAccessor<TProperty>> GetProperties<TProperty>(this object instance, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null)
        {
            //return new InstancePropertyAccessor<PropertyType>(obj, propName, flags);
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var type = instance.GetType();
            var propType = typeof(TProperty);
            foreach (var propInfo in type.GetProperties(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (propInfo.PropertyType == propType)
                {
                    var accessor = new PropertyAccessor<TProperty>(instance, propInfo);
                    if (onFail != null
                        && !accessor.IsValid)
                        accessor = new PropertyAccessorStub<TProperty>(onFail);
                    yield return accessor;
                }
            }
        }

        /// <summary>
        /// Получение доступа к коллекции свойств объекта типа <paramref name="containerType"/>, имеющим тип <typeparamref name="TProperty"/>
        /// </summary>
        /// <param name="containerType"></param>
        /// <param name="flags"></param>
        /// <param name="onFail">Действие, выполняемое если производится попытка чтения/записи свойства, доступ к которому не был получен</param>
        /// <remarks>Следует иметь в виду, что доступ к виртуальным свойствам дочерних классов через делегат,
        /// полученный с использованием родительского класса, НЕВОЗМОЖЕН.
        /// Для доступа к виртуальным свойствам объекта, функтор должен быть построен для объекта конкретного типа</remarks>
        public static IEnumerable<PropertyAccessor<TProperty>> GetProperties<TProperty>(this Type containerType, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null)
        {
            //return new InstancePropertyAccessor<PropertyType>(obj, propName, flags);
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var propType = typeof(TProperty);
            foreach (var propInfo in containerType.GetProperties(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (propInfo.PropertyType == propType)
                {
                    var accessor = new PropertyAccessor<TProperty>(containerType, propInfo);
                    if (onFail != null
                        && !accessor.IsValid)
                        accessor = new PropertyAccessorStub<TProperty>(onFail);

                    yield return accessor;
                }
            }
        }
    }

    /// <summary>
    /// Класс доступа к свойству тип <typeparamref name="TProperty"/>
    /// инкапсулированного в экземпляре объекта типа <typeparamref name="TInstance"/>
    /// </summary>
    public class PropertyAccessor<TInstance, TProperty> : IInstanceMemberAccessor<TInstance, TProperty>
    {
        protected MethodInfo getter;
        protected MethodInfo setter;

        //TODO Выполнять повторную инициализацию для виртуальных свойств после смены instance
        public TInstance Instance
        {
            get => instance;
            //set => instance = value;
        }

        protected TInstance instance;

        public MemberInfo MemberInfo => propertyInfo;
        public PropertyInfo PropertyInfo => propertyInfo;
        protected PropertyInfo propertyInfo;

        public bool IsValid => propertyInfo != null;

        protected PropertyAccessor(){}

        public PropertyAccessor(Type instanceType, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var propertyType = typeof(TProperty);

            Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public PropertyAccessor(TInstance inst, string propName, BindingFlags flags = BindingFlags.Default)
        {
            if (inst == null)
                throw new ArgumentNullException(nameof(inst));
            this.instance = inst;

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var instanceType = inst.GetType();
            var propertyType = typeof(TProperty);

#if true
            Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic);
#else
            if (!Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Property '{propName}' does not found in '{instance.GetType().FullName}'"); 
#endif
        }

        public PropertyAccessor(PropertyInfo propInfo)
        {
            if (propInfo is null)
                throw new ArgumentNullException(nameof(propInfo));

            var instanceType = typeof(TInstance);
            if (propInfo.ReflectedType != instanceType)
                throw new ArgumentException($"ReflectedType of the '{propInfo.Name}' is '{propInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{instanceType.FullName}'", nameof(propInfo));

            Initialize(propInfo);
        }

        public PropertyAccessor(TInstance inst, PropertyInfo propertyInfo)
        {
            if (inst == null)
                throw new ArgumentNullException(nameof(inst));

            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var instanceType = inst.GetType();
            if (propertyInfo.ReflectedType != instanceType)
                throw new ArgumentException($"ReflectedType of the '{propertyInfo.Name}' is '{propertyInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{instanceType.FullName}'", nameof(propertyInfo));

            if (Initialize(propertyInfo))
                this.instance = inst;
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        protected bool Initialize(Type instanceType, string propertyName, Type propertyType, BindingFlags flags)
        {
            if (instanceType is null)
                return false;

            PropertyInfo propInfo = (propertyType is null) || propertyType == typeof(object)
                ? instanceType.GetProperty(propertyName, flags)
                : instanceType.GetProperty(propertyName, flags, null, propertyType, ReflectionHelper.EmptyTypeArray, null); 
            if (Initialize(propInfo))
                return true;
            return Initialize(instanceType.BaseType, propertyName, propertyType, flags);
        }

        protected bool Initialize(PropertyInfo propInfo)
        {
            if (propInfo != null)
            {
                MethodInfo[] accessors = propInfo.GetAccessors(true);
                if (accessors.Length > 0)
                {
                    getter = accessors[0];
                    if (accessors.Length > 1)
                        setter = accessors[1];
                    propertyInfo = propInfo;
                } 
            }
            return propertyInfo != null;
        }

        /// <summary>
        /// Доступ к свойству объекта <paramref name="inst"/>
        /// </summary>
        public virtual TProperty this[TInstance inst]
        {
            get
            {
                if (getter.Invoke(inst, ReflectionHelper.EmptyObjectArray) is TProperty result)
                    return result;
                return default;
            }
            set
            {
                //if (_setter != null)
                {
                    _setterParameters[0] = value;
                    setter.Invoke(inst, _setterParameters);
                }
            }
        }
        private readonly object[] _setterParameters = new object[1];

        public virtual TProperty Value
        {
            get
            {
                if(getter.Invoke(instance, ReflectionHelper.EmptyObjectArray) is TProperty result)
                    return result;
                return default;
            }
            set
            {
                //if (_setter != null)
                {
                    _setterParameters[0] = value;
                    setter.Invoke(instance, _setterParameters);
                }
            }
        }

        public virtual TProperty GetValue()
        {
            if (getter.Invoke(instance, ReflectionHelper.EmptyObjectArray) is TProperty result)
                return result;
            return default;
        }
        public virtual void SetValue(TProperty value)
        {
            _setterParameters[0] = value;
            setter.Invoke(instance, _setterParameters);
        }

        public virtual TProperty GetValueFrom(TInstance inst)
        {
            if (getter.Invoke(inst, ReflectionHelper.EmptyObjectArray) is TProperty result)
                return result;
            return default;
        }
        public virtual void SetValueTo(TInstance inst, TProperty value)
        {
            _setterParameters[0] = value;
            setter.Invoke(inst, _setterParameters);
        }

        public static implicit operator TProperty(PropertyAccessor<TInstance, TProperty> property) => property.Value;
    }

    public class PropertyAccessor<TProperty> : PropertyAccessor<object, TProperty>
    {
        protected PropertyAccessor() {}

        public PropertyAccessor(Type instanceType, string propName, BindingFlags flags = BindingFlags.Default) : base(instanceType, propName, flags) { }
        public PropertyAccessor(object instance, string propName, BindingFlags flags = BindingFlags.Default) : base(instance, propName, flags) { }

        public PropertyAccessor(Type instanceType, string propName, Type propertyType,
            BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException(nameof(propName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Initialize(instanceType, propName, propertyType, flags | BindingFlags.Instance | BindingFlags.NonPublic);
        }
        public PropertyAccessor(PropertyInfo propInfo) : base(propInfo) { }
        public PropertyAccessor(object instance, PropertyInfo propertyInfo) : base(instance, propertyInfo) { }
    }

    public class PropertyAccessor : PropertyAccessor<object, object>
    {
        protected PropertyAccessor() { }
        public PropertyAccessor(Type instanceType, string propName, BindingFlags flags = BindingFlags.Default) : base(instanceType, propName, flags) { }

        public PropertyAccessor(Type instanceType, string propName, Type propertyType,
            BindingFlags flags = BindingFlags.Default)
        {

        }
        public PropertyAccessor(object instance, string propName, BindingFlags flags = BindingFlags.Default) : base(instance, propName, flags) { }
        public PropertyAccessor(PropertyInfo propInfo) : base(propInfo) { }
        public PropertyAccessor(object instance, PropertyInfo propertyInfo) : base(instance, propertyInfo) { }
    }

    /// <summary>
    /// Класс-заглушка, возвращаемый в случае невозможности получения доступа к заданному свойству
    /// и выполняющий действие <see cref="onFail"/> при попытке чтения/записи значения свойства
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class PropertyAccessorStub<TInstance, TProperty> : PropertyAccessor<TInstance, TProperty>
    {
        private readonly Action<TInstance, MethodBase> onFail;

        public PropertyAccessorStub(Action<TInstance, MethodBase> onFail)
        {
            this.onFail = onFail;
        }

        public override TProperty this[TInstance inst]
        {
            get
            {
                onFail(inst, MethodBase.GetCurrentMethod());
                return default;
            }
            set
            {
                onFail(inst, MethodBase.GetCurrentMethod());
            }
        }

        public override TProperty GetValue()
        {
            onFail(instance, MethodBase.GetCurrentMethod());
            return default;
        }

        public override TProperty GetValueFrom(TInstance inst)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
            return default;
        }

        public override void SetValue(TProperty value)
        {
            onFail(instance, MethodBase.GetCurrentMethod());
        }

        public override void SetValueTo(TInstance inst, TProperty value)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
        }

        public override TProperty Value
        {
            get
            {
                onFail(instance, MethodBase.GetCurrentMethod());
                return default;
            }
            set
            {
                onFail(instance, MethodBase.GetCurrentMethod());
            }
        }
    }


    /// <summary>
    /// Класс-заглушка, возвращаемый в случае невозможности получения доступа к заданному свойству
    /// и выполняющий действие <see cref="onFail"/> при попытке чтения/записи значения свойства
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    public class PropertyAccessorStub<TProperty> : PropertyAccessor<TProperty>
    {
        private readonly Action<object, MethodBase> onFail;

        public PropertyAccessorStub(Action<object, MethodBase> onFail)
        {
            this.onFail = onFail;
        }

        public override TProperty this[object inst]
        {
            get
            {
                onFail(inst, MethodBase.GetCurrentMethod());
                return default;
            }
            set
            {
                onFail(inst, MethodBase.GetCurrentMethod());
            }
        }

        public override TProperty GetValue()
        {
            onFail(instance, MethodBase.GetCurrentMethod());
            return default;
        }

        public override TProperty GetValueFrom(object inst)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
            return default;
        }

        public override void SetValue(TProperty value)
        {
            onFail(instance, MethodBase.GetCurrentMethod());
        }

        public override void SetValueTo(object inst, TProperty value)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
        }

        public override TProperty Value
        {
            get
            {
                onFail(instance, MethodBase.GetCurrentMethod());
                return default;
            }
            set
            {
                onFail(instance, MethodBase.GetCurrentMethod());
            }
        }
    }

    /// <summary>
    /// Класс-заглушка, возвращаемый в случае невозможности получения доступа к заданному свойству
    /// и выполняющий действие <see cref="onFail"/> при попытке чтения/записи значения свойства
    /// </summary>
    public class PropertyAccessorStub : PropertyAccessor
    {
        private readonly Action<object, MethodBase> onFail;

        public PropertyAccessorStub(Action<object, MethodBase> onFail)
        {
            this.onFail = onFail;
        }

        public override object this[object inst]
        {
            get
            {
                onFail(inst, MethodBase.GetCurrentMethod());
                return default;
            }
            set { onFail(inst, MethodBase.GetCurrentMethod()); }
        }

        public override object GetValue()
        {
            onFail(instance, MethodBase.GetCurrentMethod());
            return default;
        }

        public override object GetValueFrom(object inst)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
            return default;
        }

        public override void SetValue(object value)
        {
            onFail(instance, MethodBase.GetCurrentMethod());
        }

        public override void SetValueTo(object inst, object value)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
        }

        public override object Value
        {
            get
            {
                onFail(instance, MethodBase.GetCurrentMethod());
                return default;
            }
            set { onFail(instance, MethodBase.GetCurrentMethod()); }
        }
    }
#if false
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
}
