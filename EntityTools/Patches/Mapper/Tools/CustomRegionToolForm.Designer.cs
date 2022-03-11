
namespace EntityTools.Patches.Mapper.Tools
{
    partial class CustomRegionToolForm
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
            this.btnCRTypeSwitcher = new DevExpress.XtraEditors.SimpleButton();
            this.cbCRSelector = new System.Windows.Forms.ComboBox();
            this.btnAccept = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.editCRName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.editCRName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCRTypeSwitcher
            // 
            this.btnCRTypeSwitcher.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCRRectang;
            this.btnCRTypeSwitcher.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCRTypeSwitcher.Location = new System.Drawing.Point(4, 4);
            this.btnCRTypeSwitcher.Name = "btnCRTypeSwitcher";
            this.btnCRTypeSwitcher.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCRTypeSwitcher.Size = new System.Drawing.Size(23, 23);
            this.btnCRTypeSwitcher.TabIndex = 0;
            this.btnCRTypeSwitcher.Click += new System.EventHandler(this.handler_ChangeCRShapeType);
            // 
            // cbCRSelector
            // 
            this.cbCRSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCRSelector.FormattingEnabled = true;
            this.cbCRSelector.Location = new System.Drawing.Point(33, 6);
            this.cbCRSelector.Name = "cbCRSelector";
            this.cbCRSelector.Size = new System.Drawing.Size(263, 21);
            this.cbCRSelector.Sorted = true;
            this.cbCRSelector.TabIndex = 1;
            this.cbCRSelector.SelectedIndexChanged += new System.EventHandler(this.handler_SelectedCustomRegionChanged);
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.ImageOptions.Image = global::EntityTools.Properties.Resources.miniValid;
            this.btnAccept.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnAccept.Location = new System.Drawing.Point(302, 4);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccept.Size = new System.Drawing.Size(23, 23);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Click += new System.EventHandler(this.handler_Accept);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnCancel.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(331, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCancel.Size = new System.Drawing.Size(23, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Click += new System.EventHandler(this.handler_Cancel);
            // 
            // editCRName
            // 
            this.editCRName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editCRName.Location = new System.Drawing.Point(33, 6);
            this.editCRName.Name = "editCRName";
            this.editCRName.Size = new System.Drawing.Size(263, 20);
            this.editCRName.TabIndex = 2;
            // 
            // CustomRegionToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 32);
            this.ControlBox = false;
            this.Controls.Add(this.cbCRSelector);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCRTypeSwitcher);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.editCRName);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.None;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.ShowIcon = false;
            this.MaximumSize = new System.Drawing.Size(360, 64);
            this.MinimumSize = new System.Drawing.Size(360, 64);
            this.Name = "CustomRegionToolForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CustomRegionTool";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.handler_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.handler_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.editCRName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cbCRSelector;
        private DevExpress.XtraEditors.SimpleButton btnCRTypeSwitcher;
        private DevExpress.XtraEditors.SimpleButton btnAccept;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.TextEdit editCRName;
    }
}