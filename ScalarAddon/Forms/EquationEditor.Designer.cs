﻿namespace AstralVariables.Forms
{
    partial class EquationEditor
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
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tbExpression = new System.Windows.Forms.TextBox();
            this.btnParse = new System.Windows.Forms.Button();
            this.dtnInsert = new DevExpress.XtraEditors.DropDownButton();
            this.popInsert = new DevExpress.XtraBars.PopupMenu(this.components);
            this.btnInsVarible = new DevExpress.XtraBars.BarButtonItem();
            this.btnInsItmCnt = new DevExpress.XtraBars.BarButtonItem();
            this.btnInsNumCnt = new DevExpress.XtraBars.BarButtonItem();
            this.btnInsRnd = new DevExpress.XtraBars.BarButtonItem();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.popInsert)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(360, 217);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(294, 217);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tbExpression
            // 
            this.tbExpression.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExpression.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbExpression.Location = new System.Drawing.Point(12, 12);
            this.tbExpression.Multiline = true;
            this.tbExpression.Name = "tbExpression";
            this.tbExpression.Size = new System.Drawing.Size(408, 189);
            this.tbExpression.TabIndex = 5;
            // 
            // btnParse
            // 
            this.btnParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnParse.Location = new System.Drawing.Point(153, 217);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(60, 23);
            this.btnParse.TabIndex = 3;
            this.btnParse.Text = "Parse";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // dtnInsert
            // 
            this.dtnInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtnInsert.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.dtnInsert.Appearance.BackColor2 = System.Drawing.SystemColors.Control;
            this.dtnInsert.Appearance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dtnInsert.Appearance.Options.UseBackColor = true;
            this.dtnInsert.Appearance.Options.UseForeColor = true;
            this.dtnInsert.AppearanceDropDown.BackColor = System.Drawing.SystemColors.Control;
            this.dtnInsert.AppearanceDropDown.BackColor2 = System.Drawing.SystemColors.Control;
            this.dtnInsert.AppearanceDropDown.Options.UseBackColor = true;
            this.dtnInsert.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.dtnInsert.DropDownControl = this.popInsert;
            this.dtnInsert.Location = new System.Drawing.Point(12, 217);
            this.dtnInsert.LookAndFeel.SkinMaskColor = System.Drawing.SystemColors.Control;
            this.dtnInsert.LookAndFeel.SkinMaskColor2 = System.Drawing.SystemColors.Control;
            this.dtnInsert.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.dtnInsert.LookAndFeel.UseWindowsXPTheme = true;
            this.dtnInsert.MenuManager = this.barManager;
            this.dtnInsert.Name = "dtnInsert";
            this.barManager.SetPopupContextMenu(this.dtnInsert, this.popInsert);
            this.dtnInsert.Size = new System.Drawing.Size(135, 23);
            this.dtnInsert.TabIndex = 6;
            this.dtnInsert.Text = "Insert";
            // 
            // popInsert
            // 
            this.popInsert.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnInsVarible),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnInsItmCnt),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnInsNumCnt),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnInsRnd)});
            this.popInsert.Manager = this.barManager;
            this.popInsert.MenuAppearance.AppearanceMenu.Normal.BackColor = System.Drawing.SystemColors.Control;
            this.popInsert.MenuAppearance.AppearanceMenu.Normal.BackColor2 = System.Drawing.SystemColors.Control;
            this.popInsert.MenuAppearance.AppearanceMenu.Normal.Options.UseBackColor = true;
            this.popInsert.Name = "popInsert";
            // 
            // btnInsVarible
            // 
            this.btnInsVarible.AllowDrawArrow = false;
            this.btnInsVarible.AllowDrawArrowInMenu = false;
            this.btnInsVarible.Caption = "Variable";
            this.btnInsVarible.Id = 0;
            this.btnInsVarible.ItemInMenuAppearance.Disabled.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsVarible.ItemInMenuAppearance.Disabled.Options.UseBackColor = true;
            this.btnInsVarible.ItemInMenuAppearance.Normal.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsVarible.ItemInMenuAppearance.Normal.Options.UseBackColor = true;
            this.btnInsVarible.ItemInMenuAppearance.Pressed.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsVarible.ItemInMenuAppearance.Pressed.Options.UseBackColor = true;
            this.btnInsVarible.Name = "btnInsVarible";
            this.btnInsVarible.RememberLastCommand = true;
            this.btnInsVarible.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnInsVarible_ItemClick);
            // 
            // btnInsItmCnt
            // 
            this.btnInsItmCnt.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.btnInsItmCnt.AllowDrawArrow = false;
            this.btnInsItmCnt.AllowDrawArrowInMenu = false;
            this.btnInsItmCnt.Caption = "ItemCount(...)";
            this.btnInsItmCnt.Id = 1;
            this.btnInsItmCnt.ItemAppearance.Normal.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsItmCnt.ItemAppearance.Normal.Options.UseBackColor = true;
            this.btnInsItmCnt.ItemInMenuAppearance.Disabled.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsItmCnt.ItemInMenuAppearance.Disabled.Options.UseBackColor = true;
            this.btnInsItmCnt.ItemInMenuAppearance.Normal.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsItmCnt.ItemInMenuAppearance.Normal.Options.UseBackColor = true;
            this.btnInsItmCnt.ItemInMenuAppearance.Pressed.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsItmCnt.ItemInMenuAppearance.Pressed.Options.UseBackColor = true;
            this.btnInsItmCnt.Name = "btnInsItmCnt";
            this.btnInsItmCnt.RememberLastCommand = true;
            this.btnInsItmCnt.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnInsItmCnt_ItemClick);
            // 
            // btnInsNumCnt
            // 
            this.btnInsNumCnt.AllowDrawArrow = false;
            this.btnInsNumCnt.AllowDrawArrowInMenu = false;
            this.btnInsNumCnt.Caption = "NumericCount(...)";
            this.btnInsNumCnt.Id = 2;
            this.btnInsNumCnt.ItemInMenuAppearance.Disabled.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsNumCnt.ItemInMenuAppearance.Disabled.Options.UseBackColor = true;
            this.btnInsNumCnt.ItemInMenuAppearance.Normal.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsNumCnt.ItemInMenuAppearance.Normal.Options.UseBackColor = true;
            this.btnInsNumCnt.ItemInMenuAppearance.Pressed.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsNumCnt.ItemInMenuAppearance.Pressed.Options.UseBackColor = true;
            this.btnInsNumCnt.Name = "btnInsNumCnt";
            this.btnInsNumCnt.RememberLastCommand = true;
            this.btnInsNumCnt.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnInsNumCnt_ItemClick);
            // 
            // btnInsRnd
            // 
            this.btnInsRnd.AllowDrawArrow = false;
            this.btnInsRnd.AllowDrawArrowInMenu = false;
            this.btnInsRnd.AllowRightClickInMenu = false;
            this.btnInsRnd.Caption = "Random()";
            this.btnInsRnd.Id = 3;
            this.btnInsRnd.ItemInMenuAppearance.Disabled.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsRnd.ItemInMenuAppearance.Disabled.Options.UseBackColor = true;
            this.btnInsRnd.ItemInMenuAppearance.Normal.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsRnd.ItemInMenuAppearance.Normal.Options.UseBackColor = true;
            this.btnInsRnd.ItemInMenuAppearance.Pressed.BackColor = System.Drawing.SystemColors.Control;
            this.btnInsRnd.ItemInMenuAppearance.Pressed.Options.UseBackColor = true;
            this.btnInsRnd.Name = "btnInsRnd";
            this.btnInsRnd.RememberLastCommand = true;
            this.btnInsRnd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnInsRnd_ItemClick);
            // 
            // barManager
            // 
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnInsVarible,
            this.btnInsItmCnt,
            this.btnInsNumCnt,
            this.btnInsRnd});
            this.barManager.MaxItemId = 4;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(432, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 253);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(432, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 253);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(432, 0);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 253);
            // 
            // EquationEditor
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(432, 253);
            this.ControlBox = false;
            this.Controls.Add(this.dtnInsert);
            this.Controls.Add(this.tbExpression);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.MinimumSize = new System.Drawing.Size(440, 150);
            this.Name = "EquationEditor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EquationEditor";
            ((System.ComponentModel.ISupportInitialize)(this.popInsert)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox tbExpression;
        private System.Windows.Forms.Button btnParse;
        private DevExpress.XtraEditors.DropDownButton dtnInsert;
        private DevExpress.XtraBars.PopupMenu popInsert;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.BarButtonItem btnInsVarible;
        private DevExpress.XtraBars.BarButtonItem btnInsItmCnt;
        private DevExpress.XtraBars.BarButtonItem btnInsNumCnt;
        private DevExpress.XtraBars.BarButtonItem btnInsRnd;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}