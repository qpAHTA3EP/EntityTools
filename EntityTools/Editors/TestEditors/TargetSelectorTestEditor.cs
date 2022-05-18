using EntityTools.Forms;
using EntityTools.Quester.Actions;
using EntityTools.UCC.Actions;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class TargetSelectorTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
#if false
            EntityTools.Core.Monitor(new PlayerTeamMonitor());
#else
            switch (context?.Instance)
            {
                case ChangeTarget cht:
                    ObjectInfoForm.Show(cht.Engine, 500);
                    break;
                case MoveToTeammate m2t:
                    ObjectInfoForm.Show(m2t.Engine, 500);
                    break;
            } 
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
