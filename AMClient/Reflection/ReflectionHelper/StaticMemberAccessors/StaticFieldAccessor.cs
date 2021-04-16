using System;
using System.Reflection;

namespace AcTp0Tools.Reflection
{
    public static partial class ReflectionHelper
    {
        public static StaticFieldAccessor<FieldType> GetStaticField<FieldType>(this Type containerType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new StaticFieldAccessor<FieldType>(containerType, fieldName, flags);
        }
    }

#if false
    /// <summary>
    /// Класс доступа к статическому полю
    /// </summary>
    /// <typeparam name="FieldType"></typeparam>
    public class StaticField<FieldType>
    {
        private readonly Type containerType;
        private FieldInfo fieldInfo;

        public StaticField(Type t, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Field name is invalid");

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            if (!Initialize(t, fieldName, flags | BindingFlags.Static | BindingFlags.NonPublic))
            {
                containerType = null;
                fieldInfo = null;
            }
        }

        /// <summary>
        /// Инициализация полей, необходимых для доступа к полю
        /// </summary>
        /// <param name="t"></param>
        /// <param name="fieldName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private bool Initialize(Type t, string fieldName, BindingFlags flags)
        {
            if (t != null)
            {
                FieldInfo fi = t.GetField(fieldName, flags);
                if (fi != null)
                {
                    fieldInfo = fi;
                    return true;
                }
                return Initialize(t.BaseType, fieldName, flags);
            }
            return false;
        }

        public bool IsValid()
        {
            return containerType != null && fieldInfo != null;
        }

        public FieldType Value
        {
            get
            {
                object result = fieldInfo?.GetValue(null);
                if (result != null)
                    return (FieldType)result;
                return default;
            }
            set
            {
                if (IsValid() && fieldInfo != null)
                {
                    fieldInfo.SetValue(null, value);
                }
            }
        }

        public static implicit operator FieldType(StaticField<FieldType> accessor) => accessor.Value;
    } 
#else
    /// <summary>
    /// Класс, инкапсулирующий доступ к полю экземпляра объекта
    /// </summary>
    public class StaticFieldAccessor<FieldType> : IMemberAccessor<FieldType>
    {
        public MemberInfo MemberInfo => _fieldInfo;
        public FieldInfo FieldInfo => _fieldInfo;
        private FieldInfo _fieldInfo;

        public bool IsValid => _fieldInfo != null;

        public StaticFieldAccessor(Type containerType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (containerType is null)
                throw new ArgumentNullException(nameof(containerType));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

#if true
            Initialize(containerType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic);
#else
            if (!Initialize(containerType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
                throw new TargetException($"Field '{fieldName}' does not found in '{containerType.FullName}'"); 
#endif
        }

        public StaticFieldAccessor(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

#if true
            Initialize(fieldInfo);
#else
            if (!Initialize(fieldInfo))
                throw new TargetException($"Field '{fieldInfo.Name}' does not present in '{fieldInfo.ReflectedType.FullName}'"); 
#endif
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

        public FieldType Value
        {
            get
            {
                if (_fieldInfo.GetValue(null) is FieldType result)
                    return result;
                return default;
            }
            set
            {
                _fieldInfo.SetValue(null, value);
            }
        }

        public FieldType GetValue()
        {
            if (_fieldInfo.GetValue(null) is FieldType result)
                return result;
            return default;
        }
        public void SetValue(FieldType value)
        {
            _fieldInfo.SetValue(null, value);
        }

        public static implicit operator FieldType(StaticFieldAccessor<FieldType> accessor) => accessor.Value;
    }
#endif
}
