using System;
using System.Collections.Generic;
using System.Reflection;

namespace AcTp0Tools.Reflection
{
    public static partial class ReflectionHelper
    {
        //TODO Добавить нетипизированную перегрузку  FieldAccessor<object> GetField(this object instance, string propertyName, BindingFlags flags = BindingFlags.Default)
        //TODO Добавить нетипизированную перегрузку  FieldAccessor<object> GetField(this Type containerType, string propertyName, BindingFlags flags = BindingFlags.Default)

        public static FieldAccessor<ContainerType, FieldType> GetField<ContainerType, FieldType>(this ContainerType instance, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new FieldAccessor<ContainerType, FieldType>(instance, fieldName, flags);
        }
        public static FieldAccessor<FieldType>                GetField<FieldType>(this object instance, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new FieldAccessor<FieldType>(instance, fieldName, flags);
        }
        public static FieldAccessor<ContainerType, FieldType> GetField<ContainerType, FieldType>(this Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new FieldAccessor<ContainerType, FieldType>(instanceType, fieldName, flags);
        }
        public static FieldAccessor<FieldType>                GetField<FieldType>(this Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new FieldAccessor<FieldType>(instanceType, fieldName, flags);
        }

        /// <summary>
        /// Получение доступа к коллекции полей, имеющим тип <typeparamref name="FieldType"/> и инкапсулированных в объекте <paramref name="instance"/>
        /// </summary>
        public static IEnumerable<FieldAccessor<FieldType>>   GetFields<FieldType>(this object instance, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var type = instance.GetType();
            var fieldType = typeof(FieldType);
            foreach (var fieldInfo in type.GetFields(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.FieldType == fieldType)
                    yield return new FieldAccessor<FieldType>(fieldInfo);
            }
        }
        /// <summary>
        /// Получение доступа к коллекции полей, имеющим тип <typeparamref name="FieldType"/> и инкапслуированных в типе <paramref name="instanceType"/> 
        /// </summary>
        public static IEnumerable<FieldAccessor<FieldType>>   GetFields<FieldType>(this Type instanceType, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var fieldType = typeof(FieldType);
            foreach (var fieldInfo in instanceType.GetFields(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.FieldType == fieldType)
                    yield return new FieldAccessor<FieldType>(fieldInfo);
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
            get => _instance;
            set => _instance = value;
        }

        private ContainerType _instance;

        public MemberInfo MemberInfo => _fieldInfo;
        public FieldInfo FieldInfo => _fieldInfo;
        private FieldInfo _fieldInfo;

        public bool IsValid => _fieldInfo != null;

        public FieldAccessor(Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

#if true
            Initialize(instanceType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic);
#else
            if (!Initialize(instanceType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Field '{fieldName}' does not found in '{instanceType.FullName}'"); 
#endif
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

#if false
            if (!Initialize(instanceType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Field '{fieldName}' does not found in '{type.FullName}'"); 
#else
            if (Initialize(instanceType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
#endif
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

            var containerType = typeof(ContainerType);
            if (fieldInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{fieldInfo.Name}' is '{fieldInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(fieldInfo));

#if true
            Initialize(fieldInfo);
#else
            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{containerType.FullName}'"); 
#endif
        }
#endif

        public FieldAccessor(ContainerType instance, FieldInfo fieldInfo)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            var containerType = instance.GetType();
            if (fieldInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{fieldInfo.Name}' is '{fieldInfo.ReflectedType?.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(fieldInfo));

#if false
            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{containerType.FullName}'"); 
#else
            if(Initialize(fieldInfo))
#endif

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
#if false
                if (fieldInfo.FieldType.Equals(typeof(FieldType))) 
#else
                var fieldType = typeof(FieldType);
                if (fieldInfo.FieldType == fieldType
                    || fieldType.IsAssignableFrom(fieldInfo.FieldType))
#endif
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
        public FieldType this[ContainerType instance]
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

        public FieldType GetValue()
        {
            if (_fieldInfo.GetValue(_instance) is FieldType result)
                return result;
            return default;
        }
        public void SetValue(FieldType value)
        {
            _fieldInfo.SetValue(_instance, value);
        }

        public FieldType GetValueFrom(ContainerType instance)
        {
            if (_fieldInfo.GetValue(instance) is FieldType result)
                return result;
            return default;
        }
        public void SetValueTo(ContainerType instance, FieldType value)
        {
            _fieldInfo.SetValue(instance, value);
        }

        public static implicit operator FieldType(FieldAccessor<ContainerType, FieldType> accessor) => accessor.Value;
    }

#if true
    public class FieldAccessor<FieldType> : FieldAccessor<object, FieldType>
    {
        public FieldAccessor(Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default) : base(instanceType, fieldName, flags) { }
        public FieldAccessor(object instance, string fieldName, BindingFlags flags = BindingFlags.Default) : base(instance, fieldName, flags) { }
        public FieldAccessor(object instance, FieldInfo fieldInfo) : base(instance, fieldInfo) { }
        public FieldAccessor(FieldInfo fieldInfo) : base(fieldInfo) { }
    }
#else
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
