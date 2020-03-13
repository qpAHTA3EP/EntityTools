using DevExpress.XtraEditors;
using System.Drawing;

namespace EntityCore.Forms
{
    partial class InputBox
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
            this.value = new DevExpress.XtraEditors.TextEdit();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.Message = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.value.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // value
            // 
            this.value.Location = new System.Drawing.Point(12, 43);
            this.value.Name = "value";
            this.value.Size = new System.Drawing.Size(212, 20);
            this.value.TabIndex = 0;
            this.value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.value_KeyPress);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(233, 44);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(28, 19);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // Message
            // 
            this.Message.Appearance.Options.UseTextOptions = true;
            this.Message.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.Message.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.Message.Location = new System.Drawing.Point(13, 4);
            this.Message.Name = "Message";
            this.Message.Size = new System.Drawing.Size(248, 33);
            this.Message.TabIndex = 2;
            this.Message.Text = "Message text";
            // 
            // InputBox
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 71);
            this.Controls.Add(this.Message);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.value);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InputBox";
            this.Text = "InputBox";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.InputBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.value.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private string EditedValue = string.Empty;
        private TextEdit value;
        private SimpleButton btnOk;
        private LabelControl Message;
    }
}