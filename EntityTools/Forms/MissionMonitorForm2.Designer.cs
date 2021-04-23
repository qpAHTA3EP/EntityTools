namespace EntityTools.Forms
{
    partial class MissionMonitorForm2
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
            this.btnSelectMission = new DevExpress.XtraEditors.SimpleButton();
            this.btnExpand = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
            this.listMissions = new DevExpress.XtraTreeList.TreeList();
            this.splitContainerDetail = new DevExpress.XtraEditors.SplitContainerControl();
            this.listMissionDef = new DevExpress.XtraTreeList.TreeList();
            this.clmnMissionName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissionNameOverride = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnState = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listMissions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail.Panel1)).BeginInit();
            this.splitContainerDetail.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail.Panel2)).BeginInit();
            this.splitContainerDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listMissionDef)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectMission
            // 
            this.btnSelectMission.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectMission.ImageOptions.Image = global::EntityTools.Properties.Resources.miniPen;
            this.btnSelectMission.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSelectMission.ImageOptions.ImageToTextIndent = 10;
            this.btnSelectMission.Location = new System.Drawing.Point(12, 333);
            this.btnSelectMission.Name = "btnSelectMission";
            this.btnSelectMission.Size = new System.Drawing.Size(75, 23);
            this.btnSelectMission.TabIndex = 2;
            this.btnSelectMission.Text = "Select";
            this.btnSelectMission.Click += new System.EventHandler(this.handler_SelectMission);
            // 
            // btnExpand
            // 
            this.btnExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExpand.Location = new System.Drawing.Point(93, 333);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(75, 23);
            this.btnExpand.TabIndex = 2;
            this.btnExpand.Text = "Expand";
            // 
            // simpleButton1
            // 
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.simpleButton1.ImageOptions.Image = global::EntityTools.Properties.Resources.miniRefresh;
            this.simpleButton1.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.simpleButton1.Location = new System.Drawing.Point(511, 333);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 2;
            this.simpleButton1.Text = "Refresh";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
            this.splitContainer.Horizontal = false;
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listMissions);
            this.splitContainer.Panel1.Text = "Panel1";
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.splitContainerDetail);
            this.splitContainer.Panel2.Text = "Panel2";
            this.splitContainer.Size = new System.Drawing.Size(574, 309);
            this.splitContainer.SplitterPosition = 130;
            this.splitContainer.TabIndex = 3;
            // 
            // listMissions
            // 
            this.listMissions.ChildListFieldName = "Childrens";
            this.listMissions.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.clmnMissionName,
            this.clmnMissionNameOverride,
            this.clmnState});
            this.listMissions.CustomizationFormBounds = new System.Drawing.Rectangle(382, 26, 252, 266);
            this.listMissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMissions.KeyFieldName = "MissionName";
            this.listMissions.Location = new System.Drawing.Point(0, 0);
            this.listMissions.Name = "listMissions";
            this.listMissions.ParentFieldName = "MainMissionName";
            this.listMissions.PreviewFieldName = "MissionName";
            this.listMissions.Size = new System.Drawing.Size(574, 130);
            this.listMissions.TabIndex = 4;
            this.listMissions.TreeViewFieldName = "MissionName";
            // 
            // splitContainerDetail
            // 
            this.splitContainerDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDetail.Horizontal = false;
            this.splitContainerDetail.Location = new System.Drawing.Point(0, 0);
            this.splitContainerDetail.Name = "splitContainerDetail";
            // 
            // splitContainerDetail.Panel1
            // 
            this.splitContainerDetail.Panel1.Controls.Add(this.listMissionDef);
            this.splitContainerDetail.Panel1.Text = "Panel1";
            // 
            // splitContainerDetail.Panel2
            // 
            this.splitContainerDetail.Panel2.Text = "Panel2";
            this.splitContainerDetail.Size = new System.Drawing.Size(574, 169);
            this.splitContainerDetail.SplitterPosition = 101;
            this.splitContainerDetail.TabIndex = 0;
            // 
            // listMissionDef
            // 
            this.listMissionDef.ChildListFieldName = "SubMissions";
            this.listMissionDef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMissionDef.KeyFieldName = "Name";
            this.listMissionDef.Location = new System.Drawing.Point(0, 0);
            this.listMissionDef.Name = "listMissionDef";
            this.listMissionDef.ParentFieldName = "MainName";
            this.listMissionDef.PreviewFieldName = "Name";
            this.listMissionDef.Size = new System.Drawing.Size(574, 101);
            this.listMissionDef.TabIndex = 0;
            // 
            // clmnMissionName
            // 
            this.clmnMissionName.Caption = "MissionName";
            this.clmnMissionName.FieldName = "MissionName";
            this.clmnMissionName.Name = "clmnMissionName";
            this.clmnMissionName.OptionsColumn.AllowEdit = false;
            this.clmnMissionName.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
            this.clmnMissionName.Visible = true;
            this.clmnMissionName.VisibleIndex = 0;
            // 
            // clmnMissionNameOverride
            // 
            this.clmnMissionNameOverride.Caption = "MissionNameOverride";
            this.clmnMissionNameOverride.FieldName = "MissionNameOverride";
            this.clmnMissionNameOverride.Name = "clmnMissionNameOverride";
            this.clmnMissionNameOverride.OptionsColumn.AllowEdit = false;
            this.clmnMissionNameOverride.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
            this.clmnMissionNameOverride.Visible = true;
            this.clmnMissionNameOverride.VisibleIndex = 1;
            // 
            // clmnState
            // 
            this.clmnState.Caption = "State";
            this.clmnState.FieldName = "State";
            this.clmnState.Name = "clmnState";
            this.clmnState.OptionsColumn.AllowEdit = false;
            this.clmnState.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.clmnState.Visible = true;
            this.clmnState.VisibleIndex = 2;
            // 
            // MissionMonitorForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 368);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.btnSelectMission);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "MissionMonitorForm2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MissionMonitorForm";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).EndInit();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listMissions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail.Panel1)).EndInit();
            this.splitContainerDetail.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail.Panel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail)).EndInit();
            this.splitContainerDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listMissionDef)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnSelectMission;
        private DevExpress.XtraEditors.SimpleButton btnExpand;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainer;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerDetail;
        private DevExpress.XtraTreeList.TreeList listMissions;
        private DevExpress.XtraTreeList.TreeList listMissionDef;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissionName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissionNameOverride;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnState;
    }
}