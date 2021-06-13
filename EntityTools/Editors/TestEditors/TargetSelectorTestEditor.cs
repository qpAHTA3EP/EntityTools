using EntityTools.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.UCC.Actions;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class TargetSelectorTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
#if false
            string msg = EntityTools.Core.EntityDiagnosticInfos(context?.Instance);
            if (!string.IsNullOrEmpty(msg))
                //Task.Factory.StartNew(() => XtraMessageBox.Show(/*Form.ActiveForm, */sb.ToString(), "Test of '" + obj.ToString() + '\''));
                XtraMessageBox.Show(msg, string.Concat("Test of '", value, '\'')); 
#else
            if (context?.Instance is ChangeTarget changeTarget)
                new ObjectInfoForm().Show(changeTarget.Engine, 500);
#endif
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
