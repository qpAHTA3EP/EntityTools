using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTP0Tools.Classes.TypeDescriptorTools
{
    public static class TypeDescriptorHelper
    {
        /// <summary>
        /// Декорирование свойства, соответствующего критерию <paramref name="propertyPredicate"/>, атрибутами <paramref name="attributes"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyPredicate"></param>
        /// <param name="attributes"></param>
        public static void DecoratePropertyWithAttribute(this Type type, Predicate<PropertyDescriptor> propertyPredicate, params Attribute[] attributes)
        {
            if (propertyPredicate is null)
                return;

            // Формирование дескриптора, переопределяющего свойства типа
            // prepare our property overriding type descriptor
            var typeDescriptor = TypeDescriptor.GetProvider(type).GetTypeDescriptor(type);
            if (typeDescriptor is null)
                return;

            PropertyOverridingTypeDescriptor ctd = new PropertyOverridingTypeDescriptor(typeDescriptor);
            // Производим поиск свойств, подлежащего декорированию
            // iterate through properies in the supplied object/type
            foreach (PropertyDescriptor pd in typeDescriptor.GetProperties())
            {
                // Проверяем каждое свойство, на соответствие критерию propertyPredicate
                // for every property that complies to our criteria
                if (propertyPredicate(pd))
                {
                    // Конструируем дескриптор переопределяемого свойства
                    // we first construct the custom PropertyDescriptor with the TypeDescriptor's built-in capabilities
                    PropertyDescriptor pd2 = TypeDescriptor.CreateProperty(
                        type,
                        // Базовый дейскриптор свойства, к которому необходимо добавить атрибуты
                        // base property descriptor to which we want to add attributes
                        // The PropertyDescriptor which we'll get will just wrap that base one returning attributes we need.
                        pd,       
                        attributes
                    );
                    // Добавляем дескриптор свойства в дескриптор, переопределяющий свойства типа
                    // and then we tell our new PropertyOverridingTypeDescriptor to override that property
                    ctd.OverrideProperty(pd2);
                }
            }
            // Добавляем провайдер, возвращающий переопределяющий дескриптор типа, взамен дефолтного
            // then we add new descriptor provider that will return our descriptor instead of default
            TypeDescriptor.AddProviderTransparent(new TypeDescriptorOverridingProvider(ctd), type);
        }
    }
}
