using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using ComboBox = System.Windows.Forms.ComboBox;

namespace EntityTools.Core
{
    partial class EntityToolsMainPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.fldrBroserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.tbclMain = new DevExpress.XtraTab.XtraTabControl();
            this.tabUtilities = new DevExpress.XtraTab.XtraTabPage();
            this.cbEnchantHelperActivator = new System.Windows.Forms.CheckBox();
            this.btnTest1 = new System.Windows.Forms.Button();
            this.bntTest2 = new System.Windows.Forms.Button();
            this.btnTest3 = new System.Windows.Forms.Button();
            this.btnCheckCore = new System.Windows.Forms.Button();
            this.btnUiViewer = new System.Windows.Forms.Button();
            this.btnAuraViewer = new System.Windows.Forms.Button();
            this.btnEntities = new System.Windows.Forms.Button();
            this.ckbSpellStuckMonitor = new System.Windows.Forms.CheckBox();
            this.gbxExport = new System.Windows.Forms.GroupBox();
            this.cbxExportSelector = new System.Windows.Forms.ComboBox();
            this.tbExportFileSelector = new DevExpress.XtraEditors.ButtonEdit();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.ckbMapperPatch = new System.Windows.Forms.CheckBox();
            this.tabSettings = new DevExpress.XtraTab.XtraTabPage();
            this.pgConfigs = new System.Windows.Forms.PropertyGrid();
            this.tabOptions = new DevExpress.XtraTab.XtraTabPage();
            this.cbxSlideMonitor = new System.Windows.Forms.ComboBox();
            this.gbxSlideMonitor = new System.Windows.Forms.GroupBox();
            this.tbSlidingAuras = new System.Windows.Forms.TextBox();
            this.lblSlideFilter = new System.Windows.Forms.Label();
            this.lblSlidingAuras = new System.Windows.Forms.Label();
            this.editSlideTimeout = new DevExpress.XtraEditors.SpinEdit();
            this.editSlideFilter = new DevExpress.XtraEditors.SpinEdit();
            this.lblSlideTimer = new System.Windows.Forms.Label();
            this.gbxEntityCache = new System.Windows.Forms.GroupBox();
            this.editCombatCacheTime = new DevExpress.XtraEditors.SpinEdit();
            this.lblCombatCacheTime = new System.Windows.Forms.Label();
            this.editLocalCacheTime = new DevExpress.XtraEditors.SpinEdit();
            this.lblLocalCacheTime = new System.Windows.Forms.Label();
            this.editGlobalCacheTime = new DevExpress.XtraEditors.SpinEdit();
            this.lblGlobalCacheTime = new System.Windows.Forms.Label();
            this.tabRelogger = new DevExpress.XtraTab.XtraTabPage();
            this.lblMachinId = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.btnGetMachineId = new DevExpress.XtraEditors.SimpleButton();
            this.btnSetMachineId = new DevExpress.XtraEditors.SimpleButton();
            this.tbMashingId = new System.Windows.Forms.TextBox();
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
            this.tabSettings.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.gbxSlideMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editSlideTimeout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editSlideFilter.Properties)).BeginInit();
            this.gbxEntityCache.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editCombatCacheTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editLocalCacheTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editGlobalCacheTime.Properties)).BeginInit();
            this.tabRelogger.SuspendLayout();
            this.tabLogger.SuspendLayout();
            this.gbxLogger.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.SuspendLayout();
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
            this.tabSettings,
            this.tabOptions,
            this.tabRelogger,
            this.tabLogger,
            this.tabDebug});
            // 
            // tabUtilities
            // 
            this.tabUtilities.Controls.Add(this.cbEnchantHelperActivator);
            this.tabUtilities.Controls.Add(this.btnTest1);
            this.tabUtilities.Controls.Add(this.bntTest2);
            this.tabUtilities.Controls.Add(this.btnTest3);
            this.tabUtilities.Controls.Add(this.btnCheckCore);
            this.tabUtilities.Controls.Add(this.btnUiViewer);
            this.tabUtilities.Controls.Add(this.btnAuraViewer);
            this.tabUtilities.Controls.Add(this.btnEntities);
            this.tabUtilities.Controls.Add(this.ckbSpellStuckMonitor);
            this.tabUtilities.Controls.Add(this.gbxExport);
            this.tabUtilities.Controls.Add(this.ckbMapperPatch);
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
            this.cbEnchantHelperActivator.CheckedChanged += new System.EventHandler(this.handler_EnchantHelperActivation);
            // 
            // btnTest1
            // 
            this.btnTest1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest1.Location = new System.Drawing.Point(301, 160);
            this.btnTest1.Name = "btnTest1";
            this.btnTest1.Size = new System.Drawing.Size(54, 23);
            this.btnTest1.TabIndex = 2;
            this.btnTest1.Text = "Test 1";
            this.btnTest1.UseVisualStyleBackColor = true;
            this.btnTest1.Visible = false;
            this.btnTest1.Click += new System.EventHandler(this.handler_Test_1);
            // 
            // bntTest2
            // 
            this.bntTest2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntTest2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntTest2.Location = new System.Drawing.Point(301, 189);
            this.bntTest2.Name = "bntTest2";
            this.bntTest2.Size = new System.Drawing.Size(54, 23);
            this.bntTest2.TabIndex = 2;
            this.bntTest2.Text = "Test 2";
            this.bntTest2.UseVisualStyleBackColor = true;
            this.bntTest2.Visible = false;
            this.bntTest2.Click += new System.EventHandler(this.handler_Test_2);
            // 
            // btnTest3
            // 
            this.btnTest3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest3.Location = new System.Drawing.Point(185, 218);
            this.btnTest3.Name = "btnTest3";
            this.btnTest3.Size = new System.Drawing.Size(170, 23);
            this.btnTest3.TabIndex = 2;
            this.btnTest3.Text = "Test current Graph";
            this.btnTest3.UseVisualStyleBackColor = true;
            this.btnTest3.Visible = false;
            this.btnTest3.Click += new System.EventHandler(this.handler_Test_3);
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
            this.btnCheckCore.Click += new System.EventHandler(this.handler_CheckCore);
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
            this.btnUiViewer.Click += new System.EventHandler(this.handler_OpenUiViewer);
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
            this.btnAuraViewer.Click += new System.EventHandler(this.handler_OpenAuraViewer);
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
            this.btnEntities.Click += new System.EventHandler(this.handler_OpenEntitiesViewer);
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
            this.ckbSpellStuckMonitor.CheckedChanged += new System.EventHandler(this.handler_SpellStuckMonitorActivation);
            // 
            // gbxExport
            // 
            this.gbxExport.Controls.Add(this.cbxExportSelector);
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
            // cbxExportSelector
            // 
            this.cbxExportSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxExportSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxExportSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxExportSelector.Location = new System.Drawing.Point(6, 20);
            this.cbxExportSelector.Name = "cbxExportSelector";
            this.cbxExportSelector.Size = new System.Drawing.Size(219, 21);
            this.cbxExportSelector.Sorted = true;
            this.cbxExportSelector.TabIndex = 11;
            this.cbxExportSelector.SelectedIndexChanged += new System.EventHandler(this.handler_ChangeExportingData);
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
            this.tbExportFileSelector.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.handler_ChangeExportingFileName);
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
            this.btnExport.Click += new System.EventHandler(this.handler_Export);
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
            this.btnDefault.Click += new System.EventHandler(this.handler_ResetExportSettings);
            // 
            // ckbMapperPatch
            // 
            this.ckbMapperPatch.AutoSize = true;
            this.ckbMapperPatch.Location = new System.Drawing.Point(15, 137);
            this.ckbMapperPatch.Name = "ckbMapperPatch";
            this.ckbMapperPatch.Size = new System.Drawing.Size(157, 17);
            this.ckbMapperPatch.TabIndex = 3;
            this.ckbMapperPatch.Text = "Mapper Patch (need Relog)";
            this.ckbMapperPatch.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.pgConfigs);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(6);
            this.tabSettings.Size = new System.Drawing.Size(364, 388);
            this.tabSettings.Text = "Settings";
            // 
            // pgConfigs
            // 
            this.pgConfigs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgConfigs.HelpBackColor = System.Drawing.SystemColors.Window;
            this.pgConfigs.Location = new System.Drawing.Point(9, 9);
            this.pgConfigs.Name = "pgConfigs";
            this.pgConfigs.Size = new System.Drawing.Size(346, 370);
            this.pgConfigs.TabIndex = 6;
            this.pgConfigs.ToolbarVisible = false;
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.cbxSlideMonitor);
            this.tabOptions.Controls.Add(this.gbxSlideMonitor);
            this.tabOptions.Controls.Add(this.gbxEntityCache);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(6);
            this.tabOptions.Size = new System.Drawing.Size(364, 388);
            this.tabOptions.Text = "Options";
            // 
            // cbxSlideMonitor
            // 
            this.cbxSlideMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxSlideMonitor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSlideMonitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxSlideMonitor.FormattingEnabled = true;
            this.cbxSlideMonitor.Location = new System.Drawing.Point(196, 114);
            this.cbxSlideMonitor.Name = "cbxSlideMonitor";
            this.cbxSlideMonitor.Size = new System.Drawing.Size(154, 21);
            this.cbxSlideMonitor.TabIndex = 4;
            // 
            // gbxSlideMonitor
            // 
            this.gbxSlideMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxSlideMonitor.Controls.Add(this.tbSlidingAuras);
            this.gbxSlideMonitor.Controls.Add(this.lblSlideFilter);
            this.gbxSlideMonitor.Controls.Add(this.lblSlidingAuras);
            this.gbxSlideMonitor.Controls.Add(this.editSlideTimeout);
            this.gbxSlideMonitor.Controls.Add(this.editSlideFilter);
            this.gbxSlideMonitor.Controls.Add(this.lblSlideTimer);
            this.gbxSlideMonitor.Location = new System.Drawing.Point(9, 117);
            this.gbxSlideMonitor.Name = "gbxSlideMonitor";
            this.gbxSlideMonitor.Size = new System.Drawing.Size(347, 78);
            this.gbxSlideMonitor.TabIndex = 4;
            this.gbxSlideMonitor.TabStop = false;
            this.gbxSlideMonitor.Text = "Slide Monitor";
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
            this.tbSlidingAuras.Size = new System.Drawing.Size(335, 0);
            this.tbSlidingAuras.TabIndex = 3;
            this.tbSlidingAuras.Text = "M10_Becritter_Boat_Costume\r\nVolume_Ground_Slippery\r\nVolume_Ground_Slippery_Player" +
    "only";
            this.tbSlidingAuras.Visible = false;
            // 
            // lblSlideFilter
            // 
            this.lblSlideFilter.AutoSize = true;
            this.lblSlideFilter.Location = new System.Drawing.Point(71, 23);
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
            this.lblSlidingAuras.Visible = false;
            // 
            // editSlideTimeout
            // 
            this.editSlideTimeout.EditValue = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.editSlideTimeout.Location = new System.Drawing.Point(9, 46);
            this.editSlideTimeout.Name = "editSlideTimeout";
            this.editSlideTimeout.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editSlideTimeout.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editSlideTimeout.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.editSlideTimeout.Properties.IsFloatValue = false;
            this.editSlideTimeout.Properties.Mask.EditMask = "N00";
            this.editSlideTimeout.Properties.MaxValue = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.editSlideTimeout.Size = new System.Drawing.Size(56, 20);
            this.editSlideTimeout.TabIndex = 0;
            // 
            // editSlideFilter
            // 
            this.editSlideFilter.EditValue = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.editSlideFilter.Location = new System.Drawing.Point(9, 20);
            this.editSlideFilter.Name = "editSlideFilter";
            this.editSlideFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editSlideFilter.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editSlideFilter.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.editSlideFilter.Properties.IsFloatValue = false;
            this.editSlideFilter.Properties.Mask.EditMask = "N00";
            this.editSlideFilter.Properties.MaxValue = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.editSlideFilter.Size = new System.Drawing.Size(56, 20);
            this.editSlideFilter.TabIndex = 0;
            // 
            // lblSlideTimer
            // 
            this.lblSlideTimer.AutoSize = true;
            this.lblSlideTimer.Location = new System.Drawing.Point(71, 49);
            this.lblSlideTimer.Name = "lblSlideTimer";
            this.lblSlideTimer.Size = new System.Drawing.Size(216, 13);
            this.lblSlideTimer.TabIndex = 2;
            this.lblSlideTimer.Text = "Time between waypoint check (millisecond):";
            // 
            // gbxEntityCache
            // 
            this.gbxEntityCache.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxEntityCache.Controls.Add(this.editCombatCacheTime);
            this.gbxEntityCache.Controls.Add(this.lblCombatCacheTime);
            this.gbxEntityCache.Controls.Add(this.editLocalCacheTime);
            this.gbxEntityCache.Controls.Add(this.lblLocalCacheTime);
            this.gbxEntityCache.Controls.Add(this.editGlobalCacheTime);
            this.gbxEntityCache.Controls.Add(this.lblGlobalCacheTime);
            this.gbxEntityCache.Location = new System.Drawing.Point(9, 9);
            this.gbxEntityCache.Name = "gbxEntityCache";
            this.gbxEntityCache.Size = new System.Drawing.Size(346, 102);
            this.gbxEntityCache.TabIndex = 5;
            this.gbxEntityCache.TabStop = false;
            this.gbxEntityCache.Text = "EntityCache settings";
            // 
            // editCombatCacheTime
            // 
            this.editCombatCacheTime.EditValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.editCombatCacheTime.Location = new System.Drawing.Point(9, 72);
            this.editCombatCacheTime.Name = "editCombatCacheTime";
            this.editCombatCacheTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editCombatCacheTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editCombatCacheTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.editCombatCacheTime.Properties.IsFloatValue = false;
            this.editCombatCacheTime.Properties.Mask.EditMask = "N00";
            this.editCombatCacheTime.Size = new System.Drawing.Size(58, 20);
            this.editCombatCacheTime.TabIndex = 3;
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
            // editLocalCacheTime
            // 
            this.editLocalCacheTime.EditValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.editLocalCacheTime.Location = new System.Drawing.Point(9, 46);
            this.editLocalCacheTime.Name = "editLocalCacheTime";
            this.editLocalCacheTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editLocalCacheTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editLocalCacheTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.editLocalCacheTime.Properties.IsFloatValue = false;
            this.editLocalCacheTime.Properties.Mask.EditMask = "N00";
            this.editLocalCacheTime.Size = new System.Drawing.Size(58, 20);
            this.editLocalCacheTime.TabIndex = 3;
            this.editLocalCacheTime.ToolTip = "Time between the refresh of the local Entities cache of the actions and condition" +
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
            // editGlobalCacheTime
            // 
            this.editGlobalCacheTime.EditValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.editGlobalCacheTime.Location = new System.Drawing.Point(9, 20);
            this.editGlobalCacheTime.Name = "editGlobalCacheTime";
            this.editGlobalCacheTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editGlobalCacheTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editGlobalCacheTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.editGlobalCacheTime.Properties.IsFloatValue = false;
            this.editGlobalCacheTime.Properties.Mask.EditMask = "N00";
            this.editGlobalCacheTime.Size = new System.Drawing.Size(58, 20);
            this.editGlobalCacheTime.TabIndex = 3;
            this.editGlobalCacheTime.ToolTip = "Time between the refresh of the global entity cache";
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
            this.btnGetMachineId.Click += new System.EventHandler(this.handler_GetMachineId);
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
            // tabLogger
            // 
            this.tabLogger.Controls.Add(this.ckbEnableLogger);
            this.tabLogger.Controls.Add(this.gbxLogger);
            this.tabLogger.Name = "tabLogger";
            this.tabLogger.Padding = new System.Windows.Forms.Padding(6);
            this.tabLogger.PageVisible = false;
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
            this.ckbEnableLogger.CheckedChanged += new System.EventHandler(this.handler_EnableLogger);
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
            this.btnOpenLog.Click += new System.EventHandler(this.handler_OpenLogFile);
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.tbDebugMonitorInfo);
            this.tabDebug.Controls.Add(this.ckbDebugMonitor);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(6);
            this.tabDebug.PageVisible = false;
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
            this.ckbDebugMonitor.CheckedChanged += new System.EventHandler(this.handler_DebugMonitorActivate);
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
            this.tabSettings.ResumeLayout(false);
            this.tabOptions.ResumeLayout(false);
            this.gbxSlideMonitor.ResumeLayout(false);
            this.gbxSlideMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editSlideTimeout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editSlideFilter.Properties)).EndInit();
            this.gbxEntityCache.ResumeLayout(false);
            this.gbxEntityCache.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editCombatCacheTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editLocalCacheTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editGlobalCacheTime.Properties)).EndInit();
            this.tabRelogger.ResumeLayout(false);
            this.tabRelogger.PerformLayout();
            this.tabLogger.ResumeLayout(false);
            this.tabLogger.PerformLayout();
            this.gbxLogger.ResumeLayout(false);
            this.gbxLogger.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private FolderBrowserDialog fldrBroserDlg;
        private XtraTabControl tbclMain;
        private XtraTabPage tabUtilities;
        private XtraTabPage tabOptions;
        private CheckBox ckbSpellStuckMonitor;
        private Label lblSlideTimer;
        private Label lblSlideFilter;
        private SpinEdit editSlideFilter;
        private GroupBox gbxSlideMonitor;
        private TextBox tbSlidingAuras;
        private Label lblSlidingAuras;
        private Button btnUiViewer;
        private CheckBox cbEnchantHelperActivator;
        private Button btnTest3;
        private Button btnEntities;
        private XtraTabPage tabRelogger;
        private Label lblMachinId;
        private Label lblAccount;
        private SimpleButton btnGetMachineId;
        private SimpleButton btnSetMachineId;
        private TextBox tbMashingId;
        private Button btnAuraViewer;
        private ComboBox cbxExportSelector;
        private ButtonEdit tbExportFileSelector;
        private SaveFileDialog dlgSaveFile;
        private Button btnExport;
        private GroupBox gbxExport;
        private Button btnDefault;
        private CheckBox ckbMapperPatch;
        private XtraTabPage tabLogger;
        private CheckBox ckbEnableLogger;
        private Button btnOpenLog;
        private GroupBox gbxLogger;
        private GroupBox gbxEntityCache;
        private SpinEdit editGlobalCacheTime;
        private Label lblGlobalCacheTime;
        private SpinEdit editCombatCacheTime;
        private Label lblCombatCacheTime;
        private SpinEdit editLocalCacheTime;
        private Label lblLocalCacheTime;
        private ToolTip toolTip;
        private CheckBox ckbExtendedActionDebugInfo;
        private Button btnCheckCore;
        private XtraTabPage tabDebug;
        private TextBox tbDebugMonitorInfo;
        private CheckBox ckbDebugMonitor;
        private Button btnTest1;
        private Button bntTest2;
        private ComboBox cbxSlideMonitor;
        private SpinEdit editSlideTimeout;
        private PropertyGrid pgConfigs;
        private XtraTabPage tabSettings;
    }
}
