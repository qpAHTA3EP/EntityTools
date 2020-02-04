using Astral.Controllers;
using DevExpress.XtraEditors;
using EntityTools.Tools;
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
        private SimpleButton btnOK;
        private LabelControl lblMessage;

        public TargetSelectForm()
        {
            InitializeComponent();
        }

        public static DialogResult TargetGuiRequest(string caption, Form parent = null)
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
                /*if (parent != null)
                    parent.Focus();
                else CommonTools.FocusForm(typeof(Astral.Forms.Main));*/
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}
