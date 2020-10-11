using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Extensions;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;


namespace EntityTools.Editors
{
#if DEVELOPER
    class UCCConditionListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            ConditionList conditions = value as ConditionList;

            if (EntityTools.Core.GUIRequest_UCCConditions(ref conditions))
            {
                return conditions.Clone();
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
