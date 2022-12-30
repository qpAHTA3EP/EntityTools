using System;
using System.Collections.Generic;
using System.Reflection;

namespace Infrastructure.Reflection
{
    public static partial class ReflectionHelper
    {
        //TODO Добавить нетипизированную перегрузку  FieldAccessor<object> GetField(this object instance, string propertyName, BindingFlags flags = BindingFlags.Default)
        //TODO Добавить нетипизированную перегрузку  FieldAccessor<object> GetField(this Type containerType, string propertyName, BindingFlags flags = BindingFlags.Default)
        // TODO Добавить FieldAccessorStub
        public static FieldAccessor<ContainerType, FieldType> GetField<ContainerType, FieldType>(this ContainerType instance, string fieldName, BindingFlags flags = BindingFlags.Default, Action<ContainerType, MethodBase> onFail = null)
        {
            var accessor = new FieldAccessor<ContainerType, FieldType>(instance, fieldName, flags);
            if (accessor is null
                && onFail != null)
                accessor = new FieldAccessorStub<ContainerType, FieldType>(onFail);

            return accessor;
        }
        public static FieldAccessor<FieldType>                GetField<FieldType>(this object instance, string fieldName, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null)
        {
            var accessor = new FieldAccessor<FieldType>(instance, fieldName, flags);
            if (accessor is null
                && onFail != null)
                accessor = new FieldAccessorStub<FieldType>(instance, onFail);

            return accessor;
        }
        public static FieldAccessor<ContainerType, FieldType> GetField<ContainerType, FieldType>(this Type containerType, string fieldName, BindingFlags flags = BindingFlags.Default, Action<ContainerType, MethodBase> onFail = null)
        {
            var accessor = new FieldAccessor<ContainerType, FieldType>(containerType, fieldName, flags);
            if (accessor is null
                && onFail != null)
                accessor = new FieldAccessorStub<ContainerType, FieldType>(onFail);

            return accessor;
        }
        public static FieldAccessor<FieldType>                GetField<FieldType>(this Type containerType, string fieldName, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null)
        {
            var accessor = new FieldAccessor<FieldType>(containerType, fieldName, flags);
            if (accessor is null
                && onFail != null)
                accessor = new FieldAccessorStub<FieldType>(onFail);

            return accessor;
        }

