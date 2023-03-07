using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Core.Interfaces;
using EntityTools.Tools.Entities;

namespace EntityTools.Editors
{
    public class EntityTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            string message;
            switch (context?.Instance)
            {
                case IEntityDescriptor entityDescriptor:
                    message = EntityDiagnosticTools.Construct(entityDescriptor);
                    break;
                case IEntityIdentifier entityIdentifier:
                    message = EntityDiagnosticTools.Construct(entityIdentifier);
                    break;
                default:
                    message = "Unable recognize test context!";
                    break;
            }
            if (!string.IsNullOrEmpty(message))
                XtraMessageBox.Show(message, $"Test of '{value}'");

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
