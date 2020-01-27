using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EntityTools.Tools
{
    class
    CollectionTypeConverter : TypeConverter
    {
        /// <summary>
        /// Только в строку
        /// </summary>
        public override bool CanConvertTo(
        ITypeDescriptorContext context, Type destType)
        {
        return destType == typeof (string);
        }

        /// <summary>
        /// И только так
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {            
            return "(Collection)";
        }
    }
}
