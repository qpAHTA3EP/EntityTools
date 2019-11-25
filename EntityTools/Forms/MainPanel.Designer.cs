using EntityTools.Tools;
using System.IO;

namespace EntityTools.Forms
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
            this.components = new System.ComponentModel.Container();
            this.btnUccEditor = new System.Windows.Forms.Button();
            this.btnAuras = new System.Windows.Forms.Button();
            this.lblAuras = new System.Windows.Forms.Label();
            this.btnMissions = new System.Windows.Forms.Button();
            this.lblMissions = new System.Windows.Forms.Label();
            this.bteMissions = new DevExpress.XtraEditors.ButtonEdit();
            this.bteAuras = new DevExpress.XtraEditors.ButtonEdit();
            this.fldrBroserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.tbclMain = new DevExpress.XtraTab.XtraTabControl();
            this.tabUtilities = new DevExpress.XtraTab.XtraTabPage();
            this.cbEnchantHelperActivator = new System.Windows.Forms.CheckBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnStates = new System.Windows.Forms.Button();
            this.btnInterfaces = new System.Windows.Forms.Button();
            this.btnUiViewer = new System.Windows.Forms.Button();
            this.bteStates = new DevExpress.XtraEditors.ButtonEdit();
            this.bteInterfaces = new DevExpress.XtraEditors.ButtonEdit();
            this.lblState = new System.Windows.Forms.Label();
            this.lblInterfaces = new System.Windows.Forms.Label();
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
            this.sbSettings = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bteMissions.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteAuras.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).BeginInit();
            this.tbclMain.SuspendLayout();
            this.tabUtilities.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bteStates.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteInterfaces.Properties)).BeginInit();
            this.tabOptions.SuspendLayout();
            this.gbSlideMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seTimerSlide.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seTimerUnslide.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seSlideFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUccEditor
            // 
            this.btnUccEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUccEditor.Enabled = false;
            this.btnUccEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUccEditor.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUccEditor.Location = new System.Drawing.Point(4, 297);
            this.btnUccEditor.Name = "btnUccEditor";
            this.btnUccEditor.Size = new System.Drawing.Size(355, 40);
            this.btnUccEditor.TabIndex = 0;
            this.btnUccEditor.Text = "Extended UCC Editor";
            this.btnUccEditor.UseVisualStyleBackColor = true;
            this.btnUccEditor.Visible = false;
            this.btnUccEditor.Click += new System.EventHandler(this.btnUccEditor_Click);
            // 
            // btnAuras
            // 
            this.btnAuras.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAuras.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuras.Location = new System.Drawing.Point(311, 26);
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
            this.lblAuras.Size = new System.Drawing.Size(161, 13);
            this.lblAuras.TabIndex = 3;
            this.lblAuras.Text = "Export current Auras to the file:";
            // 
            // btnMissions
            // 
            this.btnMissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMissions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMissions.Location = new System.Drawing.Point(311, 65);
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
            this.lblMissions.Size = new System.Drawing.Size(172, 13);
            this.lblMissions.TabIndex = 3;
            this.lblMissions.Text = "Export current Missions to the file:";
            // 
            // bteMissions
            // 
            this.bteMissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bteMissions.EditValue = ".\\Logs\\Missions\\%character%_Missions.xml";
            this.bteMissions.Location = new System.Drawing.Point(6, 67);
            this.bteMissions.Name = "bteMissions";
            this.bteMissions.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteMissions.Properties.NullText = "Enter the Filename where store the Missions";
            this.bteMissions.Properties.NullValuePromptShowForEmptyValue = true;
            this.bteMissions.Properties.ReadOnly = true;
            this.bteMissions.Size = new System.Drawing.Size(299, 20);
            this.bteMissions.TabIndex = 6;
            this.bteMissions.ToolTip = "File name to store Missions of the current Character. \r\nAllow mask %character%, %" +
    "account%, %dateTime%.";
            this.bteMissions.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ButtonClick);
            // 
            // bteAuras
            // 
            this.bteAuras.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bteAuras.EditValue = ".\\Logs\\Auras\\%character%_Auras.xml";
            this.bteAuras.Location = new System.Drawing.Point(6, 28);
            this.bteAuras.Name = "bteAuras";
            this.bteAuras.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.bteAuras.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteAuras.Properties.NullText = "Enter the Filename where store the Auras";
            this.bteAuras.Properties.NullValuePromptShowForEmptyValue = true;
            this.bteAuras.Properties.ReadOnly = true;
            this.bteAuras.Size = new System.Drawing.Size(299, 20);
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
            this.tbclMain.SelectedTabPage = this.tabUtilities;
            this.tbclMain.Size = new System.Drawing.Size(370, 416);
            this.tbclMain.TabIndex = 7;
            this.tbclMain.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabUtilities,
            this.tabOptions});
            // 
            // tabUtilities
            // 
            this.tabUtilities.Controls.Add(this.cbEnchantHelperActivator);
            this.tabUtilities.Controls.Add(this.lblAuras);
            this.tabUtilities.Controls.Add(this.btnTest);
            this.tabUtilities.Controls.Add(this.btnStates);
            this.tabUtilities.Controls.Add(this.btnInterfaces);
            this.tabUtilities.Controls.Add(this.btnMissions);
            this.tabUtilities.Controls.Add(this.btnUiViewer);
            this.tabUtilities.Controls.Add(this.btnUccEditor);
            this.tabUtilities.Controls.Add(this.bteStates);
            this.tabUtilities.Controls.Add(this.bteInterfaces);
            this.tabUtilities.Controls.Add(this.bteMissions);
            this.tabUtilities.Controls.Add(this.bteAuras);
            this.tabUtilities.Controls.Add(this.lblState);
            this.tabUtilities.Controls.Add(this.lblInterfaces);
            this.tabUtilities.Controls.Add(this.lblMissions);
            this.tabUtilities.Controls.Add(this.btnAuras);
            this.tabUtilities.Name = "tabUtilities";
            this.tabUtilities.Size = new System.Drawing.Size(364, 388);
            this.tabUtilities.Text = "Utilities";
            // 
            // cbEnchantHelperActivator
            // 
            this.cbEnchantHelperActivator.AutoSize = true;
            this.cbEnchantHelperActivator.Location = new System.Drawing.Point(6, 187);
            this.cbEnchantHelperActivator.Name = "cbEnchantHelperActivator";
            this.cbEnchantHelperActivator.Size = new System.Drawing.Size(131, 17);
            this.cbEnchantHelperActivator.TabIndex = 10;
            this.cbEnchantHelperActivator.Text = "Enable EnchantHelper";
            this.cbEnchantHelperActivator.UseVisualStyleBackColor = true;
            this.cbEnchantHelperActivator.CheckedChanged += new System.EventHandler(this.cbEnchantHelperActivator_CheckedChanged);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Location = new System.Drawing.Point(309, 314);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(50, 23);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnStates
            // 
            this.btnStates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStates.Location = new System.Drawing.Point(311, 143);
            this.btnStates.Name = "btnStates";
            this.btnStates.Size = new System.Drawing.Size(50, 23);
            this.btnStates.TabIndex = 2;
            this.btnStates.Text = "Export";
            this.btnStates.UseVisualStyleBackColor = true;
            this.btnStates.Click += new System.EventHandler(this.btnStates_Click);
            // 
            // btnInterfaces
            // 
            this.btnInterfaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInterfaces.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInterfaces.Location = new System.Drawing.Point(311, 104);
            this.btnInterfaces.Name = "btnInterfaces";
            this.btnInterfaces.Size = new System.Drawing.Size(50, 23);
            this.btnInterfaces.TabIndex = 2;
            this.btnInterfaces.Text = "Export";
            this.btnInterfaces.UseVisualStyleBackColor = true;
            this.btnInterfaces.Click += new System.EventHandler(this.btnInterfaces_Click);
            // 
            // btnUiViewer
            // 
            this.btnUiViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUiViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUiViewer.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUiViewer.Location = new System.Drawing.Point(4, 343);
            this.btnUiViewer.Name = "btnUiViewer";
            this.btnUiViewer.Size = new System.Drawing.Size(355, 40);
            this.btnUiViewer.TabIndex = 0;
            this.btnUiViewer.Text = "UI Viewer";
            this.btnUiViewer.UseVisualStyleBackColor = true;
            this.btnUiViewer.Click += new System.EventHandler(this.btnUiViewer_Click);
            // 
            // bteStates
            // 
            this.bteStates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bteStates.EditValue = ".\\Logs\\States\\%character%_States.xml";
            this.bteStates.Location = new System.Drawing.Point(6, 145);
            this.bteStates.Name = "bteStates";
            this.bteStates.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteStates.Properties.NullText = "Enter the Filename where store the Missions";
            this.bteStates.Properties.NullValuePromptShowForEmptyValue = true;
            this.bteStates.Properties.ReadOnly = true;
            this.bteStates.Size = new System.Drawing.Size(299, 20);
            this.bteStates.TabIndex = 6;
            this.bteStates.ToolTip = "File name to store engine States. \r\nAllow mask %character%, %account%, %dateTime%" +
    ".";
            this.bteStates.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ButtonClick);
            // 
            // bteInterfaces
            // 
            this.bteInterfaces.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bteInterfaces.EditValue = ".\\Logs\\Interfaces\\%character%_Interfaces.xml";
            this.bteInterfaces.Location = new System.Drawing.Point(6, 106);
            this.bteInterfaces.Name = "bteInterfaces";
            this.bteInterfaces.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteInterfaces.Properties.NullText = "Enter the Filename where store the Missions";
            this.bteInterfaces.Properties.NullValuePromptShowForEmptyValue = true;
            this.bteInterfaces.Properties.ReadOnly = true;
            this.bteInterfaces.Size = new System.Drawing.Size(299, 20);
            this.bteInterfaces.TabIndex = 6;
            this.bteInterfaces.ToolTip = "File name to store Game Interfaces. \r\nAllow mask %character%, %account%, %dateTim" +
    "e%.";
            this.bteInterfaces.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ButtonClick);
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(14, 129);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(196, 13);
            this.lblState.TabIndex = 3;
            this.lblState.Text = "Export loaded engine States to the file:";
            // 
            // lblInterfaces
            // 
            this.lblInterfaces.AutoSize = true;
            this.lblInterfaces.Location = new System.Drawing.Point(14, 90);
            this.lblInterfaces.Name = "lblInterfaces";
            this.lblInterfaces.Size = new System.Drawing.Size(216, 13);
            this.lblInterfaces.TabIndex = 3;
            this.lblInterfaces.Text = "Export Game Interfaces (UIGen) to the file:";
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.cbSlideMonitor);
            this.tabOptions.Controls.Add(this.ckbSpellStuckMonitor);
            this.tabOptions.Controls.Add(this.gbSlideMonitor);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Size = new System.Drawing.Size(364, 388);
            this.tabOptions.Text = "Options";
            // 
            // cbSlideMonitor
            // 
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
            this.gbSlideMonitor.Size = new System.Drawing.Size(359, 160);
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
            this.tbSlidingAuras.Size = new System.Drawing.Size(347, 60);
            this.tbSlidingAuras.TabIndex = 3;
            this.tbSlidingAuras.Text = "M10_Becritter_Boat_Costume\r\nVolume_Ground_Slippery\r\nVolume_Ground_Slippery_Player" +
    "only";
            this.tbSlidingAuras.Visible = false;
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
            // MainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbclMain);
            this.Name = "MainPanel";
            this.Size = new System.Drawing.Size(370, 416);
            this.Load += new System.EventHandler(this.MainPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bteMissions.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteAuras.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).EndInit();
            this.tbclMain.ResumeLayout(false);
            this.tabUtilities.ResumeLayout(false);
            this.tabUtilities.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bteStates.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteInterfaces.Properties)).EndInit();
            this.tabOptions.ResumeLayout(false);
            this.tabOptions.PerformLayout();
            this.gbSlideMonitor.ResumeLayout(false);
            this.gbSlideMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seTimerSlide.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seTimerUnslide.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seSlideFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbSettings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUccEditor;
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
        private System.Windows.Forms.BindingSource sbSettings;
        private System.Windows.Forms.Button btnInterfaces;
        private DevExpress.XtraEditors.ButtonEdit bteInterfaces;
        private System.Windows.Forms.Label lblInterfaces;
        private System.Windows.Forms.Button btnStates;
        private DevExpress.XtraEditors.ButtonEdit bteStates;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.CheckBox cbEnchantHelperActivator;
        private System.Windows.Forms.Button btnTest;
    }
}
