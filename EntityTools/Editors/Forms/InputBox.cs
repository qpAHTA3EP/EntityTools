using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityTools.Editors.Forms
{
    public partial class InputBox : XtraForm //*/ Form
    {
        private InputBox()
        {
            InitializeComponent();
        }

        public static string GetValue(string message)
        {
            InputBox inputBox = new InputBox();
            inputBox.StartPosition = FormStartPosition.CenterParent;
            inputBox.Message.Text = message;
            if(inputBox.ShowDialog() == DialogResult.OK)
                return inputBox.EditedValue;
            return string.Empty;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            EditedValue = value.Text;
            DialogResult = DialogResult.OK;
            base.Close();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            value.Focus();
        }

        private void value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                DialogResult = DialogResult.OK;
                btnOk.PerformClick();
            }
        }
    }
}
