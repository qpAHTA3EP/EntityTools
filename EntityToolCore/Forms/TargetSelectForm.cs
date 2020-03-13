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

namespace EntityCore.Forms
{
    public partial class TargetSelectForm : XtraForm
    {
        static TargetSelectForm @this = null;
        private SimpleButton btnOK;
        private LabelControl lblMessage;

        public TargetSelectForm()
        {
            InitializeComponent();
        }

        public static DialogResult TargetGuiRequest(string caption, Form parent = null)
        {
            if (@this == null)
                @this = new TargetSelectForm();

            try
            {
                @this.lblMessage.Text = caption;

                Binds.AddAction(Keys.F12, new Action(@this.btnOK.PerformClick));
                return @this.ShowDialog();
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
