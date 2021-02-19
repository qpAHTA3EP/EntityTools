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
            this.gbxETLog = new System.Windows.Forms.GroupBox();
            this.ckbETLogger = new System.Windows.Forms.CheckBox();
            this.bntOpenLogFile = new System.Windows.Forms.Button();
            this.cbEnchantHelperActivator = new System.Windows.Forms.CheckBox();
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
            this.tabRelogger = new DevExpress.XtraTab.XtraTabPage();
            this.lblMachinId = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.btnGetMachineId = new DevExpress.XtraEditors.SimpleButton();
            this.btnSetMachineId = new DevExpress.XtraEditors.SimpleButton();
            this.tbMashingId = new System.Windows.Forms.TextBox();
            this.tabDebug = new DevExpress.XtraTab.XtraTabPage();
            this.tbDebugMonitorInfo = new System.Windows.Forms.TextBox();
            this.ckbDebugMonitor = new System.Windows.Forms.CheckBox();
            this.tbText = new System.Windows.Forms.TextBox();
            this.btnTest1 = new System.Windows.Forms.Button();
            this.bntTest2 = new System.Windows.Forms.Button();
            this.bntInteract = new System.Windows.Forms.Button();
            this.bntJump = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnTest3 = new System.Windows.Forms.Button();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).BeginInit();
            this.tbclMain.SuspendLayout();
            this.tabUtilities.SuspendLayout();
            this.gbxETLog.SuspendLayout();
            this.gbxExport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbExportFileSelector.Properties)).BeginInit();
            this.tabSettings.SuspendLayout();
            this.tabRelogger.SuspendLayout();
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
            this.tabRelogger,
            this.tabDebug});
            // 
            // tabUtilities
            // 
            this.tabUtilities.Controls.Add(this.gbxETLog);
            this.tabUtilities.Controls.Add(this.cbEnchantHelperActivator);
            this.tabUtilities.Controls.Add(this.btnCheckCore);
            this.tabUtilities.Controls.Add(this.btnUiViewer);
            this.tabUtilities.Controls.Add(this.btnAuraViewer);
            this.tabUtilities.Controls.Add(this.btnEntities);
            this.tabUtilities.Controls.Add(this.ckbSpellStuckMonitor);
            this.tabUtilities.Controls.Add(this.gbxExport);
            this.tabUtilities.Controls.Add(this.ckbMapperPatch);
            this.tabUtilities.Name = "tabUtilities";
            this.tabUtilities.Padding = new System.Windows.Forms.Padding(6);
            this.tabUtilities.Size = new System.Drawing.Size(368, 391);
            this.tabUtilities.Text = "Utilities";
            // 
            // gbxETLog
            // 
            this.gbxETLog.Controls.Add(this.ckbETLogger);
            this.gbxETLog.Controls.Add(this.bntOpenLogFile);
            this.gbxETLog.Location = new System.Drawing.Point(9, 91);
            this.gbxETLog.Name = "gbxETLog";
            this.gbxETLog.Size = new System.Drawing.Size(350, 49);
            this.gbxETLog.TabIndex = 13;
            this.gbxETLog.TabStop = false;
            this.gbxETLog.Text = "Logger";
            // 
            // ckbETLogger
            // 
            this.ckbETLogger.AutoSize = true;
            this.ckbETLogger.Location = new System.Drawing.Point(6, 20);
            this.ckbETLogger.Name = "ckbETLogger";
            this.ckbETLogger.Size = new System.Drawing.Size(150, 17);
            this.ckbETLogger.TabIndex = 3;
            this.ckbETLogger.Text = "Enable EntityTools Logger";
            this.ckbETLogger.UseVisualStyleBackColor = true;
            this.ckbETLogger.CheckedChanged += new System.EventHandler(this.handler_EnableLogger);
            // 
            // bntOpenLogFile
            // 
            this.bntOpenLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOpenLogFile.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.bntOpenLogFile.Location = new System.Drawing.Point(235, 16);
            this.bntOpenLogFile.Name = "bntOpenLogFile";
            this.bntOpenLogFile.Size = new System.Drawing.Size(109, 23);
            this.bntOpenLogFile.TabIndex = 0;
            this.bntOpenLogFile.Text = "Open LogFile";
            this.bntOpenLogFile.UseVisualStyleBackColor = true;
            this.bntOpenLogFile.Click += new System.EventHandler(this.handler_OpenLogFile);
            // 
            // cbEnchantHelperActivator
            // 
            this.cbEnchantHelperActivator.AutoSize = true;
            this.cbEnchantHelperActivator.Location = new System.Drawing.Point(15, 169);
            this.cbEnchantHelperActivator.Name = "cbEnchantHelperActivator";
            this.cbEnchantHelperActivator.Size = new System.Drawing.Size(131, 17);
            this.cbEnchantHelperActivator.TabIndex = 10;
            this.cbEnchantHelperActivator.Text = "Enable EnchantHelper";
            this.cbEnchantHelperActivator.UseVisualStyleBackColor = true;
            this.cbEnchantHelperActivator.CheckedChanged += new System.EventHandler(this.handler_EnchantHelperActivation);
            // 
            // btnCheckCore
            // 
            this.btnCheckCore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckCore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckCore.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCheckCore.Location = new System.Drawing.Point(189, 339);
            this.btnCheckCore.Name = "btnCheckCore";
            this.btnCheckCore.Size = new System.Drawing.Size(170, 40);
            this.btnCheckCore.TabIndex = 0;
            this.btnCheckCore.Text = "Check";
            this.btnCheckCore.UseVisualStyleBackColor = true;
            this.btnCheckCore.Click += new System.EventHandler(this.handler_CheckCore);
            // 
            // btnUiViewer
            // 
            this.btnUiViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUiViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUiViewer.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUiViewer.Location = new System.Drawing.Point(189, 293);
            this.btnUiViewer.Name = "btnUiViewer";
            this.btnUiViewer.Size = new System.Drawing.Size(170, 40);
            this.btnUiViewer.TabIndex = 0;
            this.btnUiViewer.Text = "UI Viewer";
            this.btnUiViewer.UseVisualStyleBackColor = true;
            this.btnUiViewer.Click += new System.EventHandler(this.handler_OpenUiViewer);
            // 
            // btnAuraViewer
            // 
            this.btnAuraViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAuraViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuraViewer.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAuraViewer.Location = new System.Drawing.Point(9, 339);
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
            this.ckbSpellStuckMonitor.Location = new System.Drawing.Point(15, 146);
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
            this.gbxExport.Size = new System.Drawing.Size(350, 82);
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
            this.cbxExportSelector.Size = new System.Drawing.Size(223, 21);
            this.cbxExportSelector.Sorted = true;
            this.cbxExportSelector.TabIndex = 11;
            this.cbxExportSelector.SelectedIndexChanged += new System.EventHandler(this.handler_ChangeExportingData);
            // 
            // tbExportFileSelector
            // 
            this.tbExportFileSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExportFileSelector.EditValue = "";
            this.tbExportFileSelector.Location = new System.Drawing.Point(6, 49);
            this.tbExportFileSelector.Name = "tbExportFileSelector";
            this.tbExportFileSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.tbExportFileSelector.Properties.HideSelection = false;
            this.tbExportFileSelector.Properties.NullText = "Enter the Filename";
            this.tbExportFileSelector.Size = new System.Drawing.Size(338, 20);
            this.tbExportFileSelector.TabIndex = 6;
            this.tbExportFileSelector.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.handler_ChangeExportingFileName);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Location = new System.Drawing.Point(294, 18);
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
            this.btnDefault.Location = new System.Drawing.Point(235, 18);
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
            this.ckbMapperPatch.Location = new System.Drawing.Point(15, 192);
            this.ckbMapperPatch.Name = "ckbMapperPatch";
            this.ckbMapperPatch.Size = new System.Drawing.Size(154, 17);
            this.ckbMapperPatch.TabIndex = 3;
            this.ckbMapperPatch.Text = "Mapper Patch (need relog)";
            this.ckbMapperPatch.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.pgConfigs);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(6);
            this.tabSettings.Size = new System.Drawing.Size(368, 391);
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
            this.tabRelogger.Size = new System.Drawing.Size(368, 391);
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
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.tbDebugMonitorInfo);
            this.tabDebug.Controls.Add(this.ckbDebugMonitor);
            this.tabDebug.Controls.Add(this.tbText);
            this.tabDebug.Controls.Add(this.btnTest1);
            this.tabDebug.Controls.Add(this.bntTest2);
            this.tabDebug.Controls.Add(this.bntInteract);
            this.tabDebug.Controls.Add(this.bntJump);
            this.tabDebug.Controls.Add(this.btnLeft);
            this.tabDebug.Controls.Add(this.btnRight);
            this.tabDebug.Controls.Add(this.btnUp);
            this.tabDebug.Controls.Add(this.btnStop);
            this.tabDebug.Controls.Add(this.btnDown);
            this.tabDebug.Controls.Add(this.btnTest3);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(6);
            this.tabDebug.Size = new System.Drawing.Size(368, 391);
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
            this.tbDebugMonitorInfo.Size = new System.Drawing.Size(346, 265);
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
            // tbText
            // 
            this.tbText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbText.Location = new System.Drawing.Point(67, 304);
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(164, 21);
            this.tbText.TabIndex = 13;
            // 
            // btnTest1
            // 
            this.btnTest1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest1.Location = new System.Drawing.Point(9, 303);
            this.btnTest1.Name = "btnTest1";
            this.btnTest1.Size = new System.Drawing.Size(52, 23);
            this.btnTest1.TabIndex = 2;
            this.btnTest1.Text = "Test 1";
            this.btnTest1.UseVisualStyleBackColor = true;
            this.btnTest1.Click += new System.EventHandler(this.handler_Test_1);
            // 
            // bntTest2
            // 
            this.bntTest2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntTest2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntTest2.Location = new System.Drawing.Point(9, 332);
            this.bntTest2.Name = "bntTest2";
            this.bntTest2.Size = new System.Drawing.Size(52, 23);
            this.bntTest2.TabIndex = 2;
            this.bntTest2.Text = "Test 2";
            this.bntTest2.UseVisualStyleBackColor = true;
            this.bntTest2.Click += new System.EventHandler(this.handler_Test_2);
            // 
            // bntInteract
            // 
            this.bntInteract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntInteract.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntInteract.Location = new System.Drawing.Point(173, 361);
            this.bntInteract.Name = "bntInteract";
            this.bntInteract.Size = new System.Drawing.Size(58, 23);
            this.bntInteract.TabIndex = 2;
            this.bntInteract.Text = "Interact";
            this.bntInteract.UseVisualStyleBackColor = true;
            this.bntInteract.Click += new System.EventHandler(this.handler_Interact);
            // 
            // bntJump
            // 
            this.bntJump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntJump.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntJump.Location = new System.Drawing.Point(301, 361);
            this.bntJump.Name = "bntJump";
            this.bntJump.Size = new System.Drawing.Size(58, 23);
            this.bntJump.TabIndex = 2;
            this.bntJump.Text = "Jump";
            this.bntJump.UseVisualStyleBackColor = true;
            this.bntJump.Click += new System.EventHandler(this.handler_Jump);
            // 
            // btnLeft
            // 
            this.btnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Location = new System.Drawing.Point(173, 332);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(58, 23);
            this.btnLeft.TabIndex = 2;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.handler_Left);
            // 
            // btnRight
            // 
            this.btnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Location = new System.Drawing.Point(301, 332);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(58, 23);
            this.btnRight.TabIndex = 2;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.handler_Right);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Location = new System.Drawing.Point(237, 303);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(58, 23);
            this.btnUp.TabIndex = 2;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.handler_Up);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(237, 332);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(58, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.handler_Stop);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Location = new System.Drawing.Point(237, 361);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(58, 23);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.handler_Down);
            // 
            // btnTest3
            // 
            this.btnTest3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest3.Location = new System.Drawing.Point(9, 361);
            this.btnTest3.Name = "btnTest3";
            this.btnTest3.Size = new System.Drawing.Size(52, 23);
            this.btnTest3.TabIndex = 2;
            this.btnTest3.Text = "Test 3";
            this.btnTest3.UseVisualStyleBackColor = true;
            this.btnTest3.Click += new System.EventHandler(this.handler_Test_3);
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
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).EndInit();
            this.tbclMain.ResumeLayout(false);
            this.tabUtilities.ResumeLayout(false);
            this.tabUtilities.PerformLayout();
            this.gbxETLog.ResumeLayout(false);
            this.gbxETLog.PerformLayout();
            this.gbxExport.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbExportFileSelector.Properties)).EndInit();
            this.tabSettings.ResumeLayout(false);
            this.tabRelogger.ResumeLayout(false);
            this.tabRelogger.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private FolderBrowserDialog fldrBroserDlg;
        private XtraTabControl tbclMain;
        private XtraTabPage tabUtilities;
        private CheckBox ckbSpellStuckMonitor;
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
        private ToolTip toolTip;
        private Button btnCheckCore;
        private XtraTabPage tabDebug;
        private TextBox tbDebugMonitorInfo;
        private CheckBox ckbDebugMonitor;
        private Button btnTest1;
        private Button bntTest2;
        private PropertyGrid pgConfigs;
        private XtraTabPage tabSettings;
        private CheckBox ckbETLogger;
        private TextBox tbText;
        private Button btnRight;
        private Button btnUp;
        private Button btnStop;
        private Button btnDown;
        private Button btnLeft;
        private Button bntJump;
        private Button bntInteract;
        private Button bntOpenLogFile;
        private GroupBox gbxETLog;
    }
}
