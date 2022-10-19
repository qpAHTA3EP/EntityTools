using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Classes;
using Astral.Quester.Forms;

namespace EntityTools.Editors
{
#if DEVELOPER
    internal class CampaignTaskEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return GetACampaignTask.Show(value as CampaignTask);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
