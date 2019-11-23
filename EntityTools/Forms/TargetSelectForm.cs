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

namespace EntityTools.Forms
{
    public partial class TargetSelectForm : XtraForm
    {

        static TargetSelectForm selectForm = null;
        public TargetSelectForm()
        {
            InitializeComponent();
        }

        public static DialogResult TargetGuiRequest(string caption, Form form_0 = null)
        {
            if (selectForm == null)
                selectForm = new TargetSelectForm();

            try
            {
                selectForm.lblMessage.Text = caption;
                Binds.AddAction(Keys.F12, new Action(selectForm.btnOK.PerformClick));
                return selectForm.ShowDialog();
            }
            finally
            {
                Binds.RemoveAction(Keys.F12);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private SimpleButton btnOK;

        private LabelControl lblMessage;
    }

}
