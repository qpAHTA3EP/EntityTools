using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Astral.Quester.Classes;
using DevExpress.XtraEditors.Controls;
using MyNW.Classes;
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
                return conditions;
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