        /// <summary>
        /// Получение доступа к коллекции полей, имеющим тип <typeparamref name="FieldType"/> и инкапсулированных в объекте <paramref name="instance"/>
        /// </summary>
        public static IEnumerable<FieldAccessor<FieldType>>   GetFields<FieldType>(this object instance, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var type = instance.GetType();
            var fieldType = typeof(FieldType);
            foreach (var fieldInfo in type.GetFields(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.FieldType == fieldType)
                {
                    var accessor = new FieldAccessor<FieldType>(instance, fieldInfo);
                    if (accessor is null
                        && onFail != null)
                        accessor = new FieldAccessorStub<FieldType>(instance, onFail);

                    yield return accessor;
                }
            }
        }
        /// <summary>
        /// Получение доступа к коллекции полей, имеющим тип <typeparamref name="FieldType"/> и инкапсулированных в типе <paramref name="containerType"/> 
        /// </summary>
        public static IEnumerable<FieldAccessor<FieldType>>  GetFields<FieldType>(this Type containerType, BindingFlags flags = BindingFlags.Default, Action<object, MethodBase> onFail = null)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var fieldType = typeof(FieldType);
            foreach (var fieldInfo in containerType.GetFields(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.FieldType == fieldType)
                {
                    var accessor = new FieldAccessor<FieldType>(fieldInfo);
                    if (accessor is null
                        && onFail != null)
                        accessor = new FieldAccessorStub<FieldType>(onFail);

                    yield return accessor;
                }
            }
        }
    }

    /// <summary>
    /// Класс, инкапсулирующий доступ к полю экземпляра объекта
    /// </summary>
    public class FieldAccessor<ContainerType, FieldType> : IInstanceMemberAccessor<ContainerType, FieldType>
    {
        public ContainerType Instance
        {
            get => instance;
            set => instance = value;
        }

        protected ContainerType instance;

        public MemberInfo MemberInfo => fieldInfo;
        public FieldInfo FieldInfo => fieldInfo;
        protected FieldInfo fieldInfo;

        public bool IsValid => fieldInfo != null;

        protected FieldAccessor() { }

        public FieldAccessor(Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Initialize(instanceType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public FieldAccessor(ContainerType instance, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if(instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type instanceType = instance.GetType();

            if (Initialize(instanceType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                this.instance = instance;
        }

        public FieldAccessor(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            var containerType = typeof(ContainerType);
            if (fieldInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{fieldInfo.Name}' is '{fieldInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(fieldInfo));

            Initialize(fieldInfo);
        }

        public FieldAccessor(ContainerType instance, FieldInfo fieldInfo)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            var containerType = instance.GetType();
            if (fieldInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{fieldInfo.Name}' is '{fieldInfo.ReflectedType?.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(fieldInfo));

            if(Initialize(fieldInfo))

            this.instance = instance;
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        private bool Initialize(Type containerType, string fieldName, BindingFlags flags)
        {
            if (containerType is null)
                return false;

            FieldInfo fieldInfo = containerType.GetField(fieldName, flags);
            if (Initialize(fieldInfo))
                return true;
            return Initialize(containerType.BaseType, fieldName, flags);
        }

        private bool Initialize(FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                var fieldType = typeof(FieldType);
                if (fieldInfo.FieldType == fieldType
                    || fieldType.IsAssignableFrom(fieldInfo.FieldType))
                {
                    this.fieldInfo = fieldInfo;
                    return true;
                }   
            }
            return false;
        }

        /// <summary>
        /// Доступ к полю объекта <paramref name="inst"/>
        /// </summary>
        public virtual FieldType this[ContainerType inst]
        {
            get
            {
                if (fieldInfo.GetValue(inst) is FieldType result)
                    return result;
                return default;
            }
            set
            {
                fieldInfo.SetValue(inst, value);
            }
        }

        public virtual FieldType Value
        {
            get
            {
                if (fieldInfo.GetValue(instance) is FieldType result)
                    return result;
                return default;
            }
            set
            {
                fieldInfo.SetValue(instance, value);
            }
        }

        public virtual FieldType GetValue()
        {
            if (fieldInfo.GetValue(instance) is FieldType result)
                return result;
            return default;
        }
        public virtual void SetValue(FieldType value)
        {
            fieldInfo.SetValue(instance, value);
        }

        public virtual FieldType GetValueFrom(ContainerType inst)
        {
            if (fieldInfo.GetValue(inst) is FieldType result)
                return result;
            return default;
        }
        public virtual void SetValueTo(ContainerType inst, FieldType value)
        {
            fieldInfo.SetValue(inst, value);
        }

        public static implicit operator FieldType(FieldAccessor<ContainerType, FieldType> accessor) => accessor.Value;
    }

    public class FieldAccessor<FieldType> : FieldAccessor<object, FieldType>
    {
        protected FieldAccessor(){}

        public FieldAccessor(Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default) : base(instanceType, fieldName, flags) { }
        public FieldAccessor(object instance, string fieldName, BindingFlags flags = BindingFlags.Default) : base(instance, fieldName, flags) { }
        public FieldAccessor(object instance, FieldInfo fieldInfo) : base(instance, fieldInfo) { }
        public FieldAccessor(FieldInfo fieldInfo) : base(fieldInfo) { }
    }

    /// <summary>
    /// Класс-заглушка, возвращаемый в случае невозможности получения доступа к заданному полю
    /// и выполняющий действие <see cref="onFail"/> при попытке чтения/записи значения свойства
    /// </summary>
    /// <typeparam name="ContainerType"></typeparam>
    /// <typeparam name="FieldType"></typeparam>
    public class FieldAccessorStub<ContainerType, FieldType> : FieldAccessor<ContainerType, FieldType>
    {
        private readonly Action<ContainerType, MethodBase> onFail;

        public FieldAccessorStub(Action<ContainerType, MethodBase> onFail)
        {
            this.onFail = onFail;
        }
        public FieldAccessorStub(ContainerType inst, Action<ContainerType, MethodBase> onFail)
        {
            instance = inst;
            this.onFail = onFail;
        }

        public override FieldType this[ContainerType inst]
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

        public override FieldType GetValue()
        {
            onFail(instance, MethodBase.GetCurrentMethod());
            return default;
        }

        public override FieldType GetValueFrom(ContainerType inst)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
            return default;
        }

        public override void SetValue(FieldType value)
        {
            onFail(instance, MethodBase.GetCurrentMethod());
        }

        public override void SetValueTo(ContainerType inst, FieldType value)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
        }

        public override FieldType Value
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
    /// Класс-заглушка, возвращаемый в случае невозможности получения доступа к заданному полю
    /// и выполняющий действие <see cref="onFail"/> при попытке чтения/записи значения свойства
    /// </summary>
    /// <typeparam name="FieldType"></typeparam>
    public class FieldAccessorStub<FieldType> : FieldAccessor<FieldType>
    {
        private readonly Action<object, MethodBase> onFail;

        public FieldAccessorStub(Action<object, MethodBase> onFail)
        {
            this.onFail = onFail;
        }
        public FieldAccessorStub(object inst, Action<object, MethodBase> onFail)
        {
            instance = inst;
            this.onFail = onFail;
        }

        public override FieldType this[object inst]
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

        public override FieldType GetValue()
        {
            onFail(instance, MethodBase.GetCurrentMethod());
            return default;
        }

        public override FieldType GetValueFrom(object inst)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
            return default;
        }

        public override void SetValue(FieldType value)
        {
            onFail(instance, MethodBase.GetCurrentMethod());
        }

        public override void SetValueTo(object inst, FieldType value)
        {
            onFail(inst, MethodBase.GetCurrentMethod());
        }

        public override FieldType Value
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

#if false
    /// <summary>
    /// Класс, инкапсулирующий доступ к полю экземпляра объекта
    /// </summary>
    public class FieldAccessor<FieldType>
    {
        public object Instance => _instance;
        private object _instance;

        public FieldInfo FieldInfo => _fieldInfo;
        private FieldInfo _fieldInfo;

        public bool IsValid => _fieldInfo != null;

        public FieldAccessor(Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType == null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            if (!Initialize(instanceType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Field '{fieldName}' does not found in '{instanceType.FullName}'");
        }

        public FieldAccessor(object instance, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = instance.GetType();

            if (!Initialize(type, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Field '{fieldName}' does not found in '{instance.GetType().FullName}'");

            _instance = instance;
        }

#if false
        public Field(Type instanceType, FieldInfo fieldInfo)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));
            _fieldInfo = fieldInfo;

            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{instanceType.FullName}'");
        } 
#else
        public FieldAccessor(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{fieldInfo.ReflectedType.FullName}'");
        }
#endif

        public FieldAccessor(object instance, FieldInfo fieldInfo)
        {
            if (instance != null)
                throw new ArgumentNullException(nameof(instance));

            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            var instanceType = instance.GetType();
            if (fieldInfo.ReflectedType != instanceType)
                throw new ArgumentException($"ReflectedType of the '{fieldInfo.Name}' is '{fieldInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{instanceType.FullName}'", nameof(fieldInfo));

            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{instance.GetType().FullName}'");

            _instance = instance;
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        private bool Initialize(Type containerType, string fieldName, BindingFlags flags)
        {
            if (containerType is null)
                return false;

            FieldInfo fieldInfo = containerType.GetField(fieldName, flags);
            if (Initialize(fieldInfo))
                return true;
            return Initialize(containerType.BaseType, fieldName, flags);
        }

        private bool Initialize(FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType.Equals(typeof(FieldType)))
                {
                    _fieldInfo = fieldInfo;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Доступ к полю объекта <paramref name="instance"/>
        /// </summary>
        public FieldType this[object instance]
        {
            get
            {
                if (_fieldInfo.GetValue(instance) is FieldType result)
                    return result;
                return default;
            }
            set
            {
                _fieldInfo.SetValue(instance, value);
            }
        }

        public FieldType Value
        {
            get
            {
                if (_fieldInfo.GetValue(_instance) is FieldType result)
                    return result;
                return default;
            }
            set
            {
                _fieldInfo.SetValue(_instance, value);
            }
        }

        public static implicit operator FieldType(FieldAccessor<FieldType> accessor) => accessor.Value;
    } 
#endif
}
