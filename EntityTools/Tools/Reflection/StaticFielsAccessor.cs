using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Reflection
{
    public static class StaticFielsAccessorFactory
    {
        public static StaticFielsAccessor<FieldType> GetStaticField<FieldType>(this Type containerType, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new StaticFielsAccessor<FieldType>(containerType, fieldName, flags);
        }
    }

    /// <summary>
    /// Класс доступа к статическому полю
    /// </summary>
    /// <typeparam name="FieldType"></typeparam>
    public class StaticFielsAccessor<FieldType>
    {
        private readonly Type containerType;
        private FieldInfo fieldInfo;

        public StaticFielsAccessor(Type t, string fieldName, BindingFlags flags = BindingFlags.Default)
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
                else return default(FieldType);                
            }
            set
            {
                if (IsValid() && fieldInfo != null)
                {
                    fieldInfo.SetValue(null,  value);
                }
            }
        }

        public static implicit operator FieldType(StaticFielsAccessor<FieldType> accessor) => accessor.Value;
    }
}
