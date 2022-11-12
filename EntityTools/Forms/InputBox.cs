using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
using DevExpress.Utils;

namespace EntityCore.Forms
{
    public partial class InputBox : XtraForm //*/ Form
    {
        private static InputBox inputBox;
        private InputBox()
        {
            InitializeComponent();
        }

        public static string GetValue(string message, string caption = "", FormatInfo formatInfo = null)
        {
            if(inputBox is null || inputBox.IsDisposed)
                inputBox = new InputBox();

            inputBox.StartPosition = FormStartPosition.CenterParent;
            inputBox.lblMessage.Text = message;
            inputBox.Text = caption;
            if (formatInfo != null && !formatInfo.IsEmpty)
                inputBox.tbValue.Properties.EditFormat.Assign(formatInfo);
            else inputBox.tbValue.Properties.EditFormat.Reset();

            return inputBox.ShowDialog() == DialogResult.OK 
                ? inputBox.EditedValue 
                : string.Empty;
        }

        public static bool EditValue(ref string value, string message, string caption = "", FormatInfo formatInfo = null)
        {
            if (inputBox is null || inputBox.IsDisposed)
                inputBox = new InputBox();

            inputBox.StartPosition = FormStartPosition.CenterParent;
            inputBox.lblMessage.Text = message;
            inputBox.Text = caption;
            inputBox.tbValue.EditValue = value;
            if (formatInfo != null && !formatInfo.IsEmpty)
                inputBox.tbValue.Properties.EditFormat.Assign(formatInfo);
            else inputBox.tbValue.Properties.EditFormat.Reset();

            if (inputBox.ShowDialog() == DialogResult.OK)
            {
                value = inputBox.EditedValue;
                return true;
            }

            return false;
        }

        private void handler_OK(object sender, EventArgs e)
        {
            EditedValue = tbValue.Text;
            DialogResult = string.IsNullOrEmpty(EditedValue) 
                ? DialogResult.Cancel 
                : DialogResult.OK;
            Close();
        }

        private void handler_FormLoad(object sender, EventArgs e)
        {
            tbValue.Focus();
        }

        private void handler_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                DialogResult = DialogResult.OK;
                btnOK.PerformClick();
            }
        }
    }
}
