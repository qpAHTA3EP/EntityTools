using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
#if DEVELOPER
    class NodePositionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Vector3 pos = new Vector3();
            if (EntityTools.Core.GUIRequest_NodeLocation(ref pos, "Target the node and press ok."))
            {
                return pos.Clone();
            }
            else return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
