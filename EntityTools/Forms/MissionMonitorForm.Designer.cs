namespace EntityTools.Forms
{
    partial class MissionMonitorForm
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
            this.treeViewMissions = new System.Windows.Forms.TreeView();
            this.splitContainerDetail = new DevExpress.XtraEditors.SplitContainerControl();
            this.pgDetail = new System.Windows.Forms.PropertyGrid();
            this.pgDetail2 = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail.Panel1)).BeginInit();
            this.splitContainerDetail.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail.Panel2)).BeginInit();
            this.splitContainerDetail.Panel2.SuspendLayout();
            this.splitContainerDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pgDetail2)).BeginInit();
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
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeViewMissions);
            this.splitContainer.Panel1.Text = "Panel1";
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.splitContainerDetail);
            this.splitContainer.Panel2.Text = "Panel2";
            this.splitContainer.Size = new System.Drawing.Size(574, 309);
            this.splitContainer.SplitterPosition = 281;
            this.splitContainer.TabIndex = 3;
            // 
            // treeViewMissions
            // 
            this.treeViewMissions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewMissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewMissions.HideSelection = false;
            this.treeViewMissions.Location = new System.Drawing.Point(0, 0);
            this.treeViewMissions.Name = "treeViewMissions";
            this.treeViewMissions.Size = new System.Drawing.Size(281, 309);
            this.treeViewMissions.TabIndex = 0;
            this.treeViewMissions.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.handler_ExpandNode);
            this.treeViewMissions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.handler_SelectNode);
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
            this.splitContainerDetail.Panel1.Controls.Add(this.pgDetail);
            this.splitContainerDetail.Panel1.Text = "Panel1";
            // 
            // splitContainerDetail.Panel2
            // 
            this.splitContainerDetail.Panel2.Controls.Add(this.pgDetail2);
            this.splitContainerDetail.Panel2.Text = "Panel2";
            this.splitContainerDetail.Size = new System.Drawing.Size(283, 309);
            this.splitContainerDetail.SplitterPosition = 165;
            this.splitContainerDetail.TabIndex = 0;
            // 
            // pgDetail
            // 
            this.pgDetail.CommandsVisibleIfAvailable = false;
            this.pgDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgDetail.HelpVisible = false;
            this.pgDetail.Location = new System.Drawing.Point(0, 0);
            this.pgDetail.Name = "pgDetail";
            this.pgDetail.Size = new System.Drawing.Size(283, 165);
            this.pgDetail.TabIndex = 0;
            this.pgDetail.ToolbarVisible = false;
            // 
            // pgDetail2
            // 
            this.pgDetail2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pgDetail2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgDetail2.Location = new System.Drawing.Point(0, 0);
            this.pgDetail2.Name = "pgDetail2";
            this.pgDetail2.Size = new System.Drawing.Size(283, 134);
            this.pgDetail2.TabIndex = 0;
            // 
            // MissionMonitorForm
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
            this.Name = "MissionMonitorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MissionMonitorForm";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).EndInit();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail.Panel1)).EndInit();
            this.splitContainerDetail.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail.Panel2)).EndInit();
            this.splitContainerDetail.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDetail)).EndInit();
            this.splitContainerDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pgDetail2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnSelectMission;
        private DevExpress.XtraEditors.SimpleButton btnExpand;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainer;
        private System.Windows.Forms.TreeView treeViewMissions;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerDetail;
        private System.Windows.Forms.PropertyGrid pgDetail;
        private DevExpress.XtraVerticalGrid.PropertyGridControl pgDetail2;
    }
}