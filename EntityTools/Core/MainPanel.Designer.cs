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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.fldrBroserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.tbclMain = new DevExpress.XtraTab.XtraTabControl();
            this.tabUtilities = new DevExpress.XtraTab.XtraTabPage();
            this.gbxETLog = new System.Windows.Forms.GroupBox();
            this.ckbETLogger = new System.Windows.Forms.CheckBox();
            this.bntOpenLogFile = new System.Windows.Forms.Button();
            this.cbEnchantHelperActivator = new System.Windows.Forms.CheckBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnTeamMonitor = new System.Windows.Forms.Button();
            this.btnUiViewer = new System.Windows.Forms.Button();
            this.btnQuesterEditor = new System.Windows.Forms.Button();
            this.btnUcc = new System.Windows.Forms.Button();
            this.btnAuraViewer = new System.Windows.Forms.Button();
            this.btnMissionMonitor = new System.Windows.Forms.Button();
            this.btnEntityCacheMonitor = new System.Windows.Forms.Button();
            this.btnEntities = new System.Windows.Forms.Button();
            this.ckbSpellStuckMonitor = new System.Windows.Forms.CheckBox();
            this.gbxExport = new System.Windows.Forms.GroupBox();
            this.tbExportFileSelector = new DevExpress.XtraEditors.ButtonEdit();
            this.cbxExportSelector = new DevExpress.XtraEditors.ComboBoxEdit();
            this.btnExport = new System.Windows.Forms.Button();
            this.ckbMapperPatch = new System.Windows.Forms.CheckBox();
            this.tabSettings = new DevExpress.XtraTab.XtraTabPage();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.pgConfigs = new System.Windows.Forms.PropertyGrid();
            this.tabRelogger = new DevExpress.XtraTab.XtraTabPage();
            this.lblMachinId = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.btnCredentialManager = new DevExpress.XtraEditors.SimpleButton();
            this.btnGetMachineId = new DevExpress.XtraEditors.SimpleButton();
            this.btnSetMachineId = new DevExpress.XtraEditors.SimpleButton();
            this.tbMashineId = new System.Windows.Forms.TextBox();
            this.tabDebug = new DevExpress.XtraTab.XtraTabPage();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
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
            this.tabProfilePreprocessing = new DevExpress.XtraTab.XtraTabPage();
            this.bntSaveQuesterProfilePreprocessor = new DevExpress.XtraEditors.SimpleButton();
            this.btnTestProcessingItem = new System.Windows.Forms.Button();
            this.btnClearProcessingItem = new System.Windows.Forms.Button();
            this.btnDeleteProcessingItem = new System.Windows.Forms.Button();
            this.btnHelpQuesterProfilePreprocessor = new System.Windows.Forms.Button();
            this.btnAddProcessingItem = new System.Windows.Forms.Button();
            this.btnExportQuesterProfilePreprocessor = new System.Windows.Forms.Button();
            this.btnImportQuesterProfilePreprocessor = new System.Windows.Forms.Button();
            this.gridReplacements = new DevExpress.XtraGrid.GridControl();
            this.gridViewPreprocessing = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.clmnType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.clmnPattern = new DevExpress.XtraGrid.Columns.GridColumn();
            this.clmnReplacement = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ckbAutoSavePreprocessedProfile = new System.Windows.Forms.CheckBox();
            this.ckbEnapleQuesterProfilePreprocessing = new System.Windows.Forms.CheckBox();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.lblVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).BeginInit();
            this.tbclMain.SuspendLayout();
            this.tabUtilities.SuspendLayout();
            this.gbxETLog.SuspendLayout();
            this.gbxExport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbExportFileSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbxExportSelector.Properties)).BeginInit();
            this.tabSettings.SuspendLayout();
            this.tabRelogger.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.tabProfilePreprocessing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReplacements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPreprocessing)).BeginInit();
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
            this.tabDebug,
            this.tabProfilePreprocessing});
            // 
            // tabUtilities
            // 
            this.tabUtilities.Controls.Add(this.gbxETLog);
            this.tabUtilities.Controls.Add(this.cbEnchantHelperActivator);
            this.tabUtilities.Controls.Add(this.btnTest);
            this.tabUtilities.Controls.Add(this.btnTeamMonitor);
            this.tabUtilities.Controls.Add(this.btnUiViewer);
            this.tabUtilities.Controls.Add(this.btnQuesterEditor);
            this.tabUtilities.Controls.Add(this.btnUcc);
            this.tabUtilities.Controls.Add(this.btnAuraViewer);
            this.tabUtilities.Controls.Add(this.btnMissionMonitor);
            this.tabUtilities.Controls.Add(this.btnEntityCacheMonitor);
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
            this.gbxETLog.Padding = new System.Windows.Forms.Padding(6);
            this.gbxETLog.Size = new System.Drawing.Size(350, 49);
            this.gbxETLog.TabIndex = 13;
            this.gbxETLog.TabStop = false;
            this.gbxETLog.Text = "Logger";
            // 
            // ckbETLogger
            // 
            this.ckbETLogger.AutoSize = true;
            this.ckbETLogger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckbETLogger.Location = new System.Drawing.Point(10, 23);
            this.ckbETLogger.Name = "ckbETLogger";
            this.ckbETLogger.Size = new System.Drawing.Size(147, 17);
            this.ckbETLogger.TabIndex = 3;
            this.ckbETLogger.Text = "Enable EntityTools Logger";
            this.ckbETLogger.UseVisualStyleBackColor = true;
            this.ckbETLogger.CheckedChanged += new System.EventHandler(this.handler_EnableLogger);
            // 
            // bntOpenLogFile
            // 
            this.bntOpenLogFile.FlatAppearance.BorderSize = 0;
            this.bntOpenLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOpenLogFile.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.bntOpenLogFile.Image = global::EntityTools.Properties.Resources.Load;
            this.bntOpenLogFile.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.bntOpenLogFile.Location = new System.Drawing.Point(246, 17);
            this.bntOpenLogFile.Name = "bntOpenLogFile";
            this.bntOpenLogFile.Size = new System.Drawing.Size(95, 23);
            this.bntOpenLogFile.TabIndex = 0;
            this.bntOpenLogFile.Text = "Open LogFile";
            this.bntOpenLogFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bntOpenLogFile.UseVisualStyleBackColor = true;
            this.bntOpenLogFile.Click += new System.EventHandler(this.handler_OpenLogFile);
            // 
            // cbEnchantHelperActivator
            // 
            this.cbEnchantHelperActivator.AutoSize = true;
            this.cbEnchantHelperActivator.Enabled = false;
            this.cbEnchantHelperActivator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbEnchantHelperActivator.Location = new System.Drawing.Point(231, 146);
            this.cbEnchantHelperActivator.Name = "cbEnchantHelperActivator";
            this.cbEnchantHelperActivator.Size = new System.Drawing.Size(128, 17);
            this.cbEnchantHelperActivator.TabIndex = 10;
            this.cbEnchantHelperActivator.Text = "Enable EnchantHelper";
            this.cbEnchantHelperActivator.UseVisualStyleBackColor = true;
            this.cbEnchantHelperActivator.Visible = false;
            this.cbEnchantHelperActivator.CheckedChanged += new System.EventHandler(this.handler_EnchantHelperActivation);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnTest.Location = new System.Drawing.Point(246, 204);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(113, 40);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "TEST";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.handler_Test_1);
            // 
            // btnTeamMonitor
            // 
            this.btnTeamMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTeamMonitor.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnTeamMonitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeamMonitor.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnTeamMonitor.Location = new System.Drawing.Point(245, 250);
            this.btnTeamMonitor.Name = "btnTeamMonitor";
            this.btnTeamMonitor.Size = new System.Drawing.Size(113, 40);
            this.btnTeamMonitor.TabIndex = 0;
            this.btnTeamMonitor.Text = "Team";
            this.btnTeamMonitor.UseVisualStyleBackColor = true;
            this.btnTeamMonitor.Click += new System.EventHandler(this.handler_TeamMonitor);
            // 
            // btnUiViewer
            // 
            this.btnUiViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUiViewer.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnUiViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUiViewer.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUiViewer.Location = new System.Drawing.Point(9, 342);
            this.btnUiViewer.Name = "btnUiViewer";
            this.btnUiViewer.Size = new System.Drawing.Size(113, 40);
            this.btnUiViewer.TabIndex = 0;
            this.btnUiViewer.Text = "Game UI";
            this.btnUiViewer.UseVisualStyleBackColor = true;
            this.btnUiViewer.Click += new System.EventHandler(this.handler_OpenUiViewer);
            // 
            // btnQuesterEditor
            // 
            this.btnQuesterEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnQuesterEditor.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnQuesterEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuesterEditor.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnQuesterEditor.Location = new System.Drawing.Point(9, 250);
            this.btnQuesterEditor.Name = "btnQuesterEditor";
            this.btnQuesterEditor.Size = new System.Drawing.Size(113, 40);
            this.btnQuesterEditor.TabIndex = 0;
            this.btnQuesterEditor.Text = "Quester";
            this.btnQuesterEditor.UseVisualStyleBackColor = true;
            this.btnQuesterEditor.Click += new System.EventHandler(this.handler_EditQuester);
            // 
            // btnUcc
            // 
            this.btnUcc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUcc.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnUcc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUcc.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUcc.Location = new System.Drawing.Point(126, 250);
            this.btnUcc.Name = "btnUcc";
            this.btnUcc.Size = new System.Drawing.Size(113, 40);
            this.btnUcc.TabIndex = 0;
            this.btnUcc.Text = "UCC";
            this.btnUcc.UseVisualStyleBackColor = true;
            this.btnUcc.Click += new System.EventHandler(this.handler_EditUcc);
            // 
            // btnAuraViewer
            // 
            this.btnAuraViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAuraViewer.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnAuraViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuraViewer.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAuraViewer.Location = new System.Drawing.Point(126, 342);
            this.btnAuraViewer.Name = "btnAuraViewer";
            this.btnAuraViewer.Size = new System.Drawing.Size(114, 40);
            this.btnAuraViewer.TabIndex = 0;
            this.btnAuraViewer.Text = "Auras";
            this.btnAuraViewer.UseVisualStyleBackColor = true;
            this.btnAuraViewer.Click += new System.EventHandler(this.handler_OpenAuraViewer);
            // 
            // btnMissionMonitor
            // 
            this.btnMissionMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMissionMonitor.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnMissionMonitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMissionMonitor.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMissionMonitor.Location = new System.Drawing.Point(245, 342);
            this.btnMissionMonitor.Name = "btnMissionMonitor";
            this.btnMissionMonitor.Size = new System.Drawing.Size(113, 40);
            this.btnMissionMonitor.TabIndex = 0;
            this.btnMissionMonitor.Text = "Mission";
            this.btnMissionMonitor.UseVisualStyleBackColor = true;
            this.btnMissionMonitor.Click += new System.EventHandler(this.handler_OpenMissionMonitor);
            // 
            // btnEntityCacheMonitor
            // 
            this.btnEntityCacheMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEntityCacheMonitor.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnEntityCacheMonitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntityCacheMonitor.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.btnEntityCacheMonitor.Location = new System.Drawing.Point(126, 296);
            this.btnEntityCacheMonitor.Name = "btnEntityCacheMonitor";
            this.btnEntityCacheMonitor.Size = new System.Drawing.Size(232, 40);
            this.btnEntityCacheMonitor.TabIndex = 0;
            this.btnEntityCacheMonitor.Text = "Entity cache monitor";
            this.btnEntityCacheMonitor.UseVisualStyleBackColor = true;
            this.btnEntityCacheMonitor.Click += new System.EventHandler(this.handler_EntityCacheMonitor);
            // 
            // btnEntities
            // 
            this.btnEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEntities.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnEntities.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntities.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEntities.Location = new System.Drawing.Point(9, 296);
            this.btnEntities.Name = "btnEntities";
            this.btnEntities.Size = new System.Drawing.Size(113, 40);
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
            this.ckbSpellStuckMonitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckbSpellStuckMonitor.Location = new System.Drawing.Point(18, 146);
            this.ckbSpellStuckMonitor.Name = "ckbSpellStuckMonitor";
            this.ckbSpellStuckMonitor.Size = new System.Drawing.Size(142, 17);
            this.ckbSpellStuckMonitor.TabIndex = 1;
            this.ckbSpellStuckMonitor.Text = "Enable SpellStuckMonitor";
            this.ckbSpellStuckMonitor.UseVisualStyleBackColor = true;
            this.ckbSpellStuckMonitor.CheckedChanged += new System.EventHandler(this.handler_SpellStuckMonitorActivation);
            // 
            // gbxExport
            // 
            this.gbxExport.Controls.Add(this.tbExportFileSelector);
            this.gbxExport.Controls.Add(this.cbxExportSelector);
            this.gbxExport.Controls.Add(this.btnExport);
            this.gbxExport.Location = new System.Drawing.Point(9, 3);
            this.gbxExport.Name = "gbxExport";
            this.gbxExport.Padding = new System.Windows.Forms.Padding(6);
            this.gbxExport.Size = new System.Drawing.Size(350, 82);
            this.gbxExport.TabIndex = 12;
            this.gbxExport.TabStop = false;
            this.gbxExport.Text = "Export";
            // 
            // tbExportFileSelector
            // 
            this.tbExportFileSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExportFileSelector.EditValue = "";
            this.tbExportFileSelector.Location = new System.Drawing.Point(9, 52);
            this.tbExportFileSelector.Name = "tbExportFileSelector";
            this.tbExportFileSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Undo, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Restore defaul export file", null, null, DevExpress.Utils.ToolTipAnchor.Default),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Set export file location", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.tbExportFileSelector.Properties.HideSelection = false;
            this.tbExportFileSelector.Properties.NullText = "Enter the Filename";
            this.tbExportFileSelector.Size = new System.Drawing.Size(332, 20);
            this.tbExportFileSelector.TabIndex = 6;
            this.tbExportFileSelector.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.handler_ChangeExportingFileName);
            // 
            // cbxExportSelector
            // 
            this.cbxExportSelector.Location = new System.Drawing.Point(9, 20);
            this.cbxExportSelector.Name = "cbxExportSelector";
            this.cbxExportSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbxExportSelector.Properties.Sorted = true;
            this.cbxExportSelector.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbxExportSelector.Size = new System.Drawing.Size(303, 20);
            this.cbxExportSelector.TabIndex = 14;
            this.cbxExportSelector.SelectedIndexChanged += new System.EventHandler(this.handler_ChangeExportingData);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Image = global::EntityTools.Properties.Resources.SaveTo;
            this.btnExport.Location = new System.Drawing.Point(318, 21);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(23, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.handler_Export);
            // 
            // ckbMapperPatch
            // 
            this.ckbMapperPatch.AutoSize = true;
            this.ckbMapperPatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckbMapperPatch.Location = new System.Drawing.Point(18, 169);
            this.ckbMapperPatch.Name = "ckbMapperPatch";
            this.ckbMapperPatch.Size = new System.Drawing.Size(151, 17);
            this.ckbMapperPatch.TabIndex = 3;
            this.ckbMapperPatch.Text = "Mapper Patch (need relog)";
            this.ckbMapperPatch.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.lblVersion);
            this.tabSettings.Controls.Add(this.btnSave);
            this.tabSettings.Controls.Add(this.pgConfigs);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(6);
            this.tabSettings.Size = new System.Drawing.Size(368, 391);
            this.tabSettings.Text = "Settings";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.ImageOptions.Image = global::EntityTools.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(308, 359);
            this.btnSave.Name = "btnSave";
            this.btnSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSave.Size = new System.Drawing.Size(51, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.handler_SaveSettings);
            // 
            // pgConfigs
            // 
            this.pgConfigs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgConfigs.HelpBackColor = System.Drawing.SystemColors.Window;
            this.pgConfigs.Location = new System.Drawing.Point(9, 9);
            this.pgConfigs.Name = "pgConfigs";
            this.pgConfigs.Size = new System.Drawing.Size(350, 344);
            this.pgConfigs.TabIndex = 6;
            this.pgConfigs.ToolbarVisible = false;
            // 
            // tabRelogger
            // 
            this.tabRelogger.Controls.Add(this.lblMachinId);
            this.tabRelogger.Controls.Add(this.lblAccount);
            this.tabRelogger.Controls.Add(this.btnCredentialManager);
            this.tabRelogger.Controls.Add(this.btnGetMachineId);
            this.tabRelogger.Controls.Add(this.btnSetMachineId);
            this.tabRelogger.Controls.Add(this.tbMashineId);
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
            // btnCredentialManager
            // 
            this.btnCredentialManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCredentialManager.ImageOptions.Image = global::EntityTools.Properties.Resources.Customization;
            this.btnCredentialManager.Location = new System.Drawing.Point(199, 11);
            this.btnCredentialManager.Name = "btnCredentialManager";
            this.btnCredentialManager.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCredentialManager.Size = new System.Drawing.Size(157, 23);
            this.btnCredentialManager.TabIndex = 1;
            this.btnCredentialManager.Text = "Open CredentialManager";
            this.btnCredentialManager.Click += new System.EventHandler(this.handler_OpenCredentialManager);
            // 
            // btnGetMachineId
            // 
            this.btnGetMachineId.ImageOptions.Image = global::EntityTools.Properties.Resources.Import;
            this.btnGetMachineId.Location = new System.Drawing.Point(250, 67);
            this.btnGetMachineId.Name = "btnGetMachineId";
            this.btnGetMachineId.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnGetMachineId.Size = new System.Drawing.Size(50, 23);
            this.btnGetMachineId.TabIndex = 1;
            this.btnGetMachineId.Text = "Get";
            this.btnGetMachineId.Click += new System.EventHandler(this.handler_GetMachineId);
            // 
            // btnSetMachineId
            // 
            this.btnSetMachineId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetMachineId.ImageOptions.Image = global::EntityTools.Properties.Resources.Export;
            this.btnSetMachineId.Location = new System.Drawing.Point(306, 67);
            this.btnSetMachineId.Name = "btnSetMachineId";
            this.btnSetMachineId.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSetMachineId.Size = new System.Drawing.Size(50, 23);
            this.btnSetMachineId.TabIndex = 1;
            this.btnSetMachineId.Text = "Set";
            this.btnSetMachineId.Click += new System.EventHandler(this.handler_SetMachineId);
            // 
            // tbMashineId
            // 
            this.tbMashineId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMashineId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbMashineId.Location = new System.Drawing.Point(68, 40);
            this.tbMashineId.Name = "tbMashineId";
            this.tbMashineId.Size = new System.Drawing.Size(288, 21);
            this.tbMashineId.TabIndex = 0;
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.propertyGrid);
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
            // propertyGrid
            // 
            this.propertyGrid.CommandsVisibleIfAvailable = false;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(9, 167);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(346, 130);
            this.propertyGrid.TabIndex = 14;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // tbDebugMonitorInfo
            // 
            this.tbDebugMonitorInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDebugMonitorInfo.BackColor = System.Drawing.SystemColors.Window;
            this.tbDebugMonitorInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDebugMonitorInfo.Location = new System.Drawing.Point(9, 32);
            this.tbDebugMonitorInfo.Multiline = true;
            this.tbDebugMonitorInfo.Name = "tbDebugMonitorInfo";
            this.tbDebugMonitorInfo.ReadOnly = true;
            this.tbDebugMonitorInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDebugMonitorInfo.Size = new System.Drawing.Size(346, 129);
            this.tbDebugMonitorInfo.TabIndex = 9;
            // 
            // ckbDebugMonitor
            // 
            this.ckbDebugMonitor.AutoSize = true;
            this.ckbDebugMonitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckbDebugMonitor.Location = new System.Drawing.Point(9, 9);
            this.ckbDebugMonitor.Name = "ckbDebugMonitor";
            this.ckbDebugMonitor.Size = new System.Drawing.Size(125, 17);
            this.ckbDebugMonitor.TabIndex = 8;
            this.ckbDebugMonitor.Text = "Enable DebugMonitor";
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
            this.tbText.Text = "Enchantment_Standard_B_R1";
            // 
            // btnTest1
            // 
            this.btnTest1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest1.FlatAppearance.BorderSize = 0;
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
            this.bntTest2.FlatAppearance.BorderSize = 0;
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
            this.bntInteract.FlatAppearance.BorderSize = 0;
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
            this.bntJump.FlatAppearance.BorderSize = 0;
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
            this.btnLeft.FlatAppearance.BorderSize = 0;
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
            this.btnRight.FlatAppearance.BorderSize = 0;
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
            this.btnUp.FlatAppearance.BorderSize = 0;
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
            this.btnStop.FlatAppearance.BorderSize = 0;
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
            this.btnDown.FlatAppearance.BorderSize = 0;
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
            this.btnTest3.FlatAppearance.BorderSize = 0;
            this.btnTest3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest3.Location = new System.Drawing.Point(9, 361);
            this.btnTest3.Name = "btnTest3";
            this.btnTest3.Size = new System.Drawing.Size(52, 23);
            this.btnTest3.TabIndex = 2;
            this.btnTest3.Text = "Test 3";
            this.btnTest3.UseVisualStyleBackColor = true;
            this.btnTest3.Click += new System.EventHandler(this.handler_Test_3);
            // 
            // tabProfilePreprocessing
            // 
            this.tabProfilePreprocessing.Controls.Add(this.bntSaveQuesterProfilePreprocessor);
            this.tabProfilePreprocessing.Controls.Add(this.btnTestProcessingItem);
            this.tabProfilePreprocessing.Controls.Add(this.btnClearProcessingItem);
            this.tabProfilePreprocessing.Controls.Add(this.btnDeleteProcessingItem);
            this.tabProfilePreprocessing.Controls.Add(this.btnHelpQuesterProfilePreprocessor);
            this.tabProfilePreprocessing.Controls.Add(this.btnAddProcessingItem);
            this.tabProfilePreprocessing.Controls.Add(this.btnExportQuesterProfilePreprocessor);
            this.tabProfilePreprocessing.Controls.Add(this.btnImportQuesterProfilePreprocessor);
            this.tabProfilePreprocessing.Controls.Add(this.gridReplacements);
            this.tabProfilePreprocessing.Controls.Add(this.ckbAutoSavePreprocessedProfile);
            this.tabProfilePreprocessing.Controls.Add(this.ckbEnapleQuesterProfilePreprocessing);
            this.tabProfilePreprocessing.Name = "tabProfilePreprocessing";
            this.tabProfilePreprocessing.Padding = new System.Windows.Forms.Padding(6);
            this.tabProfilePreprocessing.Size = new System.Drawing.Size(368, 391);
            this.tabProfilePreprocessing.Text = "Profile Preprocessing";
            // 
            // bntSaveQuesterProfilePreprocessor
            // 
            this.bntSaveQuesterProfilePreprocessor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntSaveQuesterProfilePreprocessor.ImageOptions.Image = global::EntityTools.Properties.Resources.Save;
            this.bntSaveQuesterProfilePreprocessor.Location = new System.Drawing.Point(308, 359);
            this.bntSaveQuesterProfilePreprocessor.Name = "bntSaveQuesterProfilePreprocessor";
            this.bntSaveQuesterProfilePreprocessor.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.bntSaveQuesterProfilePreprocessor.Size = new System.Drawing.Size(51, 23);
            this.bntSaveQuesterProfilePreprocessor.TabIndex = 8;
            this.bntSaveQuesterProfilePreprocessor.Text = "Save";
            this.bntSaveQuesterProfilePreprocessor.Click += new System.EventHandler(this.handler_SaveQuesterProfilePreprocessor);
            // 
            // btnTestProcessingItem
            // 
            this.btnTestProcessingItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTestProcessingItem.FlatAppearance.BorderSize = 0;
            this.btnTestProcessingItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestProcessingItem.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnTestProcessingItem.Image = global::EntityTools.Properties.Resources.PlayAll;
            this.btnTestProcessingItem.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnTestProcessingItem.Location = new System.Drawing.Point(102, 359);
            this.btnTestProcessingItem.Name = "btnTestProcessingItem";
            this.btnTestProcessingItem.Size = new System.Drawing.Size(23, 23);
            this.btnTestProcessingItem.TabIndex = 3;
            this.btnTestProcessingItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnTestProcessingItem, "Test selected processing Item");
            this.btnTestProcessingItem.UseVisualStyleBackColor = true;
            this.btnTestProcessingItem.Click += new System.EventHandler(this.handler_TestProcessingItem);
            // 
            // btnClearProcessingItem
            // 
            this.btnClearProcessingItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearProcessingItem.FlatAppearance.BorderSize = 0;
            this.btnClearProcessingItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearProcessingItem.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnClearProcessingItem.Image = global::EntityTools.Properties.Resources.Trashcan;
            this.btnClearProcessingItem.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnClearProcessingItem.Location = new System.Drawing.Point(60, 359);
            this.btnClearProcessingItem.Name = "btnClearProcessingItem";
            this.btnClearProcessingItem.Size = new System.Drawing.Size(23, 23);
            this.btnClearProcessingItem.TabIndex = 3;
            this.btnClearProcessingItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnClearProcessingItem, "Clear processing Item list");
            this.btnClearProcessingItem.UseVisualStyleBackColor = true;
            this.btnClearProcessingItem.Click += new System.EventHandler(this.handler_ClearProcessingItems);
            // 
            // btnDeleteProcessingItem
            // 
            this.btnDeleteProcessingItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteProcessingItem.FlatAppearance.BorderSize = 0;
            this.btnDeleteProcessingItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteProcessingItem.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnDeleteProcessingItem.Image = global::EntityTools.Properties.Resources.Cancel;
            this.btnDeleteProcessingItem.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDeleteProcessingItem.Location = new System.Drawing.Point(34, 359);
            this.btnDeleteProcessingItem.Name = "btnDeleteProcessingItem";
            this.btnDeleteProcessingItem.Size = new System.Drawing.Size(23, 23);
            this.btnDeleteProcessingItem.TabIndex = 3;
            this.btnDeleteProcessingItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnDeleteProcessingItem, "Delete selected processing Item");
            this.btnDeleteProcessingItem.UseVisualStyleBackColor = true;
            this.btnDeleteProcessingItem.Click += new System.EventHandler(this.handler_DeleteProcessingItem);
            // 
            // btnHelpQuesterProfilePreprocessor
            // 
            this.btnHelpQuesterProfilePreprocessor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHelpQuesterProfilePreprocessor.FlatAppearance.BorderSize = 0;
            this.btnHelpQuesterProfilePreprocessor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelpQuesterProfilePreprocessor.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnHelpQuesterProfilePreprocessor.Image = global::EntityTools.Properties.Resources.Eye;
            this.btnHelpQuesterProfilePreprocessor.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnHelpQuesterProfilePreprocessor.Location = new System.Drawing.Point(336, 9);
            this.btnHelpQuesterProfilePreprocessor.Name = "btnHelpQuesterProfilePreprocessor";
            this.btnHelpQuesterProfilePreprocessor.Size = new System.Drawing.Size(23, 23);
            this.btnHelpQuesterProfilePreprocessor.TabIndex = 3;
            this.btnHelpQuesterProfilePreprocessor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHelpQuesterProfilePreprocessor.UseVisualStyleBackColor = true;
            this.btnHelpQuesterProfilePreprocessor.Click += new System.EventHandler(this.handler_Help);
            // 
            // btnAddProcessingItem
            // 
            this.btnAddProcessingItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddProcessingItem.FlatAppearance.BorderSize = 0;
            this.btnAddProcessingItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddProcessingItem.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnAddProcessingItem.Image = global::EntityTools.Properties.Resources.Add;
            this.btnAddProcessingItem.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAddProcessingItem.Location = new System.Drawing.Point(8, 359);
            this.btnAddProcessingItem.Name = "btnAddProcessingItem";
            this.btnAddProcessingItem.Size = new System.Drawing.Size(23, 23);
            this.btnAddProcessingItem.TabIndex = 3;
            this.btnAddProcessingItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnAddProcessingItem, "Add new processing Item");
            this.btnAddProcessingItem.UseVisualStyleBackColor = true;
            this.btnAddProcessingItem.Click += new System.EventHandler(this.handler_AddProcessingItem);
            // 
            // btnExportQuesterProfilePreprocessor
            // 
            this.btnExportQuesterProfilePreprocessor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportQuesterProfilePreprocessor.FlatAppearance.BorderSize = 0;
            this.btnExportQuesterProfilePreprocessor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportQuesterProfilePreprocessor.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnExportQuesterProfilePreprocessor.Image = global::EntityTools.Properties.Resources.Export;
            this.btnExportQuesterProfilePreprocessor.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnExportQuesterProfilePreprocessor.Location = new System.Drawing.Point(237, 359);
            this.btnExportQuesterProfilePreprocessor.Name = "btnExportQuesterProfilePreprocessor";
            this.btnExportQuesterProfilePreprocessor.Size = new System.Drawing.Size(65, 23);
            this.btnExportQuesterProfilePreprocessor.TabIndex = 3;
            this.btnExportQuesterProfilePreprocessor.Text = "Export";
            this.btnExportQuesterProfilePreprocessor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportQuesterProfilePreprocessor.UseVisualStyleBackColor = true;
            this.btnExportQuesterProfilePreprocessor.Click += new System.EventHandler(this.handler_ExportPreprocessingProfile);
            // 
            // btnImportQuesterProfilePreprocessor
            // 
            this.btnImportQuesterProfilePreprocessor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportQuesterProfilePreprocessor.FlatAppearance.BorderSize = 0;
            this.btnImportQuesterProfilePreprocessor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportQuesterProfilePreprocessor.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnImportQuesterProfilePreprocessor.Image = global::EntityTools.Properties.Resources.Import;
            this.btnImportQuesterProfilePreprocessor.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnImportQuesterProfilePreprocessor.Location = new System.Drawing.Point(166, 359);
            this.btnImportQuesterProfilePreprocessor.Name = "btnImportQuesterProfilePreprocessor";
            this.btnImportQuesterProfilePreprocessor.Size = new System.Drawing.Size(65, 23);
            this.btnImportQuesterProfilePreprocessor.TabIndex = 3;
            this.btnImportQuesterProfilePreprocessor.Text = "Import";
            this.btnImportQuesterProfilePreprocessor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportQuesterProfilePreprocessor.UseVisualStyleBackColor = true;
            this.btnImportQuesterProfilePreprocessor.Click += new System.EventHandler(this.handler_ImportPreprocessingProfile);
            // 
            // gridReplacements
            // 
            this.gridReplacements.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridReplacements.EmbeddedNavigator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gridReplacements.Location = new System.Drawing.Point(8, 55);
            this.gridReplacements.MainView = this.gridViewPreprocessing;
            this.gridReplacements.Name = "gridReplacements";
            this.gridReplacements.Size = new System.Drawing.Size(351, 298);
            this.gridReplacements.TabIndex = 2;
            this.gridReplacements.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPreprocessing});
            // 
            // gridViewPreprocessing
            // 
            this.gridViewPreprocessing.Appearance.Row.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gridViewPreprocessing.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.clmnType,
            this.clmnPattern,
            this.clmnReplacement});
            this.gridViewPreprocessing.GridControl = this.gridReplacements;
            this.gridViewPreprocessing.Name = "gridViewPreprocessing";
            this.gridViewPreprocessing.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridViewPreprocessing.OptionsMenu.ShowAddNewSummaryItem = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewPreprocessing.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gridViewPreprocessing.OptionsNavigation.AutoFocusNewRow = true;
            this.gridViewPreprocessing.OptionsView.EnableAppearanceOddRow = true;
            this.gridViewPreprocessing.OptionsView.ShowGroupPanel = false;
            this.gridViewPreprocessing.OptionsView.ShowIndicator = false;
            this.gridViewPreprocessing.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False;
            // 
            // clmnType
            // 
            this.clmnType.Caption = "Type";
            this.clmnType.FieldName = "Type";
            this.clmnType.MaxWidth = 60;
            this.clmnType.MinWidth = 60;
            this.clmnType.Name = "clmnType";
            this.clmnType.OptionsColumn.AllowSize = false;
            this.clmnType.Visible = true;
            this.clmnType.VisibleIndex = 0;
            this.clmnType.Width = 60;
            // 
            // clmnPattern
            // 
            this.clmnPattern.Caption = "Pattern";
            this.clmnPattern.FieldName = "Pattern";
            this.clmnPattern.Name = "clmnPattern";
            this.clmnPattern.Visible = true;
            this.clmnPattern.VisibleIndex = 1;
            // 
            // clmnReplacement
            // 
            this.clmnReplacement.Caption = "Replacement";
            this.clmnReplacement.FieldName = "Replacement";
            this.clmnReplacement.Name = "clmnReplacement";
            this.clmnReplacement.Visible = true;
            this.clmnReplacement.VisibleIndex = 2;
            // 
            // ckbAutoSavePreprocessedProfile
            // 
            this.ckbAutoSavePreprocessedProfile.AutoSize = true;
            this.ckbAutoSavePreprocessedProfile.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.ckbAutoSavePreprocessedProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckbAutoSavePreprocessedProfile.Location = new System.Drawing.Point(8, 32);
            this.ckbAutoSavePreprocessedProfile.Name = "ckbAutoSavePreprocessedProfile";
            this.ckbAutoSavePreprocessedProfile.Size = new System.Drawing.Size(216, 17);
            this.ckbAutoSavePreprocessedProfile.TabIndex = 1;
            this.ckbAutoSavePreprocessedProfile.Text = "Auto save preprocessed Quester-profile";
            this.ckbAutoSavePreprocessedProfile.UseVisualStyleBackColor = true;
            // 
            // ckbEnapleQuesterProfilePreprocessing
            // 
            this.ckbEnapleQuesterProfilePreprocessing.AutoSize = true;
            this.ckbEnapleQuesterProfilePreprocessing.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.ckbEnapleQuesterProfilePreprocessing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckbEnapleQuesterProfilePreprocessing.Location = new System.Drawing.Point(8, 9);
            this.ckbEnapleQuesterProfilePreprocessing.Name = "ckbEnapleQuesterProfilePreprocessing";
            this.ckbEnapleQuesterProfilePreprocessing.Size = new System.Drawing.Size(201, 17);
            this.ckbEnapleQuesterProfilePreprocessing.TabIndex = 0;
            this.ckbEnapleQuesterProfilePreprocessing.Text = "Enable Quester-profile preprocessing";
            this.ckbEnapleQuesterProfilePreprocessing.UseVisualStyleBackColor = true;
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.CheckPathExists = false;
            this.dlgSaveFile.Filter = "XML|*.xml";
            this.dlgSaveFile.ValidateNames = false;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.work_PowerSearch);
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(9, 359);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(293, 23);
            this.lblVersion.TabIndex = 15;
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            ((System.ComponentModel.ISupportInitialize)(this.cbxExportSelector.Properties)).EndInit();
            this.tabSettings.ResumeLayout(false);
            this.tabRelogger.ResumeLayout(false);
            this.tabRelogger.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.tabProfilePreprocessing.ResumeLayout(false);
            this.tabProfilePreprocessing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReplacements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPreprocessing)).EndInit();
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
        private TextBox tbMashineId;
        private Button btnAuraViewer;
        private ButtonEdit tbExportFileSelector;
        private SaveFileDialog dlgSaveFile;
        private Button btnExport;
        private GroupBox gbxExport;
        private CheckBox ckbMapperPatch;
        private ToolTip toolTip;
        private XtraTabPage tabDebug;
        private TextBox tbDebugMonitorInfo;
        private CheckBox ckbDebugMonitor;
        private Button btnTest1;
        private Button bntTest2;
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
        private Button btnMissionMonitor;
        private SimpleButton btnSave;
        private PropertyGrid pgConfigs;
        private SimpleButton btnCredentialManager;
        private ComboBoxEdit cbxExportSelector;
        private Button btnTeamMonitor;
        private BackgroundWorker backgroundWorker;
        private XtraTabPage tabProfilePreprocessing;
        private SimpleButton bntSaveQuesterProfilePreprocessor;
        private Button btnExportQuesterProfilePreprocessor;
        private Button btnImportQuesterProfilePreprocessor;
        private DevExpress.XtraGrid.GridControl gridReplacements;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPreprocessing;
        private CheckBox ckbAutoSavePreprocessedProfile;
        private CheckBox ckbEnapleQuesterProfilePreprocessing;
        private Button btnClearProcessingItem;
        private Button btnDeleteProcessingItem;
        private Button btnAddProcessingItem;
        private Button btnTestProcessingItem;
        private Button btnHelpQuesterProfilePreprocessor;
        private DevExpress.XtraGrid.Columns.GridColumn clmnType;
        private DevExpress.XtraGrid.Columns.GridColumn clmnPattern;
        private DevExpress.XtraGrid.Columns.GridColumn clmnReplacement;
        private Button btnEntityCacheMonitor;
        private Button btnUcc;
        private Button btnQuesterEditor;
        private PropertyGrid propertyGrid;
        private Button btnTest;
        private Label lblVersion;
    }
}
