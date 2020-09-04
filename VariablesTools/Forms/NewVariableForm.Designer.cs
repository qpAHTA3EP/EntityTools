namespace VariableTools.Forms
{
    partial class NewVariableForm
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
            this.lblName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.cbAccountScope = new System.Windows.Forms.ComboBox();
            this.cbProfileScope = new System.Windows.Forms.ComboBox();
            this.lblAccountScope = new System.Windows.Forms.Label();
            this.lblProfileScope = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblValue = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.ckbSave = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(55, 14);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(34, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbName.Location = new System.Drawing.Point(96, 12);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(207, 21);
            this.tbName.TabIndex = 1;
            // 
            // cbAccountScope
            // 
            this.cbAccountScope.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAccountScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAccountScope.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbAccountScope.FormattingEnabled = true;
            this.cbAccountScope.Location = new System.Drawing.Point(95, 38);
            this.cbAccountScope.Name = "cbAccountScope";
            this.cbAccountScope.Size = new System.Drawing.Size(207, 21);
            this.cbAccountScope.TabIndex = 2;
            // 
            // cbProfileScope
            // 
            this.cbProfileScope.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbProfileScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProfileScope.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbProfileScope.FormattingEnabled = true;
            this.cbProfileScope.Location = new System.Drawing.Point(95, 64);
            this.cbProfileScope.Name = "cbProfileScope";
            this.cbProfileScope.Size = new System.Drawing.Size(207, 21);
            this.cbProfileScope.TabIndex = 3;
            // 
            // lblAccountScope
            // 
            this.lblAccountScope.AutoSize = true;
            this.lblAccountScope.Location = new System.Drawing.Point(15, 39);
            this.lblAccountScope.Name = "lblAccountScope";
            this.lblAccountScope.Size = new System.Drawing.Size(75, 13);
            this.lblAccountScope.TabIndex = 0;
            this.lblAccountScope.Text = "AccountScope";
            // 
            // lblProfileScope
            // 
            this.lblProfileScope.AutoSize = true;
            this.lblProfileScope.Location = new System.Drawing.Point(23, 66);
            this.lblProfileScope.Name = "lblProfileScope";
            this.lblProfileScope.Size = new System.Drawing.Size(66, 13);
            this.lblProfileScope.TabIndex = 0;
            this.lblProfileScope.Text = "ProfileScope";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(227, 137);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(146, 137);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(57, 92);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(33, 13);
            this.lblValue.TabIndex = 0;
            this.lblValue.Text = "Value";
            // 
            // tbValue
            // 
            this.tbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbValue.Location = new System.Drawing.Point(96, 90);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(207, 21);
            this.tbValue.TabIndex = 4;
            this.tbValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbValue_KeyPress);
            // 
            // ckbSave
            // 
            this.ckbSave.AutoSize = true;
            this.ckbSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckbSave.Location = new System.Drawing.Point(22, 117);
            this.ckbSave.Name = "ckbSave";
            this.ckbSave.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckbSave.Size = new System.Drawing.Size(85, 17);
            this.ckbSave.TabIndex = 5;
            this.ckbSave.Text = "Save to File  ";
            this.ckbSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbSave.UseVisualStyleBackColor = true;
            // 
            // NewVariableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 172);
            this.ControlBox = false;
            this.Controls.Add(this.ckbSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbProfileScope);
            this.Controls.Add(this.cbAccountScope);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lblProfileScope);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.lblAccountScope);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NewVariableForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NewVariable";
            this.Load += new System.EventHandler(this.NewVariableForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.ComboBox cbAccountScope;
        private System.Windows.Forms.ComboBox cbProfileScope;
        private System.Windows.Forms.Label lblAccountScope;
        private System.Windows.Forms.Label lblProfileScope;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.CheckBox ckbSave;
    }
}