namespace EntityTools.Forms
{
    partial class CustomRegionCollectionEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomRegionCollectionEditorForm));
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
            this.btnItemSelection = new DevExpress.XtraEditors.DropDownButton();
            this.btnOpenMapper = new DevExpress.XtraEditors.SimpleButton();
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
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.ImageOptions.Image")));
            this.btnSelect.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSelect.ImageOptions.ImageToTextIndent = 6;
            this.btnSelect.Location = new System.Drawing.Point(256, 374);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSelect.Size = new System.Drawing.Size(70, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.handler_Select);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.ImageOptions.Image = global::EntityTools.Properties.Resources.miniRefresh;
            this.btnReload.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnReload.Location = new System.Drawing.Point(88, 374);
            this.btnReload.Name = "btnReload";
            this.btnReload.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnReload.Size = new System.Drawing.Size(70, 23);
            this.btnReload.TabIndex = 1;
            this.btnReload.Text = "Reload";
            this.btnReload.ToolTip = "Reload";
            this.btnReload.Click += new System.EventHandler(this.handler_Reload);
            // 
            // crList
            // 
            this.crList.CheckOnClick = true;
            this.crList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crList.Location = new System.Drawing.Point(0, 0);
            this.crList.MultiColumn = true;
            this.crList.Name = "crList";
            this.crList.Size = new System.Drawing.Size(314, 248);
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
            this.spliter.Size = new System.Drawing.Size(314, 350);
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
            this.tabPane.RegularSize = new System.Drawing.Size(314, 98);
            this.tabPane.SelectedPage = this.tabUnion;
            this.tabPane.Size = new System.Drawing.Size(314, 98);
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
            this.tabUnion.Size = new System.Drawing.Size(314, 71);
            // 
            // textUnion
            // 
            this.textUnion.Appearance.BackColor = System.Drawing.SystemColors.Window;
            this.textUnion.Appearance.Options.UseBackColor = true;
            this.textUnion.Appearance.Options.UseTextOptions = true;
            this.textUnion.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.textUnion.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.textUnion.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.textUnion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textUnion.Location = new System.Drawing.Point(0, 0);
            this.textUnion.Name = "textUnion";
            this.textUnion.Size = new System.Drawing.Size(314, 71);
            this.textUnion.TabIndex = 8;
            this.textUnion.Text = resources.GetString("textUnion.Text");
            // 
            // tabIntersection
            // 
            this.tabIntersection.Caption = "INTERSECTION";
            this.tabIntersection.Controls.Add(this.textIntersection);
            this.tabIntersection.Name = "tabIntersection";
            this.tabIntersection.Size = new System.Drawing.Size(314, 72);
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
            this.textIntersection.Size = new System.Drawing.Size(314, 72);
            this.textIntersection.TabIndex = 7;
            this.textIntersection.Text = resources.GetString("textIntersection.Text");
            // 
            // tabExclusion
            // 
            this.tabExclusion.Caption = "EXCLUSION";
            this.tabExclusion.Controls.Add(this.textExclusion);
            this.tabExclusion.Name = "tabExclusion";
            this.tabExclusion.Size = new System.Drawing.Size(314, 72);
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
            this.textExclusion.Size = new System.Drawing.Size(314, 72);
            this.textExclusion.TabIndex = 6;
            this.textExclusion.Text = "Отметьте несколько регионов, которые будут ИСКЛЮЧЕНЫ из итоговой области. Персона" +
    "жу запрещено находиться в любом из отмеченных регионов.";
            // 
            // btnItemSelection
            // 
            this.btnItemSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnItemSelection.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
            this.btnItemSelection.Enabled = false;
            this.btnItemSelection.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemSelection.ImageOptions.Image")));
            this.btnItemSelection.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnItemSelection.ImageOptions.ImageToTextIndent = 6;
            this.btnItemSelection.Location = new System.Drawing.Point(164, 374);
            this.btnItemSelection.Name = "btnItemSelection";
            this.btnItemSelection.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnItemSelection.Size = new System.Drawing.Size(37, 23);
            this.btnItemSelection.TabIndex = 9;
            this.btnItemSelection.Text = "Selection...";
            this.btnItemSelection.Visible = false;
            // 
            // btnOpenMapper
            // 
            this.btnOpenMapper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenMapper.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenMapper.ImageOptions.Image")));
            this.btnOpenMapper.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnOpenMapper.Location = new System.Drawing.Point(12, 374);
            this.btnOpenMapper.Name = "btnOpenMapper";
            this.btnOpenMapper.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnOpenMapper.Size = new System.Drawing.Size(70, 23);
            this.btnOpenMapper.TabIndex = 1;
            this.btnOpenMapper.Text = "Mapper";
            this.btnOpenMapper.ToolTip = "Open Mapper";
            this.btnOpenMapper.Click += new System.EventHandler(this.handler_Mapper);
            // 
            // CustomRegionCollectionEditorForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 409);
            this.Controls.Add(this.btnItemSelection);
            this.Controls.Add(this.spliter);
            this.Controls.Add(this.btnOpenMapper);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HelpButton = true;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(318, 419);
            this.Name = "CustomRegionCollectionEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Shown += new System.EventHandler(this.handler_FormShown);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.handler_HelpRequested);
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
        private DevExpress.XtraEditors.DropDownButton btnItemSelection;
        internal DevExpress.XtraEditors.SimpleButton btnOpenMapper;
    }
}