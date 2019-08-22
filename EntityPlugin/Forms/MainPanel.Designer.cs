using EntityPlugin.Tools;
using System.IO;

namespace EntityPlugin.Forms
{
    partial class MainPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnEntities = new System.Windows.Forms.Button();
            this.ckbDebugInfo = new System.Windows.Forms.CheckBox();
            this.btnAuras = new System.Windows.Forms.Button();
            this.lblAuras = new System.Windows.Forms.Label();
            this.btnMissions = new System.Windows.Forms.Button();
            this.lblMissions = new System.Windows.Forms.Label();
            this.bteMissions = new DevExpress.XtraEditors.ButtonEdit();
            this.bteAuras = new DevExpress.XtraEditors.ButtonEdit();
            this.fldrBroserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.tbclMain = new DevExpress.XtraTab.XtraTabControl();
            this.tabOptions = new DevExpress.XtraTab.XtraTabPage();
            this.cbSlideMonitor = new System.Windows.Forms.CheckBox();
            this.ckbSpellStuckMonitor = new System.Windows.Forms.CheckBox();
            this.gbSlideMonitor = new System.Windows.Forms.GroupBox();
            this.tbSlidingAuras = new System.Windows.Forms.TextBox();
            this.seTimerSlide = new DevExpress.XtraEditors.SpinEdit();
            this.lblTimerSlide = new System.Windows.Forms.Label();
            this.lblSlideFilter = new System.Windows.Forms.Label();
            this.lblSlidingAuras = new System.Windows.Forms.Label();
            this.lblTimerUnslide = new System.Windows.Forms.Label();
            this.seTimerUnslide = new DevExpress.XtraEditors.SpinEdit();
            this.seSlideFilter = new DevExpress.XtraEditors.SpinEdit();
            this.lblSlideTimer = new System.Windows.Forms.Label();
            this.tabUtilities = new DevExpress.XtraTab.XtraTabPage();
            this.btnUiViewer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bteMissions.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteAuras.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).BeginInit();
            this.tbclMain.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.gbSlideMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seTimerSlide.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seTimerUnslide.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seSlideFilter.Properties)).BeginInit();
            this.tabUtilities.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEntities
            // 
            this.btnEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEntities.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntities.Location = new System.Drawing.Point(6, 135);
            this.btnEntities.Name = "btnEntities";
            this.btnEntities.Size = new System.Drawing.Size(380, 23);
            this.btnEntities.TabIndex = 0;
            this.btnEntities.Text = "Open the list of the Entities";
            this.btnEntities.UseVisualStyleBackColor = true;
            this.btnEntities.Visible = false;
            this.btnEntities.Click += new System.EventHandler(this.btnEntities_Click);
            // 
            // ckbDebugInfo
            // 
            this.ckbDebugInfo.AutoSize = true;
            this.ckbDebugInfo.Location = new System.Drawing.Point(12, 9);
            this.ckbDebugInfo.Name = "ckbDebugInfo";
            this.ckbDebugInfo.Size = new System.Drawing.Size(206, 17);
            this.ckbDebugInfo.TabIndex = 1;
            this.ckbDebugInfo.Text = "Enable DubugInfo for InteractEntities";
            this.ckbDebugInfo.UseVisualStyleBackColor = true;
            // 
            // btnAuras
            // 
            this.btnAuras.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAuras.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuras.Location = new System.Drawing.Point(336, 26);
            this.btnAuras.Name = "btnAuras";
            this.btnAuras.Size = new System.Drawing.Size(50, 23);
            this.btnAuras.TabIndex = 2;
            this.btnAuras.Text = "Export";
            this.btnAuras.UseVisualStyleBackColor = true;
            this.btnAuras.Click += new System.EventHandler(this.btnAuras_Click);
            // 
            // lblAuras
            // 
            this.lblAuras.AutoSize = true;
            this.lblAuras.Location = new System.Drawing.Point(14, 12);
            this.lblAuras.Name = "lblAuras";
            this.lblAuras.Size = new System.Drawing.Size(142, 13);
            this.lblAuras.TabIndex = 3;
            this.lblAuras.Text = "Export current Auras to file:";
            // 
            // btnMissions
            // 
            this.btnMissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMissions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMissions.Location = new System.Drawing.Point(336, 65);
            this.btnMissions.Name = "btnMissions";
            this.btnMissions.Size = new System.Drawing.Size(50, 23);
            this.btnMissions.TabIndex = 2;
            this.btnMissions.Text = "Export";
            this.btnMissions.UseVisualStyleBackColor = true;
            this.btnMissions.Click += new System.EventHandler(this.btnMissions_Click);
            // 
            // lblMissions
            // 
            this.lblMissions.AutoSize = true;
            this.lblMissions.Location = new System.Drawing.Point(14, 51);
            this.lblMissions.Name = "lblMissions";
            this.lblMissions.Size = new System.Drawing.Size(153, 13);
            this.lblMissions.TabIndex = 3;
            this.lblMissions.Text = "Export current Missions to file:";
            // 
            // bteMissions
            // 
            this.bteMissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bteMissions.EditValue = ".\\Logs\\Missions\\%character%Missions.xml";
            this.bteMissions.Location = new System.Drawing.Point(6, 67);
            this.bteMissions.Name = "bteMissions";
            this.bteMissions.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteMissions.Properties.NullText = "Enter the Filename where store the Missions";
            this.bteMissions.Properties.NullValuePromptShowForEmptyValue = true;
            this.bteMissions.Properties.ReadOnly = true;
            this.bteMissions.Size = new System.Drawing.Size(324, 20);
            this.bteMissions.TabIndex = 6;
            this.bteMissions.ToolTip = "File name to store Missions of the current Character. \r\nAllow mask %character%, %" +
    "account%, %dateTime%.";
            this.bteMissions.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ButtonClick);
            // 
            // bteAuras
            // 
            this.bteAuras.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bteAuras.EditValue = ".\\Logs\\Auras\\%character%Auras.xml";
            this.bteAuras.Location = new System.Drawing.Point(6, 28);
            this.bteAuras.Name = "bteAuras";
            this.bteAuras.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.bteAuras.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteAuras.Properties.NullText = "Enter the Filename where store the Auras";
            this.bteAuras.Properties.NullValuePromptShowForEmptyValue = true;
            this.bteAuras.Properties.ReadOnly = true;
            this.bteAuras.Size = new System.Drawing.Size(324, 20);
            this.bteAuras.TabIndex = 6;
            this.bteAuras.TabStop = false;
            this.bteAuras.ToolTip = "File name to store Auras of the current Character. \r\nAllow mask %character%, %acc" +
    "ount%, %dateTime%.";
            this.bteAuras.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ButtonClick);
            // 
            // tbclMain
            // 
            this.tbclMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbclMain.Location = new System.Drawing.Point(0, 0);
            this.tbclMain.Name = "tbclMain";
            this.tbclMain.SelectedTabPage = this.tabOptions;
            this.tbclMain.Size = new System.Drawing.Size(398, 416);
            this.tbclMain.TabIndex = 7;
            this.tbclMain.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabOptions,
            this.tabUtilities});
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.cbSlideMonitor);
            this.tabOptions.Controls.Add(this.ckbSpellStuckMonitor);
            this.tabOptions.Controls.Add(this.ckbDebugInfo);
            this.tabOptions.Controls.Add(this.gbSlideMonitor);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Size = new System.Drawing.Size(392, 388);
            this.tabOptions.Text = "Options";
            // 
            // cbSlideMonitor
            // 
            this.cbSlideMonitor.AutoSize = true;
            this.cbSlideMonitor.Location = new System.Drawing.Point(12, 55);
            this.cbSlideMonitor.Name = "cbSlideMonitor";
            this.cbSlideMonitor.Size = new System.Drawing.Size(119, 17);
            this.cbSlideMonitor.TabIndex = 3;
            this.cbSlideMonitor.Text = "Enable SlideMonitor";
            this.cbSlideMonitor.UseVisualStyleBackColor = true;
            this.cbSlideMonitor.CheckedChanged += new System.EventHandler(this.cbSlideMonitor_CheckedChanged);
            // 
            // ckbSpellStuckMonitor
            // 
            this.ckbSpellStuckMonitor.AutoSize = true;
            this.ckbSpellStuckMonitor.Location = new System.Drawing.Point(12, 32);
            this.ckbSpellStuckMonitor.Name = "ckbSpellStuckMonitor";
            this.ckbSpellStuckMonitor.Size = new System.Drawing.Size(145, 17);
            this.ckbSpellStuckMonitor.TabIndex = 1;
            this.ckbSpellStuckMonitor.Text = "Enable SpellStuckMonitor";
            this.ckbSpellStuckMonitor.UseVisualStyleBackColor = true;
            this.ckbSpellStuckMonitor.CheckedChanged += new System.EventHandler(this.cbSpellStuckMonitor_CheckedChanged);
            // 
            // gbSlideMonitor
            // 
            this.gbSlideMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSlideMonitor.Controls.Add(this.tbSlidingAuras);
            this.gbSlideMonitor.Controls.Add(this.seTimerSlide);
            this.gbSlideMonitor.Controls.Add(this.lblTimerSlide);
            this.gbSlideMonitor.Controls.Add(this.lblSlideFilter);
            this.gbSlideMonitor.Controls.Add(this.lblSlidingAuras);
            this.gbSlideMonitor.Controls.Add(this.lblTimerUnslide);
            this.gbSlideMonitor.Controls.Add(this.seTimerUnslide);
            this.gbSlideMonitor.Controls.Add(this.seSlideFilter);
            this.gbSlideMonitor.Controls.Add(this.lblSlideTimer);
            this.gbSlideMonitor.Location = new System.Drawing.Point(2, 57);
            this.gbSlideMonitor.Name = "gbSlideMonitor";
            this.gbSlideMonitor.Size = new System.Drawing.Size(387, 160);
            this.gbSlideMonitor.TabIndex = 4;
            this.gbSlideMonitor.TabStop = false;
            // 
            // tbSlidingAuras
            // 
            this.tbSlidingAuras.AcceptsReturn = true;
            this.tbSlidingAuras.AcceptsTab = true;
            this.tbSlidingAuras.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSlidingAuras.Location = new System.Drawing.Point(6, 94);
            this.tbSlidingAuras.Multiline = true;
            this.tbSlidingAuras.Name = "tbSlidingAuras";
            this.tbSlidingAuras.Size = new System.Drawing.Size(375, 60);
            this.tbSlidingAuras.TabIndex = 3;
            this.tbSlidingAuras.Text = "M10_Becritter_Boat_Costume\r\nVolume_Ground_Slippery\r\nVolume_Ground_Slippery_Player" +
    "only";
            // 
            // seTimerSlide
            // 
            this.seTimerSlide.EditValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.seTimerSlide.Location = new System.Drawing.Point(77, 55);
            this.seTimerSlide.Name = "seTimerSlide";
            this.seTimerSlide.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seTimerSlide.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seTimerSlide.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seTimerSlide.Properties.IsFloatValue = false;
            this.seTimerSlide.Properties.Mask.EditMask = "N00";
            this.seTimerSlide.Size = new System.Drawing.Size(56, 20);
            this.seTimerSlide.TabIndex = 1;
            this.seTimerSlide.EditValueChanged += new System.EventHandler(this.seTimerSlide_EditValueChanged);
            // 
            // lblTimerSlide
            // 
            this.lblTimerSlide.AutoSize = true;
            this.lblTimerSlide.Location = new System.Drawing.Point(6, 58);
            this.lblTimerSlide.Name = "lblTimerSlide";
            this.lblTimerSlide.Size = new System.Drawing.Size(65, 13);
            this.lblTimerSlide.TabIndex = 2;
            this.lblTimerSlide.Text = "when sliding";
            // 
            // lblSlideFilter
            // 
            this.lblSlideFilter.AutoSize = true;
            this.lblSlideFilter.Location = new System.Drawing.Point(6, 17);
            this.lblSlideFilter.Name = "lblSlideFilter";
            this.lblSlideFilter.Size = new System.Drawing.Size(198, 13);
            this.lblSlideFilter.TabIndex = 2;
            this.lblSlideFilter.Text = "Distance to target waypoint when slide:";
            // 
            // lblSlidingAuras
            // 
            this.lblSlidingAuras.AutoSize = true;
            this.lblSlidingAuras.Location = new System.Drawing.Point(6, 78);
            this.lblSlidingAuras.Name = "lblSlidingAuras";
            this.lblSlidingAuras.Size = new System.Drawing.Size(72, 13);
            this.lblSlidingAuras.TabIndex = 2;
            this.lblSlidingAuras.Text = "Sliding Auras:";
            // 
            // lblTimerUnslide
            // 
            this.lblTimerUnslide.AutoSize = true;
            this.lblTimerUnslide.Location = new System.Drawing.Point(139, 58);
            this.lblTimerUnslide.Name = "lblTimerUnslide";
            this.lblTimerUnslide.Size = new System.Drawing.Size(84, 13);
            this.lblTimerUnslide.TabIndex = 2;
            this.lblTimerUnslide.Text = "when not sliding";
            // 
            // seTimerUnslide
            // 
            this.seTimerUnslide.EditValue = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.seTimerUnslide.Location = new System.Drawing.Point(229, 55);
            this.seTimerUnslide.Name = "seTimerUnslide";
            this.seTimerUnslide.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seTimerUnslide.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seTimerUnslide.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seTimerUnslide.Properties.IsFloatValue = false;
            this.seTimerUnslide.Properties.Mask.EditMask = "N00";
            this.seTimerUnslide.Size = new System.Drawing.Size(56, 20);
            this.seTimerUnslide.TabIndex = 1;
            this.seTimerUnslide.EditValueChanged += new System.EventHandler(this.seTimerUnslide_EditValueChanged);
            // 
            // seSlideFilter
            // 
            this.seSlideFilter.EditValue = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.seSlideFilter.Location = new System.Drawing.Point(210, 14);
            this.seSlideFilter.Name = "seSlideFilter";
            this.seSlideFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seSlideFilter.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seSlideFilter.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seSlideFilter.Properties.IsFloatValue = false;
            this.seSlideFilter.Properties.Mask.EditMask = "N00";
            this.seSlideFilter.Properties.MaxValue = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.seSlideFilter.Size = new System.Drawing.Size(56, 20);
            this.seSlideFilter.TabIndex = 0;
            this.seSlideFilter.EditValueChanged += new System.EventHandler(this.seSlideFilter_EditValueChanged);
            // 
            // lblSlideTimer
            // 
            this.lblSlideTimer.AutoSize = true;
            this.lblSlideTimer.Location = new System.Drawing.Point(6, 39);
            this.lblSlideTimer.Name = "lblSlideTimer";
            this.lblSlideTimer.Size = new System.Drawing.Size(194, 13);
            this.lblSlideTimer.TabIndex = 2;
            this.lblSlideTimer.Text = "Time between aura check (millisecond):";
            // 
            // tabUtilities
            // 
            this.tabUtilities.Controls.Add(this.lblAuras);
            this.tabUtilities.Controls.Add(this.btnMissions);
            this.tabUtilities.Controls.Add(this.btnUiViewer);
            this.tabUtilities.Controls.Add(this.btnEntities);
            this.tabUtilities.Controls.Add(this.bteMissions);
            this.tabUtilities.Controls.Add(this.bteAuras);
            this.tabUtilities.Controls.Add(this.lblMissions);
            this.tabUtilities.Controls.Add(this.btnAuras);
            this.tabUtilities.Name = "tabUtilities";
            this.tabUtilities.Size = new System.Drawing.Size(392, 388);
            this.tabUtilities.Text = "Utilities";
            // 
            // btnUiViewer
            // 
            this.btnUiViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUiViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUiViewer.Location = new System.Drawing.Point(6, 106);
            this.btnUiViewer.Name = "btnUiViewer";
            this.btnUiViewer.Size = new System.Drawing.Size(380, 23);
            this.btnUiViewer.TabIndex = 0;
            this.btnUiViewer.Text = "UI Viewer";
            this.btnUiViewer.UseVisualStyleBackColor = true;
            this.btnUiViewer.Click += new System.EventHandler(this.btnUiViewer_Click);
            // 
            // MainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbclMain);
            this.Name = "MainPanel";
            this.Size = new System.Drawing.Size(398, 416);
            this.Load += new System.EventHandler(this.MainPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bteMissions.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteAuras.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).EndInit();
            this.tbclMain.ResumeLayout(false);
            this.tabOptions.ResumeLayout(false);
            this.tabOptions.PerformLayout();
            this.gbSlideMonitor.ResumeLayout(false);
            this.gbSlideMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seTimerSlide.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seTimerUnslide.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seSlideFilter.Properties)).EndInit();
            this.tabUtilities.ResumeLayout(false);
            this.tabUtilities.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEntities;
        private System.Windows.Forms.CheckBox ckbDebugInfo;
        private System.Windows.Forms.Button btnAuras;
        private System.Windows.Forms.Label lblAuras;
        private System.Windows.Forms.Button btnMissions;
        private System.Windows.Forms.Label lblMissions;
        private DevExpress.XtraEditors.ButtonEdit bteMissions;
        private DevExpress.XtraEditors.ButtonEdit bteAuras;
        private System.Windows.Forms.FolderBrowserDialog fldrBroserDlg;
        private DevExpress.XtraTab.XtraTabControl tbclMain;
        private DevExpress.XtraTab.XtraTabPage tabUtilities;
        private DevExpress.XtraTab.XtraTabPage tabOptions;
        private System.Windows.Forms.CheckBox ckbSpellStuckMonitor;
        private System.Windows.Forms.Label lblTimerSlide;
        private System.Windows.Forms.Label lblTimerUnslide;
        private System.Windows.Forms.Label lblSlideTimer;
        private System.Windows.Forms.Label lblSlideFilter;
        private DevExpress.XtraEditors.SpinEdit seTimerSlide;
        private DevExpress.XtraEditors.SpinEdit seTimerUnslide;
        private DevExpress.XtraEditors.SpinEdit seSlideFilter;
        private System.Windows.Forms.CheckBox cbSlideMonitor;
        private System.Windows.Forms.GroupBox gbSlideMonitor;
        private System.Windows.Forms.TextBox tbSlidingAuras;
        private System.Windows.Forms.Label lblSlidingAuras;
        private System.Windows.Forms.Button btnUiViewer;
    }
}
