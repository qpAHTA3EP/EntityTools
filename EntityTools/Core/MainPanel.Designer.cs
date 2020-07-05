using EntityTools.Tools;
using System.IO;

namespace EntityTools.Core
{
    partial class EntityToolsMainPanel
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
            this.components = new System.ComponentModel.Container();
            this.btnUccEditor = new System.Windows.Forms.Button();
            this.fldrBroserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.tbclMain = new DevExpress.XtraTab.XtraTabControl();
            this.tabUtilities = new DevExpress.XtraTab.XtraTabPage();
            this.cbEnchantHelperActivator = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnCheckCore = new System.Windows.Forms.Button();
            this.btnUiViewer = new System.Windows.Forms.Button();
            this.btnAuraViewer = new System.Windows.Forms.Button();
            this.btnEntities = new System.Windows.Forms.Button();
            this.ckbSpellStuckMonitor = new System.Windows.Forms.CheckBox();
            this.gbxExport = new System.Windows.Forms.GroupBox();
            this.cbbxExportSelector = new System.Windows.Forms.ComboBox();
            this.tbExportFileSelector = new DevExpress.XtraEditors.ButtonEdit();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.tabOptions = new DevExpress.XtraTab.XtraTabPage();
            this.cbSlideMonitor = new System.Windows.Forms.CheckBox();
            this.gbxSlideMonitor = new System.Windows.Forms.GroupBox();
            this.tbSlidingAuras = new System.Windows.Forms.TextBox();
            this.lblSlideFilter = new System.Windows.Forms.Label();
            this.lblSlidingAuras = new System.Windows.Forms.Label();
            this.lblTimerUnslide = new System.Windows.Forms.Label();
            this.seSlideFilter = new DevExpress.XtraEditors.SpinEdit();
            this.lblSlideTimer = new System.Windows.Forms.Label();
            this.gbxEntityCache = new System.Windows.Forms.GroupBox();
            this.seCombatCacheTime = new DevExpress.XtraEditors.SpinEdit();
            this.lblCombatCacheTime = new System.Windows.Forms.Label();
            this.seLocalCacheTime = new DevExpress.XtraEditors.SpinEdit();
            this.lblLocalCacheTime = new System.Windows.Forms.Label();
            this.seGlobalCacheTime = new DevExpress.XtraEditors.SpinEdit();
            this.lblGlobalCacheTime = new System.Windows.Forms.Label();
            this.tabRelogger = new DevExpress.XtraTab.XtraTabPage();
            this.lblMachinId = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.btnGetMachineId = new DevExpress.XtraEditors.SimpleButton();
            this.btnSetMachineId = new DevExpress.XtraEditors.SimpleButton();
            this.tbMashingId = new System.Windows.Forms.TextBox();
            this.tabMapper = new DevExpress.XtraTab.XtraTabPage();
            this.ckbMapperPatch = new System.Windows.Forms.CheckBox();
            this.gbxMapperSettings = new System.Windows.Forms.GroupBox();
            this.seMapperWaipointDistance = new DevExpress.XtraEditors.SpinEdit();
            this.btnMapperTest = new System.Windows.Forms.Button();
            this.seWaypointEquivalenceDistance = new DevExpress.XtraEditors.SpinEdit();
            this.lblWaypointEquivalenceDistance = new System.Windows.Forms.Label();
            this.ckbMapperForceLinkingWaypoint = new System.Windows.Forms.CheckBox();
            this.lblMapperMaxZDif = new System.Windows.Forms.Label();
            this.lblNodeDistance = new System.Windows.Forms.Label();
            this.seMapperMaxZDif = new DevExpress.XtraEditors.SpinEdit();
            this.ckbMapperLinearPath = new System.Windows.Forms.CheckBox();
            this.btnShowMapper = new System.Windows.Forms.Button();
            this.tabLogger = new DevExpress.XtraTab.XtraTabPage();
            this.ckbEnableLogger = new System.Windows.Forms.CheckBox();
            this.gbxLogger = new System.Windows.Forms.GroupBox();
            this.ckbExtendedActionDebugInfo = new System.Windows.Forms.CheckBox();
            this.btnOpenLog = new System.Windows.Forms.Button();
            this.tabDebug = new DevExpress.XtraTab.XtraTabPage();
            this.tbDebugMonitorInfo = new System.Windows.Forms.TextBox();
            this.ckbDebugMonitor = new System.Windows.Forms.CheckBox();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).BeginInit();
            this.tbclMain.SuspendLayout();
            this.tabUtilities.SuspendLayout();
            this.gbxExport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbExportFileSelector.Properties)).BeginInit();
            this.tabOptions.SuspendLayout();
            this.gbxSlideMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seSlideFilter.Properties)).BeginInit();
            this.gbxEntityCache.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seCombatCacheTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLocalCacheTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seGlobalCacheTime.Properties)).BeginInit();
            this.tabRelogger.SuspendLayout();
            this.tabMapper.SuspendLayout();
            this.gbxMapperSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seMapperWaipointDistance.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointEquivalenceDistance.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMapperMaxZDif.Properties)).BeginInit();
            this.tabLogger.SuspendLayout();
            this.gbxLogger.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUccEditor
            // 
            this.btnUccEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUccEditor.Enabled = false;
            this.btnUccEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUccEditor.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUccEditor.Location = new System.Drawing.Point(9, 201);
            this.btnUccEditor.Name = "btnUccEditor";
            this.btnUccEditor.Size = new System.Drawing.Size(287, 40);
            this.btnUccEditor.TabIndex = 0;
            this.btnUccEditor.Text = "Extended UCC Editor";
            this.btnUccEditor.UseVisualStyleBackColor = true;
            this.btnUccEditor.Visible = false;
            this.btnUccEditor.Click += new System.EventHandler(this.event_OpenUccEditor);
            // 
            // tbclMain
            // 
            this.tbclMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbclMain.Location = new System.Drawing.Point(0, 0);
            this.tbclMain.Name = "tbclMain";
            this.tbclMain.SelectedTabPage = this.tabUtilities;
            this.tbclMain.Size = new System.Drawing.Size(370, 416);
            this.tbclMain.TabIndex = 7;
            this.tbclMain.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabUtilities,
            this.tabOptions,
            this.tabRelogger,
            this.tabMapper,
            this.tabLogger,
            this.tabDebug});
            // 
            // tabUtilities
            // 
            this.tabUtilities.Controls.Add(this.cbEnchantHelperActivator);
            this.tabUtilities.Controls.Add(this.button2);
            this.tabUtilities.Controls.Add(this.button1);
            this.tabUtilities.Controls.Add(this.btnTest);
            this.tabUtilities.Controls.Add(this.btnCheckCore);
            this.tabUtilities.Controls.Add(this.btnUiViewer);
            this.tabUtilities.Controls.Add(this.btnAuraViewer);
            this.tabUtilities.Controls.Add(this.btnEntities);
            this.tabUtilities.Controls.Add(this.btnUccEditor);
            this.tabUtilities.Controls.Add(this.ckbSpellStuckMonitor);
            this.tabUtilities.Controls.Add(this.gbxExport);
            this.tabUtilities.Name = "tabUtilities";
            this.tabUtilities.Padding = new System.Windows.Forms.Padding(6);
            this.tabUtilities.Size = new System.Drawing.Size(364, 388);
            this.tabUtilities.Text = "Utilities";
            // 
            // cbEnchantHelperActivator
            // 
            this.cbEnchantHelperActivator.AutoSize = true;
            this.cbEnchantHelperActivator.Location = new System.Drawing.Point(15, 114);
            this.cbEnchantHelperActivator.Name = "cbEnchantHelperActivator";
            this.cbEnchantHelperActivator.Size = new System.Drawing.Size(131, 17);
            this.cbEnchantHelperActivator.TabIndex = 10;
            this.cbEnchantHelperActivator.Text = "Enable EnchantHelper";
            this.cbEnchantHelperActivator.UseVisualStyleBackColor = true;
            this.cbEnchantHelperActivator.CheckedChanged += new System.EventHandler(this.event_EnchantHelperActivation);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(301, 160);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(54, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Test 1";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.event_Test_1);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(301, 189);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Test 2";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.event_Test_2);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Location = new System.Drawing.Point(301, 218);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(54, 23);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test 3";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.event_Test_3);
            // 
            // btnCheckCore
            // 
            this.btnCheckCore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckCore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckCore.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCheckCore.Location = new System.Drawing.Point(9, 339);
            this.btnCheckCore.Name = "btnCheckCore";
            this.btnCheckCore.Size = new System.Drawing.Size(346, 40);
            this.btnCheckCore.TabIndex = 0;
            this.btnCheckCore.Text = "Check EntityTools";
            this.btnCheckCore.UseVisualStyleBackColor = true;
            this.btnCheckCore.Click += new System.EventHandler(this.event_CheckCore);
            // 
            // btnUiViewer
            // 
            this.btnUiViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUiViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUiViewer.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUiViewer.Location = new System.Drawing.Point(185, 293);
            this.btnUiViewer.Name = "btnUiViewer";
            this.btnUiViewer.Size = new System.Drawing.Size(170, 40);
            this.btnUiViewer.TabIndex = 0;
            this.btnUiViewer.Text = "UI Viewer";
            this.btnUiViewer.UseVisualStyleBackColor = true;
            this.btnUiViewer.Click += new System.EventHandler(this.event_OpenUiViewer);
            // 
            // btnAuraViewer
            // 
            this.btnAuraViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAuraViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuraViewer.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAuraViewer.Location = new System.Drawing.Point(185, 247);
            this.btnAuraViewer.Name = "btnAuraViewer";
            this.btnAuraViewer.Size = new System.Drawing.Size(170, 40);
            this.btnAuraViewer.TabIndex = 0;
            this.btnAuraViewer.Text = "Auras";
            this.btnAuraViewer.UseVisualStyleBackColor = true;
            this.btnAuraViewer.Click += new System.EventHandler(this.event_OpenAuraViewer);
            // 
            // btnEntities
            // 
            this.btnEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEntities.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntities.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEntities.Location = new System.Drawing.Point(9, 293);
            this.btnEntities.Name = "btnEntities";
            this.btnEntities.Size = new System.Drawing.Size(170, 40);
            this.btnEntities.TabIndex = 0;
            this.btnEntities.Text = "Entities";
            this.btnEntities.UseVisualStyleBackColor = true;
            this.btnEntities.Click += new System.EventHandler(this.event_OpenEntitiesViewer);
            // 
            // ckbSpellStuckMonitor
            // 
            this.ckbSpellStuckMonitor.AutoSize = true;
            this.ckbSpellStuckMonitor.Checked = true;
            this.ckbSpellStuckMonitor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbSpellStuckMonitor.Location = new System.Drawing.Point(15, 91);
            this.ckbSpellStuckMonitor.Name = "ckbSpellStuckMonitor";
            this.ckbSpellStuckMonitor.Size = new System.Drawing.Size(145, 17);
            this.ckbSpellStuckMonitor.TabIndex = 1;
            this.ckbSpellStuckMonitor.Text = "Enable SpellStuckMonitor";
            this.ckbSpellStuckMonitor.UseVisualStyleBackColor = true;
            this.ckbSpellStuckMonitor.CheckedChanged += new System.EventHandler(this.event_SpellStuckMonitorActivation);
            // 
            // gbxExport
            // 
            this.gbxExport.Controls.Add(this.cbbxExportSelector);
            this.gbxExport.Controls.Add(this.tbExportFileSelector);
            this.gbxExport.Controls.Add(this.btnExport);
            this.gbxExport.Controls.Add(this.btnDefault);
            this.gbxExport.Location = new System.Drawing.Point(9, 3);
            this.gbxExport.Name = "gbxExport";
            this.gbxExport.Size = new System.Drawing.Size(346, 82);
            this.gbxExport.TabIndex = 12;
            this.gbxExport.TabStop = false;
            this.gbxExport.Text = "Export";
            // 
            // cbbxExportSelector
            // 
            this.cbbxExportSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbxExportSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbxExportSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbbxExportSelector.Location = new System.Drawing.Point(6, 20);
            this.cbbxExportSelector.Name = "cbbxExportSelector";
            this.cbbxExportSelector.Size = new System.Drawing.Size(219, 21);
            this.cbbxExportSelector.Sorted = true;
            this.cbbxExportSelector.TabIndex = 11;
            this.cbbxExportSelector.SelectedIndexChanged += new System.EventHandler(this.event_ChangeExportingData);
            // 
            // tbExportFileSelector
            // 
            this.tbExportFileSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExportFileSelector.EditValue = "";
            this.tbExportFileSelector.Location = new System.Drawing.Point(6, 51);
            this.tbExportFileSelector.Name = "tbExportFileSelector";
            this.tbExportFileSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.tbExportFileSelector.Properties.HideSelection = false;
            this.tbExportFileSelector.Properties.NullText = "Enter the Filename";
            this.tbExportFileSelector.Properties.NullValuePromptShowForEmptyValue = true;
            this.tbExportFileSelector.Size = new System.Drawing.Size(334, 20);
            this.tbExportFileSelector.TabIndex = 6;
            this.tbExportFileSelector.ToolTip = "File name to store selected data. \r\nAllow mask %character%, %account%, %dateTime%" +
    ".";
            this.tbExportFileSelector.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.event_ChangeExportingFileName);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Location = new System.Drawing.Point(290, 20);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(50, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.event_Export);
            // 
            // btnDefault
            // 
            this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDefault.Location = new System.Drawing.Point(231, 20);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(53, 23);
            this.btnDefault.TabIndex = 2;
            this.btnDefault.Text = "Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.event_ResetExportSettings);
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.cbSlideMonitor);
            this.tabOptions.Controls.Add(this.gbxSlideMonitor);
            this.tabOptions.Controls.Add(this.gbxEntityCache);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(6);
            this.tabOptions.Size = new System.Drawing.Size(364, 388);
            this.tabOptions.Text = "Options";
            // 
            // cbSlideMonitor
            // 
            this.cbSlideMonitor.Location = new System.Drawing.Point(18, 220);
            this.cbSlideMonitor.Name = "cbSlideMonitor";
            this.cbSlideMonitor.Size = new System.Drawing.Size(119, 17);
            this.cbSlideMonitor.TabIndex = 3;
            this.cbSlideMonitor.Text = "Enable SlideMonitor";
            this.cbSlideMonitor.UseVisualStyleBackColor = true;
            this.cbSlideMonitor.Visible = false;
            // 
            // gbxSlideMonitor
            // 
            this.gbxSlideMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxSlideMonitor.Controls.Add(this.tbSlidingAuras);
            this.gbxSlideMonitor.Controls.Add(this.lblSlideFilter);
            this.gbxSlideMonitor.Controls.Add(this.lblSlidingAuras);
            this.gbxSlideMonitor.Controls.Add(this.lblTimerUnslide);
            this.gbxSlideMonitor.Controls.Add(this.seSlideFilter);
            this.gbxSlideMonitor.Controls.Add(this.lblSlideTimer);
            this.gbxSlideMonitor.Location = new System.Drawing.Point(9, 219);
            this.gbxSlideMonitor.Name = "gbxSlideMonitor";
            this.gbxSlideMonitor.Size = new System.Drawing.Size(347, 160);
            this.gbxSlideMonitor.TabIndex = 4;
            this.gbxSlideMonitor.TabStop = false;
            this.gbxSlideMonitor.Visible = false;
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
            this.tbSlidingAuras.Size = new System.Drawing.Size(335, 60);
            this.tbSlidingAuras.TabIndex = 3;
            this.tbSlidingAuras.Text = "M10_Becritter_Boat_Costume\r\nVolume_Ground_Slippery\r\nVolume_Ground_Slippery_Player" +
    "only";
            this.tbSlidingAuras.Visible = false;
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
            // gbxEntityCache
            // 
            this.gbxEntityCache.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxEntityCache.Controls.Add(this.seCombatCacheTime);
            this.gbxEntityCache.Controls.Add(this.lblCombatCacheTime);
            this.gbxEntityCache.Controls.Add(this.seLocalCacheTime);
            this.gbxEntityCache.Controls.Add(this.lblLocalCacheTime);
            this.gbxEntityCache.Controls.Add(this.seGlobalCacheTime);
            this.gbxEntityCache.Controls.Add(this.lblGlobalCacheTime);
            this.gbxEntityCache.Location = new System.Drawing.Point(9, 9);
            this.gbxEntityCache.Name = "gbxEntityCache";
            this.gbxEntityCache.Size = new System.Drawing.Size(346, 102);
            this.gbxEntityCache.TabIndex = 5;
            this.gbxEntityCache.TabStop = false;
            this.gbxEntityCache.Text = "EntityCache settings";
            // 
            // seCombatCacheTime
            // 
            this.seCombatCacheTime.EditValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.seCombatCacheTime.Location = new System.Drawing.Point(9, 72);
            this.seCombatCacheTime.Name = "seCombatCacheTime";
            this.seCombatCacheTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seCombatCacheTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seCombatCacheTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seCombatCacheTime.Properties.IsFloatValue = false;
            this.seCombatCacheTime.Properties.Mask.EditMask = "N00";
            this.seCombatCacheTime.Size = new System.Drawing.Size(58, 20);
            this.seCombatCacheTime.TabIndex = 3;
            // 
            // lblCombatCacheTime
            // 
            this.lblCombatCacheTime.AutoSize = true;
            this.lblCombatCacheTime.Location = new System.Drawing.Point(73, 75);
            this.lblCombatCacheTime.Name = "lblCombatCacheTime";
            this.lblCombatCacheTime.Size = new System.Drawing.Size(199, 13);
            this.lblCombatCacheTime.TabIndex = 4;
            this.lblCombatCacheTime.Text = "Combat entity cache refresh time (UCC)";
            // 
            // seLocalCacheTime
            // 
            this.seLocalCacheTime.EditValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.seLocalCacheTime.Location = new System.Drawing.Point(9, 46);
            this.seLocalCacheTime.Name = "seLocalCacheTime";
            this.seLocalCacheTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seLocalCacheTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seLocalCacheTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seLocalCacheTime.Properties.IsFloatValue = false;
            this.seLocalCacheTime.Properties.Mask.EditMask = "N00";
            this.seLocalCacheTime.Size = new System.Drawing.Size(58, 20);
            this.seLocalCacheTime.TabIndex = 3;
            this.seLocalCacheTime.ToolTip = "Time between the refresh of the local Entities cache of the actions and condition" +
    "s";
            // 
            // lblLocalCacheTime
            // 
            this.lblLocalCacheTime.AutoSize = true;
            this.lblLocalCacheTime.Location = new System.Drawing.Point(73, 49);
            this.lblLocalCacheTime.Name = "lblLocalCacheTime";
            this.lblLocalCacheTime.Size = new System.Drawing.Size(204, 13);
            this.lblLocalCacheTime.TabIndex = 4;
            this.lblLocalCacheTime.Text = "Local entity cache refresh time (Quester)";
            // 
            // seGlobalCacheTime
            // 
            this.seGlobalCacheTime.EditValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.seGlobalCacheTime.Location = new System.Drawing.Point(9, 20);
            this.seGlobalCacheTime.Name = "seGlobalCacheTime";
            this.seGlobalCacheTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seGlobalCacheTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seGlobalCacheTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seGlobalCacheTime.Properties.IsFloatValue = false;
            this.seGlobalCacheTime.Properties.Mask.EditMask = "N00";
            this.seGlobalCacheTime.Size = new System.Drawing.Size(58, 20);
            this.seGlobalCacheTime.TabIndex = 3;
            this.seGlobalCacheTime.ToolTip = "Time between the refresh of the global entity cache";
            // 
            // lblGlobalCacheTime
            // 
            this.lblGlobalCacheTime.AutoSize = true;
            this.lblGlobalCacheTime.Location = new System.Drawing.Point(73, 23);
            this.lblGlobalCacheTime.Name = "lblGlobalCacheTime";
            this.lblGlobalCacheTime.Size = new System.Drawing.Size(209, 13);
            this.lblGlobalCacheTime.TabIndex = 4;
            this.lblGlobalCacheTime.Text = "Global entity cache refresh time (Quester)";
            // 
            // tabRelogger
            // 
            this.tabRelogger.Controls.Add(this.lblMachinId);
            this.tabRelogger.Controls.Add(this.lblAccount);
            this.tabRelogger.Controls.Add(this.btnGetMachineId);
            this.tabRelogger.Controls.Add(this.btnSetMachineId);
            this.tabRelogger.Controls.Add(this.tbMashingId);
            this.tabRelogger.Name = "tabRelogger";
            this.tabRelogger.Padding = new System.Windows.Forms.Padding(5);
            this.tabRelogger.PageEnabled = false;
            this.tabRelogger.PageVisible = false;
            this.tabRelogger.Size = new System.Drawing.Size(364, 388);
            this.tabRelogger.Text = "Relogger";
            // 
            // lblMachinId
            // 
            this.lblMachinId.AutoSize = true;
            this.lblMachinId.Location = new System.Drawing.Point(8, 43);
            this.lblMachinId.Name = "lblMachinId";
            this.lblMachinId.Size = new System.Drawing.Size(54, 13);
            this.lblMachinId.TabIndex = 3;
            this.lblMachinId.Text = "MachinId:";
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(8, 18);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(101, 13);
            this.lblAccount.TabIndex = 2;
            this.lblAccount.Text = "Account: @account";
            // 
            // btnGetMachineId
            // 
            this.btnGetMachineId.Location = new System.Drawing.Point(250, 67);
            this.btnGetMachineId.Name = "btnGetMachineId";
            this.btnGetMachineId.Size = new System.Drawing.Size(50, 23);
            this.btnGetMachineId.TabIndex = 1;
            this.btnGetMachineId.Text = "Get";
            this.btnGetMachineId.Click += new System.EventHandler(this.event_GetMachineId);
            // 
            // btnSetMachineId
            // 
            this.btnSetMachineId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetMachineId.Location = new System.Drawing.Point(306, 67);
            this.btnSetMachineId.Name = "btnSetMachineId";
            this.btnSetMachineId.Size = new System.Drawing.Size(50, 23);
            this.btnSetMachineId.TabIndex = 1;
            this.btnSetMachineId.Text = "Set";
            // 
            // tbMashingId
            // 
            this.tbMashingId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMashingId.Location = new System.Drawing.Point(68, 40);
            this.tbMashingId.Name = "tbMashingId";
            this.tbMashingId.Size = new System.Drawing.Size(288, 21);
            this.tbMashingId.TabIndex = 0;
            // 
            // tabMapper
            // 
            this.tabMapper.Controls.Add(this.ckbMapperPatch);
            this.tabMapper.Controls.Add(this.gbxMapperSettings);
            this.tabMapper.Controls.Add(this.btnShowMapper);
            this.tabMapper.Name = "tabMapper";
            this.tabMapper.Padding = new System.Windows.Forms.Padding(6);
            this.tabMapper.Size = new System.Drawing.Size(364, 388);
            this.tabMapper.Text = "Mapper Settings";
            // 
            // ckbMapperPatch
            // 
            this.ckbMapperPatch.AutoSize = true;
            this.ckbMapperPatch.Location = new System.Drawing.Point(15, 9);
            this.ckbMapperPatch.Name = "ckbMapperPatch";
            this.ckbMapperPatch.Size = new System.Drawing.Size(157, 17);
            this.ckbMapperPatch.TabIndex = 3;
            this.ckbMapperPatch.Text = "Mapper Patch (need Relog)";
            this.ckbMapperPatch.UseVisualStyleBackColor = true;
            // 
            // gbxMapperSettings
            // 
            this.gbxMapperSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxMapperSettings.Controls.Add(this.seMapperWaipointDistance);
            this.gbxMapperSettings.Controls.Add(this.btnMapperTest);
            this.gbxMapperSettings.Controls.Add(this.seWaypointEquivalenceDistance);
            this.gbxMapperSettings.Controls.Add(this.lblWaypointEquivalenceDistance);
            this.gbxMapperSettings.Controls.Add(this.ckbMapperForceLinkingWaypoint);
            this.gbxMapperSettings.Controls.Add(this.lblMapperMaxZDif);
            this.gbxMapperSettings.Controls.Add(this.lblNodeDistance);
            this.gbxMapperSettings.Controls.Add(this.seMapperMaxZDif);
            this.gbxMapperSettings.Controls.Add(this.ckbMapperLinearPath);
            this.gbxMapperSettings.Location = new System.Drawing.Point(9, 32);
            this.gbxMapperSettings.Name = "gbxMapperSettings";
            this.gbxMapperSettings.Size = new System.Drawing.Size(346, 301);
            this.gbxMapperSettings.TabIndex = 6;
            this.gbxMapperSettings.TabStop = false;
            this.gbxMapperSettings.Text = "Settings";
            // 
            // seMapperWaipointDistance
            // 
            this.seMapperWaipointDistance.EditValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.seMapperWaipointDistance.Location = new System.Drawing.Point(6, 20);
            this.seMapperWaipointDistance.Name = "seMapperWaipointDistance";
            this.seMapperWaipointDistance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seMapperWaipointDistance.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seMapperWaipointDistance.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seMapperWaipointDistance.Properties.IsFloatValue = false;
            this.seMapperWaipointDistance.Properties.Mask.EditMask = "N00";
            this.seMapperWaipointDistance.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.seMapperWaipointDistance.Properties.MinValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.seMapperWaipointDistance.Size = new System.Drawing.Size(58, 20);
            this.seMapperWaipointDistance.TabIndex = 1;
            // 
            // btnMapperTest
            // 
            this.btnMapperTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapperTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMapperTest.Location = new System.Drawing.Point(290, 272);
            this.btnMapperTest.Name = "btnMapperTest";
            this.btnMapperTest.Size = new System.Drawing.Size(50, 23);
            this.btnMapperTest.TabIndex = 5;
            this.btnMapperTest.Text = "Test";
            this.btnMapperTest.UseVisualStyleBackColor = true;
            // 
            // seWaypointEquivalenceDistance
            // 
            this.seWaypointEquivalenceDistance.EditValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.seWaypointEquivalenceDistance.Location = new System.Drawing.Point(6, 72);
            this.seWaypointEquivalenceDistance.Name = "seWaypointEquivalenceDistance";
            this.seWaypointEquivalenceDistance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seWaypointEquivalenceDistance.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seWaypointEquivalenceDistance.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seWaypointEquivalenceDistance.Properties.IsFloatValue = false;
            this.seWaypointEquivalenceDistance.Properties.Mask.EditMask = "N00";
            this.seWaypointEquivalenceDistance.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.seWaypointEquivalenceDistance.Properties.MinValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.seWaypointEquivalenceDistance.Size = new System.Drawing.Size(58, 20);
            this.seWaypointEquivalenceDistance.TabIndex = 1;
            // 
            // lblWaypointEquivalenceDistance
            // 
            this.lblWaypointEquivalenceDistance.AutoSize = true;
            this.lblWaypointEquivalenceDistance.Location = new System.Drawing.Point(70, 75);
            this.lblWaypointEquivalenceDistance.Name = "lblWaypointEquivalenceDistance";
            this.lblWaypointEquivalenceDistance.Size = new System.Drawing.Size(156, 13);
            this.lblWaypointEquivalenceDistance.TabIndex = 2;
            this.lblWaypointEquivalenceDistance.Text = "Waypoint equivalence distance";
            // 
            // ckbMapperForceLinkingWaypoint
            // 
            this.ckbMapperForceLinkingWaypoint.AutoSize = true;
            this.ckbMapperForceLinkingWaypoint.Location = new System.Drawing.Point(6, 121);
            this.ckbMapperForceLinkingWaypoint.Name = "ckbMapperForceLinkingWaypoint";
            this.ckbMapperForceLinkingWaypoint.Size = new System.Drawing.Size(343, 17);
            this.ckbMapperForceLinkingWaypoint.TabIndex = 3;
            this.ckbMapperForceLinkingWaypoint.Text = "The forced linking of the new waypoint and the previous waypoint";
            this.ckbMapperForceLinkingWaypoint.UseVisualStyleBackColor = true;
            // 
            // lblMapperMaxZDif
            // 
            this.lblMapperMaxZDif.AutoSize = true;
            this.lblMapperMaxZDif.Location = new System.Drawing.Point(70, 49);
            this.lblMapperMaxZDif.Name = "lblMapperMaxZDif";
            this.lblMapperMaxZDif.Size = new System.Drawing.Size(244, 13);
            this.lblMapperMaxZDif.TabIndex = 2;
            this.lblMapperMaxZDif.Text = "The max elevation difference between waypoints";
            // 
            // lblNodeDistance
            // 
            this.lblNodeDistance.AutoSize = true;
            this.lblNodeDistance.Location = new System.Drawing.Point(70, 23);
            this.lblNodeDistance.Name = "lblNodeDistance";
            this.lblNodeDistance.Size = new System.Drawing.Size(226, 13);
            this.lblNodeDistance.TabIndex = 2;
            this.lblNodeDistance.Text = "The distance between waypoints in edit mode";
            // 
            // seMapperMaxZDif
            // 
            this.seMapperMaxZDif.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.seMapperMaxZDif.Location = new System.Drawing.Point(6, 46);
            this.seMapperMaxZDif.Name = "seMapperMaxZDif";
            this.seMapperMaxZDif.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seMapperMaxZDif.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seMapperMaxZDif.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.seMapperMaxZDif.Properties.IsFloatValue = false;
            this.seMapperMaxZDif.Properties.Mask.EditMask = "N00";
            this.seMapperMaxZDif.Properties.MaxValue = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.seMapperMaxZDif.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.seMapperMaxZDif.Size = new System.Drawing.Size(58, 20);
            this.seMapperMaxZDif.TabIndex = 1;
            // 
            // ckbMapperLinearPath
            // 
            this.ckbMapperLinearPath.AutoSize = true;
            this.ckbMapperLinearPath.Location = new System.Drawing.Point(6, 98);
            this.ckbMapperLinearPath.Name = "ckbMapperLinearPath";
            this.ckbMapperLinearPath.Size = new System.Drawing.Size(321, 17);
            this.ckbMapperLinearPath.TabIndex = 3;
            this.ckbMapperLinearPath.Text = "Linear path (Links new waypoint with only previous waypoint)";
            this.ckbMapperLinearPath.UseVisualStyleBackColor = true;
            // 
            // btnShowMapper
            // 
            this.btnShowMapper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowMapper.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowMapper.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnShowMapper.Location = new System.Drawing.Point(9, 339);
            this.btnShowMapper.Name = "btnShowMapper";
            this.btnShowMapper.Size = new System.Drawing.Size(346, 40);
            this.btnShowMapper.TabIndex = 4;
            this.btnShowMapper.Text = "Show Mapper";
            this.btnShowMapper.UseVisualStyleBackColor = true;
            this.btnShowMapper.Click += new System.EventHandler(this.event_ShowMapper);
            // 
            // tabLogger
            // 
            this.tabLogger.Controls.Add(this.ckbEnableLogger);
            this.tabLogger.Controls.Add(this.gbxLogger);
            this.tabLogger.Name = "tabLogger";
            this.tabLogger.Padding = new System.Windows.Forms.Padding(6);
            this.tabLogger.Size = new System.Drawing.Size(364, 388);
            this.tabLogger.Text = "Logger";
            // 
            // ckbEnableLogger
            // 
            this.ckbEnableLogger.AutoSize = true;
            this.ckbEnableLogger.Location = new System.Drawing.Point(18, 9);
            this.ckbEnableLogger.Name = "ckbEnableLogger";
            this.ckbEnableLogger.Size = new System.Drawing.Size(150, 17);
            this.ckbEnableLogger.TabIndex = 6;
            this.ckbEnableLogger.Text = "Enable EntityTools Logger";
            this.ckbEnableLogger.UseVisualStyleBackColor = true;
            this.ckbEnableLogger.CheckedChanged += new System.EventHandler(this.ckbEnableLogger_CheckedChanged);
            // 
            // gbxLogger
            // 
            this.gbxLogger.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxLogger.Controls.Add(this.ckbExtendedActionDebugInfo);
            this.gbxLogger.Controls.Add(this.btnOpenLog);
            this.gbxLogger.Location = new System.Drawing.Point(9, 9);
            this.gbxLogger.Name = "gbxLogger";
            this.gbxLogger.Size = new System.Drawing.Size(346, 370);
            this.gbxLogger.TabIndex = 7;
            this.gbxLogger.TabStop = false;
            // 
            // ckbExtendedActionDebugInfo
            // 
            this.ckbExtendedActionDebugInfo.AutoSize = true;
            this.ckbExtendedActionDebugInfo.Location = new System.Drawing.Point(9, 23);
            this.ckbExtendedActionDebugInfo.Name = "ckbExtendedActionDebugInfo";
            this.ckbExtendedActionDebugInfo.Size = new System.Drawing.Size(296, 17);
            this.ckbExtendedActionDebugInfo.TabIndex = 6;
            this.ckbExtendedActionDebugInfo.Text = "Enable the Extended debug info for the Quester-actions";
            this.ckbExtendedActionDebugInfo.UseVisualStyleBackColor = true;
            // 
            // btnOpenLog
            // 
            this.btnOpenLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenLog.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnOpenLog.Location = new System.Drawing.Point(6, 324);
            this.btnOpenLog.Name = "btnOpenLog";
            this.btnOpenLog.Size = new System.Drawing.Size(334, 40);
            this.btnOpenLog.TabIndex = 5;
            this.btnOpenLog.Text = "Open EntityTools log file";
            this.btnOpenLog.UseVisualStyleBackColor = true;
            this.btnOpenLog.Click += new System.EventHandler(this.event_OpenLogFile);
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.tbDebugMonitorInfo);
            this.tabDebug.Controls.Add(this.ckbDebugMonitor);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(6);
            this.tabDebug.Size = new System.Drawing.Size(364, 388);
            this.tabDebug.Text = "Debug";
            // 
            // tbDebugMonitorInfo
            // 
            this.tbDebugMonitorInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDebugMonitorInfo.Location = new System.Drawing.Point(9, 32);
            this.tbDebugMonitorInfo.Multiline = true;
            this.tbDebugMonitorInfo.Name = "tbDebugMonitorInfo";
            this.tbDebugMonitorInfo.ReadOnly = true;
            this.tbDebugMonitorInfo.Size = new System.Drawing.Size(346, 347);
            this.tbDebugMonitorInfo.TabIndex = 9;
            // 
            // ckbDebugMonitor
            // 
            this.ckbDebugMonitor.AutoSize = true;
            this.ckbDebugMonitor.Location = new System.Drawing.Point(9, 9);
            this.ckbDebugMonitor.Name = "ckbDebugMonitor";
            this.ckbDebugMonitor.Size = new System.Drawing.Size(93, 17);
            this.ckbDebugMonitor.TabIndex = 8;
            this.ckbDebugMonitor.Text = "DebugMonitor";
            this.ckbDebugMonitor.UseVisualStyleBackColor = true;
            this.ckbDebugMonitor.CheckedChanged += new System.EventHandler(this.event_DebugMonitorActivation);
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.CheckPathExists = false;
            this.dlgSaveFile.Filter = "XML|*.xml";
            this.dlgSaveFile.ValidateNames = false;
            // 
            // EntityToolsMainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbclMain);
            this.Name = "EntityToolsMainPanel";
            this.Size = new System.Drawing.Size(370, 416);
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).EndInit();
            this.tbclMain.ResumeLayout(false);
            this.tabUtilities.ResumeLayout(false);
            this.tabUtilities.PerformLayout();
            this.gbxExport.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbExportFileSelector.Properties)).EndInit();
            this.tabOptions.ResumeLayout(false);
            this.gbxSlideMonitor.ResumeLayout(false);
            this.gbxSlideMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seSlideFilter.Properties)).EndInit();
            this.gbxEntityCache.ResumeLayout(false);
            this.gbxEntityCache.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seCombatCacheTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seLocalCacheTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seGlobalCacheTime.Properties)).EndInit();
            this.tabRelogger.ResumeLayout(false);
            this.tabRelogger.PerformLayout();
            this.tabMapper.ResumeLayout(false);
            this.tabMapper.PerformLayout();
            this.gbxMapperSettings.ResumeLayout(false);
            this.gbxMapperSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seMapperWaipointDistance.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointEquivalenceDistance.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMapperMaxZDif.Properties)).EndInit();
            this.tabLogger.ResumeLayout(false);
            this.tabLogger.PerformLayout();
            this.gbxLogger.ResumeLayout(false);
            this.gbxLogger.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUccEditor;
        private System.Windows.Forms.FolderBrowserDialog fldrBroserDlg;
        private DevExpress.XtraTab.XtraTabControl tbclMain;
        private DevExpress.XtraTab.XtraTabPage tabUtilities;
        private DevExpress.XtraTab.XtraTabPage tabOptions;
        private System.Windows.Forms.CheckBox ckbSpellStuckMonitor;
        private System.Windows.Forms.Label lblNodeDistance;
        private System.Windows.Forms.Label lblTimerUnslide;
        private System.Windows.Forms.Label lblSlideTimer;
        private System.Windows.Forms.Label lblSlideFilter;
        private DevExpress.XtraEditors.SpinEdit seMapperWaipointDistance;
        private DevExpress.XtraEditors.SpinEdit seSlideFilter;
        private System.Windows.Forms.CheckBox cbSlideMonitor;
        private System.Windows.Forms.GroupBox gbxSlideMonitor;
        private System.Windows.Forms.TextBox tbSlidingAuras;
        private System.Windows.Forms.Label lblSlidingAuras;
        private System.Windows.Forms.Button btnUiViewer;
        private System.Windows.Forms.CheckBox cbEnchantHelperActivator;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnEntities;
        private DevExpress.XtraTab.XtraTabPage tabRelogger;
        private System.Windows.Forms.Label lblMachinId;
        private System.Windows.Forms.Label lblAccount;
        private DevExpress.XtraEditors.SimpleButton btnGetMachineId;
        private DevExpress.XtraEditors.SimpleButton btnSetMachineId;
        private System.Windows.Forms.TextBox tbMashingId;
        private System.Windows.Forms.Button btnAuraViewer;
        private System.Windows.Forms.ComboBox cbbxExportSelector;
        private DevExpress.XtraEditors.ButtonEdit tbExportFileSelector;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.GroupBox gbxExport;
        private System.Windows.Forms.Button btnDefault;
        private DevExpress.XtraEditors.SpinEdit seMapperMaxZDif;
        private System.Windows.Forms.Label lblMapperMaxZDif;
        private System.Windows.Forms.CheckBox ckbMapperForceLinkingWaypoint;
        private System.Windows.Forms.CheckBox ckbMapperLinearPath;
        private System.Windows.Forms.Label lblWaypointEquivalenceDistance;
        private DevExpress.XtraEditors.SpinEdit seWaypointEquivalenceDistance;
        private System.Windows.Forms.Button btnShowMapper;
        private DevExpress.XtraTab.XtraTabPage tabMapper;
        private System.Windows.Forms.Button btnMapperTest;
        private System.Windows.Forms.GroupBox gbxMapperSettings;
        private System.Windows.Forms.CheckBox ckbMapperPatch;
        private DevExpress.XtraTab.XtraTabPage tabLogger;
        private System.Windows.Forms.CheckBox ckbEnableLogger;
        private System.Windows.Forms.Button btnOpenLog;
        private System.Windows.Forms.GroupBox gbxLogger;
        private System.Windows.Forms.GroupBox gbxEntityCache;
        private DevExpress.XtraEditors.SpinEdit seGlobalCacheTime;
        private System.Windows.Forms.Label lblGlobalCacheTime;
        private DevExpress.XtraEditors.SpinEdit seCombatCacheTime;
        private System.Windows.Forms.Label lblCombatCacheTime;
        private DevExpress.XtraEditors.SpinEdit seLocalCacheTime;
        private System.Windows.Forms.Label lblLocalCacheTime;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox ckbExtendedActionDebugInfo;
        private System.Windows.Forms.Button btnCheckCore;
        private DevExpress.XtraTab.XtraTabPage tabDebug;
        private System.Windows.Forms.TextBox tbDebugMonitorInfo;
        private System.Windows.Forms.CheckBox ckbDebugMonitor;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}
