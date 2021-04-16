using System;
using System.Collections.Generic;
using System.Reflection;

namespace AcTp0Tools.Reflection
{
    public static partial class ReflectionHelper
    {
#if false
        public static Func<ContainerType, FieldType> GetInstanceFieldAccessor<ContainerType, FieldType>(string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Field name is invalid");

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);
            if (type is null)
                throw new ArgumentException("'ContainerType' template parameter is invalid");

            FieldInfo fi = type.GetField(fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                if (fi.FieldType.Equals(typeof(FieldType)))
                {
                    return instance =>
                    {
                        object result = fi.GetValue(instance);
                        return (FieldType)result;
                    };
                }
            }
            return null;
        } 
#endif
        public static Field<ContainerType, FieldType> GetField<ContainerType, FieldType>(this ContainerType instance, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new Field<ContainerType, FieldType>(instance, fieldName, flags);
        }
        public static Field<FieldType> GetField<FieldType>(this object instance, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new Field<FieldType>(instance, fieldName, flags);
        }
        public static Field<ContainerType, FieldType> GetField<ContainerType, FieldType>(this Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new Field<ContainerType, FieldType>(instanceType, fieldName, flags);
        }
        public static Field<FieldType> GetField<FieldType>(this Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new Field<FieldType>(instanceType, fieldName, flags);
        }

        /// <summary>
        /// Получение доступа к коллекции полей объекта <paramref name="instance"/> имеющим тип <typeparamref name="FieldType"/>
        /// </summary>
        public static IEnumerable<Field<FieldType>> GetFields<FieldType>(this object instance, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var type = instance.GetType();
            var fieldType = typeof(FieldType);
            foreach (var fieldInfo in type.GetFields(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.FieldType == fieldType)
                    yield return new Field<FieldType>(fieldInfo);
            }
        }
        /// <summary>
        /// Получение доступа к коллекции полей объекта <paramref name="instanceType"/> имеющим тип <typeparamref name="FieldType"/>
        /// </summary>
        public static IEnumerable<Field<FieldType>> GetFields<FieldType>(this Type instanceType, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            var fieldType = typeof(FieldType);
            foreach (var fieldInfo in instanceType.GetFields(flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.FieldType == fieldType)
                    yield return new Field<FieldType>(fieldInfo);
            }
        }
    }

    /// <summary>
    /// Класс доступа к статическому свойству
    /// </summary>
    public class Field<ContainerType, FieldType>
    {
        public ContainerType Instance => _instance;
        private ContainerType _instance;

        public FieldInfo FieldInfo => _fieldInfo;
        private FieldInfo _fieldInfo;

        public bool IsValid => _fieldInfo != null;

        public Field(Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
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

        public Field(ContainerType instance, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if(instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = instance.GetType();

            if (!Initialize(type, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Field '{fieldName}' does not found in '{type.FullName}'");

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
        public Field(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            var containerType = typeof(ContainerType);
            if (fieldInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{fieldInfo.Name}' is '{fieldInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(fieldInfo));

            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{containerType.FullName}'");
        }
#endif

        public Field(ContainerType instance, FieldInfo fieldInfo)
        {
            if (instance != null)
                throw new ArgumentNullException(nameof(instance));

            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            var containerType = instance.GetType();
            if (fieldInfo.ReflectedType != containerType)
                throw new ArgumentException($"ReflectedType of the '{fieldInfo.Name}' is '{fieldInfo.ReflectedType.FullName}' does not equal to parameter ContainerType '{containerType.FullName}'", nameof(fieldInfo));

            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{containerType.FullName}'");

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

        public static implicit operator FieldType(Field<ContainerType, FieldType> accessor) => accessor.Value;
    }

    /// <summary>
    /// Класс доступа к статическому свойству
    /// </summary>
    public class Field<FieldType>
    {
        public object Instance => _instance;
        private object _instance;

        public FieldInfo FieldInfo => _fieldInfo;
        private FieldInfo _fieldInfo;

        public bool IsValid => _fieldInfo != null;

        public Field(Type instanceType, string fieldName, BindingFlags flags = BindingFlags.Default)
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

        public Field(object instance, string fieldName, BindingFlags flags = BindingFlags.Default)
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
        public Field(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{fieldInfo.ReflectedType.FullName}'");
        }
#endif

        public Field(object instance, FieldInfo fieldInfo)
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

        public static implicit operator FieldType(Field<FieldType> accessor) => accessor.Value;
    }
}
