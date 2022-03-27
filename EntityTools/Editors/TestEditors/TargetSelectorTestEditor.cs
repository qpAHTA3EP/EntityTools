using EntityTools.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Quester.Actions;
using EntityTools.UCC.Actions;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class TargetSelectorTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            switch (context?.Instance)
            {
                case ChangeTarget changeTarget:
                    ObjectInfoForm.Show(changeTarget.Engine, 500);
                    break;
                case MoveToTeammate m2t:
                    ObjectInfoForm.Show(m2t.Engine, 500);
                    break;
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
