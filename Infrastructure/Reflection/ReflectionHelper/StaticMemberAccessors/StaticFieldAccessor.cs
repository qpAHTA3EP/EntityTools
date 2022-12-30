using System;
using System.Reflection;

namespace Infrastructure.Reflection
{
    public static partial class ReflectionHelper
    {
        /// <summary>
        /// Конструирование функтора <see cref="StaticFieldAccessor{FieldType}"/>, осуществляющего через механизм рефлексии, доступ к члену <paramref name="fieldName"/> типа <typeparamref name="FieldType"/>, объявленного в типа <paramref name="containerType"/>.
        /// </summary>
        /// <typeparam name="FieldType"></typeparam>
        /// <param name="containerType"></param>
        /// <param name="fieldName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static StaticFieldAccessor<FieldType> GetStaticField<FieldType>(this Type containerType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new StaticFieldAccessor<FieldType>(containerType, fieldName, flags);
        }
    }

    /// <summary>
    /// Класс, инкапсулирующий доступ к статическому члену типа <typeparamref name="FieldType"/>, заданному в конструкторе
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

            Initialize(containerType, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public StaticFieldAccessor(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            Initialize(fieldInfo);
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
                    _fieldInfo = fieldInfo;
                    return true;
                }
            }
            return false;
        }

        public virtual FieldType Value
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

        public virtual FieldType GetValue()
        {
            if (_fieldInfo.GetValue(null) is FieldType result)
                return result;
            return default;
        }
        public virtual void SetValue(FieldType value)
        {
            _fieldInfo.SetValue(null, value);
        }

        public static implicit operator FieldType(StaticFieldAccessor<FieldType> accessor) => accessor.Value;
    }
}
