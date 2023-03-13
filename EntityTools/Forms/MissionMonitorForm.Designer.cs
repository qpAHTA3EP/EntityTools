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
            this.components = new System.ComponentModel.Container();
            this.btnSelectMission = new DevExpress.XtraEditors.SimpleButton();
            this.btnStop = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
            this.treeMissions = new System.Windows.Forms.TreeView();
            this.txtMissionLog = new System.Windows.Forms.TextBox();
            this.tabControl = new DevExpress.XtraTab.XtraTabControl();
            this.tabMission = new DevExpress.XtraTab.XtraTabPage();
            this.tabOption = new DevExpress.XtraTab.XtraTabPage();
            this.pgSettings = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.trackMonitoring = new DevExpress.XtraEditors.TrackBarControl();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabMission.SuspendLayout();
            this.tabOption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pgSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackMonitoring)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackMonitoring.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectMission
            // 
            this.btnSelectMission.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectMission.ImageOptions.Image = global::EntityTools.Properties.Resources.Quest;
            this.btnSelectMission.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSelectMission.ImageOptions.ImageToTextIndent = 6;
            this.btnSelectMission.Location = new System.Drawing.Point(525, 333);
            this.btnSelectMission.Name = "btnSelectMission";
            this.btnSelectMission.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSelectMission.Size = new System.Drawing.Size(60, 23);
            this.btnSelectMission.TabIndex = 0;
            this.btnSelectMission.Text = "Select";
            this.btnSelectMission.Click += new System.EventHandler(this.handler_SelectMission);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.ImageOptions.Image = global::EntityTools.Properties.Resources.Stop;
            this.btnStop.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnStop.Location = new System.Drawing.Point(459, 333);
            this.btnStop.Name = "btnStop";
            this.btnStop.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnStop.Size = new System.Drawing.Size(60, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.handler_Stop);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.ImageOptions.Image = global::EntityTools.Properties.Resources.Save;
            this.btnSave.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSave.Location = new System.Drawing.Point(393, 333);
            this.btnSave.Name = "btnSave";
            this.btnSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.handler_Save);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
            this.splitContainer.Horizontal = false;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeMissions);
            this.splitContainer.Panel1.Text = "Panel1";
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.txtMissionLog);
            this.splitContainer.Panel2.Text = "Panel2";
            this.splitContainer.Size = new System.Drawing.Size(548, 307);
            this.splitContainer.SplitterPosition = 240;
            this.splitContainer.TabIndex = 3;
            // 
            // treeMissions
            // 
            this.treeMissions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeMissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMissions.HideSelection = false;
            this.treeMissions.Location = new System.Drawing.Point(0, 0);
            this.treeMissions.Name = "treeMissions";
            this.treeMissions.Size = new System.Drawing.Size(548, 241);
            this.treeMissions.TabIndex = 0;
            // 
            // txtMissionLog
            // 
            this.txtMissionLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMissionLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMissionLog.Location = new System.Drawing.Point(0, 0);
            this.txtMissionLog.Multiline = true;
            this.txtMissionLog.Name = "txtMissionLog";
            this.txtMissionLog.ReadOnly = true;
            this.txtMissionLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMissionLog.Size = new System.Drawing.Size(548, 56);
            this.txtMissionLog.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedTabPage = this.tabMission;
            this.tabControl.Size = new System.Drawing.Size(574, 309);
            this.tabControl.TabIndex = 5;
            this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabMission,
            this.tabOption});
            // 
            // tabMission
            // 
            this.tabMission.Controls.Add(this.splitContainer);
            this.tabMission.Name = "tabMission";
            this.tabMission.Size = new System.Drawing.Size(548, 307);
            this.tabMission.Text = "Mission";
            // 
            // tabOption
            // 
            this.tabOption.Controls.Add(this.pgSettings);
            this.tabOption.Name = "tabOption";
            this.tabOption.Size = new System.Drawing.Size(548, 307);
            this.tabOption.Text = "Option";
            // 
            // pgSettings
            // 
            this.pgSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pgSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSettings.Location = new System.Drawing.Point(0, 0);
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            this.pgSettings.Size = new System.Drawing.Size(548, 307);
            this.pgSettings.TabIndex = 0;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Monitoring);
            // 
            // trackMonitoring
            // 
            this.trackMonitoring.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackMonitoring.EditValue = null;
            this.trackMonitoring.Location = new System.Drawing.Point(37, 333);
            this.trackMonitoring.Name = "trackMonitoring";
            this.trackMonitoring.Properties.AllowMouseWheel = false;
            this.trackMonitoring.Properties.LabelAppearance.Options.UseTextOptions = true;
            this.trackMonitoring.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.trackMonitoring.Properties.Maximum = 1;
            this.trackMonitoring.Size = new System.Drawing.Size(350, 45);
            this.trackMonitoring.TabIndex = 6;
            this.trackMonitoring.Visible = false;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            this.saveFileDialog.Title = "Save mission monitor log";
            // 
            // MissionMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 368);
            this.Controls.Add(this.trackMonitoring);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnSelectMission);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "MissionMonitorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mission Monitor";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).EndInit();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabMission.ResumeLayout(false);
            this.tabOption.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pgSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackMonitoring.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackMonitoring)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnSelectMission;
        private DevExpress.XtraEditors.SimpleButton btnStop;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SplitContainerControl splitContainer;
        private System.Windows.Forms.TreeView treeMissions;
        private DevExpress.XtraTab.XtraTabControl tabControl;
        private DevExpress.XtraTab.XtraTabPage tabMission;
        private DevExpress.XtraTab.XtraTabPage tabOption;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.TextBox txtMissionLog;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private DevExpress.XtraVerticalGrid.PropertyGridControl pgSettings;
        private DevExpress.XtraEditors.TrackBarControl trackMonitoring;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}