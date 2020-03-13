using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Quester.Actions;
using EntityTools.Tools;
using EntityTools.Quester.Conditions;
using EntityTools.UCC;
using EntityTools.Quester.Actions.Deprecated;
using Astral.Classes.ItemFilter;
using MyNW.Classes;
using MyNW.Internals;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using System.Threading.Tasks;
using Astral.Quester.Classes;
using System.Windows.Forms;
using EntityTools.Reflection;
using EntityTools.Extentions;
using EntityTools;

namespace EntityTools.Editors
{
    public class EntityTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            EntityTools.Core.EntityDiagnosticInfos(context.Instance);
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
