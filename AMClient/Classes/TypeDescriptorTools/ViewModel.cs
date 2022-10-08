using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ACTP0Tools.Classes.TypeDescriptorTools
{
    /// <summary>
    /// <see href="https://stackoverflow.com/questions/849202/how-do-i-inject-a-custom-uitypeeditor-for-all-properties-of-a-closed-source-type"/>
    /// <code>var pretty = ViewModel&lt;Report&gt;.Decorate(datum);<br/>
    /// pretty.PropertyAttributeReplacements[typeof(Smiley)] = new List&lt;Attribute&gt;() {<br/>
    ///     new EditorAttribute(typeof(SmileyEditor),typeof(UITypeEditor))<br/>
    /// };<br/>
    /// propertyGrid.SelectedObject = pretty;</code>
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if false
    // Оригинальный код
    public class ViewModel<T> : CustomTypeDescriptor
    {
        private T _instance;
        private ICustomTypeDescriptor _originalDescriptor;
        public ViewModel(T instance, ICustomTypeDescriptor originalDescriptor) : base(originalDescriptor)
        {
            _instance = instance;
            _originalDescriptor = originalDescriptor;
            PropertyAttributeReplacements = new Dictionary<Type,IList<Attribute>>();
        }

        public static ViewModel<T> DressUp(T instance)
        {
            return new ViewModel<T>(instance, TypeDescriptor.GetProvider(instance).GetTypeDescriptor(instance));
        }

        /// <summary>
        /// Most useful for changing EditorAttribute and TypeConvertorAttribute
        /// </summary>
        public IDictionary<Type,IList<Attribute>> PropertyAttributeReplacements {get; set; } 

        public override PropertyDescriptorCollection GetProperties (Attribute[] attributes)
        {
            var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>();

            var bettered = properties.Select(pd =>
                {
                    if (PropertyAttributeReplacements.ContainsKey(pd.PropertyType))
                    {
                        return TypeDescriptor.CreateProperty(typeof(T), pd, PropertyAttributeReplacements[pd.PropertyType].ToArray());
                    }
                    else
                    {
                        return pd;
                    }
                });
            return new PropertyDescriptorCollection(bettered.ToArray());
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null);
        }
    }
#elif false
    // Модифицированный код
    public class ViewModel<T> : CustomTypeDescriptor
    {
        public ViewModel(ICustomTypeDescriptor originalDescriptor) : base(originalDescriptor)
        {
            PropertyAttributeReplacements = new Dictionary<Type, IList<Attribute>>();
        }

        public static ViewModel<T> Decorate(T instance)
        {
            return new ViewModel<T>(TypeDescriptor.GetProvider(instance).GetTypeDescriptor(instance));
        }

        /// <summary>
        /// Most useful for changing EditorAttribute and TypeConvertorAttribute
        /// </summary>
        public IDictionary<Type, IList<Attribute>> PropertyAttributeReplacements { get; set; }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor Selector(PropertyDescriptor pd)
            {
                return PropertyAttributeReplacements.ContainsKey(pd.PropertyType)
                    ? TypeDescriptor.CreateProperty(typeof(T), pd,
                        PropertyAttributeReplacements[pd.PropertyType].ToArray())
                    : pd;
            }

            return new PropertyDescriptorCollection(base.GetProperties(attributes)
                .Cast<PropertyDescriptor>()
                .Select(Selector)
                .ToArray());
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null);
        }
    }
#else
    public class ViewModelDecorator<T>
    {
        public ViewModelDecorator(params ValueTuple<Type, Attribute[]>[] attributes)
        {
            PropertyAttributeReplacements = attributes?.Length > 0
                ? new ConcurrentDictionary<Type, IList<Attribute>>(attributes.Select(tuple =>
                    new KeyValuePair<Type, IList<Attribute>>(tuple.Item1, tuple.Item2)))
                : new ConcurrentDictionary<Type, IList<Attribute>>();
        }
        /// <summary>
        /// Most useful for changing EditorAttribute and TypeConvertorAttribute
        /// </summary>
        public IDictionary<Type, IList<Attribute>> PropertyAttributeReplacements { get; }

        public CustomTypeDescriptor Decorate(T instance)
        {
            return new ViewModel(TypeDescriptor.GetProvider(instance).GetTypeDescriptor(instance), this);
        }

        protected class ViewModel : CustomTypeDescriptor
        {
            private readonly ViewModelDecorator<T> decorator;

            public ViewModel(ICustomTypeDescriptor originalDescriptor, ViewModelDecorator<T> decorator) : base(
                originalDescriptor)
            {
                this.decorator = decorator;
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                PropertyDescriptor Selector(PropertyDescriptor pd)
                {
                    return decorator.PropertyAttributeReplacements.ContainsKey(pd.PropertyType)
                        ? TypeDescriptor.CreateProperty(typeof(T), pd,
                            decorator.PropertyAttributeReplacements[pd.PropertyType].ToArray())
                        : pd;
                }

                return new PropertyDescriptorCollection(base.GetProperties(attributes)
                    .Cast<PropertyDescriptor>()
                    .Select(Selector)
                    .ToArray());
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(null);
            }
        }
    }
#endif
}
