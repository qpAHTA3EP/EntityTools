using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Reflection
{
    public static class InstanceFieldAccessorFactory
    {
        public static Func<ContainerType, FieldType> GetInstanceFieldAccessor<ContainerType, FieldType>(string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Field name is invalid");

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = typeof(ContainerType);
            if(type is null)
                throw new ArgumentException("'ContainerType' template parameter is invalid");

            FieldInfo fi = type.GetField(fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                if (fi.FieldType.Equals(typeof(FieldType)))
                {
                    return (ContainerType instance) =>
                    {
                        object result = fi.GetValue(instance);
                        return (FieldType)result;
                    };
                }
            }
            return null;
        }
        public static InstanceFieldAccessor<ContainerType, FieldType> GetInstanceField<ContainerType, FieldType>(this ContainerType instance, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            return new InstanceFieldAccessor<ContainerType, FieldType>(instance, fieldName, flags);
        }
    }

    /// <summary>
    /// Класс доступа к статическому свойству
    /// </summary>
    /// <typeparam name="FieldType"></typeparam>
    public class InstanceFieldAccessor<ContainerType, FieldType>
    {
        private FieldInfo fieldInfo;

        private ContainerType instance;

        public InstanceFieldAccessor(ContainerType inst, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if(inst == null)
                throw new ArgumentException("Instance is NULL");

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Field name is invalid");

            if (flags == BindingFlags.Default)
                flags = ReflectionHelper.DefaultFlags;

            Type type = inst.GetType();

            if (!Initialize(type, fieldName, flags | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                fieldInfo = null;
                instance = default;
            }
            else instance = inst;
        }

        /// <summary>
        /// Инициализация полей, необходимых для работы со свойством
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private bool Initialize(Type type, string fieldName, BindingFlags flags)
        {
            if (type != null)
            {
                FieldInfo fi = type.GetField(fieldName, flags);
                if (fi != null)
                {
                    if (fi.FieldType.Equals(typeof(FieldType)))
                    {
                        fieldInfo = fi;
                        return true;
                    }
                    else return false;
                }
                return Initialize(type.BaseType, fieldName, flags);
            }
            return false;
        }

        public bool IsValid()
        {
            return instance != null && fieldInfo != null;
        }

        public FieldType Value
        {
            get
            {
                if (IsValid() && fieldInfo.GetValue(instance) is FieldType result)
                    return result;
                else return default;                
            }
            set
            {
                if (IsValid())
                    fieldInfo.SetValue(instance, value);
            }
        }

        public static implicit operator FieldType(InstanceFieldAccessor<ContainerType, FieldType> accessor) => accessor.Value;
    }
}
