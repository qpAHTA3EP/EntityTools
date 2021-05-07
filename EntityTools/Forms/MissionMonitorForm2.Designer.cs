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
            this.clmnMissionName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissionDef = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnUIStringMsg = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnState = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.splitContainerDetail = new DevExpress.XtraEditors.SplitContainerControl();
            this.listMissionDef = new DevExpress.XtraTreeList.TreeList();
            this.clmnHiden = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissionNameOverride = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnRootDefOverride = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnStartTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnExpirationTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissDef_Name = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissDef_DisplayName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissDef_UIStringMsg = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissDef_Summary = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissDef_RelatedMission = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissDef_MissionType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.clmnMissDef_CanRepeat = new DevExpress.XtraTreeList.Columns.TreeListColumn();
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
            this.btnSelectMission.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnSelectMission.ImageOptions.Image = global::EntityTools.Properties.Resources.miniTarget;
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
            this.btnExpand.Visible = false;
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
            this.simpleButton1.Visible = false;
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
            this.clmnMissionDef,
            this.clmnUIStringMsg,
            this.clmnState,
            this.clmnHiden,
            this.clmnMissionNameOverride,
            this.clmnRootDefOverride,
            this.clmnStartTime,
            this.clmnExpirationTime});
            this.listMissions.CustomizationFormBounds = new System.Drawing.Rectangle(382, 26, 252, 266);
            this.listMissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMissions.EnableDynamicLoading = false;
            this.listMissions.KeyFieldName = "MissionName";
            this.listMissions.Location = new System.Drawing.Point(0, 0);
            this.listMissions.Name = "listMissions";
            this.listMissions.ParentFieldName = "";
            this.listMissions.Size = new System.Drawing.Size(574, 130);
            this.listMissions.TabIndex = 4;
            this.listMissions.AfterFocusNode += new DevExpress.XtraTreeList.NodeEventHandler(this.handler_AfterFocusNode);
            // 
            // clmnMissionName
            // 
            this.clmnMissionName.Caption = "Mission Name";
            this.clmnMissionName.FieldName = "MissionName";
            this.clmnMissionName.Name = "clmnMissionName";
            this.clmnMissionName.Visible = true;
            this.clmnMissionName.VisibleIndex = 0;
            // 
            // clmnMissionDef
            // 
            this.clmnMissionDef.Caption = "Mission Definition";
            this.clmnMissionDef.FieldName = "MissionDef";
            this.clmnMissionDef.Name = "clmnMissionDef";
            this.clmnMissionDef.Visible = true;
            this.clmnMissionDef.VisibleIndex = 1;
            // 
            // clmnUIStringMsg
            // 
            this.clmnUIStringMsg.Caption = "UI String";
            this.clmnUIStringMsg.FieldName = "UIStringMsg";
            this.clmnUIStringMsg.Name = "clmnUIStringMsg";
            // 
            // clmnState
            // 
            this.clmnState.Caption = "State";
            this.clmnState.FieldName = "State";
            this.clmnState.Name = "clmnState";
            this.clmnState.OptionsColumn.AllowEdit = false;
            this.clmnState.OptionsColumn.FixedWidth = true;
            this.clmnState.Visible = true;
            this.clmnState.VisibleIndex = 2;
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
            this.listMissionDef.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.clmnMissDef_Name,
            this.clmnMissDef_DisplayName,
            this.clmnMissDef_UIStringMsg,
            this.clmnMissDef_Summary,
            this.clmnMissDef_RelatedMission,
            this.clmnMissDef_MissionType,
            this.clmnMissDef_CanRepeat});
            this.listMissionDef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMissionDef.KeyFieldName = "Name";
            this.listMissionDef.Location = new System.Drawing.Point(0, 0);
            this.listMissionDef.Name = "listMissionDef";
            this.listMissionDef.ParentFieldName = "";
            this.listMissionDef.Size = new System.Drawing.Size(574, 101);
            this.listMissionDef.TabIndex = 0;
            // 
            // clmnHiden
            // 
            this.clmnHiden.Caption = "Hidden";
            this.clmnHiden.FieldName = "Hidden";
            this.clmnHiden.Name = "clmnHiden";
            // 
            // clmnMissionNameOverride
            // 
            this.clmnMissionNameOverride.Caption = "Mission Name Override";
            this.clmnMissionNameOverride.FieldName = "MissionNameOverride";
            this.clmnMissionNameOverride.Name = "clmnMissionNameOverride";
            // 
            // clmnRootDefOverride
            // 
            this.clmnRootDefOverride.Caption = "RootDefOverride";
            this.clmnRootDefOverride.FieldName = "RootDefOverride";
            this.clmnRootDefOverride.Name = "clmnRootDefOverride";
            // 
            // clmnStartTime
            // 
            this.clmnStartTime.Caption = "StartTime";
            this.clmnStartTime.FieldName = "StartTime";
            this.clmnStartTime.Name = "clmnStartTime";
            // 
            // clmnExpirationTime
            // 
            this.clmnExpirationTime.Caption = "ExpirationTime";
            this.clmnExpirationTime.FieldName = "ExpirationTime";
            this.clmnExpirationTime.Name = "clmnExpirationTime";
            // 
            // clmnMissDef_Name
            // 
            this.clmnMissDef_Name.Caption = "Name";
            this.clmnMissDef_Name.FieldName = "Name";
            this.clmnMissDef_Name.Name = "clmnMissDef_Name";
            this.clmnMissDef_Name.Visible = true;
            this.clmnMissDef_Name.VisibleIndex = 0;
            // 
            // clmnMissDef_DisplayName
            // 
            this.clmnMissDef_DisplayName.Caption = "Display Name";
            this.clmnMissDef_DisplayName.FieldName = "Display Name";
            this.clmnMissDef_DisplayName.Name = "clmnMissDef_DisplayName";
            this.clmnMissDef_DisplayName.Visible = true;
            this.clmnMissDef_DisplayName.VisibleIndex = 1;
            // 
            // clmnMissDef_UIStringMsg
            // 
            this.clmnMissDef_UIStringMsg.Caption = "UIString Msg";
            this.clmnMissDef_UIStringMsg.FieldName = "UIStringMsg";
            this.clmnMissDef_UIStringMsg.Name = "clmnMissDef_UIStringMsg";
            // 
            // clmnMissDef_Summary
            // 
            this.clmnMissDef_Summary.Caption = "Summary";
            this.clmnMissDef_Summary.FieldName = "Summary";
            this.clmnMissDef_Summary.Name = "clmnMissDef_Summary";
            // 
            // clmnMissDef_RelatedMission
            // 
            this.clmnMissDef_RelatedMission.Caption = "RelatedMission";
            this.clmnMissDef_RelatedMission.FieldName = "RelatedMission";
            this.clmnMissDef_RelatedMission.Name = "clmnMissDef_RelatedMission";
            // 
            // clmnMissDef_MissionType
            // 
            this.clmnMissDef_MissionType.Caption = "MissionType";
            this.clmnMissDef_MissionType.FieldName = "MissionType";
            this.clmnMissDef_MissionType.Name = "clmnMissDef_MissionType";
            // 
            // clmnMissDef_CanRepeat
            // 
            this.clmnMissDef_CanRepeat.Caption = "Repeatable";
            this.clmnMissDef_CanRepeat.FieldName = "CanRepeat";
            this.clmnMissDef_CanRepeat.Name = "clmnMissDef_CanRepeat";
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
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnUIStringMsg;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnState;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissionDef;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnHiden;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissionNameOverride;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnRootDefOverride;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnStartTime;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnExpirationTime;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissDef_Name;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissDef_DisplayName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissDef_UIStringMsg;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissDef_Summary;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissDef_RelatedMission;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissDef_MissionType;
        private DevExpress.XtraTreeList.Columns.TreeListColumn clmnMissDef_CanRepeat;
    }
}