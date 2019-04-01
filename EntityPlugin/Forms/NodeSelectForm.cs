using Astral.Controllers;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityPlugin.Forms
{
    public partial class NodeSelectForm : XtraForm
    {
        public NodeSelectForm()
        {
            InitializeComponent();
        }
    //}
    //public class NodeSelectForm : XtraForm
    //{
        // Token: 0x06001895 RID: 6293 RVA: 0x000109B1 File Offset: 0x0000EBB1
        //private NodeSelectForm()
        //{
        //    this.method_0();
        //}

        // Token: 0x06001896 RID: 6294 RVA: 0x000109BF File Offset: 0x0000EBBF
        public static DialogResult smethod_0(string string_0, Form form_0 = null)
        {
            NodeSelectForm.Class113 @class = new NodeSelectForm.Class113();
            @class.string_0 = string_0;
            @class.dialogResult_0 = DialogResult.Abort;
            Action action = new Action(@class.method_0);
            //if (form_0 == null)
            //{
            //    form_0 = Astral.Controllers.Forms.Main;
            //}
            action();
            return @class.dialogResult_0;
        }

        // Token: 0x06001897 RID: 6295 RVA: 0x00002CEE File Offset: 0x00000EEE
        private void NodeSelectForm_Load(object sender, EventArgs e)
        {
        }

        // Token: 0x06001898 RID: 6296 RVA: 0x000109F4 File Offset: 0x0000EBF4
        private void simpleButton_0_Click(object sender, EventArgs e)
        {
            this.bool_0 = true;
            base.Close();
        }

        // Token: 0x06001899 RID: 6297 RVA: 0x00010A03 File Offset: 0x0000EC03
        //protected override void Dispose(bool bool_1)
        //{
        //    if (bool_1 && this.icontainer_0 != null)
        //    {
        //        this.icontainer_0.Dispose();
        //    }
        //    base.Dispose(bool_1);
        //}

        // Token: 0x0600189A RID: 6298 RVA: 0x00083BD4 File Offset: 0x00081DD4
        private void method_0()
        {
            
        }

        // Token: 0x04000BED RID: 3053
        private bool bool_0;

        // Token: 0x04000BEE RID: 3054
        private IContainer icontainer_0;

        // Token: 0x04000BEF RID: 3055
        private SimpleButton simpleButton_0;

        // Token: 0x04000BF0 RID: 3056
        private LabelControl labelControl_0;

        // Token: 0x020002E4 RID: 740
        //[CompilerGenerated]
        private sealed class Class113
        {
            // Token: 0x0600189C RID: 6300 RVA: 0x00083D8C File Offset: 0x00081F8C
            internal void method_0()
            {
                NodeSelectForm form = new NodeSelectForm();
                form.labelControl_0.Text = this.string_0;
                Binds.AddAction(Keys.F12, new Action(form.simpleButton_0.PerformClick));
                form.ShowDialog();
                Binds.RemoveAction(Keys.F12);
                if (form.bool_0)
                {
                    this.dialogResult_0 = DialogResult.OK;
                    return;
                }
                this.dialogResult_0 = DialogResult.Cancel;
            }

            // Token: 0x04000BF1 RID: 3057
            public string string_0;

            // Token: 0x04000BF2 RID: 3058
            public DialogResult dialogResult_0;
        }
    }

}
