using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Astral.Quester.Classes;
using DevExpress.XtraEditors.Controls;
using EntityTools.Forms;
using MyNW.Classes;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;


namespace EntityTools.Editors
{
    class UCCConditionListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ConditionListForm listEditor = new ConditionListForm();

            ConditionList newConditions = listEditor.GetConditionList(value as ConditionList);

            if (listEditor.DialogResult == DialogResult.OK)
            {
                return newConditions;
            }            
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
