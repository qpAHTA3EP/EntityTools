
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
            this.CustomRegionSelector = new System.Windows.Forms.ComboBox();
            this.btnAccept = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.editCRName = new DevExpress.XtraEditors.TextEdit();
            this.editX = new DevExpress.XtraEditors.TextEdit();
            this.editY = new DevExpress.XtraEditors.TextEdit();
            this.editWidth = new DevExpress.XtraEditors.TextEdit();
            this.editHeight = new DevExpress.XtraEditors.TextEdit();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.editCRName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editX.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editY.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editWidth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editHeight.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCRTypeSwitcher
            // 
            this.btnCRTypeSwitcher.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCRTypeSwitcher.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCRRectang;
            this.btnCRTypeSwitcher.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCRTypeSwitcher.Location = new System.Drawing.Point(353, 2);
            this.btnCRTypeSwitcher.Name = "btnCRTypeSwitcher";
            this.btnCRTypeSwitcher.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCRTypeSwitcher.Size = new System.Drawing.Size(23, 23);
            this.btnCRTypeSwitcher.TabIndex = 6;
            this.btnCRTypeSwitcher.Click += new System.EventHandler(this.handler_ChangeCRType);
            // 
            // CustomRegionSelector
            // 
            this.CustomRegionSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CustomRegionSelector.FormattingEnabled = true;
            this.CustomRegionSelector.Location = new System.Drawing.Point(52, 4);
            this.CustomRegionSelector.Name = "CustomRegionSelector";
            this.CustomRegionSelector.Size = new System.Drawing.Size(295, 21);
            this.CustomRegionSelector.Sorted = true;
            this.CustomRegionSelector.TabIndex = 1;
            this.CustomRegionSelector.SelectionChangeCommitted += new System.EventHandler(this.handler_SelectedCustomRegionChanged);
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.ImageOptions.Image = global::EntityTools.Properties.Resources.miniValid;
            this.btnAccept.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnAccept.Location = new System.Drawing.Point(324, 32);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccept.Size = new System.Drawing.Size(23, 23);
            this.btnAccept.TabIndex = 7;
            this.btnAccept.Click += new System.EventHandler(this.handler_Accept);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnCancel.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(353, 32);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCancel.Size = new System.Drawing.Size(23, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Click += new System.EventHandler(this.handler_Cancel);
            // 
            // editCRName
            // 
            this.editCRName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editCRName.Location = new System.Drawing.Point(52, 4);
            this.editCRName.Name = "editCRName";
            this.editCRName.Size = new System.Drawing.Size(295, 20);
            this.editCRName.TabIndex = 2;
            // 
            // editX
            // 
            this.editX.EditValue = 0;
            this.editX.Location = new System.Drawing.Point(27, 34);
            this.editX.Name = "editX";
            this.editX.Properties.BeepOnError = false;
            this.editX.Properties.EditValueChangedDelay = 1000;
            this.editX.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.editX.Properties.MaskSettings.Set("mask", "d");
            this.editX.Size = new System.Drawing.Size(50, 20);
            this.editX.TabIndex = 2;
            this.editX.ToolTip = "X-coordinate of the top left corner of the CustomRegion";
            this.editX.EditValueChanged += new System.EventHandler(this.handler_XChanged);
            // 
            // editY
            // 
            this.editY.EditValue = 0;
            this.editY.Location = new System.Drawing.Point(103, 34);
            this.editY.Name = "editY";
            this.editY.Properties.BeepOnError = false;
            this.editY.Properties.EditValueChangedDelay = 1000;
            this.editY.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.editY.Properties.MaskSettings.Set("mask", "d");
            this.editY.Size = new System.Drawing.Size(50, 20);
            this.editY.TabIndex = 3;
            this.editY.ToolTip = "Y-coordinate of the top left corner of the CustomRegion";
            this.editY.EditValueChanged += new System.EventHandler(this.handler_YChanged);
            // 
            // editWidth
            // 
            this.editWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editWidth.EditValue = 0;
            this.editWidth.Location = new System.Drawing.Point(288, 34);
            this.editWidth.Name = "editWidth";
            this.editWidth.Properties.BeepOnError = false;
            this.editWidth.Properties.EditValueChangedDelay = 1000;
            this.editWidth.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.editWidth.Properties.MaskSettings.Set("mask", "d");
            this.editWidth.Size = new System.Drawing.Size(30, 20);
            this.editWidth.TabIndex = 5;
            this.editWidth.EditValueChanged += new System.EventHandler(this.handled_WidthChanged);
            this.editWidth.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.handler_SizeChanging);
            // 
            // editHeight
            // 
            this.editHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editHeight.EditValue = 0;
            this.editHeight.Location = new System.Drawing.Point(210, 34);
            this.editHeight.Name = "editHeight";
            this.editHeight.Properties.BeepOnError = false;
            this.editHeight.Properties.EditValueChangedDelay = 1000;
            this.editHeight.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.editHeight.Properties.MaskSettings.Set("mask", "d");
            this.editHeight.Size = new System.Drawing.Size(30, 20);
            this.editHeight.TabIndex = 4;
            this.editHeight.ToolTip = "Height of the CustomRegion";
            this.editHeight.EditValueChanged += new System.EventHandler(this.handled_HeightChanged);
            this.editHeight.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.handler_SizeChanging);
            // 
            // lblWidth
            // 
            this.lblWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(246, 38);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(39, 13);
            this.lblWidth.TabIndex = 3;
            this.lblWidth.Text = "Width:";
            // 
            // lblHeight
            // 
            this.lblHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(165, 38);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(42, 13);
            this.lblHeight.TabIndex = 3;
            this.lblHeight.Text = "Height:";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(83, 38);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(17, 13);
            this.lblY.TabIndex = 3;
            this.lblY.Text = "Y:";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(7, 38);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(17, 13);
            this.lblX.TabIndex = 3;
            this.lblX.Text = "X:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblName.Location = new System.Drawing.Point(4, 7);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(42, 13);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "Name:";
            // 
            // CustomRegionToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 60);
            this.ControlBox = false;
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.editHeight);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.editWidth);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.editY);
            this.Controls.Add(this.lblY);
            this.Controls.Add(this.editX);
            this.Controls.Add(this.lblX);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCRTypeSwitcher);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.CustomRegionSelector);
            this.Controls.Add(this.editCRName);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.None;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(382, 92);
            this.Name = "CustomRegionToolForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CustomRegionTool";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.handler_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.editCRName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editX.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editY.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editWidth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editHeight.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox CustomRegionSelector;
        private DevExpress.XtraEditors.SimpleButton btnCRTypeSwitcher;
        private DevExpress.XtraEditors.SimpleButton btnAccept;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.TextEdit editCRName;
        private DevExpress.XtraEditors.TextEdit editX;
        private DevExpress.XtraEditors.TextEdit editY;
        private DevExpress.XtraEditors.TextEdit editWidth;
        private DevExpress.XtraEditors.TextEdit editHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblName;
    }
}