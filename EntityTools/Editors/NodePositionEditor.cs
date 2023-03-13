using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Tools;
using MyNW.Classes;

namespace EntityTools.Editors
{
    internal class NodePositionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Vector3 location = TargetSelectHelper.GetNodeLocation("Get Node", "Target the node and press ok.");
            return location.IsValid 
                 ? location.Clone() 
                 : value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
