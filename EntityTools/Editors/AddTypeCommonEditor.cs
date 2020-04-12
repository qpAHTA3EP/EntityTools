using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Forms;

namespace EntityTools.Editors
{
#if DEVELOPER
    class AddTypeCommonEditor<T> : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            object newValue = (value == null) ? AddAction.Show(typeof(T)) : AddAction.Show(value.GetType());
            if (newValue != null
                && !ReferenceEquals(newValue, value))
            {
                return newValue;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
