﻿namespace EntityTools.Forms
{
    partial class MultipleItemSelectForm
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
            this.lbl = new System.Windows.Forms.Label();
            this.ItemList = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.btnUncheckAll = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.ItemList)).BeginInit();
            this.SuspendLayout();
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
            // ItemList
            // 
            this.ItemList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemList.CheckOnClick = true;
            this.ItemList.Location = new System.Drawing.Point(12, 36);
            this.ItemList.Name = "ItemList";
            this.ItemList.Size = new System.Drawing.Size(314, 325);
            this.ItemList.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.ItemList.TabIndex = 5;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.ImageOptions.Image = global::EntityTools.Properties.Resources.Valid;
            this.btnSelect.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSelect.Location = new System.Drawing.Point(251, 374);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.handler_Select);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.simpleButton2.ImageOptions.Image = global::EntityTools.Properties.Resources.SelAll;
            this.simpleButton2.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.simpleButton2.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.simpleButton2.Location = new System.Drawing.Point(12, 374);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.simpleButton2.Size = new System.Drawing.Size(23, 23);
            this.simpleButton2.TabIndex = 1;
            this.simpleButton2.ToolTip = "Check All items";
            this.simpleButton2.Click += new System.EventHandler(this.handler_CheckAll);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUncheckAll.ImageOptions.Image = global::EntityTools.Properties.Resources.DeselAll;
            this.btnUncheckAll.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnUncheckAll.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUncheckAll.Location = new System.Drawing.Point(37, 374);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnUncheckAll.Size = new System.Drawing.Size(23, 23);
            this.btnUncheckAll.TabIndex = 1;
            this.btnUncheckAll.ToolTip = "Uncheck all items";
            this.btnUncheckAll.Click += new System.EventHandler(this.handler_UncheckAll);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.ImageOptions.Image = global::EntityTools.Properties.Resources.Refresh;
            this.btnReload.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnReload.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnReload.Location = new System.Drawing.Point(62, 374);
            this.btnReload.Name = "btnReload";
            this.btnReload.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnReload.Size = new System.Drawing.Size(23, 23);
            this.btnReload.TabIndex = 1;
            this.btnReload.ToolTip = "Reload item list";
            this.btnReload.Click += new System.EventHandler(this.handler_Reload);
            // 
            // MultipleItemSelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 409);
            this.Controls.Add(this.ItemList);
            this.Controls.Add(this.lbl);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.btnUncheckAll);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(318, 419);
            this.Name = "MultipleItemSelectForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MultiSelectForm";
            this.Shown += new System.EventHandler(this.handler_Reload);
            ((System.ComponentModel.ISupportInitialize)(this.ItemList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal DevExpress.XtraEditors.SimpleButton btnSelect;
        internal DevExpress.XtraEditors.SimpleButton btnReload;
        internal System.Windows.Forms.Label lbl;
        private DevExpress.XtraEditors.CheckedListBoxControl ItemList;
        internal DevExpress.XtraEditors.SimpleButton btnUncheckAll;
        internal DevExpress.XtraEditors.SimpleButton simpleButton2;
    }
}