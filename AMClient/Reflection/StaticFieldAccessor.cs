using System;
using System.Reflection;

namespace AcTp0Tools.Reflection
{
    public static class StaticFielsAccessorFactory
    {
        public static StaticFieldAccessor<FieldType> GetStaticField<FieldType>(this Type containerType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new StaticFieldAccessor<FieldType>(containerType, fieldName, flags);
        }
    }

    /// <summary>
    /// Класс доступа к статическому полю
    /// </summary>
    /// <typeparam name="FieldType"></typeparam>
    public class StaticFieldAccessor<FieldType>
    {
        private readonly Type containerType;
        private FieldInfo fieldInfo;

        public StaticFieldAccessor(Type t, string fieldName, BindingFlags flags = BindingFlags.Default)
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
                    fieldInfo.SetValue(null,  value);
                }
            }
        }

        public static implicit operator FieldType(StaticFieldAccessor<FieldType> accessor) => accessor.Value;
    }
}
