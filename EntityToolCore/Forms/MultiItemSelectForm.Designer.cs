namespace EntityCore.Forms
{
    partial class MultiItemSelectForm
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
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            this.lbl = new System.Windows.Forms.Label();
            this.Items = new DevExpress.XtraEditors.CheckedListBoxControl();
            ((System.ComponentModel.ISupportInitialize)(this.Items)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.btnSelect.ImageOptions.Image = global::EntityCore.Properties.Resources.miniValid;
            this.btnSelect.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSelect.Location = new System.Drawing.Point(12, 367);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(271, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.btnReload.ImageOptions.Image = global::EntityCore.Properties.Resources.miniRefresh;
            this.btnReload.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnReload.Location = new System.Drawing.Point(289, 367);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(23, 23);
            this.btnReload.TabIndex = 1;
            this.btnReload.ToolTip = "Reload";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Location = new System.Drawing.Point(9, 9);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(166, 13);
            this.lbl.TabIndex = 2;
            this.lbl.Text = "Select several items from the list:";
            // 
            // Items
            // 
            this.Items.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Items.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.Items.CheckOnClick = true;
            this.Items.Location = new System.Drawing.Point(12, 36);
            this.Items.Name = "Items";
            this.Items.Size = new System.Drawing.Size(300, 315);
            this.Items.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.Items.TabIndex = 5;
            // 
            // MultiItemSelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 402);
            this.Controls.Add(this.Items);
            this.Controls.Add(this.lbl);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(332, 429);
            this.Name = "MultiItemSelectForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MultiSelectForm";
            this.Shown += new System.EventHandler(this.MultiSelectForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.Items)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal DevExpress.XtraEditors.SimpleButton btnSelect;
        internal DevExpress.XtraEditors.SimpleButton btnReload;
        internal System.Windows.Forms.Label lbl;
        private DevExpress.XtraEditors.CheckedListBoxControl Items;
    }
}