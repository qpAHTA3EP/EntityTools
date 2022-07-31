using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using System.Windows.Forms;

namespace EntityCore.Forms
{
    partial class UccEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UccEditor));
            DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer dockingContainer2 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer();
            this.documentGroup = new DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup(this.components);
            this.docCombat = new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(this.components);
            this.docPatrol = new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(this.components);
            this.docTactic = new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(this.components);
            this.dockManager = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barActionTools = new DevExpress.XtraBars.Bar();
            this.bntAddAction = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelAction = new DevExpress.XtraBars.BarButtonItem();
            this.btnCopyAction = new DevExpress.XtraBars.BarButtonItem();
            this.btnPasteAction = new DevExpress.XtraBars.BarButtonItem();
            this.btnTestSelected = new DevExpress.XtraBars.BarButtonItem();
            this.btnTestAll = new DevExpress.XtraBars.BarButtonItem();
            this.barProfile = new DevExpress.XtraBars.Bar();
            this.btnNewProfile = new DevExpress.XtraBars.BarButtonItem();
            this.btnLoadProfile = new DevExpress.XtraBars.BarButtonItem();
            this.btnSaveProfile = new DevExpress.XtraBars.BarButtonItem();
            this.btnSaveProfielAs = new DevExpress.XtraBars.BarButtonItem();
            this.btnToEngine = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.standaloneBarDockControl1 = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.panLog = new DevExpress.XtraBars.Docking.DockPanel();
            this.controlContainer2 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.panConditions = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.treeConditions = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.panProperties = new DevExpress.XtraBars.Docking.DockPanel();
            this.panProperty = new DevExpress.XtraBars.Docking.ControlContainer();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.panCombat = new DevExpress.XtraBars.Docking.DockPanel();
            this.combatActionsControlContainer = new DevExpress.XtraBars.Docking.ControlContainer();
            this.treeCombatActions = new System.Windows.Forms.TreeView();
            this.panPatrol = new DevExpress.XtraBars.Docking.DockPanel();
            this.patrolActionsControlContainer = new DevExpress.XtraBars.Docking.ControlContainer();
            this.treePatrolActions = new System.Windows.Forms.TreeView();
            this.panTactic = new DevExpress.XtraBars.Docking.DockPanel();
            this.controlContainer1 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.checkerTacticActivator = new DevExpress.XtraEditors.CheckEdit();
            this.groupPotionUsage = new DevExpress.XtraEditors.GroupControl();
            this.checkerSmartPotionUsage = new DevExpress.XtraEditors.CheckEdit();
            this.checkerUsePotion = new DevExpress.XtraEditors.CheckEdit();
            this.editHealthProcent = new DevExpress.XtraEditors.SpinEdit();
            this.groupPriority = new DevExpress.XtraEditors.GroupControl();
            this.listPriorities = new DevExpress.XtraEditors.ListBoxControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.editBasePriority = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblBasePriorityRange = new DevExpress.XtraEditors.LabelControl();
            this.editChangeCooldown = new DevExpress.XtraEditors.SpinEdit();
            this.lblBaseTargetPriority = new DevExpress.XtraEditors.LabelControl();
            this.editBasePriorityRange = new DevExpress.XtraEditors.SpinEdit();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.documentManager = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
            this.tabbedView = new DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.documentGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.docCombat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.docPatrol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.docTactic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            this.panLog.SuspendLayout();
            this.controlContainer2.SuspendLayout();
            this.panelContainer1.SuspendLayout();
            this.panConditions.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            this.panProperties.SuspendLayout();
            this.panProperty.SuspendLayout();
            this.panCombat.SuspendLayout();
            this.combatActionsControlContainer.SuspendLayout();
            this.panPatrol.SuspendLayout();
            this.patrolActionsControlContainer.SuspendLayout();
            this.panTactic.SuspendLayout();
            this.controlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkerTacticActivator.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupPotionUsage)).BeginInit();
            this.groupPotionUsage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkerSmartPotionUsage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkerUsePotion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editHealthProcent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupPriority)).BeginInit();
            this.groupPriority.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listPriorities)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editBasePriority.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editChangeCooldown.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editBasePriorityRange.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedView)).BeginInit();
            this.SuspendLayout();
            // 
            // documentGroup
            // 
            this.documentGroup.Items.AddRange(new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document[] {
            this.docCombat,
            this.docPatrol,
            this.docTactic});
            // 
            // docCombat
            // 
            this.docCombat.Caption = "Combat";
            this.docCombat.ControlName = "panCombat";
            this.docCombat.FloatLocation = new System.Drawing.Point(500, 160);
            this.docCombat.FloatSize = new System.Drawing.Size(400, 400);
            this.docCombat.Properties.AllowClose = DevExpress.Utils.DefaultBoolean.False;
            this.docCombat.Properties.AllowFloat = DevExpress.Utils.DefaultBoolean.False;
            this.docCombat.Properties.AllowFloatOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            // 
            // docPatrol
            // 
            this.docPatrol.Caption = "Patrol";
            this.docPatrol.ControlName = "panPatrol";
            this.docPatrol.FloatLocation = new System.Drawing.Point(492, 163);
            this.docPatrol.FloatSize = new System.Drawing.Size(400, 400);
            this.docPatrol.Properties.AllowClose = DevExpress.Utils.DefaultBoolean.False;
            this.docPatrol.Properties.AllowFloat = DevExpress.Utils.DefaultBoolean.False;
            this.docPatrol.Properties.AllowFloatOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            // 
            // docTactic
            // 
            this.docTactic.Caption = "Tactic";
            this.docTactic.ControlName = "panTactic";
            this.docTactic.FloatLocation = new System.Drawing.Point(445, 165);
            this.docTactic.FloatSize = new System.Drawing.Size(400, 400);
            this.docTactic.Properties.AllowClose = DevExpress.Utils.DefaultBoolean.False;
            this.docTactic.Properties.AllowFloat = DevExpress.Utils.DefaultBoolean.False;
            this.docTactic.Properties.AllowFloatOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            // 
            // dockManager
            // 
            this.dockManager.Form = this;
            this.dockManager.MenuManager = this.barManager;
            this.dockManager.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.panLog,
            this.panelContainer1,
            this.panCombat,
            this.panPatrol,
            this.panTactic});
            this.dockManager.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane",
            "DevExpress.XtraBars.TabFormControl",
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl",
            "DevExpress.XtraBars.ToolbarForm.ToolbarFormControl"});
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barActionTools,
            this.barProfile});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.DockControls.Add(this.standaloneBarDockControl1);
            this.barManager.DockManager = this.dockManager;
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bntAddAction,
            this.btnDelAction,
            this.btnSaveProfile,
            this.btnLoadProfile,
            this.btnToEngine,
            this.btnNewProfile,
            this.btnSaveProfielAs,
            this.btnCopyAction,
            this.btnPasteAction,
            this.btnTestSelected,
            this.btnTestAll});
            this.barManager.MainMenu = this.barProfile;
            this.barManager.MaxItemId = 12;
            // 
            // barActionTools
            // 
            this.barActionTools.BarName = "Tools";
            this.barActionTools.DockCol = 1;
            this.barActionTools.DockRow = 0;
            this.barActionTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barActionTools.FloatLocation = new System.Drawing.Point(301, 153);
            this.barActionTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bntAddAction),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDelAction),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCopyAction),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnPasteAction),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnTestSelected, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnTestAll)});
            this.barActionTools.Offset = 215;
            this.barActionTools.Text = "Tools";
            // 
            // bntAddAction
            // 
            this.bntAddAction.Caption = "Add Item";
            this.bntAddAction.Hint = "Add new item into active collection (ucc-Action, ucc-Condition or TargetPriority)" +
    "";
            this.bntAddAction.Id = 0;
            this.bntAddAction.ImageOptions.Image = global::EntityCore.Properties.Resources.miniAdd;
            this.bntAddAction.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Insert);
            this.bntAddAction.Name = "bntAddAction";
            this.bntAddAction.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Add);
            // 
            // btnDelAction
            // 
            this.btnDelAction.Caption = "Delete Item";
            this.btnDelAction.Hint = "Delete selected item from active collection (ucc-Action, ucc-Condition or TargetP" +
    "riority)";
            this.btnDelAction.Id = 1;
            this.btnDelAction.ImageOptions.Image = global::EntityCore.Properties.Resources.miniCancel;
            this.btnDelAction.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete));
            this.btnDelAction.Name = "btnDelAction";
            this.btnDelAction.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DeleteAction);
            // 
            // btnCopyAction
            // 
            this.btnCopyAction.Caption = "Copy Item";
            this.btnCopyAction.Hint = "Copy to clipboard the selected item from active collection (ucc-Action, ucc-Condi" +
    "tion or TargetPriority)";
            this.btnCopyAction.Id = 7;
            this.btnCopyAction.ImageOptions.Image = global::EntityCore.Properties.Resources.miniCopy;
            this.btnCopyAction.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C));
            this.btnCopyAction.Name = "btnCopyAction";
            this.btnCopyAction.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Copy);
            // 
            // btnPasteAction
            // 
            this.btnPasteAction.Caption = "Paste Item";
            this.btnPasteAction.Hint = "Pasthe item from the clipboard nto active collection (ucc-Action, ucc-Condition o" +
    "r TargetPriority)";
            this.btnPasteAction.Id = 8;
            this.btnPasteAction.ImageOptions.Image = global::EntityCore.Properties.Resources.paste;
            this.btnPasteAction.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V));
            this.btnPasteAction.Name = "btnPasteAction";
            this.btnPasteAction.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Paste);
            // 
            // btnTestSelected
            // 
            this.btnTestSelected.Caption = "Test";
            this.btnTestSelected.Id = 10;
            this.btnTestSelected.ImageOptions.Image = global::EntityCore.Properties.Resources.miniPlay;
            this.btnTestSelected.Name = "btnTestSelected";
            this.btnTestSelected.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_TestSelected);
            // 
            // btnTestAll
            // 
            this.btnTestAll.Caption = "Test All";
            this.btnTestAll.Id = 11;
            this.btnTestAll.ImageOptions.Image = global::EntityCore.Properties.Resources.miniPlayAll;
            this.btnTestAll.Name = "btnTestAll";
            this.btnTestAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_TestAll);
            // 
            // barProfile
            // 
            this.barProfile.BarName = "Profile";
            this.barProfile.DockCol = 0;
            this.barProfile.DockRow = 0;
            this.barProfile.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barProfile.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnNewProfile),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnLoadProfile),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveProfile),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveProfielAs),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnToEngine)});
            this.barProfile.Text = "Profile";
            // 
            // btnNewProfile
            // 
            this.btnNewProfile.Caption = "New Profile";
            this.btnNewProfile.Id = 5;
            this.btnNewProfile.ImageOptions.Image = global::EntityCore.Properties.Resources.miniNewFile;
            this.btnNewProfile.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.btnNewProfile.Name = "btnNewProfile";
            this.btnNewProfile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_NewProfile);
            // 
            // btnLoadProfile
            // 
            this.btnLoadProfile.Caption = "Load Profile";
            this.btnLoadProfile.Id = 3;
            this.btnLoadProfile.ImageOptions.Image = global::EntityCore.Properties.Resources.miniOpen;
            this.btnLoadProfile.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O));
            this.btnLoadProfile.Name = "btnLoadProfile";
            this.btnLoadProfile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_LoadProfile);
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.Caption = "Save Profile";
            this.btnSaveProfile.Id = 2;
            this.btnSaveProfile.ImageOptions.Image = global::EntityCore.Properties.Resources.miniDisket;
            this.btnSaveProfile.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_SaveProfile);
            // 
            // btnSaveProfielAs
            // 
            this.btnSaveProfielAs.Caption = "Save Profile as ...";
            this.btnSaveProfielAs.Id = 6;
            this.btnSaveProfielAs.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveProfielAs.ImageOptions.Image")));
            this.btnSaveProfielAs.Name = "btnSaveProfielAs";
            this.btnSaveProfielAs.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_SaveProfileAs);
            // 
            // btnToEngine
            // 
            this.btnToEngine.Caption = "Export to UccEngine";
            this.btnToEngine.Hint = "Export currently editing Profile to UccEngine";
            this.btnToEngine.Id = 4;
            this.btnToEngine.ImageOptions.Image = global::EntityCore.Properties.Resources.miniImport;
            this.btnToEngine.Name = "btnToEngine";
            this.btnToEngine.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ProfileToEngine);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(698, 24);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 588);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(698, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 24);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 564);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(698, 24);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 564);
            // 
            // standaloneBarDockControl1
            // 
            this.standaloneBarDockControl1.AutoSize = true;
            this.standaloneBarDockControl1.CausesValidation = false;
            this.standaloneBarDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.standaloneBarDockControl1.Location = new System.Drawing.Point(0, 0);
            this.standaloneBarDockControl1.Manager = this.barManager;
            this.standaloneBarDockControl1.Name = "standaloneBarDockControl1";
            this.standaloneBarDockControl1.Size = new System.Drawing.Size(340, 0);
            this.standaloneBarDockControl1.Text = "standaloneBarDockControl1";
            // 
            // panLog
            // 
            this.panLog.Controls.Add(this.controlContainer2);
            this.panLog.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.panLog.FloatSize = new System.Drawing.Size(700, 200);
            this.panLog.FloatVertical = true;
            this.panLog.ID = new System.Guid("0e9b0b1f-11f2-43d8-9544-8532ec69fd0b");
            this.panLog.Location = new System.Drawing.Point(0, 488);
            this.panLog.Name = "panLog";
            this.panLog.OriginalSize = new System.Drawing.Size(200, 100);
            this.panLog.Size = new System.Drawing.Size(698, 100);
            this.panLog.Text = "Log";
            // 
            // controlContainer2
            // 
            this.controlContainer2.Controls.Add(this.txtLog);
            this.controlContainer2.Location = new System.Drawing.Point(3, 27);
            this.controlContainer2.Name = "controlContainer2";
            this.controlContainer2.Size = new System.Drawing.Size(692, 70);
            this.controlContainer2.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(692, 70);
            this.txtLog.TabIndex = 0;
            // 
            // panelContainer1
            // 
            this.panelContainer1.Controls.Add(this.panConditions);
            this.panelContainer1.Controls.Add(this.panProperties);
            this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.panelContainer1.ID = new System.Guid("2337b03b-b68b-4447-92d2-5a46dd88adef");
            this.panelContainer1.Location = new System.Drawing.Point(265, 24);
            this.panelContainer1.Name = "panelContainer1";
            this.panelContainer1.OriginalSize = new System.Drawing.Size(433, 200);
            this.panelContainer1.Size = new System.Drawing.Size(433, 464);
            this.panelContainer1.Text = "panelContainer1";
            // 
            // panConditions
            // 
            this.panConditions.Controls.Add(this.dockPanel1_Container);
            this.panConditions.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.panConditions.FloatSize = new System.Drawing.Size(400, 400);
            this.panConditions.FloatVertical = true;
            this.panConditions.ID = new System.Guid("722bd200-af38-47bf-95bf-47bd98231b95");
            this.panConditions.Location = new System.Drawing.Point(0, 0);
            this.panConditions.Name = "panConditions";
            this.panConditions.Options.ShowCloseButton = false;
            this.panConditions.OriginalSize = new System.Drawing.Size(433, 143);
            this.panConditions.Size = new System.Drawing.Size(433, 161);
            this.panConditions.Text = "Conditions";
            this.panConditions.Enter += new System.EventHandler(this.handler_Focused);
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.treeConditions);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 26);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(426, 131);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // treeConditions
            // 
            this.treeConditions.AllowDrop = true;
            this.treeConditions.BackColor = System.Drawing.SystemColors.Control;
            this.treeConditions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeConditions.CheckBoxes = true;
            this.treeConditions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeConditions.ImageIndex = 0;
            this.treeConditions.ImageList = this.imageList;
            this.treeConditions.Location = new System.Drawing.Point(0, 0);
            this.treeConditions.Name = "treeConditions";
            this.treeConditions.SelectedImageIndex = 0;
            this.treeConditions.Size = new System.Drawing.Size(426, 131);
            this.treeConditions.TabIndex = 0;
            this.treeConditions.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.handler_NodeCheckedChanged);
            this.treeConditions.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeConditions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.handler_NodeSelected);
            this.treeConditions.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeConditions.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this.treeConditions.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treeConditions.Enter += new System.EventHandler(this.handler_Focused);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Cube");
            this.imageList.Images.SetKeyName(1, "Frame");
            this.imageList.Images.SetKeyName(2, "Box");
            this.imageList.Images.SetKeyName(3, "Play");
            this.imageList.Images.SetKeyName(4, "Potion");
            this.imageList.Images.SetKeyName(5, "Flask");
            this.imageList.Images.SetKeyName(6, "Distance");
            this.imageList.Images.SetKeyName(7, "Pazl");
            this.imageList.Images.SetKeyName(8, "Spell");
            this.imageList.Images.SetKeyName(9, "Move");
            this.imageList.Images.SetKeyName(10, "Step");
            this.imageList.Images.SetKeyName(11, "Track");
            this.imageList.Images.SetKeyName(12, "List");
            this.imageList.Images.SetKeyName(13, "Cancel");
            this.imageList.Images.SetKeyName(14, "Dodge");
            this.imageList.Images.SetKeyName(15, "Art");
            this.imageList.Images.SetKeyName(16, "Gem");
            this.imageList.Images.SetKeyName(17, "miniBoxedArrow.png");
            this.imageList.Images.SetKeyName(18, "Gear");
            this.imageList.Images.SetKeyName(19, "Recycle");
            this.imageList.Images.SetKeyName(20, "Target");
            this.imageList.Images.SetKeyName(21, "Condition");
            this.imageList.Images.SetKeyName(22, "ConditionList");
            this.imageList.Images.SetKeyName(23, "Power");
            // 
            // panProperties
            // 
            this.panProperties.Controls.Add(this.panProperty);
            this.panProperties.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.panProperties.FloatSize = new System.Drawing.Size(400, 400);
            this.panProperties.ID = new System.Guid("75a27416-1cb7-4549-9526-cf83d78dcbc8");
            this.panProperties.Location = new System.Drawing.Point(0, 161);
            this.panProperties.Name = "panProperties";
            this.panProperties.Options.ShowCloseButton = false;
            this.panProperties.OriginalSize = new System.Drawing.Size(433, 321);
            this.panProperties.Size = new System.Drawing.Size(433, 303);
            this.panProperties.Text = "Properties";
            // 
            // panProperty
            // 
            this.panProperty.Controls.Add(this.propertyGrid);
            this.panProperty.Location = new System.Drawing.Point(4, 26);
            this.panProperty.Name = "panProperty";
            this.panProperty.Size = new System.Drawing.Size(426, 274);
            this.panProperty.TabIndex = 0;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(426, 274);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.handler_PropertyChanged);
            // 
            // panCombat
            // 
            this.panCombat.Controls.Add(this.combatActionsControlContainer);
            this.panCombat.DockedAsTabbedDocument = true;
            this.panCombat.FloatLocation = new System.Drawing.Point(500, 160);
            this.panCombat.FloatSize = new System.Drawing.Size(400, 400);
            this.panCombat.ID = new System.Guid("9a93fdc7-43e8-4a39-b440-f5eec9f3dd10");
            this.panCombat.Name = "panCombat";
            this.panCombat.Options.AllowFloating = false;
            this.panCombat.Options.FloatOnDblClick = false;
            this.panCombat.Options.ShowCloseButton = false;
            this.panCombat.OriginalSize = new System.Drawing.Size(200, 200);
            this.panCombat.SavedIndex = 1;
            this.panCombat.SavedMdiDocument = true;
            this.panCombat.SavedMdiDocumentIndex = 1;
            this.panCombat.Text = "Combat";
            this.panCombat.Enter += new System.EventHandler(this.handler_Focused);
            // 
            // combatActionsControlContainer
            // 
            this.combatActionsControlContainer.Controls.Add(this.treeCombatActions);
            this.combatActionsControlContainer.Location = new System.Drawing.Point(0, 0);
            this.combatActionsControlContainer.Name = "combatActionsControlContainer";
            this.combatActionsControlContainer.Size = new System.Drawing.Size(259, 435);
            this.combatActionsControlContainer.TabIndex = 0;
            // 
            // treeCombatActions
            // 
            this.treeCombatActions.AllowDrop = true;
            this.treeCombatActions.BackColor = System.Drawing.SystemColors.Control;
            this.treeCombatActions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeCombatActions.CheckBoxes = true;
            this.treeCombatActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeCombatActions.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeCombatActions.ForeColor = System.Drawing.SystemColors.WindowText;
            this.treeCombatActions.FullRowSelect = true;
            this.treeCombatActions.ImageKey = "Box";
            this.treeCombatActions.ImageList = this.imageList;
            this.treeCombatActions.Indent = 22;
            this.treeCombatActions.Location = new System.Drawing.Point(0, 0);
            this.treeCombatActions.Name = "treeCombatActions";
            this.treeCombatActions.SelectedImageIndex = 0;
            this.treeCombatActions.Size = new System.Drawing.Size(259, 435);
            this.treeCombatActions.TabIndex = 0;
            this.treeCombatActions.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.handler_NodeCheckedChanged);
            this.treeCombatActions.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeCombatActions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.handler_NodeSelected);
            this.treeCombatActions.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeCombatActions.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this.treeCombatActions.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treeCombatActions.Enter += new System.EventHandler(this.handler_Focused);
            // 
            // panPatrol
            // 
            this.panPatrol.Controls.Add(this.patrolActionsControlContainer);
            this.panPatrol.DockedAsTabbedDocument = true;
            this.panPatrol.FloatLocation = new System.Drawing.Point(492, 163);
            this.panPatrol.FloatSize = new System.Drawing.Size(400, 400);
            this.panPatrol.ID = new System.Guid("df1cc296-d64d-4aba-b3e4-11a44a1fa366");
            this.panPatrol.Name = "panPatrol";
            this.panPatrol.Options.AllowFloating = false;
            this.panPatrol.Options.FloatOnDblClick = false;
            this.panPatrol.Options.ShowCloseButton = false;
            this.panPatrol.OriginalSize = new System.Drawing.Size(200, 200);
            this.panPatrol.SavedIndex = 3;
            this.panPatrol.SavedMdiDocument = true;
            this.panPatrol.SavedMdiDocumentIndex = 1;
            this.panPatrol.Text = "Patrol";
            this.panPatrol.Enter += new System.EventHandler(this.handler_Focused);
            // 
            // patrolActionsControlContainer
            // 
            this.patrolActionsControlContainer.Controls.Add(this.treePatrolActions);
            this.patrolActionsControlContainer.Location = new System.Drawing.Point(0, 0);
            this.patrolActionsControlContainer.Name = "patrolActionsControlContainer";
            this.patrolActionsControlContainer.Size = new System.Drawing.Size(259, 435);
            this.patrolActionsControlContainer.TabIndex = 0;
            // 
            // treePatrolActions
            // 
            this.treePatrolActions.AllowDrop = true;
            this.treePatrolActions.BackColor = System.Drawing.SystemColors.Control;
            this.treePatrolActions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treePatrolActions.CheckBoxes = true;
            this.treePatrolActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treePatrolActions.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treePatrolActions.ForeColor = System.Drawing.SystemColors.WindowText;
            this.treePatrolActions.FullRowSelect = true;
            this.treePatrolActions.HideSelection = false;
            this.treePatrolActions.ImageIndex = 0;
            this.treePatrolActions.ImageList = this.imageList;
            this.treePatrolActions.Indent = 22;
            this.treePatrolActions.Location = new System.Drawing.Point(0, 0);
            this.treePatrolActions.Name = "treePatrolActions";
            this.treePatrolActions.SelectedImageIndex = 0;
            this.treePatrolActions.Size = new System.Drawing.Size(259, 435);
            this.treePatrolActions.TabIndex = 0;
            this.treePatrolActions.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.handler_NodeCheckedChanged);
            this.treePatrolActions.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treePatrolActions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.handler_NodeSelected);
            this.treePatrolActions.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treePatrolActions.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this.treePatrolActions.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treePatrolActions.Enter += new System.EventHandler(this.handler_Focused);
            // 
            // panTactic
            // 
            this.panTactic.Controls.Add(this.controlContainer1);
            this.panTactic.DockedAsTabbedDocument = true;
            this.panTactic.FloatLocation = new System.Drawing.Point(445, 165);
            this.panTactic.FloatSize = new System.Drawing.Size(400, 400);
            this.panTactic.ID = new System.Guid("29550451-4ac6-403e-98f5-897b622aff1c");
            this.panTactic.Name = "panTactic";
            this.panTactic.Options.AllowFloating = false;
            this.panTactic.Options.FloatOnDblClick = false;
            this.panTactic.Options.ShowCloseButton = false;
            this.panTactic.OriginalSize = new System.Drawing.Size(200, 200);
            this.panTactic.SavedIndex = 3;
            this.panTactic.SavedMdiDocument = true;
            this.panTactic.SavedMdiDocumentIndex = 2;
            this.panTactic.Text = "Tactic";
            this.panTactic.Enter += new System.EventHandler(this.handler_Focused);
            // 
            // controlContainer1
            // 
            this.controlContainer1.Controls.Add(this.checkerTacticActivator);
            this.controlContainer1.Controls.Add(this.groupPotionUsage);
            this.controlContainer1.Controls.Add(this.groupPriority);
            this.controlContainer1.Location = new System.Drawing.Point(0, 0);
            this.controlContainer1.Name = "controlContainer1";
            this.controlContainer1.Size = new System.Drawing.Size(259, 435);
            this.controlContainer1.TabIndex = 0;
            // 
            // checkerTacticActivator
            // 
            this.checkerTacticActivator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkerTacticActivator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkerTacticActivator.Location = new System.Drawing.Point(8, 87);
            this.checkerTacticActivator.MenuManager = this.barManager;
            this.checkerTacticActivator.Name = "checkerTacticActivator";
            this.checkerTacticActivator.Properties.Appearance.BackColor = System.Drawing.Color.LightGray;
            this.checkerTacticActivator.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.checkerTacticActivator.Properties.Appearance.Options.UseBackColor = true;
            this.checkerTacticActivator.Properties.Appearance.Options.UseFont = true;
            this.checkerTacticActivator.Properties.Caption = "Custom Target Priority settings";
            this.checkerTacticActivator.Size = new System.Drawing.Size(243, 20);
            this.checkerTacticActivator.TabIndex = 0;
            this.checkerTacticActivator.CheckedChanged += new System.EventHandler(this.handler_TacticUsageChanged);
            // 
            // groupPotionUsage
            // 
            this.groupPotionUsage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPotionUsage.Appearance.BackColor = System.Drawing.Color.LightGray;
            this.groupPotionUsage.Appearance.Options.UseBackColor = true;
            this.groupPotionUsage.Controls.Add(this.checkerSmartPotionUsage);
            this.groupPotionUsage.Controls.Add(this.checkerUsePotion);
            this.groupPotionUsage.Controls.Add(this.editHealthProcent);
            this.groupPotionUsage.Location = new System.Drawing.Point(3, 3);
            this.groupPotionUsage.MinimumSize = new System.Drawing.Size(246, 77);
            this.groupPotionUsage.Name = "groupPotionUsage";
            this.groupPotionUsage.Size = new System.Drawing.Size(253, 77);
            this.groupPotionUsage.TabIndex = 6;
            this.groupPotionUsage.Text = "Healing Potion usage";
            // 
            // checkerSmartPotionUsage
            // 
            this.checkerSmartPotionUsage.Location = new System.Drawing.Point(6, 52);
            this.checkerSmartPotionUsage.Name = "checkerSmartPotionUsage";
            this.checkerSmartPotionUsage.Properties.Caption = "Enable smart Potion usage";
            this.checkerSmartPotionUsage.Size = new System.Drawing.Size(190, 20);
            this.checkerSmartPotionUsage.TabIndex = 3;
            // 
            // checkerUsePotion
            // 
            this.checkerUsePotion.Location = new System.Drawing.Point(6, 26);
            this.checkerUsePotion.MenuManager = this.barManager;
            this.checkerUsePotion.Name = "checkerUsePotion";
            this.checkerUsePotion.Properties.Caption = "Use healing Potion at health %:";
            this.checkerUsePotion.Size = new System.Drawing.Size(175, 20);
            this.checkerUsePotion.TabIndex = 3;
            // 
            // editHealthProcent
            // 
            this.editHealthProcent.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.editHealthProcent.Location = new System.Drawing.Point(183, 26);
            this.editHealthProcent.Name = "editHealthProcent";
            this.editHealthProcent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editHealthProcent.Properties.IsFloatValue = false;
            this.editHealthProcent.Properties.MaskSettings.Set("mask", "N00");
            this.editHealthProcent.Size = new System.Drawing.Size(58, 20);
            this.editHealthProcent.TabIndex = 2;
            this.editHealthProcent.ToolTip = "The time between target changing in seconds";
            // 
            // groupPriority
            // 
            this.groupPriority.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPriority.Appearance.BackColor = System.Drawing.Color.LightGray;
            this.groupPriority.Appearance.Options.UseBackColor = true;
            this.groupPriority.ButtonInterval = 1;
            this.groupPriority.Controls.Add(this.listPriorities);
            this.groupPriority.Controls.Add(this.labelControl3);
            this.groupPriority.Controls.Add(this.editBasePriority);
            this.groupPriority.Controls.Add(this.lblBasePriorityRange);
            this.groupPriority.Controls.Add(this.editChangeCooldown);
            this.groupPriority.Controls.Add(this.lblBaseTargetPriority);
            this.groupPriority.Controls.Add(this.editBasePriorityRange);
            this.groupPriority.Location = new System.Drawing.Point(3, 86);
            this.groupPriority.Name = "groupPriority";
            this.groupPriority.Size = new System.Drawing.Size(253, 346);
            this.groupPriority.TabIndex = 5;
            // 
            // listPriorities
            // 
            this.listPriorities.AllowDrop = true;
            this.listPriorities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPriorities.Location = new System.Drawing.Point(6, 106);
            this.listPriorities.Name = "listPriorities";
            this.listPriorities.Size = new System.Drawing.Size(242, 234);
            this.listPriorities.TabIndex = 3;
            this.listPriorities.SelectedValueChanged += new System.EventHandler(this.listPriorities_SelectedValueChanged);
            this.listPriorities.DragDrop += new System.Windows.Forms.DragEventHandler(this.listPriorities_DragDrop);
            this.listPriorities.DragEnter += new System.Windows.Forms.DragEventHandler(this.listPriorities_DragEnter);
            this.listPriorities.DragOver += new System.Windows.Forms.DragEventHandler(this.listPriorities_DragOver);
            this.listPriorities.Enter += new System.EventHandler(this.listPriorities_SelectedValueChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(103, 57);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(126, 13);
            this.labelControl3.TabIndex = 4;
            this.labelControl3.Text = "Target changing cooldown";
            // 
            // editBasePriority
            // 
            this.editBasePriority.Location = new System.Drawing.Point(7, 28);
            this.editBasePriority.MenuManager = this.barManager;
            this.editBasePriority.MinimumSize = new System.Drawing.Size(80, 20);
            this.editBasePriority.Name = "editBasePriority";
            this.editBasePriority.Properties.AdvancedModeOptions.Label = "Base Target priority";
            this.editBasePriority.Properties.AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.False;
            this.editBasePriority.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.editBasePriority.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.editBasePriority.Properties.Appearance.Options.UseBackColor = true;
            this.editBasePriority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editBasePriority.Properties.Sorted = true;
            this.editBasePriority.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.editBasePriority.Properties.UseAdvancedMode = DevExpress.Utils.DefaultBoolean.False;
            this.editBasePriority.Size = new System.Drawing.Size(90, 20);
            this.editBasePriority.TabIndex = 1;
            this.editBasePriority.ToolTip = "The basic principle of target selection";
            // 
            // lblBasePriorityRange
            // 
            this.lblBasePriorityRange.Location = new System.Drawing.Point(104, 83);
            this.lblBasePriorityRange.Name = "lblBasePriorityRange";
            this.lblBasePriorityRange.Size = new System.Drawing.Size(101, 13);
            this.lblBasePriorityRange.TabIndex = 4;
            this.lblBasePriorityRange.Text = "Base priority chance:";
            // 
            // editChangeCooldown
            // 
            this.editChangeCooldown.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.editChangeCooldown.Location = new System.Drawing.Point(7, 54);
            this.editChangeCooldown.MenuManager = this.barManager;
            this.editChangeCooldown.MinimumSize = new System.Drawing.Size(80, 20);
            this.editChangeCooldown.Name = "editChangeCooldown";
            this.editChangeCooldown.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editChangeCooldown.Properties.IsFloatValue = false;
            this.editChangeCooldown.Properties.MaskSettings.Set("mask", "N00");
            this.editChangeCooldown.Size = new System.Drawing.Size(91, 20);
            this.editChangeCooldown.TabIndex = 2;
            this.editChangeCooldown.ToolTip = "The time between target changing in seconds";
            // 
            // lblBaseTargetPriority
            // 
            this.lblBaseTargetPriority.Location = new System.Drawing.Point(103, 31);
            this.lblBaseTargetPriority.Name = "lblBaseTargetPriority";
            this.lblBaseTargetPriority.Size = new System.Drawing.Size(99, 13);
            this.lblBaseTargetPriority.TabIndex = 4;
            this.lblBaseTargetPriority.Text = "Base Target priority:";
            // 
            // editBasePriorityRange
            // 
            this.editBasePriorityRange.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.editBasePriorityRange.Location = new System.Drawing.Point(7, 80);
            this.editBasePriorityRange.MinimumSize = new System.Drawing.Size(80, 20);
            this.editBasePriorityRange.Name = "editBasePriorityRange";
            this.editBasePriorityRange.Properties.AdvancedModeOptions.Label = "Base priority range";
            this.editBasePriorityRange.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editBasePriorityRange.Properties.IsFloatValue = false;
            this.editBasePriorityRange.Properties.MaskSettings.Set("mask", "N00");
            this.editBasePriorityRange.Size = new System.Drawing.Size(91, 20);
            this.editBasePriorityRange.TabIndex = 2;
            this.editBasePriorityRange.ToolTip = "The chance to apply the basic targeting.\r\n";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xml";
            this.openFileDialog.Filter = "UucProfile|*.xml";
            this.openFileDialog.InitialDirectory = "Settings\\CC\\";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.FileName = "UccProfile.xml";
            this.saveFileDialog.Filter = "UucProfile|*.xml";
            this.saveFileDialog.InitialDirectory = "Settings\\CC\\";
            // 
            // documentManager
            // 
            this.documentManager.ContainerControl = this;
            this.documentManager.MenuManager = this.barManager;
            this.documentManager.View = this.tabbedView;
            this.documentManager.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[] {
            this.tabbedView});
            // 
            // tabbedView
            // 
            this.tabbedView.DocumentGroups.AddRange(new DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup[] {
            this.documentGroup});
            this.tabbedView.DocumentProperties.AllowClose = false;
            this.tabbedView.DocumentProperties.AllowFloat = false;
            this.tabbedView.DocumentProperties.AllowFloatOnDoubleClick = false;
            this.tabbedView.Documents.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseDocument[] {
            this.docCombat,
            this.docTactic,
            this.docPatrol});
            this.tabbedView.EnableFreeLayoutMode = DevExpress.Utils.DefaultBoolean.False;
            dockingContainer2.Element = this.documentGroup;
            this.tabbedView.RootContainer.Nodes.AddRange(new DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer[] {
            dockingContainer2});
            // 
            // UccEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 588);
            this.Controls.Add(this.panelContainer1);
            this.Controls.Add(this.panLog);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.TouchUIMode = DevExpress.Utils.DefaultBoolean.False;
            this.Name = "UccEditor";
            this.Text = "UCC Profile Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.handler_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.documentGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.docCombat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.docPatrol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.docTactic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            this.panLog.ResumeLayout(false);
            this.controlContainer2.ResumeLayout(false);
            this.controlContainer2.PerformLayout();
            this.panelContainer1.ResumeLayout(false);
            this.panConditions.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.panProperties.ResumeLayout(false);
            this.panProperty.ResumeLayout(false);
            this.panCombat.ResumeLayout(false);
            this.combatActionsControlContainer.ResumeLayout(false);
            this.panPatrol.ResumeLayout(false);
            this.patrolActionsControlContainer.ResumeLayout(false);
            this.panTactic.ResumeLayout(false);
            this.controlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkerTacticActivator.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupPotionUsage)).EndInit();
            this.groupPotionUsage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkerSmartPotionUsage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkerUsePotion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editHealthProcent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupPriority)).EndInit();
            this.groupPriority.ResumeLayout(false);
            this.groupPriority.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listPriorities)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editBasePriority.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editChangeCooldown.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editBasePriorityRange.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private DockManager dockManager;
		private DockPanel panProperties;
		private ControlContainer panProperty;
		private TreeView treeCombatActions;
		private TreeView treePatrolActions;
		private TreeView treeConditions;
		private PropertyGrid propertyGrid;
        private BarManager barManager;
		private Bar barActionTools;
		private Bar barProfile;
		private BarButtonItem bntAddAction;
		private BarButtonItem btnDelAction;
		private BarDockControl barDockControlTop;
		private BarDockControl barDockControlBottom;
		private BarDockControl barDockControlLeft;
		private BarDockControl barDockControlRight;
		private StandaloneBarDockControl standaloneBarDockControl1;
		private BarButtonItem btnSaveProfile;
		private BarButtonItem btnLoadProfile;
		private BarButtonItem btnToEngine;
		private BarButtonItem btnNewProfile;
		private BarButtonItem btnSaveProfielAs;
		private BarButtonItem btnCopyAction;
		private BarButtonItem btnPasteAction;
		private OpenFileDialog openFileDialog;
		private ImageList imageList;
		private SaveFileDialog saveFileDialog;
		private ControlContainer combatActionsControlContainer;
		private ControlContainer patrolActionsControlContainer;
		private DocumentManager documentManager;
		private TabbedView tabbedView;
		private Document docCombat;
		private Document docPatrol;
		private DocumentGroup documentGroup;
		private DockPanel panCombat;
		private DockPanel panPatrol;
		private DockPanel panConditions;
        private DockPanel panelContainer1;
		private ControlContainer dockPanel1_Container;
        private Document docTactic;
        private DockPanel panTactic;
        private ControlContainer controlContainer1;
        private DevExpress.XtraEditors.GroupControl groupPriority;
        private DevExpress.XtraEditors.ListBoxControl listPriorities;
        private DevExpress.XtraEditors.ComboBoxEdit editBasePriority;
        private DevExpress.XtraEditors.LabelControl lblBasePriorityRange;
        private DevExpress.XtraEditors.SpinEdit editChangeCooldown;
        private DevExpress.XtraEditors.LabelControl lblBaseTargetPriority;
        private DevExpress.XtraEditors.SpinEdit editBasePriorityRange;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.GroupControl groupPotionUsage;
        private DevExpress.XtraEditors.CheckEdit checkerSmartPotionUsage;
        private DevExpress.XtraEditors.CheckEdit checkerUsePotion;
        private DevExpress.XtraEditors.SpinEdit editHealthProcent;
        private DevExpress.XtraEditors.CheckEdit checkerTacticActivator;
        private BarButtonItem btnTestSelected;
        private BarButtonItem btnTestAll;
        private DockPanel panLog;
        private ControlContainer controlContainer2;
        private TextBox txtLog;
    }
}