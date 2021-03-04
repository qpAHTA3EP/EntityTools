namespace EntityTools.Forms
{
    partial class CustomRegionCollectionEditorForm_Plus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomRegionCollectionEditorForm_Plus));
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            this.crList = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.spliter = new System.Windows.Forms.SplitContainer();
            this.tabPane = new DevExpress.XtraBars.Navigation.TabPane();
            this.tabUnion = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.textUnion = new DevExpress.XtraEditors.LabelControl();
            this.tabIntersection = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.textIntersection = new DevExpress.XtraEditors.LabelControl();
            this.tabExclusion = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.textExclusion = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.crList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spliter)).BeginInit();
            this.spliter.Panel1.SuspendLayout();
            this.spliter.Panel2.SuspendLayout();
            this.spliter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabPane)).BeginInit();
            this.tabPane.SuspendLayout();
            this.tabUnion.SuspendLayout();
            this.tabIntersection.SuspendLayout();
            this.tabExclusion.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnSelect.ImageOptions.Image = global::EntityTools.Properties.Resources.miniValid;
            this.btnSelect.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSelect.Location = new System.Drawing.Point(12, 367);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(271, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.handler_Select);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnReload.ImageOptions.Image = global::EntityTools.Properties.Resources.miniRefresh;
            this.btnReload.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnReload.Location = new System.Drawing.Point(289, 367);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(23, 23);
            this.btnReload.TabIndex = 1;
            this.btnReload.ToolTip = "Reload";
            this.btnReload.Click += new System.EventHandler(this.handler_Reload);
            // 
            // crList
            // 
            this.crList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.crList.CheckOnClick = true;
            this.crList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crList.Location = new System.Drawing.Point(0, 0);
            this.crList.MultiColumn = true;
            this.crList.Name = "crList";
            this.crList.Size = new System.Drawing.Size(300, 247);
            this.crList.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.crList.TabIndex = 5;
            // 
            // spliter
            // 
            this.spliter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spliter.Location = new System.Drawing.Point(12, 12);
            this.spliter.Name = "spliter";
            this.spliter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spliter.Panel1
            // 
            this.spliter.Panel1.Controls.Add(this.tabPane);
            // 
            // spliter.Panel2
            // 
            this.spliter.Panel2.Controls.Add(this.crList);
            this.spliter.Size = new System.Drawing.Size(300, 349);
            this.spliter.SplitterDistance = 98;
            this.spliter.TabIndex = 8;
            // 
            // tabPane
            // 
            this.tabPane.Appearance.BackColor = System.Drawing.SystemColors.Window;
            this.tabPane.Appearance.Options.UseBackColor = true;
            this.tabPane.Controls.Add(this.tabUnion);
            this.tabPane.Controls.Add(this.tabIntersection);
            this.tabPane.Controls.Add(this.tabExclusion);
            this.tabPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPane.Location = new System.Drawing.Point(0, 0);
            this.tabPane.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.tabPane.LookAndFeel.UseDefaultLookAndFeel = false;
            this.tabPane.Name = "tabPane";
            this.tabPane.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.tabUnion,
            this.tabIntersection,
            this.tabExclusion});
            this.tabPane.RegularSize = new System.Drawing.Size(300, 98);
            this.tabPane.SelectedPage = this.tabUnion;
            this.tabPane.Size = new System.Drawing.Size(300, 98);
            this.tabPane.TabAlignment = DevExpress.XtraEditors.Alignment.Center;
            this.tabPane.TabIndex = 8;
            this.tabPane.Text = "tabPane";
            this.tabPane.SelectedPageChanging += new DevExpress.XtraBars.Navigation.SelectedPageChangingEventHandler(this.handler_SelectedPageChanging);
            // 
            // tabUnion
            // 
            this.tabUnion.Caption = "UNION";
            this.tabUnion.Controls.Add(this.textUnion);
            this.tabUnion.Name = "tabUnion";
            this.tabUnion.Size = new System.Drawing.Size(300, 71);
            // 
            // textUnion
            // 
            this.textUnion.Appearance.Options.UseTextOptions = true;
            this.textUnion.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.textUnion.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.textUnion.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.textUnion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textUnion.Location = new System.Drawing.Point(0, 0);
            this.textUnion.Name = "textUnion";
            this.textUnion.Size = new System.Drawing.Size(300, 71);
            this.textUnion.TabIndex = 8;
            this.textUnion.Text = resources.GetString("textUnion.Text");
            // 
            // tabIntersection
            // 
            this.tabIntersection.Caption = "INTERSECTION";
            this.tabIntersection.Controls.Add(this.textIntersection);
            this.tabIntersection.Name = "tabIntersection";
            this.tabIntersection.Size = new System.Drawing.Size(300, 71);
            // 
            // textIntersection
            // 
            this.textIntersection.Appearance.Options.UseTextOptions = true;
            this.textIntersection.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.textIntersection.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.textIntersection.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.textIntersection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textIntersection.Location = new System.Drawing.Point(0, 0);
            this.textIntersection.Name = "textIntersection";
            this.textIntersection.Size = new System.Drawing.Size(300, 71);
            this.textIntersection.TabIndex = 7;
            this.textIntersection.Text = resources.GetString("textIntersection.Text");
            // 
            // tabExclusion
            // 
            this.tabExclusion.Caption = "EXCLUSION";
            this.tabExclusion.Controls.Add(this.textExclusion);
            this.tabExclusion.Name = "tabExclusion";
            this.tabExclusion.Size = new System.Drawing.Size(300, 71);
            // 
            // textExclusion
            // 
            this.textExclusion.Appearance.Options.UseTextOptions = true;
            this.textExclusion.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.textExclusion.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.textExclusion.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.textExclusion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textExclusion.Location = new System.Drawing.Point(0, 0);
            this.textExclusion.Name = "textExclusion";
            this.textExclusion.Size = new System.Drawing.Size(300, 71);
            this.textExclusion.TabIndex = 6;
            this.textExclusion.Text = "Отметьте несколько регионов, которые будут ИСКЛЮЧЕНЫ из итоговой области. Персона" +
    "жу запрещено находиться в любом из отмеченных регионов.";
            // 
            // CustomRegionCollectionEditorForm_Plus
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 402);
            this.Controls.Add(this.spliter);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(332, 426);
            this.Name = "CustomRegionCollectionEditorForm_Plus";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Shown += new System.EventHandler(this.handler_FormShown);
            ((System.ComponentModel.ISupportInitialize)(this.crList)).EndInit();
            this.spliter.Panel1.ResumeLayout(false);
            this.spliter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spliter)).EndInit();
            this.spliter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabPane)).EndInit();
            this.tabPane.ResumeLayout(false);
            this.tabUnion.ResumeLayout(false);
            this.tabIntersection.ResumeLayout(false);
            this.tabExclusion.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        internal DevExpress.XtraEditors.SimpleButton btnSelect;
        internal DevExpress.XtraEditors.SimpleButton btnReload;
        private DevExpress.XtraEditors.CheckedListBoxControl crList;
        private System.Windows.Forms.SplitContainer spliter;
        private DevExpress.XtraBars.Navigation.TabPane tabPane;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabUnion;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabIntersection;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabExclusion;
        private DevExpress.XtraEditors.LabelControl textUnion;
        private DevExpress.XtraEditors.LabelControl textIntersection;
        private DevExpress.XtraEditors.LabelControl textExclusion;
    }
}