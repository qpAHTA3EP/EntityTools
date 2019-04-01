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

        static NodeSelectForm nodeSelectForm = null;
        public NodeSelectForm()
        {
            InitializeComponent();
        }

        public static DialogResult TargetNodeGuiRequest(string string_0, Form form_0 = null)
        {
            if (nodeSelectForm == null)
                nodeSelectForm = new NodeSelectForm();

            try
            {
                nodeSelectForm.lblMessage.Text = string_0;
                Binds.AddAction(Keys.F12, new Action(nodeSelectForm.btnOK.PerformClick));
                return nodeSelectForm.ShowDialog();

            }
            finally
            {
                Binds.RemoveAction(Keys.F12);
            }

            //NodeSelectForm.Class113 @class = new NodeSelectForm.Class113();
            //@class.string_0 = string_0;
            //@class.dialogResult = DialogResult.Abort;
            //Action action = new Action(@class.method_0);
            ////if (form_0 == null)
            ////{
            ////    form_0 = Astral.Controllers.Forms.Main;
            ////}
            //action();
            //return @class.dialogResult;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //this.flagOK = true;
            base.Close();
        }

        //private bool flagOK;

        private SimpleButton btnOK;

        private LabelControl lblMessage;

        //[CompilerGenerated]
        //private sealed class Class113
        //{
        //    internal void method_0()
        //    {
        //        NodeSelectForm form = new NodeSelectForm();
        //        form.lblMessage.Text = this.string_0;
        //        Binds.AddAction(Keys.F12, new Action(form.btnOK.PerformClick));
        //        form.ShowDialog();
        //        Binds.RemoveAction(Keys.F12);
        //        if (form.flagOK)
        //        {
        //            this.dialogResult = DialogResult.OK;
        //            return;
        //        }
        //        this.dialogResult = DialogResult.Cancel;
        //    }

        //    public string string_0;

        //    public DialogResult dialogResult;
        //}
    }

}
