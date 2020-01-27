using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace EntityTools.Forms
{
    partial class TargetSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new SimpleButton();
            this.lblMessage = new LabelControl();
            base.SuspendLayout();
            this.btnOK.Location = new Point(105, 56);
            this.btnOK.Name = "simpleButton1";
            this.btnOK.Size = new Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK (F12)";
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Click += this.btnOK_Click;
            this.lblMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblMessage.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblMessage.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblMessage.Location = new Point(7, 7);
            this.lblMessage.Name = "message";
            this.lblMessage.Size = new Size(269, 41);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "-";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(284, 83);
            base.Controls.Add(this.lblMessage);
            base.Controls.Add(this.btnOK);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Name = "MessageBoxSpc";
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Message";
            base.TopMost = true;
            base.ResumeLayout(false);
        }

        #endregion
    }
}