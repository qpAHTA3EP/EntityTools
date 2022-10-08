using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ACTP0Tools.Classes.TypeDescriptorTools
{
    /// <summary>
    /// <see href="https://stackoverflow.com/questions/12143650/how-to-add-property-level-attribute-to-the-typedescriptor-at-runtime"/>
    /// <code href="https://stackoverflow.com/questions/12143650/how-to-add-property-level-attribute-to-the-typedescriptor-at-runtime">
    /// // prepare our property overriding type descriptor<br/>
    /// PropertyOverridingTypeDescriptor ctd = new PropertyOverridingTypeDescriptor(TypeDescriptor.GetProvider(_settings).GetTypeDescriptor(_settings));<br/>
    /// // iterate through properies in the supplied object/type<br/>
    /// foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(_settings)) {<br/>
    ///     // for every property that complies to our criteria<br/>
    ///     if (pd.Name.EndsWith("ConnectionString")) {<br/>
    ///         // we first construct the custom PropertyDescriptor with the TypeDescriptor's built-in capabilities<br/>
    ///         PropertyDescriptor pd2 = TypeDescriptor.CreateProperty(<br/>
    ///             _settings.GetType(), // or just _settings, if it's already a type<br/>
    ///             pd,                  // base property descriptor to which we want to add attributes<br/>
    ///             // The PropertyDescriptor which we'll get will just wrap that<br/>
    ///             // base one returning attributes we need.<br/>
    ///             new EditorAttribute( // the attribute in question<br/>
    ///                 typeof(System.Web.UI.Design.ConnectionStringEditor),<br/>
    ///                 typeof(System.Drawing.Design.UITypeEditor)<br/>
    ///             )<br/>
    ///             // this method really can take as many attributes as you like, not just one<br/>
    ///         );<br/>
    ///     
    ///         // and then we tell our new PropertyOverridingTypeDescriptor to override that property<br/>
    ///         ctd.OverrideProperty(pd2);<br/>
    ///     }<br/>
    /// }<br/>
    /// 
    /// // then we add new descriptor provider that will return our descriptor instead of default
    /// TypeDescriptor.AddProvider(new TypeDescriptorOverridingProvider(ctd), _settings);
    /// </code>
    /// </summary>
    public class PropertyOverridingTypeDescriptor : CustomTypeDescriptor
    {
        private readonly Dictionary<string, PropertyDescriptor> overridePds = new Dictionary<string, PropertyDescriptor>();

        public PropertyOverridingTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        { }

        public void OverrideProperty(PropertyDescriptor pd)
        {
            overridePds[pd.Name] = pd;
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            object o = base.GetPropertyOwner(pd);

            if (o == null)
            {
                return this;
            }

            return o;
        }

        public PropertyDescriptorCollection GetPropertiesImpl(PropertyDescriptorCollection pdc)
        {
            List<PropertyDescriptor> pdl = new List<PropertyDescriptor>(pdc.Count + 1);

            foreach (PropertyDescriptor pd in pdc)
            {
                pdl.Add(overridePds.ContainsKey(pd.Name) 
                                   ? overridePds[pd.Name] 
                                   : pd);
            }

            PropertyDescriptorCollection ret = new PropertyDescriptorCollection(pdl.ToArray());

            return ret;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetPropertiesImpl(base.GetProperties());
        }
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetPropertiesImpl(base.GetProperties(attributes));
        }
    }
}
