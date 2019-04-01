using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace EntityPlugin.Forms
{
    partial class NodeSelectForm
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
            this.simpleButton_0 = new SimpleButton();
            this.labelControl_0 = new LabelControl();
            base.SuspendLayout();
            this.simpleButton_0.Location = new Point(105, 56);
            this.simpleButton_0.Name = "simpleButton1";
            this.simpleButton_0.Size = new Size(75, 23);
            this.simpleButton_0.TabIndex = 0;
            this.simpleButton_0.Text = "OK (F12)";
            this.simpleButton_0.Click += this.simpleButton_0_Click;
            this.labelControl_0.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl_0.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.labelControl_0.AutoSizeMode = LabelAutoSizeMode.None;
            this.labelControl_0.Location = new Point(7, 7);
            this.labelControl_0.Name = "message";
            this.labelControl_0.Size = new Size(269, 41);
            this.labelControl_0.TabIndex = 1;
            this.labelControl_0.Text = "-";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(284, 83);
            base.Controls.Add(this.labelControl_0);
            base.Controls.Add(this.simpleButton_0);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Name = "MessageBoxSpc";
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Message";
            base.TopMost = true;
            base.Load += this.NodeSelectForm_Load;
            base.ResumeLayout(false);
        }

        #endregion
    }
}